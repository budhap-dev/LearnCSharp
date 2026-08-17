using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Foundations;

// Notes: docs/module-1/1.5.md
public sealed class L05_Selection : LessonBase
{
    public override string Id => "1.5";
    public override string Title => "Selection: if, else, switch, pattern matching";

    public override void Run()
    {
        Section("if / else if / else");

        foreach (int score in new[] { 85, 62, 31 })
            Out($"score {score}", Grade(score));

        Section("switch statement");

        foreach (int day in new[] { 1, 5, 6, 9 })
            Out($"day {day}", DayNameStatement(day));

        Section("switch EXPRESSION - the modern form");

        foreach (int day in new[] { 1, 5, 7, 9 })
            Out($"day {day}", DayNameExpression(day));

        Section("Range patterns");

        foreach (int temp in new[] { -5, 4, 15, 24, 35 })
            Out($"{temp} degrees", TemperatureBand(temp));

        Section("Type patterns");

        object?[] things = [42, "hello", 3.14, true, new[] { 1, 2, 3 }, 500, null];
        foreach (object? thing in things)
            Out($"Describe({thing ?? "null"})", Describe(thing));

        Section("The 'is' operator in an if");

        object boxed = 42;
        if (boxed is int number && number > 10)
            Out("boxed is int number && number > 10", $"true, number = {number}");

        Out("boxed is not string", boxed is not string);
        Out("boxed is > 40 and < 50", boxed is > 40 and < 50);

        Warn("Only the FIRST matching branch runs. Put the most specific condition first, or a "
           + "score of 85 will match 'score > 50' and print the wrong grade.");
    }

    // Classic if ladder. Order matters: 85 hits the first test and stops.
    private static string Grade(int score)
    {
        if (score > 70) return "A";
        else if (score > 50) return "B";
        else return "C";
    }

    // Each case needs break or return - C# will not fall through by accident.
    private static string DayNameStatement(int day)
    {
        switch (day)
        {
            case 1: return "Monday";
            case 5: return "Friday";
            case 6:
            case 7: return "Weekend";       // two labels, one body: allowed
            default: return "Not a day";
        }
    }

    // No case, no break: an arrow per arm, and _ for "anything else".
    private static string DayNameExpression(int day) => day switch
    {
        1 => "Monday",
        5 => "Friday",
        6 or 7 => "Weekend",
        _ => "Not a day",
    };

    private static string TemperatureBand(int temperature) => temperature switch
    {
        < 0 => "Freezing",
        >= 0 and < 10 => "Cold",
        >= 10 and < 20 => "Mild",
        >= 20 and < 30 => "Warm",
        _ => "Hot",
    };

    private static string Describe(object? value) => value switch
    {
        null => "nothing at all",
        int i when i > 100 => $"a big int: {i}",     // 'when' adds an extra condition
        int i => $"an int: {i}",
        string s => $"a string of {s.Length} chars",
        double d => $"a double: {d}",
        bool b => $"a bool: {b}",
        Array a => $"an array of {a.Length} items",
        _ => "something else",
    };
}
