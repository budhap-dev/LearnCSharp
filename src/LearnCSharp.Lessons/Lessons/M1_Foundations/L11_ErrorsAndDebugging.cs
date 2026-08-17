using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Foundations;

// Notes: docs/module-1/1.11.md
public sealed class L11_ErrorsAndDebugging : LessonBase
{
    public override string Id => "1.11";
    public override string Title => "Errors, exceptions and debugging";

    public override string Summary =>
        "The three kinds of error from the GCSE specification - syntax, runtime and logic - "
        + "what to do about each, and how to read a stack trace properly.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Distinguish syntax, runtime and logic errors",
        "Handle exceptions with try, catch and finally",
        "Read a stack trace and debug systematically",
    ];

    public override void Run()
    {
        Section("A logic error: no crash, just a wrong answer");

        int total = 23, count = 3;
        Out("total / count      (int division)", total / count);
        Out("(double)total / count  (correct)", Math.Round((double)total / count, 4));
        Warn("Nothing was thrown and nothing was flagged. Only testing finds logic errors.");

        Section("try / catch - handle the specific exception");

        foreach (string input in new[] { "42", "abc", "99999999999999999999" })
        {
            try
            {
                Out($"int.Parse(\"{input}\")", int.Parse(input));
            }
            catch (FormatException)
            {
                Out($"int.Parse(\"{input}\")", "FormatException - not a number");
            }
            catch (OverflowException)
            {
                Out($"int.Parse(\"{input}\")", "OverflowException - too big for int");
            }
        }

        Section("The exceptions you will actually meet");

        Show("null.Length", () => { string? s = null; return s!.Length; });
        Show("array[99]", () => { int[] a = [1, 2, 3]; return a[99]; });
        Show("10 / 0", () => { int zero = 0; return 10 / zero; });
        Show("int.Parse(\"abc\")", () => int.Parse("abc"));
        Show("(string)(object)42", () => ((string)(object)42).Length);

        Section("finally always runs");

        Out("DoWork()", DoWork());

        Section("Reading a stack trace");

        try
        {
            LevelOne();                         // LevelOne -> LevelTwo -> LevelThree -> throw
        }
        catch (InvalidOperationException ex)
        {
            Line();
            Line($"{ex.GetType().Name}: {ex.Message}");
            foreach (string frame in (ex.StackTrace ?? "").Split('\n').Take(4))
                Line(frame.Trim());
        }

        Note("Read a trace from the TOP: the first line is where it broke, each line below is the "
           + "caller. Find the first line naming YOUR file - the bug is almost always there.");

        Section("Throwing your own");

        try { SetAge(-5); }
        catch (ArgumentOutOfRangeException ex) { Out("SetAge(-5)", ex.Message.Split('(')[0].Trim()); }

        Section("Exceptions are slow - do not use them for validation");

        int[] inputs = Enumerable.Range(0, 20000).ToArray();
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

        int okA = 0;
        foreach (int i in inputs) { try { int.Parse("abc"); } catch (FormatException) { okA++; } }
        long exceptionMs = watch.ElapsedMilliseconds;

        watch.Restart();
        int okB = 0;
        foreach (int i in inputs) { if (!int.TryParse("abc", out _)) okB++; }

        Out("20,000 bad inputs via try/catch", $"{exceptionMs} ms");
        Out("20,000 bad inputs via TryParse", $"{watch.ElapsedMilliseconds} ms");
    }

    private static void Show(string what, Func<int> action)
    {
        try { Out(what, action()); }
        catch (Exception ex) { Out(what, ex.GetType().Name); }
    }

    private static string DoWork()
    {
        try { return "returned from try"; }
        finally { /* still runs, even though we already returned */ }
    }

    private static void LevelOne() => LevelTwo();
    private static void LevelTwo() => LevelThree();
    private static void LevelThree() => throw new InvalidOperationException("Something broke deep down.");

    // nameof(age) becomes the string "age" - rename the parameter and the message follows.
    private static void SetAge(int age)
    {
        if (age < 0) throw new ArgumentOutOfRangeException(nameof(age), "Age cannot be negative.");
    }
}
