using System.Text.RegularExpressions;

namespace DlssgManager.Core;

/// <summary>Standalone OptiScaler prerequisites. Adapter detection is not a promise that every backend works.</summary>
public static class OptiScalerSupport
{
    public static bool IsCandidateGpu(string name) => !string.IsNullOrWhiteSpace(name) &&
        (name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase) ||
         name.Contains("Radeon", StringComparison.OrdinalIgnoreCase) ||
         name.Contains("Intel", StringComparison.OrdinalIgnoreCase));

    public static HardwareInfo AdaptHardware(HardwareInfo hardware, string? systemDirectory = null)
    {
        ArgumentNullException.ThrowIfNull(hardware);
        var gpus = hardware.Gpus.Select(g => g with { Eligible = IsCandidateGpu(g.Name) }).ToArray();
        var checks = hardware.Checks.Where(c => c.Code is not ("gpu_supported" or "driver_r580" or "hags_configured"
            or "hags_runtime" or "gpu_architecture_unverified" or "gpu_native_fg")).ToList();
        checks.Add(new("opti_gpu", gpus.Any(g => g.Eligible) ? CheckLevel.Warning : CheckLevel.Blocked,
            string.Join(" / ", gpus.Select(g => g.Name))));
        checks.Add(new("opti_driver", CheckLevel.Warning,
            string.Join(" / ", gpus.Where(g => g.Eligible).Select(g => g.Name + " · " + g.DriverVersion))));
        checks.Add(new("opti_hags", CheckLevel.Info, hardware.HagsConfigured switch { true => "Configured", false => "Not configured", _ => "Unknown" }));
        var system = systemDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.System);
        var missing = new[] { "MSVCP140.dll", "VCRUNTIME140.dll", "VCRUNTIME140_1.dll" }
            .Where(name => !IsX64File(Path.Combine(system, name))).ToArray();
        checks.Add(new("opti_vc_runtime", missing.Length == 0 ? CheckLevel.Pass : CheckLevel.Blocked,
            missing.Length == 0 ? "Microsoft Visual C++ 2015–2022 x64" : string.Join(", ", missing)));
        return hardware with { Gpus = gpus, Checks = checks };
    }

    /// <summary>Checks actual choices. Null hardware skips hardware checks for transactional revalidation.</summary>
    public static IReadOnlyList<CheckResult> EvaluateOptions(HardwareInfo? hardware, ModOptions options, GameScan? scan)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.Provider != ModProvider.OptiScaler) return [];
        var checks = new List<CheckResult>();
        bool inputOff = options.OptiFgInput == "nofg", outputOff = options.OptiFgOutput == "nofg";
        bool pairValid = inputOff == outputOff && (options.OptiFgInput != "nukems" || options.OptiFgOutput == "nukems");
        checks.Add(new("opti_fg_pair", pairValid ? CheckLevel.Pass : CheckLevel.Blocked));
        bool dlssOutput = new[] { options.OptiDx11Upscaler, options.OptiDx12Upscaler, options.OptiVulkanUpscaler }.Contains("dlss");
        if (hardware is not null && dlssOutput)
        {
            bool rtx = hardware.Gpus.Any(g => g.Name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase)
                && Regex.IsMatch(g.Name, @"\bRTX\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
            checks.Add(new("opti_dlss_gpu", rtx ? CheckLevel.Pass : CheckLevel.Blocked));
        }
        if (scan is not null && dlssOutput)
            checks.Add(new("opti_dlss_runtime", scan.DlssUpscalerPath is { } dlss && DetectionPaths.IsWithin(dlss, scan.Root)
                && !DetectionPaths.HasReparsePoint(dlss, scan.Root) && IsX64File(dlss) ? CheckLevel.Pass : CheckLevel.Blocked));
        if (hardware is not null && options.OptiDx11Upscaler == "xess")
            checks.Add(new("opti_xess_dx11_gpu", hardware.Gpus.Any(g => g.Name.Contains("Intel", StringComparison.OrdinalIgnoreCase)
                && Regex.IsMatch(g.Name, @"\bArc\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) ? CheckLevel.Pass : CheckLevel.Blocked));
        if (!inputOff && !outputOff)
        {
            var api = scan?.GraphicsApi ?? "Unknown";
            bool compatible = api != "DX11" && (api != "Vulkan" || options.OptiFgInput == "nukems" && options.OptiFgOutput == "nukems");
            checks.Add(new("opti_fg_api", !compatible ? CheckLevel.Blocked : api == "Unknown" ? CheckLevel.Warning : CheckLevel.Pass, api));
            if (scan is not null && options.OptiFgInput is "dlssg" or "nukems")
                checks.Add(new("opti_native_fg", scan.NativeFrameGenerationPath is { } native &&
                    DetectionPaths.IsWithin(native, scan.Root) && !DetectionPaths.HasReparsePoint(native, scan.Root) && IsX64File(native)
                    ? CheckLevel.Pass : CheckLevel.Blocked));
            if (options.OptiFgInput == "dlssg")
                checks.Add(new("opti_streamline2", scan?.Checks.Any(c => c.Code == "opti_streamline2" && c.Level == CheckLevel.Pass) == true
                    ? CheckLevel.Pass : CheckLevel.Warning));
            if (options.OptiFgOutput == "xefg") checks.Add(new("opti_xefg_gpu", CheckLevel.Warning));
            if (options.OptiFgInput is "fsrfg" or "fsrfg30") checks.Add(new("opti_fsr_fg_input", CheckLevel.Warning));
        }
        return checks;
    }

    private static bool IsX64File(string path)
    {
        try { return File.Exists(path) && DetectionPe.Read(path).X64; }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or BadImageFormatException or ArgumentException) { return false; }
    }
}
