using System.Globalization;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Foundations;

// Notes: docs/module-1/1.3.md
public sealed class L03_InputOutput : LessonBase
{
    public override string Id => "1.3";
    public override string Title => "Console input, output and formatting";

    public override string Summary =>
        "Everything a user types arrives as text. Format output neatly, and convert input "
        + "into numbers without ever crashing.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Format numbers, currency and aligned tables",
        "Convert text to numbers safely with TryParse",
        "Write an input loop that survives any input",
    ];

    public override void Run()
    {
        Section("String interpolation");

        string name = "Ada";
        int age = 36;

        Line("concatenation:  " + "Name: " + name + ", age: " + age);
        Line($"interpolation:  Name: {name}, age: {age}");     // the $ prefix - prefer this

        Section("Format specifiers");

        double price = 1234.5678;
        int quantity = 42;

        Out("F2  fixed 2 decimals", price.ToString("F2", CultureInfo.InvariantCulture));
        Out("N2  thousands separator", price.ToString("N2", CultureInfo.InvariantCulture));
        Out("C   currency", price.ToString("C", CultureInfo.GetCultureInfo("en-GB")));
        Out("D6  zero padded", quantity.ToString("D6"));
        Out("X   hexadecimal", quantity.ToString("X"));
        Out("P1  percentage", 0.734.ToString("P1", CultureInfo.InvariantCulture));

        Section("Alignment: {value,width}");

        Line($"|{"Item",-12}|{"Qty",5}|{"Price",10}|");
        Line($"|{new string('-', 12)}|{new string('-', 5)}|{new string('-', 10)}|");
        Line($"|{"Widget",-12}|{3,5}|{9.99,10:F2}|");
        Line($"|{"Gizmo",-12}|{12,5}|{124.5,10:F2}|");

        Section("Reading input");

        // The real thing, commented out so the course can run unattended:
        //
        //     Console.Write("Your name? ");
        //     string? typed = Console.ReadLine();      // ALWAYS a string, and may be null
        //     string safe = typed ?? "stranger";
        //
        // ReadLine never gives you a number, so you must convert.

        Section("Three ways to turn text into a number");

        Out("int.Parse(\"42\")", int.Parse("42"));
        Out("Convert.ToInt32(\"42\")", Convert.ToInt32("42"));

        // TryParse is the safe one: it returns true/false instead of throwing.
        if (int.TryParse("42", out int parsed))
            Out("int.TryParse(\"42\") -> true, value", parsed);

        if (!int.TryParse("forty-two", out int failed))
            Out("int.TryParse(\"forty-two\") -> false, value", failed);

        try { int.Parse("forty-two"); }
        catch (FormatException) { Out("int.Parse(\"forty-two\")", "throws FormatException"); }

        Section("Validating input - the pattern you will reuse forever");

        // The real loop keeps asking until the value is good:
        //
        //     while (true)
        //     {
        //         Console.Write("Pick 1-10: ");
        //         if (int.TryParse(Console.ReadLine(), out int v) && v is >= 1 and <= 10)
        //             return v;
        //         Console.WriteLine("Try again.");
        //     }
        //
        // Here we feed it pretend keystrokes instead:

        foreach (string typed in new[] { "abc", "999", "-4", "7" })
        {
            bool accepted = int.TryParse(typed, out int value) && value is >= 1 and <= 10;
            Out($"user typed \"{typed}\"", accepted ? $"accepted ({value})" : "rejected, ask again");
        }

        Section("Parsing other types");

        Out("double.TryParse(\"3.14\")",
            double.TryParse("3.14", CultureInfo.InvariantCulture, out double d) ? d : 0);
        Out("bool.TryParse(\"true\")", bool.TryParse("true", out bool flag) && flag);
        Out("DateTime.TryParse(\"2026-08-16\")",
            DateTime.TryParse("2026-08-16", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime when)
                ? when.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "failed");

        Note("Console.ReadKey(intercept: true) reads one key without waiting for Enter - handy for "
           + "menus. It is not used here so the whole course can run without a human present.");
    }
}
