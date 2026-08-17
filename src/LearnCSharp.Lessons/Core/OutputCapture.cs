using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LearnCSharp.Core;

/// <summary>
/// Runs every lesson, captures what it actually prints, and writes it to JSON for the website.
///
/// This is what makes the site trustworthy: no output block on any lesson page is typed by
/// hand. If the code changes, the captured output changes with it, and if a lesson throws,
/// the capture fails rather than shipping a lie.
/// </summary>
public static partial class OutputCapture
{
    /// <summary>Matches the "-- Section title -------" banner that Ui.Section writes.</summary>
    [GeneratedRegex(@"^-- (?<title>.+?) -+\s*$")]
    private static partial Regex SectionBanner();

    /// <summary>
    /// Writes a small syllabus index plus one file per lesson, so the website can load a
    /// lesson's output on demand instead of shipping all 57 in its main bundle.
    /// </summary>
    public static int WriteTo(string folder)
    {
        Dictionary<string, CapturedLesson> captured = new();
        int failures = 0;

        // Colour codes would end up in the JSON, so turn them off for the whole run.
        bool colourBefore = Ui.UseColour;
        Ui.UseColour = false;
        TextWriter consoleBefore = Console.Out;

        try
        {
            foreach (ILesson lesson in LessonRegistry.All)
            {
                StringWriter buffer = new();
                Console.SetOut(buffer);

                string? error = null;
                try
                {
                    lesson.Run();
                }
                catch (Exception ex)
                {
                    error = $"{ex.GetType().Name}: {ex.Message}";
                }

                Console.SetOut(consoleBefore);

                string text = buffer.ToString();

                if (error is not null)
                {
                    failures++;
                    Console.Error.WriteLine($"  FAILED {lesson.Id}: {error}");
                }

                captured[lesson.Id] = new CapturedLesson(
                    lesson.Id,
                    lesson.Title,
                    lesson.Doc,
                    text.TrimEnd(),
                    SplitIntoSections(text),
                    error);
            }
        }
        finally
        {
            Console.SetOut(consoleBefore);
            Ui.UseColour = colourBefore;
        }

        JsonSerializerOptions options = new() { WriteIndented = true };

        string lessonFolder = Path.Combine(folder, "lessons");
        Directory.CreateDirectory(lessonFolder);

        foreach (CapturedLesson lesson in captured.Values)
            File.WriteAllText(
                Path.Combine(lessonFolder, $"{lesson.Id}.json"),
                JsonSerializer.Serialize(lesson, options));

        // The index is deliberately tiny - it is the only file loaded up front.
        List<SyllabusEntry> syllabus = captured.Values
            .Select(l => new SyllabusEntry(
                l.Id,
                l.Title,
                int.Parse(l.Id.Split('.')[0]),
                l.Doc,
                l.Sections.Keys.ToList()))
            .ToList();

        File.WriteAllText(
            Path.Combine(folder, "syllabus.json"),
            JsonSerializer.Serialize(syllabus, options));

        Console.WriteLine($"Captured {captured.Count} lessons -> {folder}");
        if (failures > 0) Console.Error.WriteLine($"{failures} lesson(s) failed - capture is not trustworthy.");

        return failures == 0 ? 0 : 1;
    }

    /// <summary>
    /// Splits a lesson's output on its section banners, so a page can embed just the part
    /// it is talking about instead of the whole lesson.
    /// </summary>
    private static Dictionary<string, string> SplitIntoSections(string text)
    {
        Dictionary<string, string> sections = new();
        string? current = null;
        StringBuilder body = new();

        foreach (string line in text.Split('\n'))
        {
            Match match = SectionBanner().Match(line.TrimEnd());

            if (match.Success)
            {
                if (current is not null) sections[current] = body.ToString().Trim('\n');
                current = match.Groups["title"].Value.Trim();
                body.Clear();
                continue;
            }

            if (current is not null) body.AppendLine(line.TrimEnd());
        }

        if (current is not null) sections[current] = body.ToString().Trim('\n');
        return sections;
    }

    private record SyllabusEntry(string Id, string Title, int Module, string Doc, List<string> Sections);

    private record CapturedLesson(
        string Id,
        string Title,
        string Doc,
        string FullOutput,
        Dictionary<string, string> Sections,
        string? Error);
}
