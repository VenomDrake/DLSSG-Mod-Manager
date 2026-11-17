using SharpCompress.Archives.SevenZip;
using SharpCompressException = SharpCompress.Common.SharpCompressException;
using SharpCompress.Readers;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DlssgManager.Core;

/// <summary>Downloads official OptiScaler assets as data. No downloaded script, executable or DLL is run.</summary>
public sealed class OptiScalerReleaseService : IDisposable
{
    public const string RepositoryUrl = "https://github.com/optiscaler/OptiScaler";
    public const string LatestReleaseUrl = "https://api.github.com/repos/optiscaler/OptiScaler/releases/latest";
    public const string ReviewedRelease = "v0.9.5";
    public const string ReviewedArchiveSha256 = "77e3ed76bed04e1fb32bacb5a72799ff172f3da22ed5231c0cf3e84fe6761662";
    public const long MaximumArchiveBytes = 128L * 1024 * 1024;
    public const long MaximumExpandedBytes = 512L * 1024 * 1024;
    public const int MaximumEntries = 4096;
    private const int MaximumMetadataBytes = 4 * 1024 * 1024;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };
    private static readonly HashSet<string> RedirectHosts = new(StringComparer.OrdinalIgnoreCase)
    { "github.com", "api.github.com", "release-assets.githubusercontent.com", "objects.githubusercontent.com", "github-releases.githubusercontent.com" };
    private static readonly IReadOnlyDictionary<string, (long Size, string Sha256)> ReviewedFiles =
        new Dictionary<string, (long, string)>(StringComparer.OrdinalIgnoreCase)
        {
            ["!! README_EXTRACT ALL FILES TO GAME FOLDER !!.txt"] = (400, "09051e971a9a1bf7a1500a041b2a4652414a13d23266709cdb41e4ec12208ade"),
            ["amd_fidelityfx_dx12.dll"] = (39184, "e2191d3c0828be45eb8affd4e3ec0378f2293adc345b772a7170edcf3b021ecb"),
            ["amd_fidelityfx_framegeneration_dx12.dll"] = (29679376, "e6843252f58b0341a0d80f6a20bc0d840643ee95bc48c22cac08745344577bb6"),
            ["amd_fidelityfx_framegeneration_vk.dll"] = (36806928, "d002dabff8c5e71eaa715fc99438cd4dc81230262a7897b16bc402efec3498b4"),
            ["amd_fidelityfx_upscaler_dx12.dll"] = (57664272, "aad4fccccb29084a655f9a4f9f613bd22c8c03fb4a0f8840148cc6543b4ccace"),
            ["amd_fidelityfx_upscaler_vk.dll"] = (36135696, "4a7e895af6e0b615d40a84d59eb8c2b1054ad91cdd0c739567940270d51bdab7"),
            ["amd_fidelityfx_vk.dll"] = (39184, "38a56a01fb3565c0757d9f5c1e4e69de4b65e5f2e5769b323b200dc1f142bab9"),
            ["D3D12_Optiscaler/D3D12Core.dll"] = (3351064, "07d286c306f8117321422affd9e6388c12d0fb4be1c7fc689d9e899324feeb24"),
            ["dlssg_to_fsr3_amd_is_better.dll"] = (3038208, "806020c0444f7841a3a9f661295c16fc482e981e0666d0b9624bf6ccd19c4b5e"),
            ["fakenvapi.dll"] = (415232, "373e73dc06e2d66e90083fde3bc99ef5c1ed0bef98d1f7eda56c8b073269c3f1"),
            ["fakenvapi.ini"] = (466, "a4bfa3ec7b4badd93ae5e1ad74a582238082d871f54f38f68e7d9fb6384ce772"),
            ["libxell.dll"] = (415368, "d2030dcd694fda8f2ec7e044b13e6db8f0b56d4ba9113a5efad334e3f3ded8c7"),
            ["libxess.dll"] = (77795704, "251659dd84a3e84de67c886a4186e01f3eca49b00641906fe38bb6b807e5d5b7"),
            ["libxess_dx11.dll"] = (156016, "c7cfe86f0c9d94e4fb3696d3cd5035e2bbb6a8b1b0572f8b7395a4cdfd0c625e"),
            ["libxess_fg.dll"] = (22957432, "ec5e0c65e075570c6ede72618bb666d0be0c2e10b2ea9762c0fe8cb8e375ab27"),
            ["Licenses/DirectX_LICENSE.txt"] = (13147, "5239850894610071566f7ecee0b751fde43c862032d92b99d7d0f596b3433ebd"),
            ["Licenses/FidelityFX_v1_LICENSE.md"] = (1142, "a335c2bb2bd28184b9a30b6e35b0c7e4aa7826abd1c388fc2aa02405e9d04f03"),
            ["Licenses/FidelityFX_v2_LICENSE.md"] = (61784, "f0da09d71ad5c82759a179e774535d4a829e5c96c49294167c1152402b2cb400"),
            ["Licenses/XeSS_LICENSE.txt"] = (4194, "784be42c39a4a4d03cf85d88e15e59d75828fca773ce45b8dbfffbd7eaf208df"),
            ["OptiScaler.dll"] = (26052912, "e2300905b2b7e6e14c8c4b02c0c555c4582fafc92a2e36e117ce027b7e259d59"),
            ["OptiScaler.ini"] = (47713, "4f3314dc1fc4ecad800b2ce99c2adf972eda3954504dc3a273487ddaab75924d"),
            ["setup_linux.sh"] = (12328, "0571b0ae077d985616acf9f4101e8338fc225b1b22aa7b339100c7ea3f3022be"),
            ["setup_windows.bat"] = (16947, "cf731a2d308df4b48db968b911b0ea1fc490fdf79d6c2ed168363312c95d2f4e"),
        };
    public static IReadOnlyList<string> InstallableFiles { get; } = Array.AsReadOnly(ReviewedFiles.Keys.Where(path =>
        !path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) && !path.EndsWith(".sh", StringComparison.OrdinalIgnoreCase)
        && !path.StartsWith("!!", StringComparison.Ordinal)).ToArray());
    private readonly string _cacheRoot;
    private readonly HttpClient _client;
    private readonly bool _ownsClient;
    private readonly SemaphoreSlim _operationLock = new(1, 1);
    private bool _disposed;

    public OptiScalerReleaseService(string cacheRoot, HttpClient? httpClient = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cacheRoot);
        _cacheRoot = Path.GetFullPath(cacheRoot);
        _ownsClient = httpClient is null;
        _client = httpClient ?? new HttpClient(new HttpClientHandler { AllowAutoRedirect = false }) { Timeout = Timeout.InfiniteTimeSpan };
    }

    public async Task<UpdateResult> EnsureLatestAsync(IProgress<OperationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        const string variant = "full";
        await _operationLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        string? stage = null;
        ReleasePackage? cached = null;
        try
        {
            EnsureCacheRoot();
            var variantRoot = Path.Combine(_cacheRoot, variant);
            EnsureSafeDirectory(variantRoot);
            progress?.Report(new("checking_update"));
            cached = await LoadCachedAsync(cancellationToken).ConfigureAwait(false);
            ReleaseInfo release;
            try
            {
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeout.CancelAfter(TimeSpan.FromSeconds(30));
                using var response = await SendTrustedAsync(new Uri(LatestReleaseUrl), timeout.Token).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                release = ParseLatestRelease(await ReadLimitedAsync(response.Content, MaximumMetadataBytes, timeout.Token).ConfigureAwait(false));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (Exception error) when (error is HttpRequestException or OperationCanceledException or IOException)
            { return new(cached is null ? UpdateState.Failed : UpdateState.OfflineCache, cached, SafeError(error)); }
            if (cached is not null && ReleaseService.SameAsset(cached.Release, release)) return new(UpdateState.UpToDate, cached);
            stage = Path.Combine(variantRoot, ".staging-" + Guid.NewGuid().ToString("N"));
            EnsureSafeDirectory(stage);
            var archive = Path.Combine(stage, "release.7z");
            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                timeout.CancelAfter(TimeSpan.FromMinutes(10));
                try { await DownloadAsync(release, archive, progress, timeout.Token).ConfigureAwait(false); }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
                catch (Exception error) when (error is HttpRequestException or OperationCanceledException)
                { return new(cached is null ? UpdateState.Failed : UpdateState.OfflineCache, cached, SafeError(error)); }
            }
            if (!(await HashFileAsync(archive, cancellationToken).ConfigureAwait(false)).Equals(release.Sha256, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("The OptiScaler archive SHA-256 does not match GitHub metadata.");
            progress?.Report(new("verifying_package", null, release.Version));
            var packageDirectory = Path.Combine(stage, "package");
            // Future/repacked packages are authenticated downloads, but remain opaque until reviewed.
            // This prevents a changed recipe or unreviewed compression stream gaining execution/install support.
            if (IsReviewedArchive(release))
                await ExtractVerifiedArchiveAsync(archive, packageDirectory, cancellationToken).ConfigureAwait(false);
            else EnsureSafeDirectory(packageDirectory);
            var package = await ValidatePackageAsync(packageDirectory, release, cancellationToken).ConfigureAwait(false);
            var key = $"{release.ReleaseId}-{release.AssetId}-{release.Sha256[..16].ToLowerInvariant()}";
            var containers = Path.Combine(variantRoot, "packages");
            EnsureSafeDirectory(containers);
            var destination = Path.Combine(containers, key);
            PromoteStage(stage, destination);
            stage = null;
            var state = new CacheState(release, key, variant);
            var temporary = Path.Combine(variantRoot, ".current-" + Guid.NewGuid().ToString("N") + ".json");
            try
            {
                await File.WriteAllTextAsync(temporary, JsonSerializer.Serialize(state, JsonOptions), cancellationToken).ConfigureAwait(false);
                RejectReparsePoint(Path.Combine(variantRoot, "current.json"));
                File.Move(temporary, Path.Combine(variantRoot, "current.json"), true);
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
            progress?.Report(new("download_complete", 100, release.Version));
            return new(UpdateState.Downloaded, package with { Directory = Path.Combine(destination, "package") });
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception error) when (IsDataError(error)) { return new(UpdateState.Failed, cached, SafeError(error)); }
        finally { if (stage is not null) TryDeleteStage(stage); _operationLock.Release(); }
    }

    /// <summary>Rechecks the archive and every extracted file against its contents, including extra files.</summary>
    public async Task<ReleasePackage?> LoadCachedAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        const string variant = "full";
        try
        {
            var variantRoot = Path.Combine(_cacheRoot, variant);
            var statePath = Path.Combine(variantRoot, "current.json");
            if (!File.Exists(statePath)) return null;
            RejectAllAncestors(statePath);
            if (new FileInfo(statePath).Length > MaximumMetadataBytes) return null;
            var state = JsonSerializer.Deserialize<CacheState>(await File.ReadAllTextAsync(statePath, cancellationToken).ConfigureAwait(false), JsonOptions);
            if (state?.Release is null || state.Variant != variant || string.IsNullOrEmpty(state.PackageKey) || !Regex.IsMatch(state.PackageKey, @"^\d+-\d+-[a-f0-9]{16}$", RegexOptions.CultureInvariant)
                || !IsSha256(state.Release.Sha256) || state.Release.Size is <= 0 or > MaximumArchiveBytes) return null;
            ValidateReleaseUrls(state.Release);
            ValidateAssetIdentity(state.Release);
            if (state.Release.ReleaseId <= 0 || state.Release.AssetId <= 0
                || state.PackageKey != $"{state.Release.ReleaseId}-{state.Release.AssetId}-{state.Release.Sha256[..16].ToLowerInvariant()}") return null;
            var container = Path.Combine(variantRoot, "packages", state.PackageKey);
            var archive = Path.Combine(container, "release.7z");
            RejectAllAncestors(archive);
            if (!File.Exists(archive) || new FileInfo(archive).Length != state.Release.Size
                || !(await HashFileAsync(archive, cancellationToken).ConfigureAwait(false)).Equals(state.Release.Sha256, StringComparison.OrdinalIgnoreCase)) return null;
            var directory = Path.Combine(container, "package");
            if (!Directory.Exists(directory)) return null;
            if (IsReviewedArchive(state.Release))
                await VerifyExtractionAgainstArchiveAsync(archive, directory, cancellationToken).ConfigureAwait(false);
            else if (EnumerateSafeFiles(directory, cancellationToken).Any()) return null;
            return await ValidatePackageAsync(directory, state.Release, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception error) when (IsDataError(error)) { return null; }
    }

    /// <summary>Inventories inert package data. Installation support requires the exact reviewed archive and full file recipe.</summary>
    public async Task<ReleasePackage> ValidatePackageAsync(string directory, ReleaseInfo release,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(release);
        ValidateReleaseUrls(release);
        ValidateAssetIdentity(release);
        var root = Path.GetFullPath(directory);
        RejectAllAncestors(root);
        if (!Directory.Exists(root)) throw new InvalidDataException("The OptiScaler package directory is missing.");
        var files = new List<PackageFile>();
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        long expanded = 0;
        foreach (var path in EnumerateSafeFiles(root, cancellationToken))
        {
            var relative = Path.GetRelativePath(root, path).Replace('\\', '/');
            _ = ResolveSafePackagePath(root, relative);
            if (!names.Add(relative)) throw new InvalidDataException("Duplicate package paths are not accepted.");
            var size = new FileInfo(path).Length;
            if (size < 0 || (expanded = checked(expanded + size)) > MaximumExpandedBytes || files.Count >= MaximumEntries)
                throw new InvalidDataException("The OptiScaler package exceeds accepted limits.");
            files.Add(new(relative, size, await HashFileAsync(path, cancellationToken).ConfigureAwait(false)));
        }
        var supported = IsReviewedArchive(release) && files.Count == ReviewedFiles.Count
            && files.All(file => ReviewedFiles.TryGetValue(file.Path, out var expected)
                && file.Size == expected.Size && file.Sha256.Equals(expected.Sha256, StringComparison.OrdinalIgnoreCase));
        return new(release, root, files.OrderBy(f => f.Path, StringComparer.OrdinalIgnoreCase).ToArray(), supported,
            [new("package_hash", CheckLevel.Pass, release.Sha256),
             new("opti_package_layout", supported ? CheckLevel.Pass : CheckLevel.Blocked, release.Version)])
            { Provider = ModProvider.OptiScaler, Variant = "full" };
    }

    public static ReleaseInfo ParseLatestRelease(ReadOnlyMemory<byte> json)
    {
        if (json.Length > MaximumMetadataBytes) throw new InvalidDataException("OptiScaler release metadata is too large.");
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        ValidateUniqueProperties(root);
        if (root.GetProperty("draft").GetBoolean() || root.GetProperty("prerelease").GetBoolean())
            throw new InvalidDataException("A draft or prerelease OptiScaler package is not accepted.");
        var tag = root.GetProperty("tag_name").GetString() ?? "";
        var version = VersionFromTag(tag);
        var pattern = "^Optiscaler_" + Regex.Escape(version) + @"-final\.[0-9]{8}\.7z$";
        var assets = root.GetProperty("assets").EnumerateArray().Where(a =>
            Regex.IsMatch(a.GetProperty("name").GetString() ?? "", pattern, RegexOptions.CultureInvariant)).ToArray();
        if (assets.Length != 1) throw new InvalidDataException("A unique complete stable OptiScaler package was not found.");
        var asset = assets[0];
        var digest = asset.GetProperty("digest").GetString() ?? "";
        if (!digest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase) || !IsSha256(digest[7..]))
            throw new InvalidDataException("OptiScaler asset SHA-256 metadata is required.");
        var release = new ReleaseInfo(tag, root.GetProperty("id").GetInt64(), asset.GetProperty("id").GetInt64(),
            asset.GetProperty("name").GetString()!, asset.GetProperty("browser_download_url").GetString()!,
            asset.GetProperty("size").GetInt64(), digest[7..].ToLowerInvariant(),
            root.GetProperty("published_at").GetDateTimeOffset(), root.GetProperty("html_url").GetString()!);
        ValidateReleaseUrls(release);
        ValidateAssetIdentity(release);
        return release;
    }

    private static void ValidateUniqueProperties(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in value.EnumerateObject())
            {
                if (!names.Add(property.Name)) throw new InvalidDataException("Duplicate release metadata properties are not accepted.");
                ValidateUniqueProperties(property.Value);
            }
        }
        else if (value.ValueKind == JsonValueKind.Array)
            foreach (var element in value.EnumerateArray()) ValidateUniqueProperties(element);
    }

    private static string VersionFromTag(string tag)
    {
        if (tag is null || !Regex.IsMatch(tag, @"^v[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$", RegexOptions.CultureInvariant))
            throw new InvalidDataException("Unsupported stable OptiScaler tag format.");
        return tag[1..];
    }

    private static void ValidateAssetIdentity(ReleaseInfo release)
    {
        var pattern = "^Optiscaler_" + Regex.Escape(VersionFromTag(release.Version)) + @"-final\.[0-9]{8}\.7z$";
        if (release.ReleaseId <= 0 || release.AssetId <= 0 || release.Size is <= 0 or > MaximumArchiveBytes
            || !IsSha256(release.Sha256) || !Regex.IsMatch(release.AssetName, pattern, RegexOptions.CultureInvariant))
            throw new InvalidDataException("OptiScaler release identity or size is invalid.");
        if (release.DownloadUrl != RepositoryUrl + "/releases/download/" + release.Version + "/" + release.AssetName
            || release.ReleaseUrl != RepositoryUrl + "/releases/tag/" + release.Version)
            throw new InvalidDataException("OptiScaler release URLs do not match their exact release identity.");
    }

    private static bool IsReviewedArchive(ReleaseInfo release) => release.Version == ReviewedRelease
        && release.AssetName == "Optiscaler_0.9.5-final.20261116.7z" && release.Size == 70418897
        && release.Sha256.Equals(ReviewedArchiveSha256, StringComparison.OrdinalIgnoreCase);

    private static bool IsDataError(Exception error) => error is IOException or UnauthorizedAccessException or InvalidDataException
        or JsonException or CryptographicException or ArgumentException or InvalidOperationException or KeyNotFoundException or FormatException or OverflowException
        or SharpCompressException or NotSupportedException;
    private static bool IsSha256(string? value) => value is not null && Regex.IsMatch(value, "^[A-Fa-f0-9]{64}$", RegexOptions.CultureInvariant);
    private static string SafeError(Exception error) => error.Message.Length <= 500 ? error.Message : error.Message[..500];
    private static void PromoteStage(string stage, string destination)
    {
        string? previous = null;
        if (Directory.Exists(destination))
        {
            RejectReparsePoint(destination);
            previous = destination + ".previous-" + Guid.NewGuid().ToString("N");
            Directory.Move(destination, previous);
        }
        try { Directory.Move(stage, destination); }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            // A failed promotion must not leave current.json pointing at a directory
            // displaced by this operation. Keep all package bytes; do not retry a failed
            // filesystem operation or make an unverified staging directory current.
            if (previous is not null && !Directory.Exists(destination) && !File.Exists(destination))
            {
                try { RejectReparsePoint(previous); Directory.Move(previous, destination); }
                catch (Exception recovery) when (recovery is IOException or UnauthorizedAccessException)
                { throw new IOException("OptiScaler cache promotion failed; the previous package was retained but could not be restored.", new AggregateException(error, recovery)); }
            }
            throw;
        }
    }
    private void EnsureCacheRoot() { RejectAllAncestors(_cacheRoot); EnsureSafeDirectory(_cacheRoot); }
    private static void RejectAllAncestors(string path)
    { for (var current = Path.GetFullPath(path); current is not null; current = Path.GetDirectoryName(current)) RejectReparsePoint(current); }

    private static async Task VerifyExtractionAgainstArchiveAsync(string archivePath, string directory, CancellationToken token)
    {
        RejectAllAncestors(directory);
        var inventory = EnumerateSafeFiles(directory, token).Select(p => Path.GetRelativePath(directory, p).Replace('\\', '/')).ToHashSet(StringComparer.OrdinalIgnoreCase);
        using var archive = OpenArchive(archivePath);
        var entries = ValidateArchiveEntries(archive, directory, token);
        if (!inventory.SetEquals(entries.Where(e => !e.Value.Directory).Select(e => e.Key)))
            throw new InvalidDataException("The OptiScaler extracted inventory differs from its verified archive.");
        using var reader = archive.ExtractAllEntries();
        while (reader.MoveToNextEntry())
        {
            token.ThrowIfCancellationRequested();
            if (reader.Entry.IsDirectory) continue;
            var relative = reader.Entry.Key!.Replace('\\', '/');
            var item = entries[relative];
            RejectReparseAncestors(directory, item.Target);
            if (!File.Exists(item.Target) || new FileInfo(item.Target).Length != reader.Entry.Size)
                throw new InvalidDataException("A cached OptiScaler file is missing or has changed size.");
            using var input = reader.OpenEntryStream();
            using var digest = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            var actual = await CopyLimitedAsync(input, Stream.Null, reader.Entry.Size, token, digest).ConfigureAwait(false);
            if (actual != reader.Entry.Size || !(await HashFileAsync(item.Target, token).ConfigureAwait(false))
                    .Equals(Convert.ToHexString(digest.GetHashAndReset()), StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("A cached OptiScaler file differs from its verified archive.");
        }
    }

    /// <summary>Resolves a package-relative Windows-safe filename, rejecting traversal and alternate streams.</summary>
    public static string ResolveSafePackagePath(string root, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath) || relativePath.Length > 512 || relativePath.Contains('\0')
            || relativePath.StartsWith('/') || relativePath.StartsWith('\\') || relativePath.Contains(':'))
            throw new InvalidDataException("Unsafe package path.");
        var normalized = relativePath.Replace('\\', '/');
        var segments = normalized.Split('/');
        if (segments.Any(segment => string.IsNullOrEmpty(segment) || segment is "." or ".."
            || segment.EndsWith(' ') || segment.EndsWith('.') || segment.Any(c => c < 32 || "<>\"|?*".Contains(c))
            || Regex.IsMatch(segment.Split('.')[0], @"^(CON|PRN|AUX|NUL|COM[0-9]|LPT[0-9])$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)))
            throw new InvalidDataException("Unsafe package path.");
        var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(fullRoot, Path.Combine(segments)));
        if (!fullPath.StartsWith(fullRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("The package entry is outside its destination.");
        return fullPath;
    }

    /// <summary>Extracts authenticated 7z data with explicit path/size/link checks; no archive script or binary is executed.</summary>
    public static async Task ExtractVerifiedArchiveAsync(string archivePath, string destination,
        CancellationToken cancellationToken = default)
    {
        if (Directory.Exists(destination) || File.Exists(destination))
            throw new InvalidDataException("The extraction destination must be new.");
        RejectAllAncestors(destination);
        using var archive = OpenArchive(archivePath);
        var entries = ValidateArchiveEntries(archive, destination, cancellationToken);
        EnsureSafeDirectory(destination);
        using var reader = archive.ExtractAllEntries();
        while (reader.MoveToNextEntry())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var relative = reader.Entry.Key!.Replace('\\', '/');
            if (!entries.TryGetValue(relative, out var item)) throw new InvalidDataException("The archive inventory changed during extraction.");
            if (item.Directory) { EnsureSafeDirectory(item.Target); continue; }
            EnsureSafeDirectory(Path.GetDirectoryName(item.Target)!);
            RejectReparseAncestors(destination, item.Target);
            using var input = reader.OpenEntryStream();
            await using var output = new FileStream(item.Target, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                81920, FileOptions.Asynchronous | FileOptions.SequentialScan);
            var actual = await CopyLimitedAsync(input, output, reader.Entry.Size, cancellationToken).ConfigureAwait(false);
            if (actual != reader.Entry.Size) throw new InvalidDataException("An archive entry has an unexpected expanded size.");
        }
    }

    private static SevenZipArchive OpenArchive(string archivePath)
    {
        RejectAllAncestors(archivePath);
        var info = new FileInfo(archivePath);
        if (!info.Exists || info.Length is < 32 or > MaximumArchiveBytes)
            throw new InvalidDataException("The OptiScaler archive has an invalid size.");
        using (var file = File.OpenRead(archivePath))
        {
            Span<byte> signature = stackalloc byte[6];
            file.ReadExactly(signature);
            if (!signature.SequenceEqual(new byte[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C }))
                throw new InvalidDataException("A complete 7z archive is required.");
        }
        return (SevenZipArchive)SevenZipArchive.OpenArchive(archivePath, new ReaderOptions { LookForHeader = false });
    }

    private static Dictionary<string, (string Target, bool Directory)> ValidateArchiveEntries(
        SevenZipArchive archive, string destination, CancellationToken token)
    {
        var result = new Dictionary<string, (string Target, bool Directory)>(StringComparer.OrdinalIgnoreCase);
        long expanded = 0;
        foreach (var entry in archive.Entries)
        {
            token.ThrowIfCancellationRequested();
            var name = entry.Key ?? throw new InvalidDataException("An archive entry has no name.");
            var relative = name.Replace('\\', '/');
            if (entry.IsDirectory) relative = relative.TrimEnd('/');
            var target = ResolveSafePackagePath(destination, relative);
            var attributes = entry.Attrib ?? 0;
            if (result.Count >= MaximumEntries || !result.TryAdd(relative, (target, entry.IsDirectory))
                || entry.IsEncrypted || entry.IsSplitAfter || entry.IsAnti || !string.IsNullOrEmpty(entry.LinkTarget)
                || (attributes & (int)FileAttributes.ReparsePoint) != 0 || ((attributes >> 16) & 0xF000) == 0xA000)
                throw new InvalidDataException("The archive has duplicate, linked, encrypted or unsupported entries.");
            if (entry.Size < 0 || entry.Size > MaximumExpandedBytes || (expanded = checked(expanded + entry.Size)) > MaximumExpandedBytes
                || entry.IsDirectory && entry.Size != 0)
                throw new InvalidDataException("The expanded OptiScaler archive exceeds accepted limits.");
        }
        if (result.Count == 0) throw new InvalidDataException("The OptiScaler archive is empty.");
        var fileTargets = result.Values.Where(e => !e.Directory).Select(e => e.Target).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var item in result.Values)
            for (var parent = Path.GetDirectoryName(item.Target); parent is not null && parent.Length >= Path.GetFullPath(destination).Length; parent = Path.GetDirectoryName(parent))
                if (fileTargets.Contains(parent)) throw new InvalidDataException("A file conflicts with a directory in the archive.");
        return result;
    }

    private async Task DownloadAsync(ReleaseInfo release, string destination, IProgress<OperationProgress>? progress, CancellationToken token)
    {
        using var response = await SendTrustedAsync(new Uri(release.DownloadUrl), token).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        if (response.Content.Headers.ContentLength is long contentLength && contentLength != release.Size)
            throw new InvalidDataException("The release download size does not match GitHub metadata.");
        await using var source = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
        await using var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None,
            81920, FileOptions.Asynchronous | FileOptions.SequentialScan);
        var buffer = new byte[81920];
        long received = 0;
        var lastReport = Environment.TickCount64;
        progress?.Report(new("downloading_package", 0, release.Version));
        while (true)
        {
            var count = await source.ReadAsync(buffer, token).ConfigureAwait(false);
            if (count == 0) break;
            received += count;
            if (received > release.Size || received > MaximumArchiveBytes) throw new InvalidDataException("The release download exceeded its declared size.");
            await output.WriteAsync(buffer.AsMemory(0, count), token).ConfigureAwait(false);
            if (Environment.TickCount64 - lastReport > 150)
            {
                progress?.Report(new("downloading_package", received * 100.0 / release.Size, release.Version));
                lastReport = Environment.TickCount64;
            }
        }
        if (received != release.Size) throw new IOException("The release download ended before the expected number of bytes was received.");
    }

    private async Task<HttpResponseMessage> SendTrustedAsync(Uri uri, CancellationToken token)
    {
        for (var redirects = 0; redirects <= 5; redirects++)
        {
            ValidateTrustedUri(uri);
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.UserAgent.ParseAdd("DLSSG-Mod-Manager-OptiScaler/0.5 (+https://github.com/optiscaler/OptiScaler)");
            request.Headers.Accept.ParseAdd(uri.Host.Equals("api.github.com", StringComparison.OrdinalIgnoreCase)
                ? "application/vnd.github+json" : "application/octet-stream");
            if (uri.Host.Equals("api.github.com", StringComparison.OrdinalIgnoreCase)) request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
            try
            {
                if (response.RequestMessage?.RequestUri is { } finalUri) ValidateTrustedUri(finalUri);
                if (response.StatusCode is HttpStatusCode.MovedPermanently or HttpStatusCode.Found
                    or HttpStatusCode.SeeOther or HttpStatusCode.TemporaryRedirect or HttpStatusCode.PermanentRedirect)
                {
                    var location = response.Headers.Location ?? throw new InvalidDataException("A release redirect had no destination.");
                    uri = location.IsAbsoluteUri ? location : new Uri(uri, location);
                    response.Dispose();
                    continue;
                }
                return response;
            }
            catch { response.Dispose(); throw; }
        }
        throw new HttpRequestException("Too many redirects while downloading the release.");
    }

    private static void ValidateTrustedUri(Uri uri)
    {
        if (uri.Scheme != Uri.UriSchemeHttps || !uri.IsDefaultPort || !string.IsNullOrEmpty(uri.UserInfo)
            || !RedirectHosts.Contains(uri.IdnHost)) throw new InvalidDataException("The release URL is outside the official GitHub download hosts.");
    }

    private static void ValidateReleaseUrls(ReleaseInfo release)
    {
        if (!Uri.TryCreate(release.DownloadUrl, UriKind.Absolute, out var download)
            || !Uri.TryCreate(release.ReleaseUrl, UriKind.Absolute, out var page)) throw new InvalidDataException("Invalid release URL.");
        ValidateTrustedUri(download);
        ValidateTrustedUri(page);
        if (!download.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase)
            || !download.AbsolutePath.StartsWith("/optiscaler/OptiScaler/releases/download/", StringComparison.Ordinal)
            || !page.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase)
            || !page.AbsolutePath.StartsWith("/optiscaler/OptiScaler/releases/tag/", StringComparison.Ordinal))
            throw new InvalidDataException("The release URL does not belong to the configured upstream repository.");
        var decodedName = Uri.UnescapeDataString(download.Segments.Last());
        if (!decodedName.Equals(release.AssetName, StringComparison.Ordinal)) throw new InvalidDataException("The release asset URL has an unexpected filename.");
    }

    private static async Task<byte[]> ReadLimitedAsync(HttpContent content, long limit, CancellationToken token)
    {
        if (content.Headers.ContentLength > limit) throw new InvalidDataException("The metadata response is too large.");
        await using var source = await content.ReadAsStreamAsync(token).ConfigureAwait(false);
        using var output = new MemoryStream();
        await CopyLimitedAsync(source, output, limit, token).ConfigureAwait(false);
        return output.ToArray();
    }

    private static async Task<long> CopyLimitedAsync(Stream source, Stream destination, long limit, CancellationToken token, IncrementalHash? digest = null)
    {
        var buffer = new byte[81920];
        long copied = 0;
        while (true)
        {
            var count = await source.ReadAsync(buffer, token).ConfigureAwait(false);
            if (count == 0) return copied;
            copied = checked(copied + count);
            if (copied > limit) throw new InvalidDataException("The stream exceeded the accepted size limit.");
            digest?.AppendData(buffer, 0, count);
            await destination.WriteAsync(buffer.AsMemory(0, count), token).ConfigureAwait(false);
        }
    }

    private static async Task<string> HashFileAsync(string path, CancellationToken token)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        return Convert.ToHexString(await SHA256.HashDataAsync(stream, token).ConfigureAwait(false)).ToLowerInvariant();
    }

    private static void RejectReparsePoint(string path)
    {
        if ((File.Exists(path) || Directory.Exists(path)) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
            throw new InvalidDataException("Symbolic links and directory junctions are not accepted in the package cache.");
    }

    private static void RejectReparseAncestors(string root, string target)
    {
        var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        for (var current = Path.GetFullPath(target); current is not null && current.Length >= fullRoot.Length; current = Path.GetDirectoryName(current))
            RejectReparsePoint(current);
    }

    private static void EnsureSafeDirectory(string path)
    {
        RejectReparsePoint(path);
        Directory.CreateDirectory(path);
        RejectReparsePoint(path);
    }

    private static IEnumerable<string> EnumerateSafeFiles(string directory, CancellationToken token)
    {
        var stack = new Stack<string>();
        stack.Push(directory);
        var count = 0;
        while (stack.Count > 0)
        {
            token.ThrowIfCancellationRequested();
            var current = stack.Pop();
            RejectReparsePoint(current);
            foreach (var item in Directory.EnumerateFileSystemEntries(current))
            {
                token.ThrowIfCancellationRequested();
                if (++count > MaximumEntries * 2) throw new InvalidDataException("The package contains too many filesystem entries.");
                RejectReparsePoint(item);
                if (Directory.Exists(item)) stack.Push(item);
                else yield return item;
            }
        }
    }

    private void TryDeleteStage(string stage)
    {
        try
        {
            var fullPath = Path.GetFullPath(stage);
            if (!string.Equals(Path.GetDirectoryName(fullPath), Path.Combine(_cacheRoot, "full"), StringComparison.OrdinalIgnoreCase)
                || !Regex.IsMatch(Path.GetFileName(fullPath), @"^\.staging-[a-f0-9]{32}$", RegexOptions.CultureInvariant)) return;
            if (!Directory.Exists(fullPath)) return;
            DeleteTreeWithoutFollowingLinks(fullPath);
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }

    private static void DeleteTreeWithoutFollowingLinks(string path)
    {
        if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) { Directory.Delete(path); return; }
        foreach (var entry in Directory.EnumerateFileSystemEntries(path))
        {
            var attributes = File.GetAttributes(entry);
            if ((attributes & FileAttributes.Directory) != 0) DeleteTreeWithoutFollowingLinks(entry);
            else File.Delete(entry);
        }
        Directory.Delete(path);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_ownsClient) _client.Dispose();
        _operationLock.Dispose();
    }
    private sealed record CacheState(ReleaseInfo Release, string PackageKey, string Variant);
}
