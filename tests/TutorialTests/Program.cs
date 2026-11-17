using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DlssgManager.Core;
using DlssgManager.Desktop;
using Localization = DlssgManager.Desktop.Localization;

internal static class Program
{
    private static int _checks;
    private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
    private static readonly string[] Ids = ["start", "safety", "install", "choose-provider", "screen", "first-install", "checks", "sm86", "anr", "optiscaler", "in-game", "verify", "updates", "backup", "troubleshooting", "files", "glossary", "sources"];
    private static readonly (string Section, string Key)[] Settings = [
        ("sm86", "engine"), ("sm86", "multiplier"), ("sm86", "optimization"), ("sm86", "preset"), ("sm86", "super_resolution"), ("sm86", "flow_resolution"), ("sm86", "hudless"), ("sm86", "log_level"), ("sm86", "frame_timing"), ("sm86", "proxy"),
        ("anr", "engine"), ("anr", "multiplier"), ("anr", "anr_compute_path"), ("anr", "anr_hdr_luminance"), ("anr", "anr_scene_cut"), ("anr", "proxy"),
        ("optiscaler", "opti_dx11"), ("optiscaler", "opti_dx12"), ("optiscaler", "opti_vulkan"), ("optiscaler", "opti_fg_input"), ("optiscaler", "opti_fg_output"), ("optiscaler", "opti_spoofing"), ("optiscaler", "log_level"), ("optiscaler", "proxy")];

    [STAThread]
    private static void Main()
    {
        var testParent = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "DLSSG-TutorialTests"));
        var data = Path.Combine(testParent, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(data);
        try
        {
            // Deliberately do not call App.Run, OnStartup, Window.Show or MainWindow.Loaded:
            // these tests never discover hardware/games, use a network or modify an installation.
            var app = new App { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            app.InitializeComponent();
            typeof(App).GetProperty(nameof(App.DataDirectory))!.SetValue(null, data);
            typeof(App).GetProperty(nameof(App.Arguments))!.SetValue(null, new[] { "--read-only", "--provider", "optiscaler" });
            Localization.Initialize(data);
            Localization.Change("en", false);
            TestReader();
            TestManager();
            Console.WriteLine($"PASS tutorial: {_checks} checks; 11 embedded languages, 18 topics, setting help, search, links, images and navigation-only tour.");
        }
        finally
        {
            var resolved = Path.GetFullPath(data);
            if (resolved.StartsWith(testParent + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) Directory.Delete(resolved, true);
        }
    }

    private static void TestReader()
    {
        Require(typeof(TutorialView).Assembly.GetManifestResourceNames().Count(n => n.StartsWith("Guide.", StringComparison.Ordinal)) == 11, "all guides must be in the primary assembly, not inferred satellite cultures");
        var view = new TutorialView();
        view.NavigateTo("sm86", "@setting:multiplier");
        Require(Context(view) != null, "setting help works before first Loaded");
        view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent, view));
        Require(Context(view) != null, "setting help survives Loaded");
        var images = 0;
        foreach (var language in Localization.Languages)
        {
            Localization.Change(language.Code, false);
            view.NavigateTo("start");
            Require(view.Sections.Select(s => s.Id).SequenceEqual(Ids), language.Code + " section IDs, including numeric sm86");
            Require(view.FlowDirection == Localization.Direction, language.Code + " reading direction");
            if (language.Code != "en") Require(view.Sections[0].Title != TutorialContent.Load("en")[0].Title, language.Code + " must load its translation rather than silently fall back");
            foreach (var section in view.Sections.ToArray())
            {
                view.NavigateTo(section.Id);
                var document = Element<FlowDocumentScrollViewer>(view, "Reader").Document;
                var text = new TextRange(document.ContentStart, document.ContentEnd).Text;
                Require(text.Length > 30, language.Code + "/" + section.Id + " full rendered topic");
                foreach (Match code in Regex.Matches(section.Markdown, "`([^`]+)`")) Require(text.Contains(code.Groups[1].Value, StringComparison.Ordinal), "retained code/setting token " + code.Groups[1].Value);
                var rows = section.Markdown.Split('\n').Select(l => l.Trim()).Count(l => l.StartsWith('|') && !l.Trim('|').Split('|').All(c => Regex.IsMatch(c.Trim(), @"^:?-+:?$")));
                Require(Descendants(document).OfType<TableRow>().Count() == rows, "retained complete option tables");
                foreach (var image in Descendants(document).OfType<Image>())
                {
                    Require(image.Source is BitmapSource bitmap && bitmap.PixelWidth > 0 && bitmap.PixelHeight > 0, "offline screenshot resolves");
                    images++;
                }
            }
            foreach (var (section, key) in Settings)
            {
                view.NavigateTo(section, "@setting:" + key);
                Require(Context(view) != null, language.Code + " exact setting help " + section + "/" + key);
            }
            var search = Element<TextBox>(view, "SearchBox");
            var topics = Element<ListBox>(view, "TopicList");
            search.Text = "winhttp.dll";
            Require(topics.Items.Count is > 0 and < 18, "search must inspect body and filter topics");
            search.Text = "zzzz-no-matching-topic-19286";
            Require(topics.Items.Count == 0 && !Element<Button>(view, "NextButton").IsEnabled, "no-results state");
            Click(Element<Button>(view, "ClearSearchButton"));
            Require(topics.Items.Count == 18 && search.Text.Length == 0, "clear search restores topics");
            view.NavigateTo("start");
            Require(!Element<Button>(view, "PreviousButton").IsEnabled, "first-topic boundary");
            Click(Element<Button>(view, "NextButton"));
            Require(view.SelectedSectionId == "safety", "next topic");
            Click(Element<Button>(view, "PreviousButton"));
            Require(view.SelectedSectionId == "start", "previous topic");
            var link = Descendants(Element<FlowDocumentScrollViewer>(view, "Reader").Document).OfType<Hyperlink>().First(l => l.Tag is string target && target.StartsWith('#'));
            var destination = ((string)link.Tag)[1..];
            link.RaiseEvent(new RoutedEventArgs(Hyperlink.ClickEvent));
            Require(view.SelectedSectionId == destination, "internal hyperlink actually navigates");
            view.NavigateTo("sources");
            Require(!Element<Button>(view, "NextButton").IsEnabled, "last-topic boundary");
        }
        Require(images == 44, "all four offline screenshots in every language");
        Require(TutorialContent.Load("unsupported")[0].Title == TutorialContent.Load("en")[0].Title, "unknown language fallback");
        var resolver = typeof(TutorialView).GetMethod("ResolveExternalLink", BindingFlags.Static | BindingFlags.NonPublic)!;
        foreach (var unsafeLink in new[] { "file:///C:/Windows/System32/cmd.exe", "javascript:alert(1)", "http://example.com", "../unsafe.exe", "ms-settings:display", "https://example.com/\ninvalid" })
            Require(resolver.Invoke(null, new object[] { unsafeLink }) == null, "reject unsafe/non-HTTPS link " + unsafeLink);
        Require(resolver.Invoke(null, new object[] { "../THIRD_PARTY_NOTICES.md" }) is string or Uri, "known local notices map to public HTTPS documentation");
        view.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent, view));
    }

    private static void TestManager()
    {
        Localization.Change("it", false);
        var window = new MainWindow();
        var guide = Element<TutorialView>(window, "TutorialReader");
        var guideTab = Element<TabItem>(window, "TutorialTab");
        var provider = typeof(MainWindow).GetField("_provider", PrivateInstance)!;
        guideTab.IsSelected = true;
        Require(Element<FrameworkElement>(window, "ActionFooter").Visibility == Visibility.Collapsed, "tutorial hides unrelated install footer");
        guide.NavigateTo("sm86");
        Click(Element<Button>(guide, "OpenControlsButton"));
        Require(Element<TabItem>(window, "WorkspaceTab").IsSelected && (ModProvider)provider.GetValue(window)! == ModProvider.OptiScaler, "different provider chapter navigates to selector without changing provider/downloading");
        guide.NavigateTo("optiscaler");
        Click(Element<Button>(guide, "OpenControlsButton"));
        Require(Element<TabItem>(window, "SettingsTab").IsSelected, "current-provider chapter opens settings");
        var dx12 = Element<ComboBox>(window, "OptiDx12Picker");
        ((MenuItem)dx12.ContextMenu.Items[0]).RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
        Require(guideTab.IsSelected && guide.SelectedSectionId == "optiscaler" && Context(guide) != null, "setting context menu opens specific explanation");
        foreach (var (name, choice) in new[] { ("TimingCheck", ModProvider.Sm86), ("AnrSceneCutCheck", ModProvider.Anr) })
        {
            provider.SetValue(window, choice);
            var check = Element<CheckBox>(window, name);
            var before = check.IsChecked;
            var help = Descendants(check).OfType<Button>().Single();
            Click(help);
            Require(check.IsChecked == before, "checkbox help must not toggle " + name);
            Require(Context(guide) != null, "checkbox help points to an explanation");
        }
        provider.SetValue(window, ModProvider.OptiScaler);
        Element<TabItem>(window, "SettingsTab").IsSelected = true;
        var key = new KeyEventArgs(Keyboard.PrimaryDevice, new TestPresentationSource(), 0, Key.F1) { RoutedEvent = Keyboard.PreviewKeyDownEvent };
        typeof(MainWindow).GetMethod("Tutorial_KeyDown", PrivateInstance)!.Invoke(window, new object[] { window, key });
        Require(key.Handled && guideTab.IsSelected && guide.SelectedSectionId == "optiscaler", "F1 contextual help");
        Click(Element<Button>(guide, "StartTourButton"));
        var step = typeof(MainWindow).GetField("_tutorialTourStep", PrivateInstance)!;
        var tabs = new[] { "WorkspaceTab", "WorkspaceTab", "DiagnosticsTab", "SettingsTab", "WorkspaceTab", "WorkspaceTab", "DiagnosticsTab", "DiagnosticsTab" };
        for (var index = 0; index < 8; index++)
        {
            Require((int)step.GetValue(window)! == index && Element<TabItem>(window, tabs[index]).IsSelected, "guided tour route " + index);
            foreach (var field in new[] { "_package", "_scan", "_record", "_operation" }) Require(typeof(MainWindow).GetField(field, PrivateInstance)!.GetValue(window) == null, "tour must not start work " + field);
            Require(!Element<Button>(window, "InstallButton").IsEnabled && !Element<Button>(window, "LaunchButton").IsEnabled, "tour preserves read-only guards");
            Click(Element<Button>(window, "TutorialTourNextButton"));
        }
        Require((int)step.GetValue(window)! == -1 && Element<FrameworkElement>(window, "TutorialTourPanel").Visibility == Visibility.Collapsed, "finish closes tour");
        typeof(MainWindow).GetMethod("Window_Closing", PrivateInstance)!.Invoke(window, new object[] { window, new System.ComponentModel.CancelEventArgs() });
    }

    private static T Element<T>(FrameworkElement root, string name) where T : class => (T)root.FindName(name);
    private static object? Context(TutorialView view) => typeof(TutorialView).GetField("_contextTarget", PrivateInstance)!.GetValue(view);
    private static void Click(Button button) => button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
    private static IEnumerable<DependencyObject> Descendants(DependencyObject root)
    {
        yield return root;
        foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>()) foreach (var descendant in Descendants(child)) yield return descendant;
    }
    private static void Require(bool value, string name) { if (!value) throw new InvalidOperationException("Tutorial regression: " + name); _checks++; }
    private sealed class TestPresentationSource : PresentationSource
    {
        public override Visual RootVisual { get; set; } = null!;
        public override bool IsDisposed => false;
        protected override CompositionTarget GetCompositionTargetCore() => null!;
    }
}
