using DlssgManager.Core;

// No installed game, GPU, registry or upstream binary is run. PE fixtures are inert local assemblies.
var root = Path.Combine(Path.GetTempPath(), "DLSSG.OptiDetection-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);
int passed = 0;
void Assert(bool value, string name) { if (!value) throw new InvalidOperationException("FAIL " + name); passed++; Console.WriteLine("PASS " + name); }
try
{
    var fixture = Path.Combine(AppContext.BaseDirectory, "Fixtures", "nvngx_dlssg.dll");
    Assert(File.Exists(fixture), "inert x64 fixture available");
    var game = Path.Combine(root, "Game_游戏");
    Directory.CreateDirectory(game);
    var exe = Path.Combine(game, "Render.exe");
    WritePe(exe);
    var input = Path.Combine(game, "nvngx_dlss.dll");
    File.Copy(fixture, input);
    var package = new ReleasePackage(new("v0.9.5", 1, 2, "fixture.7z", "https://example.invalid", 1,
        new string('a', 64), DateTimeOffset.UnixEpoch, "https://example.invalid"), root, [], true, []) { Provider = ModProvider.OptiScaler };
    var scanner = new GameScanner();
    var scan = await scanner.ScanAsync(game, package, provider: ModProvider.OptiScaler);
    Assert(scan.CanInstall && scan.Provider == ModProvider.OptiScaler && scan.RuntimePath == input, "SR input is sufficient without native DLSSG");
    Assert(scan.Checks.Any(c => c.Code == "game_dlssg" && c.Level == CheckLevel.Info), "absent optional native FG is informational");
    Assert(GameSupportAssessment.Evaluate(scan).State == GameSupportState.RequirementsDetected, "SR-only requirements state does not require SM86 runtime");
    Assert(scan.GraphicsApi == "Unknown" && scan.Checks.Any(c => c.Code == "opti_graphics_api" && c.Level == CheckLevel.Warning), "no API inferred from an upscaler DLL");
    File.WriteAllText(Path.Combine(game, "winhttp.dll"), "foreign");
    Assert(!(await scanner.ScanAsync(game, package, provider: ModProvider.OptiScaler)).CanInstall, "alternate foreign proxy blocks installation");
    File.Delete(Path.Combine(game, "winhttp.dll"));
    File.WriteAllText(Path.Combine(game, "OptiScaler.ini"), "[FrameGen]\nFGInput=auto\n");
    Assert((await scanner.ScanAsync(game, package, provider: ModProvider.OptiScaler)).Checks.Any(c => c.Code == "opti_conflict" && c.Level == CheckLevel.Blocked), "manual configuration requires review before adoption");
    File.Delete(Path.Combine(game, "OptiScaler.ini"));
    File.WriteAllText(Path.Combine(game, "EasyAntiCheat_EOS.dll"), "indicator");
    Assert(GameSupportAssessment.Evaluate(await scanner.ScanAsync(game, package, provider: ModProvider.OptiScaler)).State == GameSupportState.InstallationBlocked, "anti-cheat indicator blocks SR injection");
    File.Delete(Path.Combine(game, "EasyAntiCheat_EOS.dll"));
    File.Delete(input);
    var missing = await scanner.ScanAsync(game, package, provider: ModProvider.OptiScaler);
    Assert(!missing.CanInstall && GameSupportAssessment.Evaluate(missing).State == GameSupportState.NeedsReview, "missing or embedded temporal input requires review rather than unsupported claim");
    WritePe(input, 0x014c);
    Assert(!(await scanner.ScanAsync(game, package, provider: ModProvider.OptiScaler)).CanInstall, "x86 input is rejected");
    File.Delete(input);
    Directory.CreateDirectory(Path.Combine(game, "OptiScaler"));
    File.Copy(fixture, Path.Combine(game, "OptiScaler", "libxess.dll"));
    Assert(!(await scanner.ScanAsync(game, package, provider: ModProvider.OptiScaler)).CanInstall, "bundled mod runtime does not masquerade as native game input");
    File.Copy(fixture, input);
    var system = Path.Combine(root, "System"); Directory.CreateDirectory(system);
    foreach (var name in new[] { "MSVCP140.dll", "VCRUNTIME140.dll", "VCRUNTIME140_1.dll" }) File.Copy(fixture, Path.Combine(system, name));
    var raw = new HardwareInfo([new("AMD Radeon RX 7900 XT", "26.10.1", null, false)], true, false, null,
        [new("windows_x64", CheckLevel.Pass), new("gpu_supported", CheckLevel.Blocked), new("driver_r580", CheckLevel.Blocked), new("hags_configured", CheckLevel.Blocked)]);
    var hardware = OptiScalerSupport.AdaptHardware(raw, system);
    Assert(hardware.Gpus[0].Eligible && hardware.Checks.All(c => c.Level != CheckLevel.Blocked), "AMD candidate does not inherit SM86 driver or HAGS requirements");
    Assert(hardware.Checks.Any(c => c.Code == "opti_gpu" && c.Level == CheckLevel.Warning), "vendor match is not a backend support guarantee");
    Assert(hardware.Checks.All(c => c.Code != "anr_vulkan_loader"), "upscaling does not require ANR Vulkan loader");
    Assert(OptiScalerSupport.IsCandidateGpu("NVIDIA GeForce GTX 1080") && OptiScalerSupport.IsCandidateGpu("Intel Arc A770"), "SR candidates include Pascal and Intel Arc");
    Assert(!OptiScalerSupport.IsCandidateGpu("Microsoft Basic Render Driver"), "software renderer is not an eligible hardware candidate");
    File.Delete(Path.Combine(system, "VCRUNTIME140_1.dll"));
    Assert(OptiScalerSupport.AdaptHardware(raw, system).Checks.Any(c => c.Code == "opti_vc_runtime" && c.Level == CheckLevel.Blocked), "missing x64 prerequisite is explicit");
    var options = new ModOptions { Provider = ModProvider.OptiScaler, Engine = "OptiScaler", Proxy = "dxgi.dll" };
    bool Blocked(ModOptions o, GameScan? s = null, HardwareInfo? h = null) => OptiScalerSupport.EvaluateOptions(h, o, s).Any(c => c.Level == CheckLevel.Blocked);
    Assert(!Blocked(options, scan with { GraphicsApi = "DX11" }), "default FG off works with DX11 upscaling");
    Assert(Blocked(options with { OptiFgOutput = "xefg" }, scan), "one-sided FG selection is blocked");
    Assert(Blocked(options with { OptiFgInput = "nukems", OptiFgOutput = "xefg" }, scan), "Nukem input requires its matching output");
    var fg = options with { OptiFgInput = "upscaler", OptiFgOutput = "fsrfg" };
    Assert(Blocked(fg, scan with { GraphicsApi = "DX11" }) && !Blocked(fg, scan with { GraphicsApi = "DX12" }), "OptiFG is DX12-only");
    Assert(Blocked(fg, scan with { GraphicsApi = "Vulkan" }), "DX12-only FG is not enabled for Vulkan");
    Assert(Blocked(options with { OptiFgInput = "dlssg", OptiFgOutput = "fsrfg" }, scan with { GraphicsApi = "DX12" }), "native FG input needs an actual x64 DLSSG file");
    var native = Path.Combine(game, "nvngx_dlssg.dll"); File.Copy(fixture, native);
    var nativeScan = await scanner.ScanAsync(game, package, provider: ModProvider.OptiScaler);
    var nukems = options with { OptiFgInput = "nukems", OptiFgOutput = "nukems" };
    Assert(!Blocked(nukems, nativeScan with { GraphicsApi = "Vulkan" }), "reviewed Vulkan Nukem pair can use native input");
    File.Delete(native);
    Assert(Blocked(nukems, nativeScan with { GraphicsApi = "Vulkan" }), "removed native runtime is rechecked before installation");
    Assert(Blocked(options with { OptiDx12Upscaler = "dlss" }, scan, hardware), "native DLSS output needs an RTX adapter");
    Assert(!Blocked(options with { OptiDx12Upscaler = "dlss" }, scan), "transactional null hardware does not invent missing adapters");
    var rtx = hardware with { Gpus = [new("NVIDIA GeForce RTX 3090", "621.01", 8.6, true)] };
    Assert(!Blocked(options with { OptiDx12Upscaler = "dlss" }, scan, rtx), "RTX can select native DLSS output");
    Assert(Blocked(options with { OptiDx12Upscaler = "dlss" }, scan with { DlssUpscalerPath = null }, rtx), "RTX alone cannot supply a missing game DLSS input runtime");
    Assert(Blocked(options with { OptiDx11Upscaler = "xess" }, scan, hardware), "native DX11 XeSS output requires Intel Arc");
    Assert(!Blocked(options with { OptiDx11Upscaler = "xess_12" }, scan, hardware), "XeSS D3D11on12 is a distinct choice on other GPUs");
    var arc = hardware with { Gpus = [new("Intel Arc B580", "32.0", null, true)] };
    Assert(!Blocked(options with { OptiDx11Upscaler = "xess" }, scan, arc), "Intel Arc can select native DX11 XeSS");
    Assert(OptiScalerSupport.EvaluateOptions(hardware, options with { OptiFgInput = "fsrfg", OptiFgOutput = "fsrfg" }, scan)
        .Any(c => c.Code == "opti_fsr_fg_input" && c.Level == CheckLevel.Warning), "FSR native FG source needs in-game confirmation");
    File.WriteAllText(Path.Combine(game, ".dlssg-manager.json"), System.Text.Json.JsonSerializer.Serialize(new
    { SchemaVersion = 1, Provider = 2, GameDirectory = game, Proxy = "dxgi.dll", Version = "v0.9.5", Files = new[] { new { Path = "OptiScaler.ini", Sha256 = new string('a', 64) } } }));
    Assert((await scanner.ScanAsync(game, package, provider: ModProvider.OptiScaler)).Checks.Any(c => c.Code == "mod_record" && c.Level == CheckLevel.Blocked), "incomplete standalone installation inventory never shows a managed green status");
    File.Delete(Path.Combine(game, ".dlssg-manager.json"));
    var log = Path.Combine(game, "OptiScaler.log");
    string Header = "[12:00:00.123456] [W] OptiScaler v0.9.5-final (90d9568) loaded\n";
    File.WriteAllText(log, Header + "[12:00:01.123456] [I] XeFG swapchain created\n");
    var result = OptiScalerLogReader.ReadLatest(game, exe);
    Assert(result.ModLoaded && !result.FrameGenerationCreated && !result.Evaluated && result.Summary == "opti_log_loaded", "loaded and created messages never certify generated frames");
    File.AppendAllText(log, "[12:00:02.123456] [E] Dispatch failed\n");
    Assert(OptiScalerLogReader.Read(log) is { Evaluated: false, Summary: "opti_log_errors" } && OptiScalerLogReader.Read(log).Errors.Count == 1, "current session errors are surfaced");
    File.AppendAllText(log, Header);
    Assert(OptiScalerLogReader.Read(log).Errors.Count == 0, "new complete session resets older errors");
    File.AppendAllText(log, "[12:00:03.123456] [E] pending");
    Assert(OptiScalerLogReader.Read(log).PartialLog, "unfinished log line cannot silently hide an error");
    File.WriteAllText(log, Header + new string('x', 140000) + "\n");
    Assert(OptiScalerLogReader.Read(log) is { PartialLog: true, ModLoaded: false }, "oversized line discards uncertain context with bounded memory");
    File.AppendAllText(log, Header);
    Assert(OptiScalerLogReader.Read(log) is { PartialLog: false, ModLoaded: true }, "new complete session restores context");
    File.WriteAllText(log, "12:00:00.123456\tW\tOptiScaler v0.9.5-final loaded\n");
    Assert(OptiScalerLogReader.Read(log).ModLoaded, "tab-delimited upstream file format supported");
    Assert(OptiScalerLogReader.ReadLatest(game, Path.Combine(root, "Other.exe")).Summary == "opti_log_missing", "cross-game executable lookup is rejected");
    using (var stream = File.Create(log)) stream.SetLength(9 * 1024 * 1024);
    Assert(OptiScalerLogReader.Read(log).PartialLog && !OptiScalerLogReader.Read(log).Evaluated, "bounded tail cannot certify omitted context");
    Console.WriteLine($"All {passed} OptiScaler detection/options/log checks passed.");
}
finally { Directory.Delete(root, true); }

static void WritePe(string path, ushort machine = 0x8664)
{
    using var stream = File.Create(path); stream.SetLength(512); using var writer = new BinaryWriter(stream);
    writer.Write((ushort)0x5a4d); stream.Position = 0x3c; writer.Write(0x80);
    stream.Position = 0x80; writer.Write(0x00004550u); writer.Write(machine); writer.Write((ushort)0);
    stream.Position = 0x80 + 20; writer.Write((ushort)0xf0); stream.Position = 0x80 + 24; writer.Write((ushort)0x20b);
}
