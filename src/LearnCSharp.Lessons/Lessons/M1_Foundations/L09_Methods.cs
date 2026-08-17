using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Foundations;

// Notes: docs/module-1/1.9.md
public sealed class L09_Methods : LessonBase
{
    public override string Id => "1.9";
    public override string Title => "Methods: parameters, return values, overloading";

    public override string Summary =>
        "Breaking a problem into named, reusable pieces - the GCSE subroutine topic, with "
        + "C#'s parameters, overloads, ref, out and params.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Write methods with parameters and return values",
        "Explain the difference between passing a value and a reference",
        "Use overloading, optional arguments, ref, out and params",
    ];

    public override void Run()
    {
        Section("A method that returns a value");

        Out("AreaOfCircle(3)", AreaOfCircle(3).ToString("F4"));
        Out("AreaOfCircle(1)", AreaOfCircle(1).ToString("F4"));

        Section("A void method just does a job");

        Line();
        PrintBanner("MENU");

        Section("Value types are passed as a COPY");

        int number = 10;
        TryToDouble(number);
        Out("number after TryToDouble(number)", number);

        Section("Reference types share the same data");

        int[] values = [1, 2, 3];
        ModifyContents(values);                  // changes what the caller can see
        Out("after ModifyContents", string.Join(", ", values));

        ReplaceEntirely(values);                 // only rebinds the local copy of the arrow
        Out("after ReplaceEntirely", string.Join(", ", values));

        Section("ref - pass the variable itself");

        int a = 1, b = 2;
        Swap(ref a, ref b);
        Out("after Swap(ref a, ref b)", $"a = {a}, b = {b}");

        Section("out - hand back more than one value");

        if (TryDivide(17, 5, out int whole, out int remainder))
            Out("TryDivide(17, 5)", $"{whole} remainder {remainder}");

        if (!TryDivide(17, 0, out _, out _))     // _ discards a value you do not need
            Out("TryDivide(17, 0)", "returned false");

        Section("Optional and named arguments");

        Out("Greet(\"Ada\")", Greet("Ada"));
        Out("Greet(\"Ada\", \"Hi\")", Greet("Ada", "Hi"));
        Out("Greet(\"Ada\", punctuation: \"?\")", Greet("Ada", punctuation: "?"));

        Section("Overloading - same name, different parameters");

        Out("Add(2, 3)", Add(2, 3));
        Out("Add(2.5, 3.5)", Add(2.5, 3.5));
        Out("Add(1, 2, 3)", Add(1, 2, 3));
        Out("Add(\"ab\", \"cd\")", Add("ab", "cd"));

        Section("params - any number of arguments");

        Out("Total()", Total());
        Out("Total(5)", Total(5));
        Out("Total(1, 2, 3, 4, 5)", Total(1, 2, 3, 4, 5));

        Section("Recursion needs a base case");

        for (int i = 1; i <= 6; i++) Out($"Factorial({i})", Factorial(i));

        Section("Local functions");

        double net = 100;
        Out("WithVat(100)", WithVat(net));

        // A local function: a helper only this method needs.
        double WithVat(double amount) => amount * 1.2;
    }

    // Expression-bodied method: => replaces { return ...; }
    private static double AreaOfCircle(double radius) => Math.PI * radius * radius;

    private static void PrintBanner(string text)
    {
        Line(new string('=', text.Length + 4));
        Line($"= {text} =");
        Line(new string('=', text.Length + 4));
    }

    private static void TryToDouble(int value) => value *= 2;             // changes the copy only

    private static void ModifyContents(int[] array) => array[0] = 99;     // visible to the caller

    private static void ReplaceEntirely(int[] array) => array = [7, 7, 7];// NOT visible to the caller

    private static void Swap(ref int first, ref int second) => (first, second) = (second, first);

    private static bool TryDivide(int numerator, int denominator, out int whole, out int remainder)
    {
        whole = 0;
        remainder = 0;
        if (denominator == 0) return false;      // out params must be assigned on every path
        whole = numerator / denominator;
        remainder = numerator % denominator;
        return true;
    }

    private static string Greet(string name, string greeting = "Hello", string punctuation = "!")
        => $"{greeting}, {name}{punctuation}";

    private static int Add(int x, int y) => x + y;
    private static double Add(double x, double y) => x + y;
    private static int Add(int x, int y, int z) => x + y + z;
    private static string Add(string x, string y) => x + y;

    private static int Total(params int[] numbers)
    {
        int sum = 0;
        foreach (int n in numbers) sum += n;
        return sum;
    }

    private static long Factorial(int n) => n <= 1 ? 1 : n * Factorial(n - 1);
}
