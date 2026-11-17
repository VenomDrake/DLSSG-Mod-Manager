using DlssgManager.Core;
using SharpCompress.Common;
using SharpCompress.Writers;
using SharpCompress.Writers.SevenZip;
using System.Buffers.Binary;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

// Inert archive bytes only: no game, DLL or upstream setup script is ever executed.
var root = Path.Combine(Path.GetTempPath(), "DLSSG-OptiReleaseTests-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);
var count = 0;
void Require(bool condition, string name)
{
    if (!condition) throw new Exception(name);
    count++;
    Console.WriteLine("PASS " + name);
}
async Task Reject(Func<Task> action, string name)
{
    var rejected = false;
    try { await action(); }
    catch (Exception error) when (error is InvalidDataException or ArgumentException or InvalidOperationException
        or KeyNotFoundException or SharpCompressException or EndOfStreamException or OverflowException) { rejected = true; }
    Require(rejected, name);
}

Require(OptiScalerReleaseService.InstallableFiles.Count == 20, "reviewed payload has twenty files");
Require(OptiScalerReleaseService.InstallableFiles.Count(p => p.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) == 14,
    "native archive payload includes fourteen libraries");
Require(OptiScalerReleaseService.InstallableFiles.Contains("D3D12_Optiscaler/D3D12Core.dll"), "Agility SDK retains its native relative directory");
Require(OptiScalerReleaseService.InstallableFiles.Count(p => p.StartsWith("Licenses/", StringComparison.Ordinal)) == 4,
    "upstream bundled library licenses are retained");
Require(OptiScalerReleaseService.InstallableFiles.All(p => !p.EndsWith(".bat") && !p.EndsWith(".sh") && !p.StartsWith("!!")),
    "setup scripts and extraction reminder are not installable");

var fixture = Fixture.Archive(("OptiScaler.ini", "[FrameGen]\nFGInput=auto\nFGOutput=auto\n"),
    ("folder/data.txt", "inert fixture"), ("setup_windows.bat", "do not execute"));
var fake = new PackageHandler(fixture);
using var client = new HttpClient(fake);
using var service = new OptiScalerReleaseService(Path.Combine(root, "cache"), client);
var first = await service.EnsureLatestAsync();
Require(first.State == UpdateState.Downloaded && first.Package is { Provider: ModProvider.OptiScaler, Variant: "full", SupportedLayout: false },
    "unreviewed official package is downloaded as blocked data: " + first.Detail);
Require(first.Package!.Files.Count == 0 && !Directory.EnumerateFileSystemEntries(first.Package.Directory).Any(),
    "unreviewed archive remains opaque and no setup script is extracted");
Require(first.Package.Checks.Any(c => c.Code == "opti_package_layout" && c.Level == CheckLevel.Blocked),
    "unknown recipe has a specific installation block");
Require((await service.EnsureLatestAsync()).State == UpdateState.UpToDate, "unchanged asset uses a reverified cache");
Require((await service.LoadCachedAsync()) is { SupportedLayout: false, Files.Count: 0 }, "opaque cache remains inspectable and blocked");
using (var offlineClient = new HttpClient(new OfflineHandler()))
using (var offline = new OptiScalerReleaseService(Path.Combine(root, "cache"), offlineClient))
    Require((await offline.EnsureLatestAsync()).State == UpdateState.OfflineCache, "offline cache is not reported as the latest release");
await File.WriteAllTextAsync(Path.Combine(first.Package.Directory, "extra.dll"), "inert tampering");
Require(await service.LoadCachedAsync() is null, "unexpected file invalidates an opaque package cache");
Require((await service.EnsureLatestAsync()).State == UpdateState.Downloaded, "damaged cache is isolated and recovered");
var cached = (await service.LoadCachedAsync())!;
var archivePath = Path.Combine(Path.GetDirectoryName(cached.Directory)!, "release.7z");
await File.AppendAllTextAsync(archivePath, "tamper");
Require(await service.LoadCachedAsync() is null, "cached archive hash and size are rechecked");
Require((await service.EnsureLatestAsync()).State == UpdateState.Downloaded, "tampered archive is replaced by a verified download");
fake.Generation++;
Require((await service.EnsureLatestAsync()).State == UpdateState.Downloaded, "same-tag asset replacement is detected");

using (var repackedClient = new HttpClient(new PackageHandler(fixture) { Tag = "v0.9.5" }))
using (var repacked = new OptiScalerReleaseService(Path.Combine(root, "repacked"), repackedClient))
{
    var result = await repacked.EnsureLatestAsync();
    Require(result.Package is { SupportedLayout: false, Files.Count: 0 }, "same-version changed archive does not inherit reviewed trust");
}
foreach (var mutation in new[] { "missing-digest", "external-url", "wrong-repo", "wrong-tag-url", "duplicate-asset", "draft", "prerelease", "oversize", "invalid-id", "nightly", "wrong-name" })
    await Reject(() => Task.Run(() => OptiScalerReleaseService.ParseLatestRelease(Fixture.Metadata(fixture, mutation: mutation))),
        "metadata rejects " + mutation);
await Reject(() => Task.Run(() => OptiScalerReleaseService.ParseLatestRelease(new byte[5 * 1024 * 1024])), "oversized metadata is rejected before parsing");
var duplicateMetadata = Encoding.UTF8.GetBytes("{\"draft\":false," + Encoding.UTF8.GetString(Fixture.Metadata(fixture))[1..]);
await Reject(() => Task.Run(() => OptiScalerReleaseService.ParseLatestRelease(duplicateMetadata)), "ambiguous duplicate metadata properties are rejected");

var cleanBeforeFailure = (await service.LoadCachedAsync())!;
var originalArchive = await File.ReadAllBytesAsync(Path.Combine(Path.GetDirectoryName(cleanBeforeFailure.Directory)!, "release.7z"));
fake.Truncate = true;
fake.Generation++;
var partial = await service.EnsureLatestAsync();
Require(partial.State == UpdateState.Failed && (await service.LoadCachedAsync())?.Release.AssetId == cleanBeforeFailure.Release.AssetId,
    "partial download cannot replace the previous verified cache");
Require((await File.ReadAllBytesAsync(Path.Combine(Path.GetDirectoryName(cleanBeforeFailure.Directory)!, "release.7z"))).SequenceEqual(originalArchive),
    "partial-download failure preserves prior archive bytes");
fake.Truncate = false;
using (var cts = new CancellationTokenSource())
{
    var progress = new InlineProgress(stage => { if (stage.Stage == "downloading_package") cts.Cancel(); });
    var canceled = false;
    try { await service.EnsureLatestAsync(progress, cts.Token); }
    catch (OperationCanceledException) when (cts.IsCancellationRequested) { canceled = true; }
    Require(canceled, "download cancellation remains cancellation");
    Require((await service.LoadCachedAsync())?.Release.AssetId == cleanBeforeFailure.Release.AssetId,
        "cancellation does not promote a partial cache");
}
Require(!Directory.EnumerateDirectories(Path.Combine(root, "cache", "full"), ".staging-*").Any(),
    "failed and canceled stages are cleaned up");

foreach (var bad in new[] { "../outside", "folder/../outside", "C:/outside", "x:stream", "CON.dll", "folder./x", "foo//bar", "dir/COM1.txt", "trail /x", "dir\\..\\outside" })
    await Reject(() => Task.Run(() => OptiScalerReleaseService.ResolveSafePackagePath(root, bad)), "unsafe Windows path rejected: " + bad);
var validArchive = Path.Combine(root, "valid.7z");
await File.WriteAllBytesAsync(validArchive, fixture);
var extracted = Path.Combine(root, "extracted");
await OptiScalerReleaseService.ExtractVerifiedArchiveAsync(validArchive, extracted);
Require(await File.ReadAllTextAsync(Path.Combine(extracted, "folder", "data.txt")) == "inert fixture", "native 7z extraction preserves nested data");
Require(await File.ReadAllTextAsync(Path.Combine(extracted, "setup_windows.bat")) == "do not execute", "upstream scripts are extracted only as inert cache data");
await Reject(() => OptiScalerReleaseService.ExtractVerifiedArchiveAsync(validArchive, extracted), "extraction refuses an existing destination");
foreach (var item in new[] { ("traversal", new[] { ("../escape.txt", "x") }),
    ("duplicate", new[] { ("file.txt", "a"), ("FILE.txt", "b") }),
    ("slash-alias", new[] { ("dir/file.txt", "a"), ("dir\\file.txt", "b") }),
    ("file-parent", new[] { ("file", "a"), ("file/child", "b") }) })
{
    var input = Path.Combine(root, item.Item1 + ".7z");
    await File.WriteAllBytesAsync(input, Fixture.Archive(item.Item2));
    var destination = Path.Combine(root, "extract-" + item.Item1);
    await Reject(() => OptiScalerReleaseService.ExtractVerifiedArchiveAsync(input, destination), "7z rejects " + item.Item1 + " before writes");
    Require(!Directory.Exists(destination), "rejected 7z destination remains absent: " + item.Item1);
}
var hugeHeader = fixture.ToArray();
BinaryPrimitives.WriteUInt64LittleEndian(hugeHeader.AsSpan(20, 8), ulong.MaxValue);
BinaryPrimitives.WriteUInt32LittleEndian(hugeHeader.AsSpan(8, 4), Fixture.Crc(hugeHeader.AsSpan(12, 20)));
var headerPath = Path.Combine(root, "huge-header.7z");
await File.WriteAllBytesAsync(headerPath, hugeHeader);
await Reject(() => OptiScalerReleaseService.ExtractVerifiedArchiveAsync(headerPath, Path.Combine(root, "huge-header")),
    "oversized 7z next-header allocation is rejected");
var wrongType = Path.Combine(root, "wrong.7z");
await File.WriteAllBytesAsync(wrongType, Encoding.UTF8.GetBytes(new string('x', 128)));
await Reject(() => OptiScalerReleaseService.ExtractVerifiedArchiveAsync(wrongType, Path.Combine(root, "wrong")), "non-7z content is rejected");

foreach (var scenario in new[] { "redirect", "final-url" })
using (var hostileClient = new HttpClient(new HostileHandler(scenario)))
using (var hostile = new OptiScalerReleaseService(Path.Combine(root, scenario), hostileClient))
    Require((await hostile.EnsureLatestAsync()).State == UpdateState.Failed, "untrusted " + scenario + " is rejected");

var promotion = Path.Combine(root, "promotion");
Directory.CreateDirectory(promotion);
await File.WriteAllTextAsync(Path.Combine(promotion, "previous.bin"), "original");
var promotionRejected = false;
try { typeof(OptiScalerReleaseService).GetMethod("PromoteStage", BindingFlags.NonPublic | BindingFlags.Static)!
    .Invoke(null, [Path.Combine(root, "missing-stage"), promotion]); }
catch (TargetInvocationException error) when (error.InnerException is IOException) { promotionRejected = true; }
Require(promotionRejected && await File.ReadAllTextAsync(Path.Combine(promotion, "previous.bin")) == "original",
    "failed cache promotion restores the previous container");

if (args.Contains("--live", StringComparer.Ordinal))
{
    using var live = new OptiScalerReleaseService(Path.Combine(root, "live"));
    var result = await live.EnsureLatestAsync();
    Require(result.State == UpdateState.Downloaded && result.Package is { SupportedLayout: true, Files.Count: 23 },
        "live reviewed official 0.9.5 archive and all file hashes match: " + result.Detail);
    Require((await live.LoadCachedAsync()) is { SupportedLayout: true }, "live cache rechecks every decompressed byte");
    Require((await live.EnsureLatestAsync()).State == UpdateState.UpToDate, "live latest identity is rechecked");
    var library = Path.Combine(result.Package!.Directory, "OptiScaler.dll");
    await File.AppendAllTextAsync(library, "inert test tamper");
    Require(await live.LoadCachedAsync() is null, "live cache detects a modified reviewed binary without loading it");
}
Console.WriteLine($"PASS {count} OptiScaler release checks. No DLL, setup script or game was executed.");

sealed class InlineProgress(Action<OperationProgress> action) : IProgress<OperationProgress>
{ public void Report(OperationProgress value) => action(value); }
sealed class OfflineHandler : HttpMessageHandler
{ protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) => throw new HttpRequestException("offline fixture"); }
sealed class HostileHandler(string scenario) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        var response = new HttpResponseMessage(scenario == "redirect" ? HttpStatusCode.Found : HttpStatusCode.OK) { Content = new ByteArrayContent([]) };
        if (scenario == "redirect") response.Headers.Location = new Uri("https://example.com/untrusted");
        else response.RequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://example.com/untrusted");
        return Task.FromResult(response);
    }
}
sealed class PackageHandler(byte[] archive) : HttpMessageHandler
{
    public int Generation { get; set; }
    public string Tag { get; set; } = "v0.9.6";
    public bool Truncate { get; set; }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var metadata = request.RequestUri!.Host == "api.github.com";
        var bytes = metadata ? Fixture.Metadata(archive, Tag, Generation) : Truncate ? archive[..^1] : archive;
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(bytes), RequestMessage = request });
    }
}
static class Fixture
{
    public static byte[] Archive(params (string Name, string Text)[] files)
    {
        using var output = new MemoryStream();
        using (var writer = WriterFactory.OpenWriter(output, ArchiveType.SevenZip,
            new SevenZipWriterOptions(CompressionType.LZMA2) { LeaveStreamOpen = true, CompressHeader = false }))
            foreach (var file in files)
            {
                using var source = new MemoryStream(Encoding.UTF8.GetBytes(file.Text));
                writer.Write(file.Name, source, DateTime.UtcNow);
            }
        return output.ToArray();
    }
    public static byte[] Metadata(byte[] archive, string tag = "v0.9.6", int generation = 0, string mutation = "")
    {
        if (mutation == "nightly") tag = "nightly";
        var name = "Optiscaler_" + tag.TrimStart('v') + "-final.20261116.7z";
        if (mutation == "wrong-name") name = "Optiscaler-source.zip";
        var digest = "sha256:" + Convert.ToHexString(SHA256.HashData(archive)).ToLowerInvariant();
        var repo = mutation == "wrong-repo" ? "someone/OptiScaler" : "optiscaler/OptiScaler";
        var url = "https://github.com/" + repo + "/releases/download/" + tag + "/" + name;
        if (mutation == "external-url") url = "https://example.com/" + name;
        if (mutation == "wrong-tag-url") url = url.Replace("/download/" + tag + "/", "/download/v0.8.0/");
        var asset = new Dictionary<string, object?> { ["id"] = mutation == "invalid-id" ? 0 : 756098611 + generation,
            ["name"] = name, ["browser_download_url"] = url, ["digest"] = mutation == "missing-digest" ? null : digest,
            ["size"] = mutation == "oversize" ? OptiScalerReleaseService.MaximumArchiveBytes + 1 : archive.Length };
        return JsonSerializer.SerializeToUtf8Bytes(new { id = 375179769, tag_name = tag, draft = mutation == "draft", prerelease = mutation == "prerelease",
            published_at = DateTimeOffset.Parse("2026-11-16T19:17:35Z"), html_url = "https://github.com/" + repo + "/releases/tag/" + tag,
            assets = mutation == "duplicate-asset" ? new[] { asset, asset } : new[] { asset } });
    }
    public static uint Crc(ReadOnlySpan<byte> data)
    {
        uint crc = uint.MaxValue;
        foreach (var value in data)
        {
            crc ^= value;
            for (var i = 0; i < 8; i++) crc = (crc >> 1) ^ ((crc & 1) != 0 ? 0xEDB88320u : 0);
        }
        return ~crc;
    }
}
