using LearnCSharp.Core;

namespace LearnCSharp.Lessons.DataStructures;

// Notes: docs/module-5/5.6.md
public sealed class L06_StacksQueuesFromScratch : LessonBase
{
    public override string Id => "5.6";
    public override string Title => "Stacks and queues, built from scratch";

    public override void Run()
    {
        Section("A stack on an array");

        ArrayStack<int> stack = new(capacity: 4);
        Out("IsEmpty", stack.IsEmpty);

        stack.Push(10);
        stack.Push(20);
        stack.Push(30);
        Out("after pushing 10, 20, 30", stack.ToString());
        Out("Peek", stack.Peek());
        Out("Pop", stack.Pop());
        Out("Pop", stack.Pop());
        Out("now", stack.ToString());

        try { new ArrayStack<int>(2).Pop(); }
        catch (InvalidOperationException ex) { Out("Pop on empty", ex.Message); }

        Section("It grows automatically");

        ArrayStack<int> growing = new(capacity: 2);
        for (int i = 1; i <= 9; i++) growing.Push(i);
        Out("pushed 9 items into capacity 2", growing.ToString());
        Out("capacity now", growing.Capacity);

        Section("A stack on a linked list - no resizing needed");

        LinkedStack<string> undo = new();
        undo.Push("type hello");
        undo.Push("bold it");
        undo.Push("delete a word");

        Out("undo history", undo.ToString());
        Out("undo!", undo.Pop());
        Out("undo!", undo.Pop());
        Out("left", undo.ToString());

        Section("A queue on a CIRCULAR array");

        Out("the naive approach", "dequeue from index 0, then shuffle everything left - O(n)");
        Out("the fix", "keep a head and a tail index that wrap around with %  - O(1)");

        CircularQueue<string> queue = new(capacity: 4);
        queue.Enqueue("Ada");
        queue.Enqueue("Ben");
        queue.Enqueue("Cara");
        Out("after 3 enqueues", queue.Describe());

        Out("Dequeue", queue.Dequeue());
        Out("Dequeue", queue.Dequeue());
        Out("state", queue.Describe());

        queue.Enqueue("Dev");
        queue.Enqueue("Eve");
        Out("after 2 more enqueues (wrapped round)", queue.Describe());

        Out("Peek", queue.Peek());
        Out("Count", queue.Count);
        Out("IsFull", queue.IsFull);

        try { queue.Enqueue("too many"); }
        catch (InvalidOperationException ex) { Out("Enqueue when full", ex.Message); }

        Section("Using our stack: reverse Polish notation");

        foreach (string expression in new[] { "3 4 +", "5 1 2 + 4 * + 3 -", "2 3 4 * +" })
            Out($"\"{expression}\"", EvaluateRpn(expression));

        Section("Using our stack: decimal to binary");

        foreach (int number in new[] { 5, 10, 42, 255 })
            Out($"{number} in binary", ToBinary(number));

        Section("Using our queue: a round-robin scheduler");

        Line();
        RoundRobin(["task A (3)", "task B (1)", "task C (2)"], [3, 1, 2]);

        Section("Where each one is used for real");

        Out("stack", "the call stack, undo history, browser back, expression parsing, DFS");
        Out("queue", "print spoolers, keyboard buffers, message queues, BFS, task scheduling");
    }

    /// <summary>Reverse Polish: operands go on the stack, an operator pops two and pushes the result.</summary>
    private static int EvaluateRpn(string expression)
    {
        ArrayStack<int> values = new(8);

        foreach (string token in expression.Split(' '))
        {
            if (int.TryParse(token, out int number))
            {
                values.Push(number);
            }
            else
            {
                int right = values.Pop();          // note: the SECOND operand comes off first
                int left = values.Pop();

                values.Push(token switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "*" => left * right,
                    "/" => left / right,
                    _ => throw new ArgumentException($"Unknown operator {token}"),
                });
            }
        }

        return values.Pop();
    }

    /// <summary>Repeated division gives the bits backwards - which is exactly what a stack fixes.</summary>
    private static string ToBinary(int number)
    {
        if (number == 0) return "0";

        ArrayStack<int> bits = new(32);
        while (number > 0)
        {
            bits.Push(number % 2);
            number /= 2;
        }

        System.Text.StringBuilder result = new();
        while (!bits.IsEmpty) result.Append(bits.Pop());
        return result.ToString();
    }

    private static void RoundRobin(string[] names, int[] work)
    {
        CircularQueue<int> ready = new(names.Length);
        for (int i = 0; i < names.Length; i++) ready.Enqueue(i);

        int[] remaining = (int[])work.Clone();
        int slice = 0;

        while (ready.Count > 0)
        {
            int job = ready.Dequeue();
            remaining[job]--;
            slice++;

            Console.WriteLine($"      slice {slice}: ran {names[job]}, {remaining[job]} left");

            if (remaining[job] > 0) ready.Enqueue(job);       // not finished - back to the end
        }
    }
}

/// <summary>A stack over an array. Push and Pop both work at the END, so both are O(1).</summary>
public class ArrayStack<T>
{
    private T[] _items;

    public ArrayStack(int capacity) => _items = new T[Math.Max(1, capacity)];

    public int Count { get; private set; }
    public int Capacity => _items.Length;
    public bool IsEmpty => Count == 0;

    public void Push(T item)
    {
        if (Count == _items.Length)
            Array.Resize(ref _items, _items.Length * 2);      // double when full

        _items[Count] = item;
        Count++;
    }

    public T Pop()
    {
        if (IsEmpty) throw new InvalidOperationException("The stack is empty.");

        Count--;
        T item = _items[Count];
        _items[Count] = default!;                             // let the GC reclaim it
        return item;
    }

    public T Peek() => IsEmpty
        ? throw new InvalidOperationException("The stack is empty.")
        : _items[Count - 1];

    public override string ToString() =>
        IsEmpty ? "(empty)" : $"bottom [{string.Join(", ", _items.Take(Count))}] top";
}

/// <summary>A stack over a linked list. Push and Pop both work at the HEAD.</summary>
public class LinkedStack<T>
{
    private StackNode? _top;

    public int Count { get; private set; }
    public bool IsEmpty => _top is null;

    public void Push(T item)
    {
        _top = new StackNode(item, _top);
        Count++;
    }

    public T Pop()
    {
        if (_top is null) throw new InvalidOperationException("The stack is empty.");

        T value = _top.Value;
        _top = _top.Below;
        Count--;
        return value;
    }

    public override string ToString()
    {
        List<string> parts = new();
        for (StackNode? n = _top; n is not null; n = n.Below) parts.Add($"{n.Value}");
        return parts.Count == 0 ? "(empty)" : $"top [{string.Join(", ", parts)}] bottom";
    }

    private sealed class StackNode
    {
        public StackNode(T value, StackNode? below)
        {
            Value = value;
            Below = below;
        }

        public T Value { get; }
        public StackNode? Below { get; }
    }
}

/// <summary>
/// A fixed-size queue on a circular array. _head is where to read, _tail is where to write,
/// and both wrap round with % so no shuffling is ever needed.
/// </summary>
public class CircularQueue<T>
{
    private readonly T[] _items;
    private int _head;
    private int _tail;

    public CircularQueue(int capacity) => _items = new T[capacity];

    public int Count { get; private set; }
    public bool IsEmpty => Count == 0;
    public bool IsFull => Count == _items.Length;

    public void Enqueue(T item)
    {
        if (IsFull) throw new InvalidOperationException("The queue is full.");

        _items[_tail] = item;
        _tail = (_tail + 1) % _items.Length;       // wrap round to 0 after the last slot
        Count++;
    }

    public T Dequeue()
    {
        if (IsEmpty) throw new InvalidOperationException("The queue is empty.");

        T item = _items[_head];
        _items[_head] = default!;
        _head = (_head + 1) % _items.Length;
        Count--;
        return item;
    }

    public T Peek() => IsEmpty
        ? throw new InvalidOperationException("The queue is empty.")
        : _items[_head];

    public string Describe()
    {
        List<string> order = new();
        for (int i = 0; i < Count; i++) order.Add($"{_items[(_head + i) % _items.Length]}");

        string slots = string.Join(", ", _items.Select(x => x?.ToString() ?? "_"));
        return $"front [{string.Join(", ", order)}] back   (array: {slots}, head={_head}, tail={_tail})";
    }
}
