using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Collections;

// Notes: docs/module-3/3.1.md
public sealed class L01_Lists : LessonBase
{
    public override string Id => "3.1";
    public override string Title => "List<T>: the array that grows";

    public override string Summary =>
        "The array that grows. Your default collection, with built-in methods for searching, "
        + "sorting, inserting and removing.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Add, insert, remove and search a List",
        "Explain capacity doubling and what it costs",
        "Avoid modifying a collection while iterating over it",
    ];

    public override void Run()
    {
        Section("Creating and adding");

        List<string> shopping = new();
        shopping.Add("bread");
        shopping.Add("milk");
        shopping.AddRange(["eggs", "butter"]);

        Out("shopping", string.Join(", ", shopping));
        Out("Count", shopping.Count);

        List<int> primes = [2, 3, 5, 7, 11];      // collection expression
        Out("primes", string.Join(", ", primes));

        Section("Reading, inserting, removing");

        Out("shopping[0]", shopping[0]);
        Out("shopping[^1]", shopping[^1]);

        shopping.Insert(1, "jam");
        Out("after Insert(1, \"jam\")", string.Join(", ", shopping));

        shopping.Remove("milk");                  // by value - removes the FIRST match
        Out("after Remove(\"milk\")", string.Join(", ", shopping));

        shopping.RemoveAt(0);                     // by index
        Out("after RemoveAt(0)", string.Join(", ", shopping));

        shopping.RemoveAll(item => item.StartsWith('b'));
        Out("after RemoveAll(starts with b)", string.Join(", ", shopping));

        Section("Searching");

        List<int> data = [23, 5, 91, 42, 17, 8, 42];
        Out("data", string.Join(", ", data));
        Out("Contains(42)", data.Contains(42));
        Out("IndexOf(42)  first match", data.IndexOf(42));
        Out("LastIndexOf(42)", data.LastIndexOf(42));
        Out("Find(v => v > 50)", data.Find(v => v > 50));
        Out("FindIndex(v => v > 50)", data.FindIndex(v => v > 50));
        Out("FindAll(v => v > 20)", string.Join(", ", data.FindAll(v => v > 20)));
        Out("Exists(v => v < 0)", data.Exists(v => v < 0));
        Out("TrueForAll(v => v > 0)", data.TrueForAll(v => v > 0));

        Section("Sorting");

        List<int> toSort = new(data);
        toSort.Sort();
        Out("Sort()", string.Join(", ", toSort));

        toSort.Sort((a, b) => b.CompareTo(a));    // custom comparison: descending
        Out("Sort(descending)", string.Join(", ", toSort));

        toSort.Reverse();
        Out("Reverse()", string.Join(", ", toSort));

        Section("Lists of objects");

        List<Chore> jobs =
        [
            new Chore("Homework", 2),
            new Chore("Washing up", 1),
            new Chore("Revision", 3),
        ];

        jobs.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        foreach (Chore job in jobs) Out("by priority", job.ToString());

        Out("highest priority", jobs.MaxBy(j => j.Priority)?.Name);
        Out("total effort", jobs.Sum(j => j.Priority));

        Section("Do not modify a list while foreach-ing it");

        List<int> numbers = [1, 2, 3, 4, 5, 6];
        try
        {
            foreach (int n in numbers)
                if (n % 2 == 0) numbers.Remove(n);
        }
        catch (InvalidOperationException)
        {
            Out("removing inside foreach", "InvalidOperationException");
        }

        // Safe alternatives:
        List<int> a1 = [1, 2, 3, 4, 5, 6];
        a1.RemoveAll(n => n % 2 == 0);
        Out("RemoveAll (best)", string.Join(", ", a1));

        List<int> a2 = [1, 2, 3, 4, 5, 6];
        for (int i = a2.Count - 1; i >= 0; i--)   // loop BACKWARDS when removing by index
            if (a2[i] % 2 == 0) a2.RemoveAt(i);
        Out("backwards for loop", string.Join(", ", a2));

        Section("Capacity vs Count - how the growth works");

        List<int> growing = new();
        int lastCapacity = -1;
        for (int i = 0; i < 40; i++)
        {
            growing.Add(i);
            if (growing.Capacity != lastCapacity)
            {
                Out($"after adding item {i + 1}, capacity became", growing.Capacity);
                lastCapacity = growing.Capacity;
            }
        }
        Note("The capacity DOUBLES when it runs out. Each doubling copies everything to a new array, "
           + "so if you know the final size, say new List<int>(1000) and skip all the copying.");

        Section("Converting");

        Out("ToArray()", string.Join(", ", data.ToArray()));
        Out("new List<int>(array)", string.Join(", ", new List<int>(new[] { 1, 2, 3 })));

        Section("Array vs List");

        Out("array", "fixed size, tiny bit faster, use for a known fixed set");
        Out("List<T>", "grows and shrinks, far more methods - your default choice");
    }
}

public record Chore(string Name, int Priority)
{
    public override string ToString() => $"{Name} (priority {Priority})";
}
