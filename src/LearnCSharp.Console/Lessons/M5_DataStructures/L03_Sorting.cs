using System.Diagnostics;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.DataStructures;

// Notes: docs/module-5/5.3.md
public sealed class L03_Sorting : LessonBase
{
    public override string Id => "5.3";
    public override string Title => "Sorting: bubble, insertion, selection, merge, quick";

    public override void Run()
    {
        int[] original = [5, 2, 9, 1, 7, 3];

        Section("Bubble sort - repeatedly swap neighbours that are in the wrong order");

        Line();
        BubbleSortVerbose([.. original]);

        Out("result", string.Join(", ", BubbleSort([.. original])));
        Out("complexity", "O(n^2) worst and average, O(n) best if already sorted");
        Out("space", "O(1) - sorts in place");
        Out("stable", "yes - equal items keep their original order");

        Section("Insertion sort - build a sorted section on the left");

        Line();
        InsertionSortVerbose([.. original]);

        Out("result", string.Join(", ", InsertionSort([.. original])));
        Out("complexity", "O(n^2) worst, O(n) if nearly sorted - genuinely fast on small arrays");

        Section("Selection sort - repeatedly pick the smallest remaining");

        Out("result", string.Join(", ", SelectionSort([.. original])));
        Out("complexity", "O(n^2) always - it always scans the rest");
        Out("advantage", "makes the fewest SWAPS - useful if writing is expensive");

        Section("Merge sort - divide and conquer");

        Line();
        Line("split:   [5,2,9,1,7,3]");
        Line("         [5,2,9]        [1,7,3]");
        Line("         [5] [2,9]      [1] [7,3]");
        Line("         [5] [2] [9]    [1] [7] [3]");
        Line("merge:   [5] [2,9]      [1] [3,7]");
        Line("         [2,5,9]        [1,3,7]");
        Line("         [1,2,3,5,7,9]");

        Out("result", string.Join(", ", MergeSort([.. original])));
        Out("complexity", "O(n log n) in ALL cases - log n levels, n work per level");
        Out("space", "O(n) - it needs temporary arrays");
        Out("stable", "yes");

        Section("Quicksort - partition around a pivot");

        int[] quick = [.. original];
        QuickSort(quick, 0, quick.Length - 1);
        Out("result", string.Join(", ", quick));
        Out("complexity", "O(n log n) average, O(n^2) worst (a bad pivot every time)");
        Out("space", "O(log n) for the recursion stack");
        Out("in practice", "usually the fastest general sort - great cache behaviour");

        Section("Racing them on real data");

        foreach (int size in new[] { 2_000, 8_000 })
        {
            int[] data = MakeRandom(size, seed: 42);

            Out($"--- n = {size:N0}", "");
            Out("bubble sort", Time(() => BubbleSort([.. data])));
            Out("insertion sort", Time(() => InsertionSort([.. data])));
            Out("selection sort", Time(() => SelectionSort([.. data])));
            Out("merge sort", Time(() => MergeSort([.. data])));
            Out("Array.Sort (built in)", Time(() => { int[] copy = [.. data]; Array.Sort(copy); return copy; }));
        }

        Note("Quadrupling the data makes the O(n^2) sorts about 16 times slower, while merge sort "
           + "roughly quadruples. That gap only widens.");

        Section("Best case matters: nearly-sorted data");

        int[] nearlySorted = Enumerable.Range(0, 8000).ToArray();
        nearlySorted[100] = -1;

        Out("insertion sort on nearly sorted", Time(() => InsertionSort([.. nearlySorted])));
        Out("selection sort on nearly sorted", Time(() => SelectionSort([.. nearlySorted])));
        Note("Insertion sort spots that the data is nearly ordered and finishes in almost O(n). "
           + "Selection sort cannot - it always does the same work.");

        Section("What .NET actually uses");

        Out("Array.Sort / List.Sort", "introsort: quicksort, switching to heapsort if it goes badly,");
        Out("", "and to insertion sort for small partitions - the best of all three");
        Out("OrderBy (LINQ)", "a stable merge sort, and it returns a NEW sequence");

        int[] builtIn = [.. original];
        Array.Sort(builtIn);
        Out("Array.Sort", string.Join(", ", builtIn));
        Out("OrderByDescending", string.Join(", ", original.OrderByDescending(n => n)));

        Section("Choosing a sort");

        Out("in an exam", "know bubble, insertion, merge - AQA asks for these");
        Out("in real code", "Array.Sort or OrderBy, every time");
        Out("tiny arrays (< 20)", "insertion sort is genuinely the fastest");
        Out("must be stable", "merge sort or OrderBy");
        Out("memory is tight", "quicksort or heapsort - they sort in place");
    }

    private static int[] BubbleSort(int[] data)
    {
        for (int pass = 0; pass < data.Length - 1; pass++)
        {
            bool swapped = false;

            // After each pass the largest remaining value has bubbled to the end,
            // so we can stop one place earlier each time.
            for (int i = 0; i < data.Length - 1 - pass; i++)
            {
                if (data[i] > data[i + 1])
                {
                    (data[i], data[i + 1]) = (data[i + 1], data[i]);
                    swapped = true;
                }
            }

            if (!swapped) break;                    // already sorted - stop early
        }

        return data;
    }

    private static void BubbleSortVerbose(int[] data)
    {
        Console.WriteLine($"      start:  {string.Join(", ", data)}");

        for (int pass = 0; pass < data.Length - 1; pass++)
        {
            bool swapped = false;
            for (int i = 0; i < data.Length - 1 - pass; i++)
            {
                if (data[i] > data[i + 1])
                {
                    (data[i], data[i + 1]) = (data[i + 1], data[i]);
                    swapped = true;
                }
            }
            Console.WriteLine($"      pass {pass + 1}: {string.Join(", ", data)}"
                            + (swapped ? "" : "   (no swaps - stop)"));
            if (!swapped) break;
        }
    }

    private static int[] InsertionSort(int[] data)
    {
        for (int i = 1; i < data.Length; i++)
        {
            int current = data[i];
            int j = i - 1;

            // Shuffle everything bigger than 'current' one place to the right.
            while (j >= 0 && data[j] > current)
            {
                data[j + 1] = data[j];
                j--;
            }

            data[j + 1] = current;
        }

        return data;
    }

    private static void InsertionSortVerbose(int[] data)
    {
        Console.WriteLine($"      start:  {string.Join(", ", data)}");

        for (int i = 1; i < data.Length; i++)
        {
            int current = data[i];
            int j = i - 1;
            while (j >= 0 && data[j] > current)
            {
                data[j + 1] = data[j];
                j--;
            }
            data[j + 1] = current;
            Console.WriteLine($"      insert {current,2}: {string.Join(", ", data)}");
        }
    }

    private static int[] SelectionSort(int[] data)
    {
        for (int i = 0; i < data.Length - 1; i++)
        {
            int smallest = i;

            for (int j = i + 1; j < data.Length; j++)
                if (data[j] < data[smallest]) smallest = j;

            if (smallest != i)
                (data[i], data[smallest]) = (data[smallest], data[i]);
        }

        return data;
    }

    private static int[] MergeSort(int[] data)
    {
        if (data.Length <= 1) return data;           // base case

        int middle = data.Length / 2;
        int[] left = MergeSort(data[..middle]);      // divide
        int[] right = MergeSort(data[middle..]);

        return Merge(left, right);                   // conquer
    }

    private static int[] Merge(int[] left, int[] right)
    {
        int[] result = new int[left.Length + right.Length];
        int i = 0, j = 0, k = 0;

        // Repeatedly take the smaller front item from the two sorted halves.
        while (i < left.Length && j < right.Length)
            result[k++] = left[i] <= right[j] ? left[i++] : right[j++];

        while (i < left.Length) result[k++] = left[i++];
        while (j < right.Length) result[k++] = right[j++];

        return result;
    }

    private static void QuickSort(int[] data, int low, int high)
    {
        if (low >= high) return;                     // base case

        int pivotIndex = Partition(data, low, high);
        QuickSort(data, low, pivotIndex - 1);        // everything left of the pivot
        QuickSort(data, pivotIndex + 1, high);       // everything right of it
    }

    private static int Partition(int[] data, int low, int high)
    {
        int pivot = data[high];                      // use the last item as the pivot
        int boundary = low - 1;

        for (int i = low; i < high; i++)
        {
            if (data[i] <= pivot)
            {
                boundary++;
                (data[boundary], data[i]) = (data[i], data[boundary]);
            }
        }

        (data[boundary + 1], data[high]) = (data[high], data[boundary + 1]);
        return boundary + 1;                         // the pivot's final resting place
    }

    private static int[] MakeRandom(int size, int seed)
    {
        Random random = new(seed);
        return Enumerable.Range(0, size).Select(_ => random.Next(0, 10000)).ToArray();
    }

    private static string Time(Func<int[]> sort)
    {
        Stopwatch watch = Stopwatch.StartNew();
        int[] result = sort();
        watch.Stop();

        bool ordered = result.Zip(result.Skip(1)).All(pair => pair.First <= pair.Second);
        return $"{watch.Elapsed.TotalMilliseconds,8:F2} ms {(ordered ? "" : " (NOT SORTED)")}";
    }
}
