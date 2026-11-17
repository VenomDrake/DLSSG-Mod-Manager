using System.IO;
using System.Windows;
using System.Windows.Controls;
using DlssgManager.Core;

namespace DlssgManager.Desktop;

public partial class MainWindow
{
    private static string ProviderName(ModProvider provider) => Localization.Get(provider switch
    { ModProvider.Anr => "provider_anr", ModProvider.OptiScaler => "provider_opti", _ => "provider_sm86" });

    private string ProviderLogScope() => Localization.Get(_provider switch
    { ModProvider.Anr => "anr_log_scope", ModProvider.OptiScaler => "opti_log_scope", _ => "log_scope" });

    private IReadOnlyList<CheckResult> OptionChecks() => _provider == ModProvider.OptiScaler
        ? OptiScalerSupport.EvaluateOptions(_hardware, GetOptions(), _scan) : [];

    private void InitializeProvider()
    {
        try
        {
            var saved = File.ReadAllText(Path.Combine(App.DataDirectory, "provider.txt")).Trim();
            _provider = saved switch { "Anr" => ModProvider.Anr, "OptiScaler" => ModProvider.OptiScaler, _ => ModProvider.Sm86 };
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException) { }
        var argument = Array.IndexOf(App.Arguments, "--provider");
        if (argument >= 0 && argument + 1 < App.Arguments.Length)
            _provider = App.Arguments[argument + 1].ToLowerInvariant() switch
            { "anr" => ModProvider.Anr, "optiscaler" or "opti" => ModProvider.OptiScaler, _ => ModProvider.Sm86 };
    }

    private void RefreshProviderUi()
    {
        if (ProviderDescription is null) return;
        var anr = _provider == ModProvider.Anr;
        var opti = _provider == ModProvider.OptiScaler;
        ProviderDescription.Text = Localization.Get(opti ? "provider_opti_description" : anr ? "provider_anr_description" : "provider_sm86_description");
        if (_record is not null)
            ProviderDescription.Text += "\n" + string.Format(Localization.Get("provider_installed"), ProviderName(_record.Provider));
        AnrOptionsPanel.Visibility = anr ? Visibility.Visible : Visibility.Collapsed;
        AnrSettingsHint.Visibility = anr ? Visibility.Visible : Visibility.Collapsed;
        AnrGameHelp.Visibility = anr ? Visibility.Visible : Visibility.Collapsed;
        OptiOptionsPanel.Visibility = opti ? Visibility.Visible : Visibility.Collapsed;
        OptiSettingsHint.Visibility = opti ? Visibility.Visible : Visibility.Collapsed;
        RecommendedText.Text = Localization.Get(opti ? "opti_recommended" : "recommended");
        VerificationScopeText.Text = ProviderLogScope();
        if (_verification is null) VerificationStatus.Text = Localization.Get(opti ? "opti_log_missing" : "not_verified");
        var gpus = _rawHardware?.Gpus ?? _hardware?.Gpus;
        var detected = gpus is { Count: > 0 };
        var hasAmd = gpus?.Any(g => g.Name.Contains("Radeon", StringComparison.OrdinalIgnoreCase)) == true;
        var hasNvidia = gpus?.Any(g => g.Name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase)) == true;
        var hasIntel = gpus?.Any(g => g.Name.Contains("Intel", StringComparison.OrdinalIgnoreCase)) == true;
        HardwareHelpText.Text = Localization.Get(opti ? "opti_driver" : anr ? "anr_driver_requirements" : "hags_help");
        AnrAmdTestNote.Visibility = anr && hasAmd ? Visibility.Visible : Visibility.Collapsed;
        NvidiaDriverButton.Visibility = _provider == ModProvider.Sm86 || !detected || hasNvidia ? Visibility.Visible : Visibility.Collapsed;
        AmdDriverButton.Visibility = (anr || opti) && (!detected || hasAmd) ? Visibility.Visible : Visibility.Collapsed;
        IntelDriverButton.Visibility = opti && (!detected || hasIntel) ? Visibility.Visible : Visibility.Collapsed;
        Sm86OptionsHelp.Visibility = anr || opti ? Visibility.Collapsed : Visibility.Visible;
        ProxyPicker.ToolTip = Localization.Get(opti ? "opti_proxy_help" : anr ? "anr_options_hint" : "proxy_help");
        foreach (var control in new Control[] { OptimizationPicker, PresetPicker, SrPicker, FlowPicker, HudPicker, LogPicker })
        {
            control.IsEnabled = !anr && !opti;
            if (control.Parent is FrameworkElement panel) panel.Visibility = anr || opti ? Visibility.Collapsed : Visibility.Visible;
        }
        foreach (var control in new[] { EnginePicker, MultiplierPicker })
            if (control.Parent is FrameworkElement panel) panel.Visibility = opti ? Visibility.Collapsed : Visibility.Visible;
        TimingCheck.Visibility = anr || opti ? Visibility.Collapsed : Visibility.Visible;
        ProxyPicker.IsEnabled = !anr || _anrVariant != "addon";
        if (anr && _anrVariant == "addon")
        {
            var proxy = _record?.ExternalProxy;
            if (proxy is null && _scan?.RenderingDirectory is { } directory)
                proxy = new[] { "dxgi.dll", "winmm.dll", "version.dll" }.FirstOrDefault(p => File.Exists(Path.Combine(directory, p)));
            if (proxy is not null) Select(ProxyPicker, proxy);
        }
    }

    private async void Provider_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (_changingChoices || _operation is not null || ProviderPicker.SelectedItem is not Choice selected) return;
        var provider = selected.Value switch { "Anr" => ModProvider.Anr, "OptiScaler" => ModProvider.OptiScaler, _ => ModProvider.Sm86 };
        if (provider == _provider) return;
        _provider = provider;
        _package = null; _updateState = null; _anrVariant = "full";
        ResetGameAnalysis(false);
        RebuildChoices();
        Select(ProxyPicker, provider == ModProvider.Sm86 ? "version.dll" : "dxgi.dll");
        try { File.WriteAllText(Path.Combine(App.DataDirectory, "provider.txt"), provider.ToString()); }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException) { SaveError(error); }
        await RunAsync(async token =>
        {
            await UpdatePackageAsync(token);
            await InspectHardwareAsync(token);
            if (!string.IsNullOrWhiteSpace(FolderPath.Text)) await AnalyzeAsync(token);
        });
    }

    private void OptiOption_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (_changingChoices || _provider != ModProvider.OptiScaler || OptiOptionsPanel is null) return;
        RenderChecks();
        RefreshButtons();
        RefreshGameSupport();
    }

    private void FillOptiChoices()
    {
        string UpscalerLabel(string value) => value switch
        {
            "auto" => Localization.Get("hud_auto"), "dlss" => "NVIDIA DLSS", "xess" => "Intel XeSS",
            "xess_12" => "Intel XeSS · D3D11on12", "fsr21" => "AMD FSR 2.1", "fsr22" => "AMD FSR 2.2",
            "fsr31" => "AMD FSR 3.1 / 4", "fsr21_12" => "AMD FSR 2.1 · DX12 interop",
            "fsr22_12" => "AMD FSR 2.2 · D3D11on12", "fsr31_12" => "AMD FSR 3.1 / 4 · DX12 interop", _ => value
        };
        Fill(OptiDx11Picker, new[] { "auto", "fsr22", "fsr31", "xess", "xess_12", "fsr21_12", "fsr22_12", "fsr31_12", "dlss" }.Select(v => new Choice(v, v == "fsr31" ? "AMD FSR 3.1" : UpscalerLabel(v))), "auto");
        Fill(OptiDx12Picker, new[] { "auto", "xess", "fsr21", "fsr22", "fsr31", "dlss" }.Select(v => new Choice(v, UpscalerLabel(v))), "auto");
        Fill(OptiVulkanPicker, new[] { "auto", "fsr21", "fsr22", "fsr31", "xess", "fsr21_12", "fsr31_12", "dlss" }.Select(v => new Choice(v, UpscalerLabel(v))), "auto");
        Fill(OptiFgInputPicker, new[] { "nofg", "dlssg", "nukems", "fsrfg", "upscaler", "fsrfg30" }.Select(v => new Choice(v, v switch
        {
            "nofg" => Localization.Get("opti_off"), "dlssg" => "DLSS Frame Generation", "nukems" => "DLSS-G · Nukem",
            "fsrfg" => "FSR Frame Generation", "fsrfg30" => "FSR 3.0 Frame Generation", _ => Localization.Get("opti_upscaler_input")
        })), "nofg");
        Fill(OptiFgOutputPicker, new[] { "nofg", "fsrfg", "xefg", "nukems" }.Select(v => new Choice(v, v switch
        { "nofg" => Localization.Get("opti_off"), "fsrfg" => "AMD FSR Frame Generation", "xefg" => "Intel XeSS Frame Generation", _ => "Nukem · FSR 3" })), "nofg");
        Fill(OptiSpoofingPicker, new[] { "auto", "true", "false" }.Select(v => new Choice(v, Localization.Get(v switch
        { "auto" => "hud_auto", "true" => "opti_on", _ => "opti_off" }))), "auto");
        Fill(OptiLogPicker, new[] { "opti_log_trace", "opti_log_debug", "opti_log_info", "opti_log_warning", "opti_log_error" }
            .Select((key, i) => new Choice(i.ToString(), Localization.Get(key))), "2");
    }
}
