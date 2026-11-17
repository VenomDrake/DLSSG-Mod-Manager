using System.Text;

namespace DlssgManager.Core;

public sealed partial class InstallService
{
    // The library directory may also contain user plugins and custom providers. Only
    // these reviewed files are transaction roots; its parent directory is never owned.
    private static readonly string[] OptiScalerInstalledPaths = OptiScalerReleaseService.InstallableFiles
        .Where(path => !SameRelative(path, "OptiScaler.dll"))
        .Select(OptiScalerDestination).ToArray();

    private static string OptiScalerDestination(string nativePath) =>
        SameRelative(nativePath, "OptiScaler.ini") || SameRelative(nativePath, "D3D12_Optiscaler/D3D12Core.dll")
            ? Normalize(nativePath) : "OptiScaler/" + Normalize(nativePath);

    private static bool IsOptiScalerInstalledFile(string relative) =>
        OptiScalerInstalledPaths.Contains(Normalize(relative), StringComparer.OrdinalIgnoreCase);

    private static bool IsValidOptiScalerRecord(InstallationRecord record, string actualProxy) =>
        record.Variant == "full" && !record.ExternalOptiScaler && record.Files.Count == OptiScalerInstalledPaths.Length + 1 &&
        record.Files.All(file => SameRelative(file.Path, actualProxy) || IsOptiScalerInstalledFile(file.Path)) &&
        OptiScalerInstalledPaths.All(path => record.Files.Any(file => SameRelative(file.Path, path)));

    private static void ValidateOptiScalerOptions(ModOptions options)
    {
        if (options.Engine != "OptiScaler" || options.Multiplier != 2 || options.LoggingLevel is < 0 or > 4 ||
            options.Proxy is not ("dxgi.dll" or "winmm.dll" or "version.dll" or "d3d12.dll" or "dbghelp.dll" or "wininet.dll" or "winhttp.dll") ||
            options.OptiDx11Upscaler is not ("auto" or "fsr22" or "fsr31" or "xess" or "xess_12" or "fsr21_12" or "fsr22_12" or "fsr31_12" or "dlss") ||
            options.OptiDx12Upscaler is not ("auto" or "xess" or "fsr21" or "fsr22" or "fsr31" or "dlss") ||
            options.OptiVulkanUpscaler is not ("auto" or "fsr21" or "fsr22" or "fsr31" or "xess" or "fsr21_12" or "fsr31_12" or "dlss") ||
            options.OptiFgInput is not ("nofg" or "dlssg" or "nukems" or "fsrfg" or "upscaler" or "fsrfg30") ||
            options.OptiFgOutput is not ("nofg" or "fsrfg" or "xefg" or "nukems") ||
            (options.OptiFgInput == "nofg") != (options.OptiFgOutput == "nofg") ||
            options.OptiFgInput == "nukems" && options.OptiFgOutput != "nukems" ||
            options.OptiSpoofing is not ("auto" or "true" or "false"))
            throw new ArgumentException("One or more OptiScaler options are invalid or unsupported.", nameof(options));
    }

    private static void ValidateOptiScalerScan(GameScan scan, ModOptions options)
    {
        if (options.Provider != ModProvider.OptiScaler) return;
        if (OptiScalerSupport.EvaluateOptions(null, options, scan).Any(check => check.Level == CheckLevel.Blocked))
            throw new InvalidOperationException("The selected OptiScaler options do not match the game's detected graphics API or inputs. Scan the game again.");
    }

    public static string ConfigureOptiScalerIni(string source, ModOptions options)
    {
        ValidateOptiScalerOptions(options);
        foreach (var section in new[] { "Libraries", "Upscalers", "FrameGen", "Spoofing", "Log" })
        {
            var headers = source.Replace("\r\n", "\n").Split('\n').Select(line => line.Trim())
                .Where(line => line.StartsWith('[') && line.IndexOf(']') > 0 &&
                    line[1..line.IndexOf(']')].Trim().Equals(section, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (headers.Length > 1 || headers.Any(line => !line.Equals("[" + section + "]", StringComparison.OrdinalIgnoreCase)))
                throw new InvalidDataException("The OptiScaler configuration has duplicate or ambiguous managed sections.");
        }
        // ANR uses this custom key in its bundled loader. Stable OptiScaler 0.9.5
        // uses FGInput/FGOutput; never retain a removed ANR provider selection.
        var text = SetOrRemoveIniValue(source, "FrameGen", "FGNvngxReplacement", null);
        foreach (var (section, key, value) in new (string, string, string)[]
        {
            ("Libraries", "OptiDllPath", "OptiScaler"),
            ("Upscalers", "Dx11Upscaler", options.OptiDx11Upscaler),
            ("Upscalers", "Dx12Upscaler", options.OptiDx12Upscaler),
            ("Upscalers", "VulkanUpscaler", options.OptiVulkanUpscaler),
            ("FrameGen", "Enabled", options.OptiFgInput == "nofg" ? "false" : "true"),
            ("FrameGen", "FGInput", options.OptiFgInput), ("FrameGen", "FGOutput", options.OptiFgOutput),
            ("Spoofing", "Dxgi", options.OptiSpoofing),
            ("Log", "LogToFile", "true"),
            ("Log", "LogLevel", options.LoggingLevel.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("Log", "LogFileName", "OptiScaler.log")
        })
        {
            // Reject duplicate keys rather than choosing a different effective value
            // from the upstream INI reader.
            _ = GetIniValue(text, section, key);
            text = SetIniValue(text, section, key, value);
        }
        return text;
    }

    private static async Task<bool> OptiScalerConfigurationMatchesAsync(string directory, ModOptions options, CancellationToken token)
    {
        var path = Child(directory, "OptiScaler.ini");
        if (!File.Exists(path) || new FileInfo(path).Length > 1024 * 1024) return false;
        var text = await File.ReadAllTextAsync(path, token);
        return ConfigureOptiScalerIni(text, options) == text;
    }

    private static async Task<List<InstalledFile>> StageOptiScalerPackageAsync(ReleasePackage package, string stage, ModOptions options,
        string directory, InstallationRecord? previous, CancellationToken token)
    {
        if (package.Variant != "full") throw new InvalidDataException("Unknown OptiScaler package variant.");
        EnsureNoReparse(package.Directory);
        var installed = new List<InstalledFile>();
        foreach (var nativePath in OptiScalerReleaseService.InstallableFiles)
        {
            token.ThrowIfCancellationRequested();
            var file = package.Files.SingleOrDefault(item => SameRelative(item.Path, nativePath))
                ?? throw new InvalidDataException("The reviewed OptiScaler package is missing a required file: " + nativePath);
            var source = Child(package.Directory, file.Path);
            await VerifyFileAsync(source, file.Size, file.Sha256, token);
            var relative = SameRelative(nativePath, "OptiScaler.dll") ? options.Proxy : OptiScalerDestination(nativePath);
            ValidateInstalledRelative(relative, options.Proxy);
            var target = Child(stage, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            if (SameRelative(nativePath, "OptiScaler.ini"))
            {
                var text = await File.ReadAllTextAsync(source, token);
                if (previous?.Provider == ModProvider.OptiScaler)
                {
                    var existing = Child(directory, relative);
                    if (!File.Exists(existing) || new FileInfo(existing).Length > 1024 * 1024)
                        throw new InvalidDataException("The installed OptiScaler configuration is missing or too large.");
                    text = await File.ReadAllTextAsync(existing, token);
                }
                await File.WriteAllTextAsync(target, ConfigureOptiScalerIni(text, options), new UTF8Encoding(false), token);
            }
            else if (SameRelative(nativePath, "fakenvapi.ini") && previous?.Provider == ModProvider.OptiScaler)
            {
                var existing = Child(directory, relative);
                if (!File.Exists(existing) || new FileInfo(existing).Length > 64 * 1024)
                    throw new InvalidDataException("The installed Fakenvapi configuration is missing or too large.");
                await CopyFileAsync(existing, target, token);
            }
            else
            {
                await CopyFileAsync(source, target, token);
                await VerifyFileAsync(target, file.Size, file.Sha256, token);
            }
            installed.Add(await InventoryFileAsync(stage, relative, token));
        }
        if (installed.Select(file => file.Path).Distinct(StringComparer.OrdinalIgnoreCase).Count() != installed.Count)
            throw new InvalidDataException("The OptiScaler inventory contains duplicate paths.");
        return installed;
    }

    private static void ValidateOptiScalerCollisions(string directory, GameScan scan, ReleasePackage package, InstallationRecord? previous)
    {
        if (scan.ModState == ModState.UnknownConflict)
            throw new InvalidOperationException("An unknown mod or proxy is already installed.");
        if (previous?.ExternalOptiScaler == true)
            throw new InvalidOperationException("Restore the original external OptiScaler installation before switching to standalone OptiScaler.");
        if (previous?.Provider != ModProvider.Sm86 && (Exists(Child(directory, "dlssg_sm86")) || Exists(Child(directory, "dlssg_sm86.ini"))))
            throw new InvalidOperationException("Restore or adopt the existing SM86 installation before switching providers.");
        if (Exists(Child(directory, AnrProviderRoot)) && previous?.Provider != ModProvider.Anr)
            throw new InvalidOperationException("An unmanaged ANR provider exists and will not be overwritten.");
        if (package.Provider == ModProvider.Anr && package.Variant != "full")
            throw new InvalidOperationException("Switching from managed OptiScaler requires the full ANR package.");
        if (package.Provider == ModProvider.OptiScaler && previous?.Provider != ModProvider.OptiScaler)
        {
            // An ANR full installation owns only its loader/config/provider. Its
            // other OptiScaler children still belong to the user and are protected.
            foreach (var path in OptiScalerInstalledPaths)
                if (Exists(Child(directory, path)) && (previous is null || !previous.Files.Any(file => SameRelative(file.Path, path))))
                    throw new InvalidOperationException("An unmanaged OptiScaler file already exists and will not be overwritten: " + path);
            if (Exists(Child(directory, "OptiScaler.dll")))
                throw new InvalidOperationException("An unmanaged OptiScaler installation exists. Restore it before using standalone OptiScaler.");
        }
        var allowed = previous is null ? new HashSet<string>(StringComparer.OrdinalIgnoreCase) :
            new HashSet<string>([previous.State == "Disabled" ? previous.Proxy + ".disabled" : previous.Proxy], StringComparer.OrdinalIgnoreCase);
        foreach (var proxy in ProxyNames)
            if (Exists(Child(directory, proxy)) && !allowed.Contains(proxy))
                throw new InvalidOperationException("An unrecognized proxy exists: " + proxy);
    }
}
