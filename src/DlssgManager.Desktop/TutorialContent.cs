using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DlssgManager.Desktop;

public sealed record TutorialSection(string Id, string Title, string Markdown, int Index)
{
    public override string ToString() => Title;
}

/// <summary>The same reviewed, translated guide used by the project documentation, bundled for offline use.</summary>
public static partial class TutorialContent
{
    public static IReadOnlyList<TutorialSection> Load(string language)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var name = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith($"Guide.{language}.md", StringComparison.Ordinal) || n.EndsWith($"USER_GUIDE.{language}.md", StringComparison.Ordinal))
            ?? assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith("Guide.en.md", StringComparison.Ordinal));
        if (name is null) return [];
        using var stream = assembly.GetManifestResourceStream(name);
        if (stream is null) return [];
        using var reader = new StreamReader(stream);
        return Parse(reader.ReadToEnd());
    }

    public static IReadOnlyList<TutorialSection> Parse(string markdown)
    {
        var matches = SectionStart().Matches(markdown);
        var sections = new List<TutorialSection>(matches.Count);
        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var bodyStart = match.Index + match.Length;
            var bodyEnd = i + 1 < matches.Count ? matches[i + 1].Index : markdown.Length;
            sections.Add(new(match.Groups[1].Value, match.Groups[2].Value.Trim(), markdown[bodyStart..bodyEnd].Trim(), i));
        }
        return sections;
    }

    [GeneratedRegex("<a id=\"([a-z0-9-]+)\"></a>\\s*\\r?\\n## ([^\\r\\n]+)\\r?\\n", RegexOptions.CultureInvariant)]
    private static partial Regex SectionStart();
}
