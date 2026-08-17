using LearnCSharp.Core;

namespace LearnCSharp.Lessons.DataStructures;

// Notes: docs/module-5/5.5.md
public sealed class L05_LinkedLists : LessonBase
{
    public override string Id => "5.5";
    public override string Title => "Linked lists, built from scratch";

    public override void Run()
    {
        Section("The idea");

        Out("array", "one block of memory - index arithmetic finds any item instantly");
        Out("linked list", "separate nodes, each holding a value and a pointer to the next");
        Out("consequence", "no index maths, so no O(1) access - but inserting is cheap");

        Line();
        Line("  head");
        Line("   |");
        Line("   v");
        Line(" [10|.]-->[20|.]-->[30|/]        '/' means null: the end");

        Section("Building one");

        SinglyLinkedList<int> list = new();
        list.AddLast(10);
        list.AddLast(20);
        list.AddLast(30);

        Out("list", list.ToString());
        Out("Count", list.Count);
        Out("First", list.First);
        Out("Last", list.Last);

        Section("Adding at the front is O(1)");

        list.AddFirst(5);
        Out("after AddFirst(5)", list.ToString());
        Note("With an array you would have to shuffle every item one place right - O(n). "
           + "Here you just point the new node at the old head.");

        Section("Inserting in the middle");

        list.InsertAfter(20, 25);
        Out("after InsertAfter(20, 25)", list.ToString());

        Section("Removing");

        Out("Remove(5)", list.Remove(5));
        Out("list", list.ToString());
        Out("Remove(999) - not there", list.Remove(999));

        Section("Searching is O(n) - you must walk the chain");

        Out("Contains(25)", list.Contains(25));
        Out("IndexOf(30)", list.IndexOf(30));
        Out("IndexOf(99)", list.IndexOf(99));

        Section("Reversing a linked list - the classic interview question");

        Out("before", list.ToString());
        list.Reverse();
        Out("after Reverse()", list.ToString());

        Section("It works with any type");

        SinglyLinkedList<string> words = new();
        words.AddLast("never");
        words.AddLast("gonna");
        words.AddLast("give");
        Out("string list", words.ToString());

        Section("foreach works because we implemented IEnumerable");

        int total = 0;
        foreach (int value in list) total += value;
        Out("sum via foreach", total);
        Out("LINQ works too", string.Join(", ", list.Where(v => v > 20)));

        Section("Doubly linked: each node also points backwards");

        Line();
        Line(" null<--[10|.]<-->[20|.]<-->[30|.]-->null");

        DoublyLinkedList<int> doubly = new();
        doubly.AddLast(10);
        doubly.AddLast(20);
        doubly.AddLast(30);

        Out("forwards", doubly.ToForwardString());
        Out("backwards", doubly.ToBackwardString());
        Out("RemoveLast()", doubly.RemoveLast());
        Out("after removing", doubly.ToForwardString());

        Note("The extra pointer costs memory but makes it possible to walk backwards and to remove "
           + "a known node in O(1). That is how a browser's back/forward history works.");

        Section("Array vs linked list");

        Line();
        Line($"{"operation",-28}{"array/List<T>",-16}{"linked list"}");
        Line($"{"access by index",-28}{"O(1)",-16}{"O(n)"}");
        Line($"{"insert at front",-28}{"O(n)",-16}{"O(1)"}");
        Line($"{"insert at end",-28}{"O(1) amortised",-16}{"O(1) with a tail pointer"}");
        Line($"{"insert in middle",-28}{"O(n)",-16}{"O(1) once you are there"}");
        Line($"{"search",-28}{"O(n)",-16}{"O(n)"}");
        Line($"{"memory per item",-28}{"just the value",-16}{"value + pointer(s)"}");
        Line($"{"cache friendliness",-28}{"excellent",-16}{"poor"}");

        Note("In real C# you would use List<T> almost always. Modern CPUs love contiguous memory so "
           + "much that arrays usually beat linked lists even where the theory says otherwise. "
           + "Learn linked lists because they teach pointers, and because trees are built the same way.");

        Section(".NET's own version");

        LinkedList<int> builtIn = new([1, 2, 3]);
        builtIn.AddFirst(0);
        Out("LinkedList<int>", string.Join(" -> ", builtIn));
        Out("First.Value", builtIn.First?.Value);
        Out("First.Next.Value", builtIn.First?.Next?.Value);
    }
}

/// <summary>One link in the chain: a value, plus where to go next.</summary>
public class Node<T>
{
    public Node(T value) => Value = value;

    public T Value { get; set; }
    public Node<T>? Next { get; set; }
}

public class SinglyLinkedList<T> : IEnumerable<T> where T : IEquatable<T>
{
    private Node<T>? _head;
    private Node<T>? _tail;                        // keeping a tail makes AddLast O(1)

    public int Count { get; private set; }

    public T? First => _head is null ? default : _head.Value;
    public T? Last => _tail is null ? default : _tail.Value;

    public void AddFirst(T value)
    {
        Node<T> node = new(value) { Next = _head };
        _head = node;
        _tail ??= node;                            // the first node is also the last
        Count++;
    }

    public void AddLast(T value)
    {
        Node<T> node = new(value);

        if (_tail is null) _head = node;
        else _tail.Next = node;

        _tail = node;
        Count++;
    }

    public bool InsertAfter(T target, T value)
    {
        Node<T>? current = _head;

        while (current is not null)
        {
            if (current.Value.Equals(target))
            {
                // Point the new node at the rest of the list, then re-point the current one.
                current.Next = new Node<T>(value) { Next = current.Next };
                if (ReferenceEquals(current, _tail)) _tail = current.Next;
                Count++;
                return true;
            }
            current = current.Next;
        }

        return false;
    }

    public bool Remove(T value)
    {
        Node<T>? current = _head;
        Node<T>? previous = null;

        while (current is not null)
        {
            if (current.Value.Equals(value))
            {
                if (previous is null) _head = current.Next;   // removing the head
                else previous.Next = current.Next;            // skip over the node

                if (ReferenceEquals(current, _tail)) _tail = previous;
                Count--;
                return true;
            }

            previous = current;
            current = current.Next;
        }

        return false;
    }

    public bool Contains(T value) => IndexOf(value) >= 0;

    public int IndexOf(T value)
    {
        Node<T>? current = _head;
        int index = 0;

        while (current is not null)
        {
            if (current.Value.Equals(value)) return index;
            current = current.Next;
            index++;
        }

        return -1;
    }

    /// <summary>Re-points every arrow backwards, in a single pass.</summary>
    public void Reverse()
    {
        Node<T>? previous = null;
        Node<T>? current = _head;
        _tail = _head;

        while (current is not null)
        {
            Node<T>? next = current.Next;          // remember where we were going
            current.Next = previous;               // turn the arrow around
            previous = current;                    // shuffle both pointers forward
            current = next;
        }

        _head = previous;
    }

    public IEnumerator<T> GetEnumerator()
    {
        Node<T>? current = _head;
        while (current is not null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => Count == 0 ? "(empty)" : string.Join(" -> ", this);
}

public class DoubleNode<T>
{
    public DoubleNode(T value) => Value = value;

    public T Value { get; set; }
    public DoubleNode<T>? Next { get; set; }
    public DoubleNode<T>? Previous { get; set; }
}

public class DoublyLinkedList<T>
{
    private DoubleNode<T>? _head;
    private DoubleNode<T>? _tail;

    public int Count { get; private set; }

    public void AddLast(T value)
    {
        DoubleNode<T> node = new(value) { Previous = _tail };

        if (_tail is null) _head = node;
        else _tail.Next = node;

        _tail = node;
        Count++;
    }

    /// <summary>O(1), because the tail knows what came before it.</summary>
    public T? RemoveLast()
    {
        if (_tail is null) return default;

        T value = _tail.Value;
        _tail = _tail.Previous;

        if (_tail is null) _head = null;
        else _tail.Next = null;

        Count--;
        return value;
    }

    public string ToForwardString()
    {
        List<string> parts = new();
        for (DoubleNode<T>? n = _head; n is not null; n = n.Next) parts.Add($"{n.Value}");
        return parts.Count == 0 ? "(empty)" : string.Join(" <-> ", parts);
    }

    public string ToBackwardString()
    {
        List<string> parts = new();
        for (DoubleNode<T>? n = _tail; n is not null; n = n.Previous) parts.Add($"{n.Value}");
        return parts.Count == 0 ? "(empty)" : string.Join(" <-> ", parts);
    }
}
