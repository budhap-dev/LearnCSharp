namespace LearnCSharp.Core;

/// <summary>
/// Tiny console formatting helper so every lesson looks the same.
/// Colours are switched off automatically when output is redirected to a file
/// (otherwise you get escape-code soup in the file).
/// </summary>
public static class Ui
{
    public static bool UseColour { get; set; } = !Console.IsOutputRedirected;

    private static void With(ConsoleColor colour, string text)
    {
        if (!UseColour) { Console.WriteLine(text); return; }
        ConsoleColor before = Console.ForegroundColor;
        Console.ForegroundColor = colour;
        Console.WriteLine(text);
        Console.ForegroundColor = before;
    }

    /// <summary>Big banner used at the top of a lesson.</summary>
    public static void Title(string id, string title, string? doc = null)
    {
        string line = new string('=', Math.Max(20, title.Length + id.Length + 10));
        Console.WriteLine();
        With(ConsoleColor.Cyan, line);
        With(ConsoleColor.Cyan, $"  LESSON {id}  -  {title}");
        With(ConsoleColor.Cyan, line);
        if (doc is not null) With(ConsoleColor.DarkGray, $"  notes: {doc}");
    }

    /// <summary>Heading inside a lesson.</summary>
    public static void Section(string title)
    {
        Console.WriteLine();
        With(ConsoleColor.Yellow, $"-- {title} " + new string('-', Math.Max(3, 60 - title.Length)));
    }

    /// <summary>Shows a computed value: "  label -> value".</summary>
    public static void Out(string label, object? value)
    {
        string shown = value switch
        {
            null => "null",
            string s => $"\"{s}\"",
            bool b => b ? "true" : "false",
            _ => value.ToString() ?? "null",
        };
        if (!UseColour) { Console.WriteLine($"  {label,-38} -> {shown}"); return; }
        Console.Write($"  {label,-38} -> ");
        ConsoleColor before = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(shown);
        Console.ForegroundColor = before;
    }

    public static void Note(string text) => With(ConsoleColor.Blue, "  [note] " + text);
    public static void Warn(string text) => With(ConsoleColor.Red, "  [careful] " + text);
}
