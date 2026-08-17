using System.Diagnostics;
using System.Text;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Production;

// Notes: docs/module-6/6.2.md
public sealed class L02_Performance : LessonBase
{
    public override string Id => "6.2";
    public override string Title => "Performance and benchmarking";

    public override string Summary =>
        "The first rule of performance is measure, never guess - intuition about what is slow "
        + "is wrong more often than right. Learn to time honestly, spot hidden allocations, "
        + "and know the handful of patterns that actually matter.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Measure code honestly with Stopwatch, including warm-up and repetition",
        "Explain how allocations and boxing create garbage-collector pressure",
        "Name the common wins: StringBuilder, capacity hints, Span, struct vs class",
    ];

    public override void Run()
    {
        Section("Rule one: measure, never guess");

        Out("the trap", "programmers' intuition about what is slow is famously wrong");
        Out("the tool here", "Stopwatch - fine for effects of 2x and up");
        Out("the real tool", "BenchmarkDotNet - warm-up, statistics, memory columns, for real work");
        Warn("Debug builds and the first run of any method lie (the JIT compiles on first "
           + "call). Measure Release builds, repeat the work, ignore the first pass.");

        Section("Measuring honestly with Stopwatch");

        // Warm up: run everything once so JIT compilation is not counted.
        SumWithFor(1000); SumWithLinq(1000);

        const int n = 5_000_000;
        Stopwatch watch = Stopwatch.StartNew();
        long a = SumWithFor(n);
        long forMs = watch.ElapsedMilliseconds;

        watch.Restart();
        long b = SumWithLinq(n);
        long linqMs = watch.ElapsedMilliseconds;

        Out($"for loop summing {n:N0} ints", $"{forMs} ms");
        Out("LINQ Sum() on the same range", $"{linqMs} ms");
        Out("same answer", a == b);
        Note("LINQ costs a little abstraction overhead. In a hot inner loop that can matter; "
           + "in ordinary code it never does. This is why you measure before rewriting.");

        Section("Allocations are the hidden cost");

        Out("allocating", "fast on its own - bump a pointer on the heap");
        Out("the real price", "every allocation is future GC work; millions = pauses");
        Out("gen 0 collections so far", GC.CollectionCount(0));

        Section("Boxing: invisible allocations (4.8 revisited)");

        watch.Restart();
        long noBox = 0;
        for (int i = 0; i < 2_000_000; i++) noBox += i;
        long noBoxMs = watch.ElapsedMilliseconds;
        int gen0Before = GC.CollectionCount(0);

        watch.Restart();
        long boxed = 0;
        for (int i = 0; i < 2_000_000; i++)
        {
            object o = i;                        // BOX: the int is copied to the heap
            boxed += (int)o;                     // UNBOX: copied back
        }
        long boxedMs = watch.ElapsedMilliseconds;

        Out("2M adds, plain int", $"{noBoxMs} ms");
        Out("2M adds via object (boxing)", $"{boxedMs} ms");
        Out("gen 0 collections caused", GC.CollectionCount(0) - gen0Before);
        Note("Two million tiny heap objects were created and thrown away. Generics exist "
           + "so List<int> never does this (3.4).");

        Section("The classic: string += in a loop");

        watch.Restart();
        string slow = "";
        for (int i = 0; i < 20_000; i++) slow += "x";
        long concatMs = watch.ElapsedMilliseconds;

        watch.Restart();
        StringBuilder builder = new(20_000);
        for (int i = 0; i < 20_000; i++) builder.Append('x');
        string fast = builder.ToString();
        long builderMs = watch.ElapsedMilliseconds;

        Out("20k iterations of s += \"x\"", $"{concatMs} ms - quadratic copying");
        Out("StringBuilder with capacity", $"{builderMs} ms");
        Out("same result", slow == fast);

        Section("Capacity hints: skip the resize-and-copy ladder");

        const int items = 2_000_000;

        watch.Restart();
        List<int> growing = new();
        for (int i = 0; i < items; i++) growing.Add(i);
        long growMs = watch.ElapsedMilliseconds;

        watch.Restart();
        List<int> presized = new(items);
        for (int i = 0; i < items; i++) presized.Add(i);
        long presizedMs = watch.ElapsedMilliseconds;

        Out($"List.Add x{items:N0}, default capacity", $"{growMs} ms (doubles ~21 times)");
        Out("same with new List<int>(2_000_000)", $"{presizedMs} ms");
        Note("If you know the size, say so - List, Dictionary, StringBuilder all accept a "
           + "capacity. It is the cheapest optimisation there is.");

        Section("Span<T>: slicing without copying");

        int[] data = Enumerable.Range(0, 1000).ToArray();

        // data[10..20] with an array COPIES ten items. A span is just a window.
        Span<int> window = data.AsSpan(10, 10);
        window[0] = 999;                          // writes through to the array

        Out("window.Length", window.Length);
        Out("data[10] after window[0] = 999", data[10]);
        Out("copies made", 0);
        Note("Span<T> is a view over existing memory - array slice, string middle, stack "
           + "buffer - with no allocation. High-performance parsing lives on it. "
           + "(stackalloc goes further: small buffers on the stack itself, no heap at all.)");

        Section("Struct vs class at scale (2.8 revisited)");

        watch.Restart();
        PointClass[] classes = new PointClass[1_000_000];
        for (int i = 0; i < classes.Length; i++) classes[i] = new PointClass(i, i);
        long classMs = watch.ElapsedMilliseconds;

        watch.Restart();
        PointStruct[] structs = new PointStruct[1_000_000];
        for (int i = 0; i < structs.Length; i++) structs[i] = new PointStruct(i, i);
        long structMs = watch.ElapsedMilliseconds;

        Out("1M points as classes", $"{classMs} ms - 1,000,001 heap objects");
        Out("1M points as structs", $"{structMs} ms - ONE array, values inline");
        Note("Structs in an array sit contiguously - no per-item allocation, and the CPU "
           + "cache loves the locality. This is why Point-like types should be structs.");

        Section("Premature optimisation - the other failure mode");

        Out("the famous quote", "\"premature optimisation is the root of all evil\" - Knuth");
        Out("what it means", "clarity first; optimise the measured hot spot, not everywhere");
        Out("what it does NOT mean", "ignore obvious O(n^2) or a List.Contains inside a loop (5.1)");
        Out("the workflow", "write it clearly -> measure -> fix the top item -> measure again");

        Section("The short list that usually matters");

        Out("algorithms first", "O(n^2) -> O(n log n) beats any micro-tune (module 5)");
        Out("right collection", "Dictionary/HashSet for lookups (3.2)");
        Out("fewer allocations", "StringBuilder, capacity hints, structs, Span");
        Out("batch the I/O", "one big read beats a thousand small ones (4.9)");
        Out("parallel last", "and only for big independent chunks (6.1)");
    }

    private static long SumWithFor(int n)
    {
        long total = 0;
        for (int i = 0; i < n; i++) total += i;
        return total;
    }

    private static long SumWithLinq(int n) => Enumerable.Range(0, n).Sum(i => (long)i);

    private class PointClass
    {
        public PointClass(int x, int y) { X = x; Y = y; }
        public int X { get; }
        public int Y { get; }
    }

    private readonly struct PointStruct
    {
        public PointStruct(int x, int y) { X = x; Y = y; }
        public int X { get; }
        public int Y { get; }
    }
}
