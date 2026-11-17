using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DlssgManager.Desktop;

public partial class TutorialView : UserControl
{
    private IReadOnlyList<TutorialSection> _sections = [];
    private string _selectedId = "start";
    private bool _updating;
    private bool _subscribed;
    private string? _contextText;
    private FrameworkContentElement? _contextTarget;
    private bool _contextRowMatched;
    private static readonly HashSet<string> Images = ["screenshot.png", "settings-sm86.png", "settings-anr.png", "settings-optiscaler.png"];
    private static readonly Regex InlinePattern = new(@"(!?\[[^\]]*\]\([^\)]+\)|\*\*.+?\*\*|`[^`]+`)", RegexOptions.CultureInvariant);

    public IReadOnlyList<TutorialSection> Sections => _sections;
    public string SelectedSectionId => _selectedId;
    public event Action<string>? OpenControlsRequested;
    public event Action? StartTourRequested;

    public TutorialView() => InitializeComponent();

    public void NavigateTo(string id, string? findText = null)
    {
        if (_sections.Count == 0) Reload();
        if (!_sections.Any(s => s.Id == id)) return;
        _selectedId = id;
        SearchBox.Text = "";
        _contextText = findText;
        FilterTopics();
        TopicList.ScrollIntoView(TopicList.SelectedItem);
    }

    private void Tutorial_Loaded(object sender, RoutedEventArgs e)
    {
        if (!_subscribed) { Localization.Changed += Reload; _subscribed = true; }
        Reload();
    }

    private void Tutorial_Unloaded(object sender, RoutedEventArgs e)
    {
        if (_subscribed) { Localization.Changed -= Reload; _subscribed = false; }
    }

    private void Reload()
    {
        FlowDirection = Localization.Direction;
        _sections = TutorialContent.Load(Localization.Current);
        FilterTopics();
    }

    private void FilterTopics()
    {
        if (TopicList is null || _updating) return;
        _updating = true;
        var query = SearchBox.Text.Trim();
        var terms = query.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        var compare = CultureInfo.CurrentCulture.CompareInfo;
        var results = _sections.Where(s => terms.All(term => compare.IndexOf(s.Title + "\n" + s.Markdown, term, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0)).ToArray();
        TopicList.ItemsSource = results;
        var selected = results.FirstOrDefault(s => s.Id == _selectedId) ?? results.FirstOrDefault();
        TopicList.SelectedItem = selected;
        ClearSearchButton.IsEnabled = query.Length > 0;
        NoResultsText.Visibility = results.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
        _updating = false;
        ShowTopic(selected);
    }

    private void ShowTopic(TutorialSection? section)
    {
        if (section is null)
        {
            TopicTitle.Text = Localization.Get(_sections.Count == 0 ? "tutorial_unavailable" : "tutorial_no_results");
            TopicCount.Text = "";
            Reader.Document = NewDocument();
            PreviousButton.IsEnabled = NextButton.IsEnabled = OpenControlsButton.IsEnabled = false;
            return;
        }
        _selectedId = section.Id;
        TopicTitle.Text = section.Title;
        TopicCount.Text = $"{section.Index + 1} / {_sections.Count}";
        Reader.Document = Render(section.Markdown);
        Reader.Document.FlowDirection = Localization.Direction;
        PreviousButton.IsEnabled = section.Index > 0;
        NextButton.IsEnabled = section.Index + 1 < _sections.Count;
        OpenControlsButton.IsEnabled = section.Id is not ("safety" or "install" or "in-game" or "glossary" or "sources");
        Reader.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new Action(() =>
        {
            if (_contextTarget is { } target) target.BringIntoView();
            else FindScrollViewer(Reader)?.ScrollToTop();
        }));
    }

    private FlowDocument NewDocument() => new()
    {
        FontFamily = new FontFamily("Segoe UI"), FontWeight = FontWeights.Normal, FontSize = 14, LineHeight = 23,
        Foreground = Brush("Text"), Background = Brushes.Transparent, PagePadding = new Thickness(0, 0, 8, 8),
        ColumnWidth = double.PositiveInfinity, FlowDirection = Localization.Direction,
        TextAlignment = Localization.Direction == FlowDirection.RightToLeft ? TextAlignment.Right : TextAlignment.Left,
        IsOptimalParagraphEnabled = false
    };

    private FlowDocument Render(string markdown)
    {
        _contextTarget = null;
        _contextRowMatched = false;
        var settingTarget = ResolveSettingTarget();
        var tableIndex = 0;
        var document = NewDocument();
        var lines = markdown.Replace("\r", "").Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.Length == 0 || line.StartsWith("<a ", StringComparison.Ordinal)) continue;
            if (line.StartsWith('|'))
            {
                var table = new Table { CellSpacing = 0, Margin = new Thickness(0, 8, 0, 18) };
                var group = new TableRowGroup();
                table.RowGroups.Add(group);
                var rows = new List<string[]>();
                while (i < lines.Length && lines[i].TrimStart().StartsWith('|'))
                {
                    var cells = lines[i].Trim().Trim('|').Split('|').Select(c => c.Trim()).ToArray();
                    if (!cells.All(c => Regex.IsMatch(c, @"^:?-+:?$"))) rows.Add(cells);
                    i++;
                }
                i--;
                var columns = rows.Count == 0 ? 0 : rows.Max(r => r.Length);
                for (var column = 0; column < columns; column++) table.Columns.Add(new TableColumn { Width = new GridLength(column == 0 && columns == 2 ? 1 : columns == 2 ? 2.4 : 1, GridUnitType.Star) });
                for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                {
                    var row = new TableRow { Background = rowIndex == 0 ? Brush("Input") : rowIndex % 2 == 0 ? Brush("PanelRaised") : Brushes.Transparent };
                    foreach (var cell in rows[rowIndex])
                    {
                        var paragraph = Paragraph(cell, new Thickness(0));
                        paragraph.FontSize = 13;
                        paragraph.LineHeight = 21;
                        if (rowIndex == 0) paragraph.FontWeight = FontWeights.SemiBold;
                        row.Cells.Add(new TableCell(paragraph) { Padding = new Thickness(10, 9, 10, 9), BorderBrush = Brush("Line"), BorderThickness = new Thickness(0, 0, 0, 1) });
                    }
                    if ((rowIndex > 0 && MatchesContext(string.Join(" ", rows[rowIndex])))
                        || settingTarget is { } target && target.Table == tableIndex && (target.Row == 0 || target.Row == rowIndex))
                    {
                        row.Background = new SolidColorBrush(Color.FromRgb(29, 64, 53));
                        if (!_contextRowMatched && row.Cells.Count > 0)
                        {
                            _contextTarget = row.Cells[0].Blocks.FirstBlock;
                            _contextRowMatched = true;
                        }
                    }
                    group.Rows.Add(row);
                }
                document.Blocks.Add(table);
                tableIndex++;
                continue;
            }
            var image = Regex.Match(line, @"^!\[([^\]]*)\]\(([^\)]+)\)$");
            if (image.Success)
            {
                var filename = image.Groups[2].Value.Split('/').Last();
                if (Images.Contains(filename))
                {
                    var assembly = typeof(TutorialView).Assembly.GetName().Name;
                    var bitmap = new BitmapImage(new Uri($"pack://application:,,,/{assembly};component/TutorialImages/{filename}", UriKind.Absolute));
                    var picture = new Image { Source = bitmap, Stretch = Stretch.Uniform, MaxHeight = 390, Margin = new Thickness(0, 5, 0, 4), HorizontalAlignment = HorizontalAlignment.Stretch, FlowDirection = FlowDirection.LeftToRight };
                    System.Windows.Automation.AutomationProperties.SetName(picture, image.Groups[1].Value);
                    document.Blocks.Add(new BlockUIContainer(picture) { Margin = new Thickness(0, 5, 0, 8) });
                    var caption = Paragraph(image.Groups[1].Value, new Thickness(0, 0, 0, 16));
                    caption.FontSize = 12; caption.Foreground = Brush("Muted");
                    document.Blocks.Add(caption);
                }
                continue;
            }
            if (line.StartsWith("### ", StringComparison.Ordinal))
            {
                var heading = Paragraph(line[4..], new Thickness(0, 16, 0, 9));
                heading.FontSize = 18; heading.FontWeight = FontWeights.SemiBold; heading.Foreground = Brush("Accent");
                document.Blocks.Add(heading); continue;
            }
            var number = Regex.Match(line, @"^(\d+)\. (.*)$");
            if (number.Success)
            {
                var item = Paragraph(number.Groups[1].Value + ".  " + number.Groups[2].Value, new Thickness(12, 0, 0, 10));
                document.Blocks.Add(item); continue;
            }
            if (line.StartsWith("- ", StringComparison.Ordinal)) line = "•  " + line[2..];
            var body = Paragraph(line, new Thickness(0, 0, 0, 13));
            if (settingTarget is { Table: -1 } && _contextTarget is null)
            {
                body.Background = new SolidColorBrush(Color.FromRgb(29, 64, 53));
                _contextTarget = body;
            }
            document.Blocks.Add(body);
        }
        return document;
    }

    private Paragraph Paragraph(string text, Thickness margin)
    {
        var paragraph = new Paragraph { Margin = margin };
        AddInlines(paragraph.Inlines, text);
        if (MatchesContext(text))
        {
            paragraph.Background = new SolidColorBrush(Color.FromRgb(29, 64, 53));
            _contextTarget ??= paragraph;
        }
        return paragraph;
    }

    private bool MatchesContext(string text) => !string.IsNullOrWhiteSpace(_contextText)
        && !_contextText.StartsWith("@setting:", StringComparison.Ordinal)
        && CultureInfo.CurrentCulture.CompareInfo.IndexOf(text.Replace("**", "").Replace("`", ""), _contextText.Trim(), CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;

    private readonly record struct SettingTarget(int Table, int Row);

    // The reviewed translations share the same table/row structure. Stable keys avoid depending
    // on a control caption having exactly the same wording as the prose in every language.
    private SettingTarget? ResolveSettingTarget()
    {
        if (_contextText is null || !_contextText.StartsWith("@setting:", StringComparison.Ordinal)) return null;
        return (_selectedId, _contextText[9..]) switch
        {
            ("sm86", "engine") => new(0, 1),
            ("sm86", "multiplier") => new(0, 2),
            ("sm86", "optimization") => new(0, 3),
            ("sm86", "preset") => new(0, 4),
            ("sm86", "super_resolution") => new(0, 5),
            ("sm86", "flow_resolution") => new(0, 6),
            ("sm86", "hudless") => new(0, 7),
            ("sm86", "log_level") => new(0, 8),
            ("sm86", "frame_timing") => new(0, 9),
            ("sm86", "proxy") => new(0, 10),
            ("anr", "engine") => new(-1, 0),
            ("anr", "multiplier") => new(0, 2),
            ("anr", "anr_compute_path") => new(0, 3),
            ("anr", "anr_hdr_luminance") => new(0, 4),
            ("anr", "anr_scene_cut") => new(0, 5),
            ("anr", "proxy") => new(0, 6),
            ("optiscaler", "opti_dx11") => new(0, 1),
            ("optiscaler", "opti_dx12") => new(0, 2),
            ("optiscaler", "opti_vulkan") => new(0, 3),
            ("optiscaler", "opti_fg_input") => new(1, 0),
            ("optiscaler", "opti_fg_output") => new(2, 0),
            ("optiscaler", "opti_spoofing") => new(3, 1),
            ("optiscaler", "log_level") => new(3, 2),
            ("optiscaler", "proxy") => new(3, 3),
            _ => null
        };
    }

    private void AddInlines(InlineCollection collection, string text)
    {
        var end = 0;
        foreach (Match match in InlinePattern.Matches(text))
        {
            if (match.Index > end) collection.Add(new Run(text[end..match.Index]));
            var token = match.Value;
            if (token.StartsWith("**", StringComparison.Ordinal))
            {
                var bold = new Bold(); AddInlines(bold.Inlines, token[2..^2]); collection.Add(bold);
            }
            else if (token.StartsWith('`'))
            {
                collection.Add(new Run(token[1..^1]) { FontFamily = new FontFamily("Consolas"), FontSize = 12.5, Foreground = Brush("Accent"), FlowDirection = FlowDirection.LeftToRight });
            }
            else
            {
                var link = Regex.Match(token, @"^!?\[([^\]]*)\]\(([^\)]+)\)$");
                var destination = link.Groups[2].Value;
                var hyperlink = new Hyperlink { Foreground = Brush("Accent"), Cursor = Cursors.Hand, Tag = destination };
                AddInlines(hyperlink.Inlines, link.Groups[1].Value);
                hyperlink.ToolTip = destination.StartsWith('#') ? link.Groups[1].Value : destination;
                hyperlink.Click += (_, e) => { e.Handled = true; ActivateLink(destination); };
                collection.Add(hyperlink);
            }
            end = match.Index + match.Length;
        }
        if (end < text.Length) collection.Add(new Run(text[end..]));
    }

    private void ActivateLink(string destination)
    {
        if (destination.StartsWith('#')) { NavigateTo(destination[1..]); return; }
        var uri = ResolveExternalLink(destination);
        if (uri is null) return;
        try { Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true }); }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException) { /* The reader stays usable without a browser. */ }
    }

    internal static Uri? ResolveExternalLink(string destination)
    {
        // Local documentation references have fixed public equivalents; arbitrary file/command URLs are never opened.
        if (!Uri.TryCreate(destination, UriKind.Absolute, out _))
        {
            var filename = destination.Split('/').Last();
            if (filename == "THIRD_PARTY_NOTICES.md") destination = "https://github.com/VenomDrake/DLSSG-Mod-Manager/blob/main/THIRD_PARTY_NOTICES.md";
            else if (filename == "README.md") destination = "https://github.com/VenomDrake/DLSSG-Mod-Manager";
            else
            {
                var localizedReadme = Regex.Match(filename, @"^README\.([A-Za-z-]+)\.md$");
                if (localizedReadme.Success && Localization.Languages.Any(l => l.Code == localizedReadme.Groups[1].Value))
                    destination = "https://github.com/VenomDrake/DLSSG-Mod-Manager/blob/main/docs/i18n/" + filename;
                else return null;
            }
        }
        return Uri.TryCreate(destination, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps && uri.IsWellFormedOriginalString() ? uri : null;
    }

    private Brush Brush(string key) => (Brush)FindResource(key);
    private static ScrollViewer? FindScrollViewer(DependencyObject root)
    {
        if (root is ScrollViewer viewer) return viewer;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++) if (FindScrollViewer(VisualTreeHelper.GetChild(root, i)) is { } child) return child;
        return null;
    }
    private void Search_Changed(object sender, TextChangedEventArgs e) { _contextText = SearchBox.Text.Trim(); FilterTopics(); }
    private void ClearSearch_Click(object sender, RoutedEventArgs e) { SearchBox.Clear(); SearchBox.Focus(); }
    private void Topic_Changed(object sender, SelectionChangedEventArgs e) { if (!_updating) { _contextText = SearchBox.Text.Trim(); ShowTopic(TopicList.SelectedItem as TutorialSection); } }
    private void Previous_Click(object sender, RoutedEventArgs e) => Move(-1);
    private void Next_Click(object sender, RoutedEventArgs e) => Move(1);
    private void Move(int delta)
    {
        var current = _sections.FirstOrDefault(s => s.Id == _selectedId);
        if (current is not null && current.Index + delta >= 0 && current.Index + delta < _sections.Count) NavigateTo(_sections[current.Index + delta].Id);
    }
    private void OpenControls_Click(object sender, RoutedEventArgs e) => OpenControlsRequested?.Invoke(_selectedId);
    private void StartTour_Click(object sender, RoutedEventArgs e) => StartTourRequested?.Invoke();
    private void Tutorial_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control) { SearchBox.Focus(); SearchBox.SelectAll(); e.Handled = true; }
        else if (e.Key == Key.Escape && SearchBox.IsKeyboardFocusWithin) { SearchBox.Clear(); e.Handled = true; }
    }
}
