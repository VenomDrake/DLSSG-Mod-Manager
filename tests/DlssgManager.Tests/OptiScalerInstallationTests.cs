using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using DlssgManager.Core;

namespace DlssgManager.Tests;

public static class OptiScalerInstallationTests
{
    private sealed class ImmediateProgress(Action<OperationProgress> action) : IProgress<OperationProgress>
    { public void Report(OperationProgress value) => action(value); }
    private static readonly ModOptions Opti = new() { Provider = ModProvider.OptiScaler, Engine = "OptiScaler", Proxy = "dxgi.dll" };
    private static readonly ModOptions Anr = new() { Provider = ModProvider.Anr, Engine = "ANR", Proxy = "dxgi.dll" };
    private static int _assertions;

    private sealed record Fixture(string Root, string Game, string Exe, ReleasePackage Opti, ReleasePackage Sm86, ReleasePackage Anr)
    {
        public GameScan Scan(ModProvider provider = ModProvider.OptiScaler, ModState state = ModState.Managed) => new(Game, Exe,
            [new(Exe, "SyntheticOptiGame.exe", 100)], null, null, state, null, [new("fixture", CheckLevel.Pass)])
            { Provider = provider, GraphicsApi = "DX12" };
    }

    public static async Task RunAsync()
    {
        var service = new InstallService();
        InstallService.ValidateOptions(Opti);
        foreach (var invalid in new[]
        {
            Opti with { OptiDx11Upscaler = "fsr4" }, Opti with { OptiDx12Upscaler = "fsr31_12" },
            Opti with { OptiVulkanUpscaler = "../../outside" }, Opti with { OptiSpoofing = "yes" },
            Opti with { OptiFgInput = "upscaler" }, Opti with { OptiFgInput = "nukems", OptiFgOutput = "xefg" },
            Opti with { LoggingLevel = 5 }, Opti with { Multiplier = 3 }, Opti with { Proxy = "dinput8.dll" }
        }) await ThrowsAsync<ArgumentException>(() => Task.Run(() => InstallService.ValidateOptions(invalid)));
        var configured = InstallService.ConfigureOptiScalerIni("; custom comment\n[FrameGen]\nFGNvngxReplacement=Apex Neural Rendering FG\n[User]\nKeep=42\n", Opti);
        Assert(configured.Contains("; custom comment") && configured.Contains("Keep=42"), "unrelated INI text retained");
        Assert(configured.Contains("OptiDllPath=OptiScaler") && configured.Contains("FGInput=nofg") && configured.Contains("Enabled=false"), "safe standalone defaults and relocated libraries");
        Assert(!configured.Contains("FGNvngxReplacement"), "dangling ANR custom provider removed");
        Assert(configured.Contains("LogToFile=true") && configured.Contains("LogLevel=2") && configured.Contains("LogFileName=OptiScaler.log"), "diagnostic log configured with documented info level");
        await ThrowsAsync<InvalidDataException>(() => Task.Run(() => InstallService.ConfigureOptiScalerIni("[FrameGen]\nFGInput=nofg\nFGInput=dlssg\n", Opti)));
        await ThrowsAsync<InvalidDataException>(() => Task.Run(() => InstallService.ConfigureOptiScalerIni("[FrameGen]\nFGInput=nofg\n[FrameGen]\nFGInput=dlssg\n", Opti)));
        await ThrowsAsync<InvalidDataException>(() => Task.Run(() => InstallService.ConfigureOptiScalerIni("[Libraries]\nOptiDllPath=auto\n[Libraries]\nOptiDllPath=outside\n", Opti)));
        await ThrowsAsync<InvalidDataException>(() => Task.Run(() => InstallService.ConfigureOptiScalerIni("[FrameGen]\n[FrameGen]\nFGInput=dlssg\n", Opti)));

        var fresh = await CreateAsync();
        var userPlugin = Path.Combine(fresh.Game, "OptiScaler/custom/UserPlugin/notes.txt");
        await WriteAsync(userPlugin, "never delete or rewrite");
        var first = await service.InstallAsync(fresh.Scan(state: ModState.NotInstalled), fresh.Opti, Opti);
        Assert(first.Record is { Provider: ModProvider.OptiScaler, Variant: "full", ExternalOptiScaler: false }, "standalone record owns its installation");
        Assert(first.Record!.Files.Count == OptiScalerReleaseService.InstallableFiles.Count, "only exact reviewed payload inventoried");
        Assert(File.Exists(Path.Combine(fresh.Game, "dxgi.dll")) && File.Exists(Path.Combine(fresh.Game, "OptiScaler/libxess.dll")), "loader and isolated libraries installed");
        Assert(File.Exists(Path.Combine(fresh.Game, "D3D12_Optiscaler/D3D12Core.dll")), "Agility SDK remains beside rendering EXE");
        Assert(!File.Exists(Path.Combine(fresh.Game, "OptiScaler.dll")) && !File.Exists(Path.Combine(fresh.Game, "setup_windows.bat")), "native loader is renamed and scripts are excluded");
        Assert(await File.ReadAllTextAsync(userPlugin) == "never delete or rewrite", "pre-existing unrelated OptiScaler child retained");
        Assert(!(await service.InstallAsync(fresh.Scan(), fresh.Opti, Opti)).Changed, "same package and effective options no-op");
        var iniPath = Path.Combine(fresh.Game, "OptiScaler.ini");
        var customIni = (await File.ReadAllTextAsync(iniPath)).Replace("Dx12Upscaler=auto", "Dx12Upscaler=fsr31") + "\n; player note\n[Player]\nKeep=93\n";
        await File.WriteAllTextAsync(iniPath, customIni);
        Assert((await service.InstallAsync(fresh.Scan(), fresh.Opti, Opti)).Changed, "explicit apply repairs externally changed managed options");
        Assert((await File.ReadAllTextAsync(iniPath)).Contains("Keep=93"), "explicit apply retains unrelated edited INI values");
        var fakeIni = Path.Combine(fresh.Game, "OptiScaler/fakenvapi.ini");
        const string fakeText = "; user Fakenvapi preferences\r\n[User]\r\nKeep=7\r\n";
        await File.WriteAllTextAsync(fakeIni, fakeText);
        var rescanned = await new GameScanner().ScanAsync(fresh.Exe, fresh.Opti, provider: ModProvider.OptiScaler);
        Assert(rescanned.ModState == ModState.Managed && rescanned.CanInstall, "scanner accepts user-edited mutable Fakenvapi configuration");
        var beforeUpdateIni = await File.ReadAllTextAsync(iniPath);
        var update = await PackageAsync(Path.Combine(fresh.Root, "opti-update"), ModProvider.OptiScaler, 2);
        await service.InstallAsync(fresh.Scan(), update, Opti with { OptiDx12Upscaler = "fsr31", LoggingLevel = 3 });
        Assert((await File.ReadAllTextAsync(iniPath)).Contains("Keep=93"), "upstream update preserves unrelated OptiScaler settings");
        Assert(await File.ReadAllTextAsync(fakeIni) == fakeText, "upstream update preserves edited Fakenvapi INI exactly");
        Assert(await File.ReadAllTextAsync(userPlugin) == "never delete or rewrite", "update does not delete unrelated plugin files");
        await service.RestoreAsync(fresh.Game);
        Assert(await File.ReadAllTextAsync(iniPath) == beforeUpdateIni, "restore returns exact previous configuration bytes");
        Assert(await File.ReadAllTextAsync(fakeIni) == fakeText, "restore returns exact previous Fakenvapi configuration");
        await service.DisableAsync(fresh.Game);
        Assert(!File.Exists(Path.Combine(fresh.Game, "dxgi.dll")) && File.Exists(Path.Combine(fresh.Game, "dxgi.dll.disabled")), "disable deactivates the standalone loader");
        await service.InstallAsync(fresh.Scan(), fresh.Opti, Opti);
        Assert(File.Exists(Path.Combine(fresh.Game, "dxgi.dll")) && !File.Exists(Path.Combine(fresh.Game, "dxgi.dll.disabled")), "reapply enables disabled loader without duplicates");

        foreach (var target in new[] { ModProvider.Sm86, ModProvider.Anr })
        {
            var crossing = await CreateAsync();
            var extra = Path.Combine(crossing.Game, "OptiScaler/plugins/unrelated.txt");
            await WriteAsync(extra, "keep across providers");
            await service.InstallAsync(crossing.Scan(), crossing.Opti, Opti);
            var original = await File.ReadAllTextAsync(Path.Combine(crossing.Game, "OptiScaler.ini"));
            var targetPackage = target == ModProvider.Anr ? crossing.Anr : crossing.Sm86;
            var targetOptions = target == ModProvider.Anr ? Anr : new ModOptions();
            await service.InstallAsync(crossing.Scan(target), targetPackage, targetOptions);
            Assert(!File.Exists(Path.Combine(crossing.Game, "OptiScaler/libxess.dll")), "switch removes owned standalone library: " + target);
            Assert(await File.ReadAllTextAsync(extra) == "keep across providers", "switch preserves unowned plugins: " + target);
            Assert((await service.ReadRecordAsync(crossing.Game)) is { ExternalOptiScaler: false } r && r.Provider == target, "switch retains managed ownership: " + target);
            await service.InstallAsync(crossing.Scan(), crossing.Opti, Opti);
            Assert(!Directory.Exists(Path.Combine(crossing.Game, InstallService.AnrProviderRoot)) && !Directory.Exists(Path.Combine(crossing.Game, "dlssg_sm86")), "reverse switch removes previous owned provider: " + target);
            Assert(!(await File.ReadAllTextAsync(Path.Combine(crossing.Game, "OptiScaler.ini"))).Contains("FGNvngxReplacement"), "reverse switch has no stale ANR provider selection: " + target);
            await service.RestoreAsync(crossing.Game);
            Assert((await service.ReadRecordAsync(crossing.Game))?.Provider == target, "restore returns crossed provider: " + target);
            await service.RestoreAsync(crossing.Game);
            Assert((await service.ReadRecordAsync(crossing.Game))?.Provider == ModProvider.OptiScaler, "restore chain returns older standalone installation: " + target);
            Assert(await File.ReadAllTextAsync(Path.Combine(crossing.Game, "OptiScaler.ini")) == original, "restore chain preserves original standalone INI: " + target);
            await service.DisableAsync(crossing.Game);
            await service.InstallAsync(crossing.Scan(target), targetPackage, targetOptions);
            Assert(!File.Exists(Path.Combine(crossing.Game, "dxgi.dll.disabled")), "switch removes old disabled standalone loader: " + target);
            await service.RestoreAsync(crossing.Game);
            Assert((await service.ReadRecordAsync(crossing.Game)) is { Provider: ModProvider.OptiScaler, State: "Disabled" }, "restore returns disabled standalone state: " + target);
        }

        foreach (var proxy in new[] { "dxgi.dll", "winmm.dll", "version.dll", "d3d12.dll", "dbghelp.dll", "wininet.dll", "winhttp.dll" })
        {
            var renamed = await CreateAsync();
            await service.InstallAsync(renamed.Scan(), renamed.Opti, Opti with { Proxy = proxy });
            Assert(File.Exists(Path.Combine(renamed.Game, proxy)), "reviewed proxy supported: " + proxy);
            await service.DisableAsync(renamed.Game);
            await service.RestoreAsync(renamed.Game);
            Assert(!File.Exists(Path.Combine(renamed.Game, proxy + ".disabled")), "disabled proxy removed during restore: " + proxy);
        }

        var external = await CreateAsync();
        Directory.CreateDirectory(Path.Combine(external.Game, "OptiScaler"));
        File.Copy(Path.Combine(AppContext.BaseDirectory, "fixtures/SyntheticOptiScaler.dll"), Path.Combine(external.Game, "dxgi.dll"));
        const string externalConfig = "[Libraries]\nOptiDllPath=auto\n[FrameGen]\nFGInput=upscaler\nFGNvngxReplacement=OtherProvider\n[User]\nKeep=101\n";
        await File.WriteAllTextAsync(Path.Combine(external.Game, "OptiScaler.ini"), externalConfig);
        var addon = await PackageAsync(Path.Combine(external.Root, "addon"), ModProvider.Anr, 1, "addon");
        await service.InstallAsync(external.Scan(ModProvider.Anr), addon, Anr);
        await ThrowsAsync<InvalidOperationException>(() => service.InstallAsync(external.Scan(), external.Opti, Opti));
        await service.RestoreAsync(external.Game);
        Assert(await File.ReadAllTextAsync(Path.Combine(external.Game, "OptiScaler.ini")) == externalConfig, "external addon restore keeps original settings byte-for-byte");
        await ThrowsAsync<InvalidOperationException>(() => service.InstallAsync(external.Scan(), external.Opti, Opti));

        foreach (var path in new[] { "winhttp.dll", "OptiScaler/libxess.dll", "OptiScaler.ini", "OptiScaler.dll", "D3D12_Optiscaler/D3D12Core.dll" })
        {
            var conflict = await CreateAsync();
            await WriteAsync(Path.Combine(conflict.Game, path), "unmanaged original");
            await ThrowsAsync<InvalidOperationException>(() => service.InstallAsync(conflict.Scan(), conflict.Opti, Opti));
            Assert(await File.ReadAllTextAsync(Path.Combine(conflict.Game, path)) == "unmanaged original", "foreign destination protected: " + path);
        }
        var foreignAnr = await CreateAsync();
        await WriteAsync(Path.Combine(foreignAnr.Game, InstallService.AnrProviderRoot, "unmanaged.txt"), "unmanaged ANR");
        await ThrowsAsync<InvalidOperationException>(() => service.InstallAsync(foreignAnr.Scan(), foreignAnr.Opti, Opti));
        var badVariant = await CreateAsync();
        await service.InstallAsync(badVariant.Scan(), badVariant.Opti, Opti);
        await ThrowsAsync<InvalidOperationException>(() => service.InstallAsync(badVariant.Scan(ModProvider.Anr), addon, Anr));

        var modified = await CreateAsync();
        await service.InstallAsync(modified.Scan(), modified.Opti, Opti);
        var managedDll = Path.Combine(modified.Game, "OptiScaler/libxess.dll");
        await File.WriteAllTextAsync(managedDll, "externally replaced library");
        await ThrowsAsync<IOException>(() => service.InstallAsync(modified.Scan(), modified.Opti, Opti));
        await ThrowsAsync<IOException>(() => service.DisableAsync(modified.Game));
        await ThrowsAsync<IOException>(() => service.RestoreAsync(modified.Game));
        Assert(await File.ReadAllTextAsync(managedDll) == "externally replaced library", "tampered managed binary is not overwritten");
        var missing = await CreateAsync();
        await ThrowsAsync<InvalidDataException>(() => service.InstallAsync(missing.Scan(), missing.Opti with
            { Files = missing.Opti.Files.Where(file => file.Path != "libxess.dll").ToArray() }, Opti));
        Assert(!File.Exists(Path.Combine(missing.Game, "dxgi.dll")), "incomplete package never activates loader");
        var alteredPackage = await CreateAsync();
        await File.WriteAllTextAsync(Path.Combine(alteredPackage.Opti.Directory, "libxess.dll"), "modified cache");
        await ThrowsAsync<IOException>(() => service.InstallAsync(alteredPackage.Scan(), alteredPackage.Opti, Opti));
        Assert(!File.Exists(Path.Combine(alteredPackage.Game, "dxgi.dll")), "modified cache does not touch game payload");

        var canceled = await CreateAsync();
        using (var cts = new CancellationTokenSource())
            await ThrowsAsync<OperationCanceledException>(() => service.InstallAsync(canceled.Scan(), canceled.Opti, Opti,
                new ImmediateProgress(progress => { if (progress.Stage == "install_backup") cts.Cancel(); }), cts.Token));
        Assert(!File.Exists(Path.Combine(canceled.Game, "OptiScaler.ini")) && !File.Exists(Path.Combine(canceled.Game, "dxgi.dll")), "cancellation retains original game state");
        var interrupted = await CreateAsync();
        FileStream? held = null;
        try
        {
            await ThrowsAsync<IOException>(() => service.InstallAsync(interrupted.Scan(), interrupted.Opti, Opti,
                new ImmediateProgress(progress =>
                {
                    if (progress.Stage == "install_commit") held = new FileStream(Path.Combine(Directory.GetDirectories(interrupted.Game, ".dlssg-manager-stage-*").Single(), "dxgi.dll"), FileMode.Open, FileAccess.Read, FileShare.None);
                })));
        }
        finally { held?.Dispose(); }
        Assert(!File.Exists(Path.Combine(interrupted.Game, "OptiScaler.ini")) && !File.Exists(Path.Combine(interrupted.Game, "OptiScaler/libxess.dll")), "interrupted commit rolls back all precise payload roots");
        Assert(!File.Exists(Path.Combine(interrupted.Game, ".dlssg-manager.transaction.json")), "successful rollback clears journal");

        var recovery = await CreateAsync();
        var recoveryRecord = (await service.InstallAsync(recovery.Scan(), recovery.Opti, Opti)).Record!;
        await File.WriteAllTextAsync(Path.Combine(recovery.Game, "OptiScaler/libxess.dll"), "half committed file");
        await File.WriteAllTextAsync(Path.Combine(recovery.Game, ".dlssg-manager.transaction.json"), JsonSerializer.Serialize(new
            { SchemaVersion = 1, GameDirectory = recovery.Game, SnapshotDirectory = recoveryRecord.BackupDirectory, StageDirectory = (string?)null, RenderingExe = recovery.Exe }));
        await service.InstallAsync(recovery.Scan(), recovery.Opti, Opti);
        Assert(!File.Exists(Path.Combine(recovery.Game, ".dlssg-manager.transaction.json")) && (await service.ReadRecordAsync(recovery.Game))?.Provider == ModProvider.OptiScaler, "pending precise-root transaction recovers before next install");
        var damagedBackup = await CreateAsync();
        await service.InstallAsync(damagedBackup.Scan(), damagedBackup.Opti, Opti);
        var second = (await service.InstallAsync(damagedBackup.Scan(), damagedBackup.Opti, Opti with { LoggingLevel = 3 })).Record!;
        await File.WriteAllTextAsync(Path.Combine(damagedBackup.Game, second.BackupDirectory, "original/OptiScaler/libxess.dll"), "bad backup");
        await ThrowsAsync<IOException>(() => service.RestoreAsync(damagedBackup.Game));
        Assert((await service.ReadRecordAsync(damagedBackup.Game))?.Options.LoggingLevel == 3, "corrupt backup rejected before game mutation");

        var recordAttack = await CreateAsync();
        await service.InstallAsync(recordAttack.Scan(), recordAttack.Opti, Opti);
        var recordFile = Path.Combine(recordAttack.Game, InstallService.RecordName);
        var recordJson = await File.ReadAllTextAsync(recordFile);
        foreach (var badPath in new[] { "../outside.dll", "OptiScaler/plugins/foreign.dll", "D3D12_Optiscaler/foreign.dll" })
        {
            var bad = JsonNode.Parse(recordJson)!;
            bad["Files"]!.AsArray()[0]!["Path"] = badPath;
            await File.WriteAllTextAsync(recordFile, bad.ToJsonString());
            await ThrowsAsync<InvalidDataException>(() => service.ReadRecordAsync(recordAttack.Game));
        }
        await File.WriteAllTextAsync(recordFile, recordJson);

        var fgScan = await CreateAsync();
        var fg = Opti with { OptiFgInput = "dlssg", OptiFgOutput = "fsrfg" };
        await ThrowsAsync<InvalidOperationException>(() => service.InstallAsync(fgScan.Scan(), fgScan.Opti, fg));
        await ThrowsAsync<InvalidOperationException>(() => service.InstallAsync(fgScan.Scan() with { GraphicsApi = "DX11" }, fgScan.Opti,
            Opti with { OptiFgInput = "upscaler", OptiFgOutput = "fsrfg" }));
        var native = Path.Combine(fgScan.Game, "nvngx_dlssg.dll"); File.Copy(Environment.ProcessPath!, native);
        await ThrowsAsync<InvalidOperationException>(() => service.InstallAsync(fgScan.Scan() with { NativeFrameGenerationPath = native }, fgScan.Opti, fg,
            new ImmediateProgress(progress => { if (progress.Stage == "install_backup") File.Delete(native); })));
        Assert(!File.Exists(Path.Combine(fgScan.Game, "dxgi.dll")), "native FG input revalidated before transaction commit");
        Console.WriteLine($"PASS OptiScaler transactions: {_assertions} assertions; options, precise roots, preserved user files/INIs, full provider switches, external addon protection, restore/disable/history, cancellation/rollback/recovery, tampering and technical revalidation");
    }

    private static async Task<Fixture> CreateAsync()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "test-artifacts", "opti-install-" + Guid.NewGuid().ToString("N"));
        var game = Path.Combine(root, "game"); Directory.CreateDirectory(game);
        var exe = Path.Combine(game, "SyntheticOptiGame.exe"); File.Copy(Environment.ProcessPath!, exe);
        File.Copy(Environment.ProcessPath!, Path.Combine(game, "nvngx_dlss.dll")); // inert x64 input fixture, never loaded
        return new(root, game, exe, await PackageAsync(Path.Combine(root, "opti"), ModProvider.OptiScaler),
            await PackageAsync(Path.Combine(root, "sm86"), ModProvider.Sm86), await PackageAsync(Path.Combine(root, "anr"), ModProvider.Anr));
    }

    private static async Task<ReleasePackage> PackageAsync(string directory, ModProvider provider, int id = 1, string variant = "full")
    {
        var content = new Dictionary<string, string>();
        if (provider == ModProvider.OptiScaler)
        {
            foreach (var native in OptiScalerReleaseService.InstallableFiles) content[native] = "synthetic reviewed payload " + native + " " + id;
            content["OptiScaler.ini"] = "; original fixture\n[Upscalers]\nDx11Upscaler=auto\nDx12Upscaler=auto\nVulkanUpscaler=auto\n[Libraries]\nOptiDllPath=auto\n[FrameGen]\nFGInput=auto\nFGOutput=auto\n";
            content["fakenvapi.ini"] = "; fixture Fakenvapi config\n[User]\nKeep=1\n";
            content["setup_windows.bat"] = "must never be installed or executed";
        }
        else if (provider == ModProvider.Anr)
        {
            var prefix = variant == "addon" ? "Apex Neural Rendering FG" : "game/" + InstallService.AnrProviderRoot;
            content[prefix + "/dlssg/nvngx_dlssg.dll"] = "synthetic ANR library";
            content[prefix + "/dlssg/nr_fg.ini"] = "path = auto\nmodel_size = large\nnits_per_unit = 80\nmax_generated = 1\nscene_cut = 1\n";
            content[prefix + "/dlssg/kernels/pack.nrx"] = "synthetic model";
            content[prefix + "/dlssg/programs/example/pack.nrx"] = "synthetic program";
            if (variant == "full")
            {
                content["game/dxgi.dll"] = "synthetic OptiScaler-for-ANR loader";
                content["game/OptiScaler.ini"] = "[Libraries]\nOptiDllPath=auto\n[FrameGen]\nFGInput=nvngxfg\nFGNvngxReplacement=Apex Neural Rendering FG\n";
            }
        }
        else
        {
            content["version.dll"] = "synthetic SM86 loader";
            content["dlssg_sm86.ini"] = "[FrameGeneration]\nEngine=DLSS\n[DLSSG]\nPreset=A\n";
            content["dlssg_sm86/backend/sm86_backend.dll"] = "synthetic backend";
            content["dlssg_sm86/kernels/fg_kernels.dll"] = "synthetic kernels";
            content["dlssg_sm86/runtime/nvngx_dlssg.dll"] = "synthetic runtime";
        }
        var files = new List<PackageFile>();
        foreach (var (relative, text) in content)
        {
            var path = Path.Combine(directory, relative); await WriteAsync(path, text);
            var bytes = await File.ReadAllBytesAsync(path);
            files.Add(new(relative, bytes.Length, Convert.ToHexString(SHA256.HashData(bytes))));
        }
        var version = provider switch { ModProvider.OptiScaler => "0.9.5", ModProvider.Anr => "FG-V.B17", _ => "0.4.2" };
        return new(new(version, id, id, "fixture.zip", "https://github.com/optiscaler/OptiScaler/releases/download/v0.9.5/fixture.zip",
            100, new string('a', 64), DateTimeOffset.UtcNow, "https://github.com/optiscaler/OptiScaler/releases/tag/v0.9.5"),
            directory, files, true, [new("fixture", CheckLevel.Pass)]) { Provider = provider, Variant = variant };
    }

    private static async Task WriteAsync(string path, string content)
    { Directory.CreateDirectory(Path.GetDirectoryName(path)!); await File.WriteAllTextAsync(path, content); }
    private static void Assert(bool condition, string name)
    { _assertions++; if (!condition) throw new Exception("FAIL OptiScaler installation: " + name); }
    private static async Task ThrowsAsync<T>(Func<Task> operation) where T : Exception
    {
        _assertions++;
        try { await operation(); } catch (T) { return; }
        throw new Exception("Expected " + typeof(T).Name + " for OptiScaler transaction");
    }
}
