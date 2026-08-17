using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Advanced;

// Notes: docs/module-4/4.2.md
public sealed class L02_Lambdas : LessonBase
{
    public override string Id => "4.2";
    public override string Title => "Lambdas, closures and functional style";

    public override void Run()
    {
        Section("Lambda syntax, from long to short");

        Func<int, int> a = delegate (int x) { return x * x; };   // C# 2 anonymous method
        Func<int, int> b = (int x) => { return x * x; };         // lambda, full form
        Func<int, int> c = (x) => x * x;                         // types inferred, body is an expression
        Func<int, int> d = x => x * x;                           // one parameter needs no brackets

        Out("all four give the same answer for 6", $"{a(6)} {b(6)} {c(6)} {d(6)}");

        Func<int, int, int> two = (x, y) => x + y;               // brackets needed for 2+ parameters
        Func<string> none = () => "empty brackets when there are none";
        Out("two(3, 4)", two(3, 4));
        Out("none()", none());

        Section("Multi-statement lambdas");

        Func<int, string> classify = n =>
        {
            if (n < 0) return "negative";
            if (n == 0) return "zero";
            return n % 2 == 0 ? "positive even" : "positive odd";
        };

        Out("classify(-5)", classify(-5));
        Out("classify(0)", classify(0));
        Out("classify(7)", classify(7));

        Section("Closures: a lambda remembers the variables around it");

        int factor = 3;
        Func<int, int> scale = n => n * factor;   // 'factor' is CAPTURED, not copied at this moment
        Out("factor = 3, scale(10)", scale(10));

        factor = 10;                              // change it afterwards...
        Out("factor = 10, scale(10)", scale(10));

        Warn("The lambda captured the VARIABLE, not its value. Changing factor later changes what "
           + "the lambda does.");

        Section("Each call gets its own captured copy");

        Func<int> counterA = MakeCounter();
        Func<int> counterB = MakeCounter();

        Out("counterA()", counterA());
        Out("counterA()", counterA());
        Out("counterA()", counterA());
        Out("counterB() - separate state", counterB());

        Section("The classic loop-capture trap");

        // Before C# 5 this printed 3,3,3. Modern C# gives each iteration its own copy of i.
        List<Func<int>> fromFor = new();
        for (int i = 0; i < 3; i++)
        {
            int copy = i;                         // the safe habit: copy into a local
            fromFor.Add(() => copy);
        }
        Out("captured in a for loop", string.Join(", ", fromFor.Select(f => f())));

        List<Func<int>> fromForeach = new();
        foreach (int i in Enumerable.Range(0, 3)) fromForeach.Add(() => i);
        Out("captured in a foreach loop", string.Join(", ", fromForeach.Select(f => f())));

        Section("Lambdas are what make LINQ readable");

        List<string> words = ["apple", "fig", "banana", "kiwi", "cherry"];

        Out("short words", string.Join(", ", words.Where(w => w.Length <= 4)));
        Out("lengths", string.Join(", ", words.Select(w => w.Length)));
        Out("by length then alphabetically", string.Join(", ",
            words.OrderBy(w => w.Length).ThenBy(w => w)));
        Out("longest", words.MaxBy(w => w.Length));
        Out("total letters", words.Sum(w => w.Length));

        Section("static lambdas avoid accidental capture");

        // 'static' on a lambda means "capture nothing" - the compiler enforces it,
        // which stops a hidden allocation and a whole class of bug.
        Func<int, int> pure = static n => n * 2;
        Out("static lambda pure(21)", pure(21));

        Section("Storing functions in a dictionary - a neat calculator");

        Dictionary<string, Func<double, double, double>> operations = new()
        {
            ["+"] = (x, y) => x + y,
            ["-"] = (x, y) => x - y,
            ["*"] = (x, y) => x * y,
            ["/"] = (x, y) => y == 0 ? double.NaN : x / y,
        };

        foreach (string symbol in operations.Keys)
            Out($"12 {symbol} 4", operations[symbol](12, 4));

        Section("Method group conversion");

        // When a lambda does nothing but call one method, pass the method name directly.
        Out("Select(w => w.ToUpper())", string.Join(",", words.Take(2).Select(w => w.ToUpperInvariant())));
        Out("Select(int.Parse) - method group", string.Join(",", new[] { "1", "2" }.Select(int.Parse)));
    }

    // The returned lambda keeps 'count' alive after MakeCounter has finished.
    private static Func<int> MakeCounter()
    {
        int count = 0;
        return () => ++count;
    }
}
