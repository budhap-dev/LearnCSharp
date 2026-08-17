using System.Collections;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Collections;

// Notes: docs/module-3/3.5.md
public sealed class L05_Enumerables : LessonBase
{
    public override string Id => "3.5";
    public override string Title => "IEnumerable, iterators and yield";

    public override string Summary =>
        "The interface behind foreach, and how yield lets you describe a sequence - even an "
        + "infinite one - without ever building it in memory.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Explain what foreach really does",
        "Write an iterator with yield return",
        "Explain deferred execution and when to force it with ToList",
    ];

    public override void Run()
    {
        Section("IEnumerable<T> is the thing foreach understands");

        IEnumerable<int> fromList = new List<int> { 1, 2, 3 };
        IEnumerable<int> fromArray = new[] { 4, 5, 6 };

        Out("a List IS an IEnumerable", string.Join(", ", fromList));
        Out("an array IS an IEnumerable", string.Join(", ", fromArray));

        Section("What foreach really does");

        // foreach is shorthand for this:
        IEnumerator<int> enumerator = fromList.GetEnumerator();
        List<int> collected = new();
        while (enumerator.MoveNext()) collected.Add(enumerator.Current);
        Out("MoveNext/Current by hand", string.Join(", ", collected));

        Section("yield return builds a sequence lazily");

        Out("CountTo(5)", string.Join(", ", CountTo(5)));
        Out("Fibonacci(10)", string.Join(", ", Fibonacci().Take(10)));
        Out("EvenNumbers().Take(5)", string.Join(", ", EvenNumbers().Take(5)));

        Note("Fibonacci() describes an INFINITE sequence, yet Take(10) makes it finish. Nothing is "
           + "computed until something asks for the next item.");

        Section("Lazy evaluation, made visible");

        Line();
        Line("Building the query (nothing runs yet):");
        IEnumerable<int> query = Noisy().Where(n => n % 2 == 0);

        Line();
        Line("Now we foreach it - watch the interleaving:");
        foreach (int n in query) Line($"got {n}");

        Section("Deferred execution can surprise you");

        List<int> source = [1, 2, 3];
        IEnumerable<int> doubled = source.Select(n => n * 2);

        source.Add(4);                             // changed AFTER the query was written
        Out("query written before Add(4), run after", string.Join(", ", doubled));

        List<int> source2 = [1, 2, 3];
        List<int> snapshot = source2.Select(n => n * 2).ToList();   // ToList runs it NOW
        source2.Add(4);
        Out("with ToList() taken first", string.Join(", ", snapshot));

        Warn("A LINQ query re-runs every time you enumerate it. Call ToList() or ToArray() when you "
           + "want a fixed snapshot, or when the query is expensive and you need it twice.");

        Section("yield break stops early");

        Out("TakeUntilNegative([1,2,-1,4])", string.Join(", ", TakeUntilNegative([1, 2, -1, 4])));

        Section("Making your own class foreach-able");

        Playlist playlist = new();
        playlist.Add("Song A");
        playlist.Add("Song B");
        playlist.Add("Song C");

        foreach (string song in playlist) Out("playlist item", song);
        Out("LINQ works on it too", playlist.Count());
        Out("shuffled first item", playlist.Reverse().First());

        Section("Reading a huge file without loading it all");

        // File.ReadAllLines loads everything into memory; File.ReadLines streams line by line.
        // Both are IEnumerable<string>, so the loop looks identical - only the memory differs.
        Out("File.ReadAllLines", "string[] - whole file in RAM");
        Out("File.ReadLines", "IEnumerable<string> - one line at a time");

        Section("IEnumerable vs List as a parameter type");

        Out("take IEnumerable<T>", "accepts arrays, lists, LINQ queries - most flexible");
        Out("take List<T>", "only lists - use when you need Count or indexing");
        Out("return IEnumerable<T>", "the caller cannot change your collection");
    }

    private static IEnumerable<int> CountTo(int max)
    {
        for (int i = 1; i <= max; i++)
            yield return i;                        // hands back one item, then PAUSES here
    }

    private static IEnumerable<int> Fibonacci()
    {
        int a = 0, b = 1;
        while (true)                               // infinite - safe, because it is lazy
        {
            yield return a;
            (a, b) = (b, a + b);
        }
    }

    private static IEnumerable<int> EvenNumbers()
    {
        int n = 0;
        while (true)
        {
            yield return n;
            n += 2;
        }
    }

    private static IEnumerable<int> Noisy()
    {
        for (int i = 1; i <= 4; i++)
        {
            Console.WriteLine($"      producing {i}");
            yield return i;
        }
    }

    private static IEnumerable<int> TakeUntilNegative(int[] values)
    {
        foreach (int value in values)
        {
            if (value < 0) yield break;            // stop the sequence here
            yield return value;
        }
    }
}

/// <summary>Implement IEnumerable&lt;T&gt; and your own class works with foreach and all of LINQ.</summary>
public class Playlist : IEnumerable<string>
{
    private readonly List<string> _songs = new();

    public void Add(string song) => _songs.Add(song);

    public IEnumerator<string> GetEnumerator()
    {
        foreach (string song in _songs)
            yield return song;
    }

    // The old non-generic version, required by the interface. Always just forward to the generic one.
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
