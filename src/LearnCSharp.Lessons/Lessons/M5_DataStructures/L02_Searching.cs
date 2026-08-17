using LearnCSharp.Core;

namespace LearnCSharp.Lessons.DataStructures;

// Notes: docs/module-5/5.2.md
public sealed class L02_Searching : LessonBase
{
    public override string Id => "5.2";
    public override string Title => "Searching: linear and binary";

    public override void Run()
    {
        Section("Linear search - check every item in turn");

        int[] unsorted = [42, 17, 93, 8, 55, 23, 71];
        Out("data", string.Join(", ", unsorted));
        Out("LinearSearch(55)", LinearSearch(unsorted, 55));
        Out("LinearSearch(99) - missing", LinearSearch(unsorted, 99));

        Out("works on unsorted data", "yes - that is its advantage");
        Out("complexity", "O(n)");

        Section("Watch it work");

        Line();
        LinearSearchVerbose(unsorted, 55);

        Section("Binary search - only on SORTED data");

        int[] sorted = [8, 17, 23, 42, 55, 71, 93];
        Out("sorted data", string.Join(", ", sorted));
        Out("BinarySearch(55)", BinarySearch(sorted, 55));
        Out("BinarySearch(8) - first", BinarySearch(sorted, 8));
        Out("BinarySearch(99) - missing", BinarySearch(sorted, 99));

        Section("Watch it halve the problem");

        Line();
        BinarySearchVerbose(sorted, 71);

        Line();
        BinarySearchVerbose(sorted, 20);

        Section("Why the midpoint is written that way");

        Out("(low + high) / 2", "can OVERFLOW if both are near int.MaxValue");
        Out("low + (high - low) / 2", "same answer, cannot overflow - use this");

        Section("Binary search, recursively");

        Out("BinarySearchRecursive(42)", BinarySearchRecursive(sorted, 42, 0, sorted.Length - 1));
        Out("BinarySearchRecursive(93)", BinarySearchRecursive(sorted, 93, 0, sorted.Length - 1));

        Section("How many steps on a big list?");

        foreach (int size in new[] { 1_000, 1_000_000, 1_000_000_000 })
            Out($"{size:N0} items, worst case", $"{Math.Ceiling(Math.Log2(size))} comparisons");

        Note("A billion sorted items need only 30 comparisons. That is the power of halving.");

        Section("The catch: sorting costs something");

        Out("one search on unsorted data", "linear O(n) wins - sorting first would cost more");
        Out("many searches", "sort once O(n log n), then every search is O(log n)");
        Out("already sorted", "binary search, always");

        Section("The built-in versions");

        Out("Array.BinarySearch(sorted, 55)", Array.BinarySearch(sorted, 55));
        Out("Array.BinarySearch missing value", Array.BinarySearch(sorted, 20));
        Note("A negative result is the bitwise complement of where the value WOULD go. "
           + "~(-3) = 2, so 20 belongs at index 2. That is useful for insert-in-order.");
        Out("~(-3)", ~-3);

        Out("Array.IndexOf (linear)", Array.IndexOf(unsorted, 93));
        Out("List.BinarySearch", new List<int>(sorted).BinarySearch(71));

        Section("Searching objects by a key");

        Pupil[] pupils =
        [
            new Pupil("Ada", 91),
            new Pupil("Ben", 64),
            new Pupil("Cara", 78),
        ];

        Out("LINQ First", pupils.First(p => p.Name == "Ben").Mark);
        Out("LINQ FirstOrDefault, missing", pupils.FirstOrDefault(p => p.Name == "Zoe")?.Mark);
        Out("Array.Find", Array.Find(pupils, p => p.Mark > 80)?.Name);

        Note("For repeated lookups by key, build a Dictionary once - that turns every later search "
           + "into O(1), beating even binary search.");
    }

    private static int LinearSearch(int[] data, int target)
    {
        for (int i = 0; i < data.Length; i++)
            if (data[i] == target) return i;

        return -1;                                  // the agreed "not found" answer
    }

    private static void LinearSearchVerbose(int[] data, int target)
    {
        for (int i = 0; i < data.Length; i++)
        {
            Console.WriteLine($"      step {i + 1}: is data[{i}] = {data[i]} equal to {target}? "
                            + (data[i] == target ? "YES - found" : "no"));
            if (data[i] == target) return;
        }
    }

    private static int BinarySearch(int[] data, int target)
    {
        int low = 0;
        int high = data.Length - 1;

        while (low <= high)
        {
            int middle = low + (high - low) / 2;

            if (data[middle] == target) return middle;
            if (data[middle] < target) low = middle + 1;    // discard the left half
            else high = middle - 1;                          // discard the right half
        }

        return -1;
    }

    private static void BinarySearchVerbose(int[] data, int target)
    {
        int low = 0, high = data.Length - 1, step = 0;
        Console.WriteLine($"      looking for {target} in [{string.Join(", ", data)}]");

        while (low <= high)
        {
            step++;
            int middle = low + (high - low) / 2;
            Console.Write($"      step {step}: range [{low}..{high}], middle index {middle} = {data[middle]} -> ");

            if (data[middle] == target)
            {
                Console.WriteLine("FOUND");
                return;
            }

            if (data[middle] < target)
            {
                Console.WriteLine($"too small, search the right half");
                low = middle + 1;
            }
            else
            {
                Console.WriteLine($"too big, search the left half");
                high = middle - 1;
            }
        }

        Console.WriteLine($"      not found after {step} steps");
    }

    private static int BinarySearchRecursive(int[] data, int target, int low, int high)
    {
        if (low > high) return -1;                  // base case: nothing left to search

        int middle = low + (high - low) / 2;

        if (data[middle] == target) return middle;

        return data[middle] < target
            ? BinarySearchRecursive(data, target, middle + 1, high)
            : BinarySearchRecursive(data, target, low, middle - 1);
    }
}

public record Pupil(string Name, int Mark);
