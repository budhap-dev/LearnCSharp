using System.Diagnostics;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.DataStructures;

// Notes: docs/module-5/5.1.md
public sealed class L01_BigO : LessonBase
{
    public override string Id => "5.1";
    public override string Title => "Complexity and Big-O notation";

    public override string Summary =>
        "How the work grows as the data grows. This is the language for comparing algorithms, "
        + "and the reason an O(n^2) sort is hopeless on real data.";

    public override IReadOnlyList<string> Objectives =>
    [
        "State the complexity of common operations",
        "Work out the Big-O of a loop or a nested loop",
        "Explain the practical difference between O(n log n) and O(n^2)",
    ];

    public override void Run()
    {
        Section("Big-O describes how the work GROWS with the input size");

        Out("O(1)", "constant - the size makes no difference");
        Out("O(log n)", "halving each step - binary search");
        Out("O(n)", "one pass - linear search, summing a list");
        Out("O(n log n)", "the best a general sort can do - merge sort, quicksort");
        Out("O(n^2)", "nested loops - bubble sort, comparing every pair");
        Out("O(2^n)", "doubles per extra item - naive recursion, brute force");
        Out("O(n!)", "every ordering - the travelling salesman by brute force");

        Section("What that means in practice");

        Line();
        Line($"{"n",10} {"O(log n)",10} {"O(n)",10} {"O(n log n)",12} {"O(n^2)",14}");
        foreach (int n in new[] { 10, 100, 1000, 10000, 100000 })
        {
            double log = Math.Log2(n);
            Line($"{n,10} {log,10:F1} {n,10} {n * log,12:N0} {(double)n * n,14:N0}");
        }

        Note("At n = 100,000 an O(n^2) algorithm does 10,000,000,000 steps while O(n log n) does "
           + "about 1,700,000. That is the difference between hours and milliseconds.");

        Section("Measuring it for real: linear vs binary search");

        foreach (int size in new[] { 100_000, 400_000, 1_600_000 })
        {
            int[] data = Enumerable.Range(0, size).ToArray();
            int target = size - 1;                 // worst case: the very last item

            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < 200; i++) LinearSearch(data, target);
            long linear = watch.ElapsedMilliseconds;

            watch.Restart();
            for (int i = 0; i < 200; i++) BinarySearch(data, target);
            long binary = watch.ElapsedMilliseconds;

            Out($"n = {size:N0}, 200 searches", $"linear {linear} ms, binary {binary} ms");
        }

        Note("Four times the data makes the linear search about four times slower. The binary search "
           + "barely moves - it needs only two extra steps.");

        Section("Counting operations instead of timing");

        foreach (int size in new[] { 16, 256, 4096, 65536 })
            Out($"n = {size,6}", $"linear ~{size} steps, binary ~{Math.Log2(size)} steps");

        Section("Best, average and worst case");

        int[] sample = [1, 2, 3, 4, 5, 6, 7, 8];
        Out("linear search, best case", $"{CountLinearSteps(sample, 1)} step - it is first");
        Out("linear search, worst case", $"{CountLinearSteps(sample, 8)} steps - it is last");
        Out("linear search, not present", $"{CountLinearSteps(sample, 99)} steps - checks everything");

        Note("Big-O normally describes the WORST case, because that is the guarantee you can rely on.");

        Section("The rules for working out Big-O");

        Out("drop constants", "O(2n) becomes O(n) - doubling does not change the shape");
        Out("drop lower terms", "O(n^2 + n) becomes O(n^2) - n^2 dominates");
        Out("sequential loops add", "loop n, then loop n  ->  O(n) + O(n) = O(n)");
        Out("nested loops multiply", "loop n inside loop n  ->  O(n^2)");
        Out("halving means log", "while (n > 1) n /= 2  ->  O(log n)");

        Section("Identify these");

        Out("for (i..n) sum += a[i]", "O(n)");
        Out("for (i..n) for (j..n) ...", "O(n^2)");
        Out("for (i..n) for (j..i) ...", "O(n^2) - it is n(n+1)/2, constants drop");
        Out("while (n > 1) n /= 2", "O(log n)");
        Out("for (i..n) binarySearch(...)", "O(n log n)");
        Out("dictionary.ContainsKey(k)", "O(1)");
        Out("list.Contains(v)", "O(n)");

        Section("Space complexity counts too");

        Out("bubble sort", "O(1) extra space - it sorts in place");
        Out("merge sort", "O(n) extra space - it needs temporary arrays");
        Out("recursion", "O(depth) stack space - deep recursion overflows the stack");

        Section("Why it matters to you");

        Out("1", "picking a Dictionary over a List can turn minutes into milliseconds");
        Out("2", "an O(n^2) loop is fine for 100 items and hopeless for 100,000");
        Out("3", "AQA asks you to compare algorithm efficiency - this is the language for it");
    }

    private static int LinearSearch(int[] data, int target)
    {
        for (int i = 0; i < data.Length; i++)
            if (data[i] == target) return i;
        return -1;
    }

    private static int BinarySearch(int[] data, int target)
    {
        int low = 0, high = data.Length - 1;
        while (low <= high)
        {
            int middle = low + (high - low) / 2;
            if (data[middle] == target) return middle;
            if (data[middle] < target) low = middle + 1;
            else high = middle - 1;
        }
        return -1;
    }

    private static int CountLinearSteps(int[] data, int target)
    {
        int steps = 0;
        foreach (int value in data)
        {
            steps++;
            if (value == target) break;
        }
        return steps;
    }
}
