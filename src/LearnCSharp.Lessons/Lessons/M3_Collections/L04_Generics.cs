using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Collections;

// Notes: docs/module-3/3.4.md
public sealed class L04_Generics : LessonBase
{
    public override string Id => "3.4";
    public override string Title => "Generics: one algorithm, any type";

    public override string Summary =>
        "Write an algorithm once and use it with any type, keeping full compile-time safety "
        + "and needing no casts.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Write generic classes and generic methods",
        "Apply constraints to demand capabilities of T",
        "Explain why generics are both faster and safer than using object",
    ];

    public override void Run()
    {
        Section("The problem generics solve");

        ObjectBox loose = new ObjectBox();
        loose.Item = "hello";
        // Everything comes back as object, so you must cast - and casts can fail at run time.
        Out("(string)loose.Item", (string)loose.Item!);

        try { _ = (int)loose.Item!; }
        catch (InvalidCastException) { Out("(int)loose.Item", "InvalidCastException at RUN time"); }

        Section("A generic class fixes it at COMPILE time");

        Box<string> words = new("hello");
        Box<int> numbers = new(42);

        Out("words.Item", words.Item);
        Out("numbers.Item", numbers.Item);
        Out("no cast needed, length is", words.Item.Length);
        // numbers.Item = "oops";   <- will not compile

        Section("Generic methods");

        int[] ints = [3, 1, 2];
        string[] strings = ["c", "a", "b"];

        Out("First(ints)", First(ints));
        Out("First(strings)", First(strings));
        Out("Swap on ints", string.Join(", ", SwapEnds(ints)));
        Out("Swap on strings", string.Join(", ", SwapEnds(strings)));

        // The compiler infers T, but you may state it: First<int>(ints)
        Out("explicit type argument", First<string>(strings));

        Section("Constraints: demanding capabilities of T");

        Out("Largest([3, 9, 2])", Largest([3, 9, 2]));
        Out("Largest([\"pear\", \"apple\"])", Largest(["pear", "apple"]));
        Out("Largest([2.5, 1.5])", Largest([2.5, 1.5]));
        // Largest(new[] { new object() });   <- will not compile: object is not IComparable

        Section("The common constraints");

        Out("where T : struct", "T must be a value type");
        Out("where T : class", "T must be a reference type");
        Out("where T : new()", "T must have a parameterless constructor");
        Out("where T : IComparable<T>", "T must implement that interface");
        Out("where T : Animal", "T must be Animal or inherit it");
        Out("where T : notnull", "T cannot be a nullable type");

        Section("A generic stack, written from scratch");

        SimpleStack<string> stack = new();
        stack.Push("a");
        stack.Push("b");
        stack.Push("c");

        Out("Count", stack.Count);
        Out("Pop", stack.Pop());
        Out("Peek", stack.Peek());
        Out("Count", stack.Count);

        Section("Multiple type parameters");

        Pair<string, int> score = new("Ada", 91);
        Out("pair", score);
        Out("swapped", score.Swap());

        Section("Generics cost nothing at run time");

        Note("For value types the compiler produces a specialised version per type, so List<int> "
           + "stores real ints with no boxing. That makes generics faster AND safer than the old "
           + "object-based collections.");

        Section("Where you already use generics");

        Out("List<T>, Dictionary<K,V>", "collections");
        Out("Nullable<T>, written T?", "a value type that may be null");
        Out("Func<T, TResult>, Action<T>", "delegates - lesson 4.1");
        Out("Task<T>", "an async result - lesson 4.9");
        Out("IEnumerable<T>", "anything you can foreach - lesson 3.5");
    }

    private static T First<T>(T[] items) => items[0];

    private static T[] SwapEnds<T>(T[] items)
    {
        T[] copy = (T[])items.Clone();
        (copy[0], copy[^1]) = (copy[^1], copy[0]);
        return copy;
    }

    // The constraint is what lets us call CompareTo: without it, T could be anything.
    private static T Largest<T>(T[] items) where T : IComparable<T>
    {
        T best = items[0];
        foreach (T item in items)
            if (item.CompareTo(best) > 0) best = item;
        return best;
    }
}

/// <summary>The bad old way: everything is an object, so everything needs a cast.</summary>
public class ObjectBox
{
    public object? Item { get; set; }
}

/// <summary>The generic way: T is filled in by whoever uses the class.</summary>
public class Box<T>
{
    public Box(T item) => Item = item;

    public T Item { get; set; }

    public override string ToString() => $"Box<{typeof(T).Name}>({Item})";
}

/// <summary>A stack built on an array, to show there is no magic in Stack<T>.</summary>
public class SimpleStack<T>
{
    private T[] _items = new T[4];

    public int Count { get; private set; }

    public void Push(T item)
    {
        if (Count == _items.Length) Array.Resize(ref _items, _items.Length * 2);
        _items[Count] = item;
        Count++;
    }

    public T Pop()
    {
        if (Count == 0) throw new InvalidOperationException("The stack is empty.");
        Count--;
        return _items[Count];
    }

    public T Peek() => Count == 0
        ? throw new InvalidOperationException("The stack is empty.")
        : _items[Count - 1];
}

public record Pair<TFirst, TSecond>(TFirst First, TSecond Second)
{
    public Pair<TSecond, TFirst> Swap() => new(Second, First);
}
