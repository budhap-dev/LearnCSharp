using LearnCSharp.Core;

namespace LearnCSharp.Lessons.DataStructures;

// Notes: docs/module-5/5.4.md
public sealed class L04_Recursion : LessonBase
{
    public override string Id => "5.4";
    public override string Title => "Recursion";

    public override string Summary =>
        "A method that calls itself, with a base case that stops it. Natural for trees and "
        + "folders, and disastrous if written naively for Fibonacci.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Write recursive methods with a correct base case",
        "Trace the call stack through a recursive call",
        "Fix exponential recursion with memoisation",
    ];

    public override void Run()
    {
        Section("Every recursive method needs two things");

        Out("1. a base case", "a size of problem it can answer without recursing");
        Out("2. a recursive case", "which must move TOWARDS the base case");

        Section("Factorial, traced");

        Line();
        Line("Factorial(4)");
        Line("= 4 * Factorial(3)");
        Line("= 4 * (3 * Factorial(2))");
        Line("= 4 * (3 * (2 * Factorial(1)))");
        Line("= 4 * (3 * (2 * 1))          <- base case reached, now it unwinds");
        Line("= 4 * (3 * 2)");
        Line("= 4 * 6");
        Line("= 24");

        for (int i = 1; i <= 6; i++) Out($"Factorial({i})", Factorial(i));

        Section("Watch the call stack grow and shrink");

        Line();
        FactorialVerbose(4, 0);

        Section("The same job, iteratively");

        Out("FactorialLoop(6)", FactorialLoop(6));
        Note("Anything recursive can be written with a loop, and vice versa. Recursion is not "
           + "faster - it is chosen when it makes the code CLEARER.");

        Section("Fibonacci: where naive recursion goes wrong");

        Out("FibRecursive(10)", FibRecursive(10));
        Out("FibRecursive(20)", FibRecursive(20));

        _calls = 0;
        FibCounted(20);
        Out("calls to compute Fib(20) naively", _calls);

        _calls = 0;
        FibCounted(25);
        Out("calls to compute Fib(25) naively", _calls);

        Warn("Each extra number nearly doubles the work: this is O(2^n). Fib(50) would take years. "
           + "The cause is recomputing the same values over and over.");

        Section("Memoisation fixes it");

        Dictionary<int, long> cache = new();
        Out("FibMemoised(50)", FibMemoised(50, cache));
        Out("values actually computed", cache.Count);
        Out("FibIterative(50)", FibIterative(50));

        Section("Classic recursive problems");

        Out("Sum([1..5])", SumArray([1, 2, 3, 4, 5], 0));
        Out("Reverse(\"recursion\")", ReverseString("recursion"));
        Out("Power(2, 10)", Power(2, 10));
        Out("Gcd(48, 18)", Gcd(48, 18));
        Out("CountDigits(98765)", CountDigits(98765));
        Out("IsPalindrome(\"racecar\")", IsPalindrome("racecar", 0, 6));

        Section("Towers of Hanoi");

        Line();
        int moves = Hanoi(3, 'A', 'C', 'B');
        Out("moves for 3 discs", moves);
        Out("moves for 10 discs", (1 << 10) - 1);
        Out("moves for 64 discs", "18,446,744,073,709,551,615 - about 585 billion years");

        Section("Recursion over trees and folders - where it really shines");

        Line();
        PrintFolder(SampleFolder(), 0);

        Out("total files", CountFiles(SampleFolder()));
        Out("total size", TotalSize(SampleFolder()));
        Note("A folder contains folders, which contain folders... The data is recursive, so recursive "
           + "code fits it naturally. Writing this with loops needs an explicit stack.");

        Section("Stack overflow");

        Out("each call uses stack space", "parameters, locals and the return address");
        Out("default stack size", "about 1 MB - a few tens of thousands of frames");
        Out("no base case", "StackOverflowException, and you CANNOT catch it");

        int depth = MeasureDepth(0);
        Out("recursion depth reached before failing", depth);

        Section("Recursion or iteration?");

        Out("use recursion", "trees, graphs, folders, divide-and-conquer, backtracking");
        Out("use iteration", "simple counting or accumulating over a sequence");
        Out("watch out for", "deep recursion on large inputs - convert to a loop with a Stack<T>");
    }

    private static int _calls;

    private static long Factorial(int n) => n <= 1 ? 1 : n * Factorial(n - 1);

    private static long FactorialVerbose(int n, int depth)
    {
        string indent = new string(' ', 6 + depth * 2);
        Console.WriteLine($"{indent}-> Factorial({n}) called");

        if (n <= 1)
        {
            Console.WriteLine($"{indent}<- base case, returns 1");
            return 1;
        }

        long result = n * FactorialVerbose(n - 1, depth + 1);
        Console.WriteLine($"{indent}<- Factorial({n}) returns {result}");
        return result;
    }

    private static long FactorialLoop(int n)
    {
        long result = 1;
        for (int i = 2; i <= n; i++) result *= i;
        return result;
    }

    private static long FibRecursive(int n) => n <= 1 ? n : FibRecursive(n - 1) + FibRecursive(n - 2);

    private static long FibCounted(int n)
    {
        _calls++;
        return n <= 1 ? n : FibCounted(n - 1) + FibCounted(n - 2);
    }

    // Remember every answer, so each value is computed exactly once. O(n) instead of O(2^n).
    private static long FibMemoised(int n, Dictionary<int, long> cache)
    {
        if (n <= 1) return n;
        if (cache.TryGetValue(n, out long known)) return known;

        long result = FibMemoised(n - 1, cache) + FibMemoised(n - 2, cache);
        cache[n] = result;
        return result;
    }

    private static long FibIterative(int n)
    {
        long a = 0, b = 1;
        for (int i = 0; i < n; i++) (a, b) = (b, a + b);
        return a;
    }

    private static int SumArray(int[] data, int index) =>
        index >= data.Length ? 0 : data[index] + SumArray(data, index + 1);

    private static string ReverseString(string text) =>
        text.Length <= 1 ? text : ReverseString(text[1..]) + text[0];

    private static long Power(int baseNumber, int exponent) =>
        exponent == 0 ? 1 : baseNumber * Power(baseNumber, exponent - 1);

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

    private static int CountDigits(int n) => n < 10 ? 1 : 1 + CountDigits(n / 10);

    private static bool IsPalindrome(string text, int left, int right) =>
        left >= right || (text[left] == text[right] && IsPalindrome(text, left + 1, right - 1));

    private static int Hanoi(int discs, char from, char to, char via)
    {
        if (discs == 0) return 0;

        int moves = Hanoi(discs - 1, from, via, to);          // move the tower above out of the way
        Console.WriteLine($"      move disc {discs} from {from} to {to}");
        moves++;
        moves += Hanoi(discs - 1, via, to, from);             // move it back on top

        return moves;
    }

    private static Folder SampleFolder() => new("root",
        [new FileEntry("readme.md", 2), new FileEntry("notes.txt", 5)],
        [
            new Folder("src", [new FileEntry("Program.cs", 12)], []),
            new Folder("docs",
                [new FileEntry("guide.md", 30)],
                [new Folder("images", [new FileEntry("logo.png", 100)], [])]),
        ]);

    private static void PrintFolder(Folder folder, int depth)
    {
        string indent = new string(' ', 6 + depth * 2);
        Console.WriteLine($"{indent}[{folder.Name}]");

        foreach (FileEntry file in folder.Files)
            Console.WriteLine($"{indent}  {file.Name} ({file.SizeKb} KB)");

        foreach (Folder child in folder.Folders)
            PrintFolder(child, depth + 1);                    // recurse into each subfolder
    }

    private static int CountFiles(Folder folder) =>
        folder.Files.Count + folder.Folders.Sum(CountFiles);

    private static int TotalSize(Folder folder) =>
        folder.Files.Sum(f => f.SizeKb) + folder.Folders.Sum(TotalSize);

    private static int MeasureDepth(int depth)
    {
        // Stop well before the real limit: StackOverflowException cannot be caught,
        // so we must not actually reach it.
        if (depth >= 20000) return depth;
        return MeasureDepth(depth + 1);
    }
}

public record FileEntry(string Name, int SizeKb);
public record Folder(string Name, List<FileEntry> Files, List<Folder> Folders);
