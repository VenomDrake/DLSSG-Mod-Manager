using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DlssgManager.Core;

namespace DlssgManager.Desktop;

public partial class MainWindow
{
    private sealed record TutorialTarget(Func<string> Section, string Caption);
    private readonly Dictionary<FrameworkElement, TutorialTarget> _tutorialTargets = [];
    private int _tutorialTourStep = -1;
    private AdornerLayer? _tutorialAdornerLayer;
    private TutorialFocusAdorner? _tutorialAdorner;
    private int _tutorialHighlightGeneration;
    private static readonly string[] TourNames = ["start", "game", "checks", "options", "install", "play", "logs", "backup"];
    private static readonly string[] TourSections = ["choose-provider", "first-install", "checks", "sm86", "first-install", "in-game", "verify", "backup"];

    private string ProviderTutorialSection() => _provider switch
    { ModProvider.Anr => "anr", ModProvider.OptiScaler => "optiscaler", _ => "sm86" };

    private void InitializeTutorial()
    {
        TutorialReader.OpenControlsRequested += ShowTutorialControls;
        TutorialReader.StartTourRequested += StartTutorialTour;
        foreach (var (control, key) in new (FrameworkElement, string)[]
        {
            (EnginePicker, "engine"), (MultiplierPicker, "multiplier"), (OptimizationPicker, "optimization"),
            (PresetPicker, "preset"), (SrPicker, "super_resolution"), (FlowPicker, "flow_resolution"),
            (HudPicker, "hudless"), (LogPicker, "log_level"), (ProxyPicker, "proxy"),
            (TimingCheck, "frame_timing"), (AnrPathPicker, "anr_compute_path"),
            (AnrNitsInput, "anr_hdr_luminance"), (AnrSceneCutCheck, "anr_scene_cut"),
            (OptiDx11Picker, "opti_dx11"), (OptiDx12Picker, "opti_dx12"),
            (OptiVulkanPicker, "opti_vulkan"), (OptiFgInputPicker, "opti_fg_input"),
            (OptiFgOutputPicker, "opti_fg_output"), (OptiSpoofingPicker, "opti_spoofing"), (OptiLogPicker, "log_level")
        })
        {
            RegisterTutorialTarget(control, ProviderTutorialSection, key);
            AddSettingHelp(control, key);
        }
        RegisterTutorialTarget(ProviderPicker, () => "choose-provider", "provider_title");
        foreach (var control in new FrameworkElement[] { SteamPicker, FolderButton, ExeButton, FolderPath, AnalyzeButton, ExePicker })
            RegisterTutorialTarget(control, () => "first-install", "ui_choose_step");
        foreach (var control in new FrameworkElement[] { InstallButton, LaunchButton, CancelButton })
            RegisterTutorialTarget(control, () => "first-install", "ui_apply_step");
        RegisterTutorialTarget(UpdateButton, () => "updates", "release_title");
        RegisterTutorialTarget(HardwareButton, () => "checks", "hardware_title");
        RegisterTutorialTarget(ReadLogButton, () => "verify", "read_log");
        RegisterTutorialTarget(DisableButton, () => "backup", "disable");
        RegisterTutorialTarget(RestoreButton, () => "backup", "restore");
        RegisterTutorialTarget(LanguagePicker, () => "screen", "language");
    }

    private void RegisterTutorialTarget(FrameworkElement control, Func<string> section, string caption)
    {
        _tutorialTargets.Add(control, new(section, caption));
        var menu = new ContextMenu();
        var help = new MenuItem();
        help.SetResourceReference(HeaderedItemsControl.HeaderProperty, "tutorial_help");
        help.Click += (_, _) => OpenTutorial(section(), "@setting:" + caption);
        menu.Items.Add(help);
        // Preserve TextBox's standard copy/paste menu; F1 and the adjacent help button remain available.
        if (control is not TextBox) control.ContextMenu ??= menu;
    }

    private Button SettingHelpButton(FrameworkElement target)
    {
        var button = new Button
        {
            Content = "?", Width = 27, MinHeight = 25, Padding = new Thickness(2),
            Margin = new Thickness(6, 0, 0, 5), FontSize = 13,
            Style = (Style)FindResource("QuietButton"), VerticalAlignment = VerticalAlignment.Top
        };
        button.SetResourceReference(ToolTipProperty, "tutorial_help");
        button.SetResourceReference(AutomationProperties.NameProperty, "tutorial_help");
        button.Click += (_, e) =>
        {
            e.Handled = true;
            var help = _tutorialTargets[target];
            OpenTutorial(help.Section(), "@setting:" + help.Caption);
        };
        return button;
    }

    private void AddSettingHelp(FrameworkElement control, string key)
    {
        if (control is CheckBox check)
        {
            var content = new StackPanel { Orientation = Orientation.Horizontal };
            var label = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
            label.SetResourceReference(TextBlock.TextProperty, key);
            content.Children.Add(label);
            content.Children.Add(SettingHelpButton(control));
            check.Content = content;
            check.SetResourceReference(AutomationProperties.NameProperty, key);
            return;
        }
        if (control.Parent is not StackPanel parent) return;
        var index = parent.Children.IndexOf(control);
        if (index <= 0 || parent.Children[index - 1] is not TextBlock caption) return;
        parent.Children.RemoveAt(index - 1);
        var row = new DockPanel { LastChildFill = true };
        var button = SettingHelpButton(control);
        DockPanel.SetDock(button, Dock.Right);
        row.Children.Add(button);
        row.Children.Add(caption);
        parent.Children.Insert(index - 1, row);
    }

    private void OpenTutorial(string section = "start", string? findText = null)
    {
        ClearTutorialHighlight();
        TutorialTab.IsSelected = true;
        TutorialReader.NavigateTo(section, findText);
        RefreshTutorialUi();
    }

    private void Tutorial_Click(object sender, RoutedEventArgs e) => OpenTutorial();

    private void Tutorial_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.F1) return;
        e.Handled = true;
        var focused = Keyboard.FocusedElement as DependencyObject;
        while (focused is not null && focused != this)
        {
            if (focused is FrameworkElement element && _tutorialTargets.TryGetValue(element, out var target))
            { OpenTutorial(target.Section(), "@setting:" + target.Caption); return; }
            focused = focused switch
            {
                FrameworkElement framework => framework.Parent ?? VisualTreeHelper.GetParent(framework),
                FrameworkContentElement content => content.Parent,
                _ => null
            };
        }
        OpenTutorial(SettingsTab.IsSelected ? ProviderTutorialSection() : DiagnosticsTab.IsSelected ? "checks" : "start");
    }

    private void TutorialTab_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source != MainTabs || ActionFooter is null) return;
        ClearTutorialHighlight();
        RefreshTutorialUi();
    }

    private void RefreshTutorialUi()
    {
        if (ActionFooter is null) return;
        ActionFooter.Visibility = TutorialTab.IsSelected && _operation is null ? Visibility.Collapsed : Visibility.Visible;
        if (_tutorialTourStep < 0) return;
        TutorialTourStepText.Text = Localization.Get("tutorial_tour_title") + " · " + string.Format(CultureInfo.CurrentCulture,
            Localization.Get("tutorial_tour_step"), _tutorialTourStep + 1, TourNames.Length);
        var name = TourNames[_tutorialTourStep];
        TutorialTourTitle.Text = Localization.Get("tutorial_tour_" + name + "_title");
        TutorialTourBody.Text = Localization.Get("tutorial_tour_" + name + "_body");
        TutorialTourBackButton.IsEnabled = _tutorialTourStep > 0;
        TutorialTourNextButton.Content = Localization.Get(_tutorialTourStep == TourNames.Length - 1 ? "tutorial_tour_finish" : "tutorial_tour_next");
    }

    private void StartTutorialTour()
    {
        _tutorialTourStep = 0;
        TutorialTourPanel.Visibility = Visibility.Visible;
        ShowTourStep();
    }

    private void ShowTourStep()
    {
        var (tab, target) = _tutorialTourStep switch
        {
            0 => (WorkspaceTab, (FrameworkElement)ProviderPicker),
            1 => (WorkspaceTab, SteamPicker),
            2 => (DiagnosticsTab, CheckCountsText),
            3 => (SettingsTab, RecommendedText),
            4 => (WorkspaceTab, InstallButton),
            5 => (WorkspaceTab, LaunchButton),
            6 => (DiagnosticsTab, ReadLogButton),
            _ => (DiagnosticsTab, RestoreButton)
        };
        tab.IsSelected = true;
        RefreshTutorialUi();
        HighlightTutorialControl(target);
    }

    private void TutorialTourBack_Click(object sender, RoutedEventArgs e)
    { if (_tutorialTourStep > 0) { _tutorialTourStep--; ShowTourStep(); } }
    private void TutorialTourNext_Click(object sender, RoutedEventArgs e)
    {
        if (_tutorialTourStep < 0) return;
        if (_tutorialTourStep == TourNames.Length - 1) { CloseTutorialTour(); return; }
        _tutorialTourStep++;
        ShowTourStep();
    }
    private void TutorialTourClose_Click(object sender, RoutedEventArgs e) => CloseTutorialTour();
    private void TutorialTourHelp_Click(object sender, RoutedEventArgs e)
    { if (_tutorialTourStep >= 0) OpenTutorial(_tutorialTourStep == 3 ? ProviderTutorialSection() : TourSections[_tutorialTourStep]); }
    private void CloseTutorialTour()
    {
        _tutorialTourStep = -1;
        TutorialTourPanel.Visibility = Visibility.Collapsed;
        ClearTutorialHighlight();
    }

    private void ShowTutorialControls(string section)
    {
        FrameworkElement target;
        switch (section)
        {
            case "sm86" or "anr" or "optiscaler":
                // Never switch providers from documentation: that would trigger a package download.
                if (section == ProviderTutorialSection()) { SettingsTab.IsSelected = true; target = RecommendedText; }
                else { WorkspaceTab.IsSelected = true; target = ProviderPicker; }
                break;
            case "checks" or "safety": DiagnosticsTab.IsSelected = true; target = CheckCountsText; break;
            case "verify": DiagnosticsTab.IsSelected = true; target = ReadLogButton; break;
            case "backup": DiagnosticsTab.IsSelected = true; target = RestoreButton; break;
            case "updates": DiagnosticsTab.IsSelected = true; target = UpdateButton; break;
            case "files" or "sources": DiagnosticsTab.IsSelected = true; target = AboutVersionText; break;
            case "choose-provider": WorkspaceTab.IsSelected = true; target = ProviderPicker; break;
            case "in-game": WorkspaceTab.IsSelected = true; target = LaunchButton; break;
            default: WorkspaceTab.IsSelected = true; target = SteamPicker; break;
        }
        RefreshTutorialUi();
        HighlightTutorialControl(target);
    }

    private void HighlightTutorialControl(FrameworkElement target)
    {
        ClearTutorialHighlight();
        var generation = _tutorialHighlightGeneration;
        _ = Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
        {
            if (generation != _tutorialHighlightGeneration || !target.IsVisible) return;
            target.BringIntoView();
            if (target is Control { IsEnabled: true } control) control.Focus();
            _tutorialAdornerLayer = AdornerLayer.GetAdornerLayer(target);
            if (_tutorialAdornerLayer is null) return;
            _tutorialAdorner = new TutorialFocusAdorner(target);
            _tutorialAdornerLayer.Add(_tutorialAdorner);
        }));
    }

    private void ClearTutorialHighlight()
    {
        _tutorialHighlightGeneration++;
        if (_tutorialAdorner is not null) _tutorialAdornerLayer?.Remove(_tutorialAdorner);
        _tutorialAdorner = null;
        _tutorialAdornerLayer = null;
    }

    private sealed class TutorialFocusAdorner : Adorner
    {
        public TutorialFocusAdorner(UIElement target) : base(target) => IsHitTestVisible = false;
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.MediumAquamarine, 2),
                new Rect(-4, -4, AdornedElement.RenderSize.Width + 8, AdornedElement.RenderSize.Height + 8), 9, 9);
        }
    }
}
