using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Collections;

// Notes: docs/module-3/3.2.md
public sealed class L02_Dictionaries : LessonBase
{
    public override string Id => "3.2";
    public override string Title => "Dictionary and HashSet: lookup by key";

    public override void Run()
    {
        Section("A dictionary maps keys to values");

        Dictionary<string, int> ages = new()
        {
            ["Ada"] = 36,
            ["Alan"] = 41,
            ["Grace"] = 85,
        };

        Out("ages[\"Ada\"]", ages["Ada"]);
        Out("Count", ages.Count);

        ages["Edsger"] = 72;                     // adds if missing
        ages["Ada"] = 37;                        // overwrites if present
        Out("after two assignments, count", ages.Count);
        Out("ages[\"Ada\"]", ages["Ada"]);

        Section("Add throws on a duplicate key; the indexer does not");

        try { ages.Add("Ada", 99); }
        catch (ArgumentException) { Out("ages.Add(\"Ada\", 99)", "ArgumentException - key exists"); }

        Out("TryAdd(\"Ada\", 99)", ages.TryAdd("Ada", 99));
        Out("TryAdd(\"Linus\", 54)", ages.TryAdd("Linus", 54));

        Section("Reading safely");

        try { _ = ages["Nobody"]; }
        catch (KeyNotFoundException) { Out("ages[\"Nobody\"]", "KeyNotFoundException"); }

        Out("ContainsKey(\"Nobody\")", ages.ContainsKey("Nobody"));

        if (ages.TryGetValue("Grace", out int graceAge))
            Out("TryGetValue(\"Grace\")", graceAge);

        Out("GetValueOrDefault(\"Nobody\")", ages.GetValueOrDefault("Nobody"));
        Out("GetValueOrDefault(\"Nobody\", -1)", ages.GetValueOrDefault("Nobody", -1));

        Section("Looping over a dictionary");

        foreach (KeyValuePair<string, int> pair in ages)
            Out($"  {pair.Key}", pair.Value);

        Out("Keys", string.Join(", ", ages.Keys));
        Out("Values", string.Join(", ", ages.Values));

        Note("The order is NOT guaranteed. If you need order, sort the keys or use a "
           + "SortedDictionary.");

        Section("The classic use: counting things");

        string text = "the quick brown fox jumps over the lazy dog the end";
        Dictionary<string, int> counts = new();

        foreach (string word in text.Split(' '))
        {
            // If the word is missing, GetValueOrDefault gives 0, so this always works.
            counts[word] = counts.GetValueOrDefault(word) + 1;
        }

        foreach (KeyValuePair<string, int> pair in counts.OrderByDescending(p => p.Value).Take(3))
            Out($"\"{pair.Key}\" appears", pair.Value);

        Section("Any type can be a key");

        Dictionary<int, string> byId = new() { [1] = "first", [2] = "second" };
        Out("byId[2]", byId[2]);

        // A record key works because records compare by value.
        Dictionary<Point, string> grid = new()
        {
            [new Point(0, 0)] = "origin",
            [new Point(1, 1)] = "diagonal",
        };
        Out("grid[new Point(0, 0)]", grid[new Point(0, 0)]);

        Section("Case-insensitive keys");

        Dictionary<string, int> insensitive = new(StringComparer.OrdinalIgnoreCase) { ["Ada"] = 36 };
        Out("insensitive[\"ADA\"]", insensitive["ADA"]);

        Section("HashSet: a bag of unique values");

        HashSet<string> visited = ["London", "Paris", "London", "Tokyo"];
        Out("HashSet from 4 items with a duplicate", visited.Count);
        Out("Add(\"Paris\") again", visited.Add("Paris"));
        Out("Add(\"Rome\")", visited.Add("Rome"));
        Out("Contains(\"Tokyo\")", visited.Contains("Tokyo"));

        Section("Set operations");

        HashSet<int> a = [1, 2, 3, 4, 5];
        HashSet<int> b = [4, 5, 6, 7];

        HashSet<int> union = new(a); union.UnionWith(b);
        HashSet<int> intersect = new(a); intersect.IntersectWith(b);
        HashSet<int> difference = new(a); difference.ExceptWith(b);

        Out("a", string.Join(", ", a));
        Out("b", string.Join(", ", b));
        Out("union", string.Join(", ", union));
        Out("intersection", string.Join(", ", intersect));
        Out("a except b", string.Join(", ", difference));
        Out("a.IsSubsetOf(union)", a.IsSubsetOf(union));

        Section("Why hashing is fast");

        Out("List.Contains", "O(n) - checks every item one by one");
        Out("HashSet.Contains", "O(1) - jumps straight to a bucket using the hash code");
        Out("Dictionary lookup", "O(1) for the same reason");

        // Proof, with a big collection.
        List<int> bigList = Enumerable.Range(0, 200000).ToList();
        HashSet<int> bigSet = new(bigList);

        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 2000; i++) bigList.Contains(199999);
        long listMs = watch.ElapsedMilliseconds;

        watch.Restart();
        for (int i = 0; i < 2000; i++) bigSet.Contains(199999);

        Out("2000 List.Contains on 200k items", $"{listMs} ms");
        Out("2000 HashSet.Contains on 200k items", $"{watch.ElapsedMilliseconds} ms");

        Section("Other keyed collections");

        SortedDictionary<string, int> sorted = new(ages);
        Out("SortedDictionary keys (always in order)", string.Join(", ", sorted.Keys));
    }
}

public readonly record struct Point(int X, int Y);
