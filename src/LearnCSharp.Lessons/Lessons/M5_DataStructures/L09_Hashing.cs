using LearnCSharp.Core;

namespace LearnCSharp.Lessons.DataStructures;

// Notes: docs/module-5/5.9.md
public sealed class L09_Hashing : LessonBase
{
    public override string Id => "5.9";
    public override string Title => "Hashing and hash tables";

    public override string Summary =>
        "Compute an index from the data itself and jump straight to it. Collisions are "
        + "mathematically unavoidable, and chaining and probing are the two answers.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Explain how a hash function produces an index",
        "Compare chaining with open addressing",
        "State the contract between Equals and GetHashCode",
    ];

    public override void Run()
    {
        Section("The idea");

        Out("problem", "finding a value in a list means checking items one by one - O(n)");
        Out("idea", "compute the index from the value itself, and jump straight there - O(1)");
        Out("hash function", "turns a key into a number");
        Out("index", "hash % tableSize, so it lands inside the array");

        Section("A very simple hash function");

        foreach (string name in new[] { "Ada", "Ben", "Cara", "Dev" })
        {
            int hash = SimpleHash(name);
            Out($"\"{name}\"", $"hash {hash}, index {hash % 10} in a table of 10");
        }

        Section("What makes a hash function good");

        Out("deterministic", "the same key must always give the same hash");
        Out("fast", "otherwise you lose the advantage you were buying");
        Out("well spread", "similar keys should land far apart, filling the table evenly");
        Out("uses the whole key", "not just the first letter, or everything starting 'S' collides");

        Section("Collisions are unavoidable");

        Out("pigeonhole principle", "infinitely many keys, finitely many slots - clashes MUST happen");
        Out("birthday paradox", "with 23 people there is a 50% chance two share a birthday");

        Out("SimpleHash(\"abc\")", SimpleHash("abc"));
        Out("SimpleHash(\"cba\") - collides!", SimpleHash("cba"));
        Note("A pure sum of letters ignores ORDER, so every anagram collides. A real hash multiplies "
           + "by a prime as it goes, which fixes exactly this.");

        Out("BetterHash(\"abc\")", BetterHash("abc"));
        Out("BetterHash(\"cba\")", BetterHash("cba"));

        Section("Solution 1: separate chaining (what .NET uses)");

        Line();
        Line("  index 0: -> (\"Ada\", 91)");
        Line("  index 1: -> null");
        Line("  index 2: -> (\"Ben\", 64) -> (\"Cara\", 78)     <- both hashed to 2");
        Line("  index 3: -> null");

        ChainedHashTable table = new(size: 5);
        table.Put("Ada", 91);
        table.Put("Ben", 64);
        table.Put("Cara", 78);
        table.Put("Dev", 55);
        table.Put("Eve", 88);

        Line();
        table.Print();

        Out("Get(\"Cara\")", table.Get("Cara"));
        Out("Get(\"Nobody\")", table.Get("Nobody"));
        Out("longest chain", table.LongestChain());
        Out("load factor", table.LoadFactor.ToString("F2"));

        Section("Solution 2: open addressing (linear probing)");

        Line();
        Line("  slot taken? just try the next one along, wrapping round the end.");

        ProbingHashTable probing = new(size: 7);
        foreach ((string name, int mark) in new[] { ("Ada", 91), ("Ben", 64), ("Cara", 78), ("Dev", 55) })
            probing.Put(name, mark);

        Line();
        probing.Print();
        Out("Get(\"Dev\")", probing.Get("Dev"));
        Out("probes needed for Dev", probing.LastProbeCount);

        Section("Load factor: how full is too full?");

        Out("load factor", "items / slots");
        Out("below 0.7", "collisions are rare, lookups stay near O(1)");
        Out("above 0.7", "chains lengthen, probing degrades - time to RESIZE");
        Out("resizing", "make a bigger table and re-hash EVERY key, because index = hash % newSize");

        ChainedHashTable filling = new(size: 4);
        foreach (string name in new[] { "a", "b", "c", "d", "e", "f" })
        {
            filling.Put(name, 1);
            Out($"after {filling.Count} items", $"load factor {filling.LoadFactor:F2}, longest chain {filling.LongestChain()}");
        }

        Section("Complexity");

        Out("average case", "O(1) for insert, lookup and delete - the whole point");
        Out("worst case", "O(n), when every key collides into one chain");
        Out("in practice", "O(1), because good hash functions plus resizing keep it that way");
        Out("what you give up", "ORDER. A hash table has none. Use SortedDictionary if you need it.");

        Section("C#'s GetHashCode");

        Out("\"Ada\".GetHashCode()", "a large int - randomised per process for security");
        Out("42.GetHashCode()", 42.GetHashCode());
        Out("true.GetHashCode()", true.GetHashCode());
        Out("HashCode.Combine(1, 2)", HashCode.Combine(1, 2));

        Warn("String hash codes are deliberately randomised on each run, so NEVER save one to a file "
           + "or use it as a database key. It will be different next time the program starts.");

        Section("The contract you must honour");

        Out("rule 1", "if a.Equals(b) then a.GetHashCode() == b.GetHashCode() - MANDATORY");
        Out("rule 2", "equal hash codes do NOT imply equality - that is just a collision");
        Out("rule 3", "the hash must not change while the object is in a dictionary");

        // Breaking rule 3 loses the object entirely.
        Dictionary<MutableKey, string> broken = new();
        MutableKey key = new("original");
        broken[key] = "the value";

        Out("found before mutating", broken.ContainsKey(key));
        key.Name = "changed";                        // the hash code just changed
        Out("found after mutating", broken.ContainsKey(key));
        Warn("The entry is still in the dictionary but sits in the wrong bucket, so it can never be "
           + "found again. This is why dictionary keys should be immutable - use a record or a string.");

        Section("Where hashing is used beyond dictionaries");

        Out("password storage", "store the hash, never the password (with bcrypt/argon2, plus a salt)");
        Out("file integrity", "SHA-256 checksums prove a download was not corrupted");
        Out("git", "every commit is identified by the hash of its contents");
        Out("caching", "hash the request to make the cache key");
        Out("deduplication", "identical files produce identical hashes");

        Note("Cryptographic hashes (SHA-256) and hash-table hashes (GetHashCode) solve different "
           + "problems. Cryptographic ones must be slow and impossible to reverse; table ones must "
           + "be as fast as possible. Never swap them.");
    }

    /// <summary>Deliberately poor: it ignores letter order, so all anagrams collide.</summary>
    private static int SimpleHash(string key)
    {
        int total = 0;
        foreach (char c in key) total += c;
        return total;
    }

    /// <summary>Multiplying by a prime as it goes makes position matter.</summary>
    private static int BetterHash(string key)
    {
        int hash = 17;
        foreach (char c in key) hash = hash * 31 + c;
        return Math.Abs(hash);
    }
}

/// <summary>A hash table where each slot holds a list of entries - the collision strategy .NET uses.</summary>
public class ChainedHashTable
{
    private readonly List<(string Key, int Value)>[] _buckets;

    public ChainedHashTable(int size)
    {
        _buckets = new List<(string, int)>[size];
        for (int i = 0; i < size; i++) _buckets[i] = new List<(string, int)>();
    }

    public int Count { get; private set; }

    public double LoadFactor => (double)Count / _buckets.Length;

    private int IndexFor(string key)
    {
        int hash = 17;
        foreach (char c in key) hash = hash * 31 + c;
        return Math.Abs(hash) % _buckets.Length;
    }

    public void Put(string key, int value)
    {
        List<(string Key, int Value)> bucket = _buckets[IndexFor(key)];

        for (int i = 0; i < bucket.Count; i++)
        {
            if (bucket[i].Key == key) { bucket[i] = (key, value); return; }   // overwrite
        }

        bucket.Add((key, value));
        Count++;
    }

    public int? Get(string key)
    {
        foreach ((string k, int v) in _buckets[IndexFor(key)])
            if (k == key) return v;

        return null;
    }

    public int LongestChain() => _buckets.Max(b => b.Count);

    public void Print()
    {
        for (int i = 0; i < _buckets.Length; i++)
        {
            string contents = _buckets[i].Count == 0
                ? "(empty)"
                : string.Join(" -> ", _buckets[i].Select(e => $"(\"{e.Key}\", {e.Value})"));

            Console.WriteLine($"      slot {i}: {contents}");
        }
    }
}

/// <summary>Open addressing: on a collision, walk forward to the next free slot.</summary>
public class ProbingHashTable
{
    private readonly (string Key, int Value)?[] _slots;

    public ProbingHashTable(int size) => _slots = new (string, int)?[size];

    public int LastProbeCount { get; private set; }

    private int IndexFor(string key)
    {
        int hash = 17;
        foreach (char c in key) hash = hash * 31 + c;
        return Math.Abs(hash) % _slots.Length;
    }

    public void Put(string key, int value)
    {
        int index = IndexFor(key);

        for (int probe = 0; probe < _slots.Length; probe++)
        {
            int slot = (index + probe) % _slots.Length;

            if (_slots[slot] is null || _slots[slot]!.Value.Key == key)
            {
                _slots[slot] = (key, value);
                return;
            }
        }

        throw new InvalidOperationException("The table is full.");
    }

    public int? Get(string key)
    {
        int index = IndexFor(key);
        LastProbeCount = 0;

        for (int probe = 0; probe < _slots.Length; probe++)
        {
            LastProbeCount++;
            int slot = (index + probe) % _slots.Length;

            if (_slots[slot] is null) return null;                    // an empty slot means: not here
            if (_slots[slot]!.Value.Key == key) return _slots[slot]!.Value.Value;
        }

        return null;
    }

    public void Print()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            string contents = _slots[i] is null
                ? "(empty)"
                : $"(\"{_slots[i]!.Value.Key}\", {_slots[i]!.Value.Value})   home slot {IndexFor(_slots[i]!.Value.Key)}";

            Console.WriteLine($"      slot {i}: {contents}");
        }
    }
}

/// <summary>A key whose hash code can change - which is exactly what you must never do.</summary>
public class MutableKey
{
    public MutableKey(string name) => Name = name;

    public string Name { get; set; }

    public override bool Equals(object? obj) => obj is MutableKey other && other.Name == Name;

    public override int GetHashCode() => Name.GetHashCode();
}
