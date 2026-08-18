// Module 5 - Data Structures and Algorithms (AQA topics in C#). Everything built by hand -
// no LINQ shortcuts for the algorithm itself.

/** @type {import('./index').Worksheet} */
export default {
  module: 5,
  intro:
    'Searching, sorting, recursion and the classic data structures - built from scratch so you can see the mechanism, the way the AQA specification expects.',
  tasks: [
    {
      id: '5.1',
      lesson: '5.2',
      title: 'Linear search',
      level: 1,
      task: 'Write `int IndexOf(int[] data, int target)` that returns the index of the first match, or -1 if the target is not present. Do not use Array.IndexOf.',
      starter: `int[] data = [4, 8, 15, 16, 23, 42];

Console.WriteLine(IndexOf(data, 16));
Console.WriteLine(IndexOf(data, 99));

int IndexOf(int[] data, int target)
{
    // TODO: check each index in turn
    return -1;
}`,
      expected: `3
-1`,
      hints: [
        'Loop i from 0 to data.Length - 1; if (data[i] == target) return i;',
        'Only return -1 after the loop finishes without a match.',
      ],
      solution: `int[] data = [4, 8, 15, 16, 23, 42];

Console.WriteLine(IndexOf(data, 16));
Console.WriteLine(IndexOf(data, 99));

int IndexOf(int[] data, int target)
{
    for (int i = 0; i < data.Length; i++)
    {
        if (data[i] == target) return i;
    }
    return -1;
}`,
    },
    {
      id: '5.2',
      lesson: '5.2',
      title: 'Binary search',
      level: 2,
      task: 'Write `int BinarySearch(int[] sorted, int target)` on an already-sorted array, returning the index or -1. Keep `low` and `high` pointers and halve the range each step.',
      starter: `int[] sorted = [1, 3, 5, 7, 9, 11, 13, 15];

foreach (int target in new[] { 7, 1, 15, 8 })
{
    Console.WriteLine($"{target} -> {BinarySearch(sorted, target)}");
}

int BinarySearch(int[] sorted, int target)
{
    // TODO: low/high pointers, look at the middle each time
    return -1;
}`,
      expected: `7 -> 3
1 -> 0
15 -> 7
8 -> -1`,
      hints: [
        'int low = 0, high = sorted.Length - 1; while (low <= high) { ... }',
        'int mid = (low + high) / 2; compare sorted[mid] with target.',
        'If target is larger, low = mid + 1; if smaller, high = mid - 1; if equal, return mid.',
      ],
      solution: `int[] sorted = [1, 3, 5, 7, 9, 11, 13, 15];

foreach (int target in new[] { 7, 1, 15, 8 })
{
    Console.WriteLine($"{target} -> {BinarySearch(sorted, target)}");
}

int BinarySearch(int[] sorted, int target)
{
    int low = 0, high = sorted.Length - 1;
    while (low <= high)
    {
        int mid = (low + high) / 2;
        if (sorted[mid] == target) return mid;
        if (sorted[mid] < target) low = mid + 1;
        else high = mid - 1;
    }
    return -1;
}`,
    },
    {
      id: '5.3',
      lesson: '5.3',
      title: 'Bubble sort',
      level: 2,
      task: 'Sort the array in place, smallest first, with bubble sort - repeatedly swapping neighbours that are out of order. Print it before and after. Do not call Array.Sort.',
      starter: `int[] data = [5, 2, 8, 1, 9, 3];
Console.WriteLine(string.Join(" ", data));

// TODO: bubble sort in place

Console.WriteLine(string.Join(" ", data));`,
      expected: `5 2 8 1 9 3
1 2 3 5 8 9`,
      hints: [
        'Two nested loops. The outer runs data.Length - 1 times.',
        'Inner loop: if (data[j] > data[j + 1]) swap them with a temporary.',
        '(data[j], data[j + 1]) = (data[j + 1], data[j]); is a neat swap.',
      ],
      solution: `int[] data = [5, 2, 8, 1, 9, 3];
Console.WriteLine(string.Join(" ", data));

for (int i = 0; i < data.Length - 1; i++)
{
    for (int j = 0; j < data.Length - 1 - i; j++)
    {
        if (data[j] > data[j + 1])
        {
            (data[j], data[j + 1]) = (data[j + 1], data[j]);
        }
    }
}

Console.WriteLine(string.Join(" ", data));`,
    },
    {
      id: '5.4',
      lesson: '5.4',
      title: 'Recursion',
      level: 2,
      task: 'Write two recursive methods: `long Factorial(int n)` and `int Gcd(int a, int b)` using Euclid\'s algorithm (`gcd(a, b) = gcd(b, a % b)`, and `gcd(a, 0) = a`). Print the results shown.',
      starter: `Console.WriteLine(Factorial(5));
Console.WriteLine(Gcd(48, 36));

long Factorial(int n)
{
    // TODO: base case then recursive case
    return 1;
}

int Gcd(int a, int b)
{
    // TODO: Euclid's algorithm, recursively
    return a;
}`,
      expected: `120
12`,
      hints: [
        'Factorial base case: n <= 1 returns 1; otherwise n * Factorial(n - 1).',
        'Gcd base case: b == 0 returns a; otherwise Gcd(b, a % b).',
        'Every recursion needs a base case it will actually reach, or it never stops.',
      ],
      solution: `Console.WriteLine(Factorial(5));
Console.WriteLine(Gcd(48, 36));

long Factorial(int n) => n <= 1 ? 1 : n * Factorial(n - 1);

int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);`,
    },
    {
      id: '5.5',
      lesson: '5.5',
      title: 'A stack from scratch',
      level: 3,
      task: 'Build a `Stack` of ints on top of a `List<int>` - with `Push`, `Pop` (removes and returns the top) and `Count`. Push 1, 2, 3 then pop everything, printing each value. Do not use System.Collections.Generic.Stack.',
      starter: `var stack = new IntStack();
stack.Push(1);
stack.Push(2);
stack.Push(3);

while (stack.Count > 0)
{
    Console.WriteLine(stack.Pop());
}

class IntStack
{
    private readonly List<int> _items = new();

    public int Count => _items.Count;

    // TODO: Push adds to the top, Pop removes and returns the top
    public void Push(int value) { }
    public int Pop() => 0;
}`,
      expected: `3
2
1`,
      hints: [
        'The "top" is the end of the list. Push: _items.Add(value).',
        'Pop: read _items[^1] (the last item), remove it with _items.RemoveAt(_items.Count - 1), and return it.',
        'Last in, first out - that is why 3 comes out first.',
      ],
      solution: `var stack = new IntStack();
stack.Push(1);
stack.Push(2);
stack.Push(3);

while (stack.Count > 0)
{
    Console.WriteLine(stack.Pop());
}

class IntStack
{
    private readonly List<int> _items = new();

    public int Count => _items.Count;

    public void Push(int value) => _items.Add(value);

    public int Pop()
    {
        int top = _items[^1];
        _items.RemoveAt(_items.Count - 1);
        return top;
    }
}`,
    },
    {
      id: '5.6',
      lesson: '5.7',
      title: 'Binary search tree',
      level: 3,
      task: 'Build a binary search tree of ints with `Insert`, then print an in-order traversal - which comes out sorted. Insert 5, 3, 8, 1, 4, 7, 9, 2 and print them space-separated on one line.',
      starter: `var tree = new Bst();
foreach (int n in new[] { 5, 3, 8, 1, 4, 7, 9, 2 }) tree.Insert(n);

Console.WriteLine(string.Join(" ", tree.InOrder()));

class Node
{
    public int Value;
    public Node? Left, Right;
    public Node(int value) => Value = value;
}

class Bst
{
    private Node? _root;

    public void Insert(int value)
    {
        // TODO: place value smaller-left, larger-right
    }

    public List<int> InOrder()
    {
        var result = new List<int>();
        // TODO: left, node, right (recursively)
        return result;
    }
}`,
      expected: `1 2 3 4 5 7 8 9`,
      hints: [
        'Insert: if the tree is empty set _root; otherwise walk down going Left when value < node.Value, Right otherwise, until the child is null.',
        'A private recursive helper is easiest: Insert(Node node, int value).',
        'In-order: visit Left, add node.Value, visit Right - that yields sorted order for a BST.',
      ],
      solution: `var tree = new Bst();
foreach (int n in new[] { 5, 3, 8, 1, 4, 7, 9, 2 }) tree.Insert(n);

Console.WriteLine(string.Join(" ", tree.InOrder()));

class Node
{
    public int Value;
    public Node? Left, Right;
    public Node(int value) => Value = value;
}

class Bst
{
    private Node? _root;

    public void Insert(int value)
    {
        _root = Insert(_root, value);
    }

    private Node Insert(Node? node, int value)
    {
        if (node is null) return new Node(value);
        if (value < node.Value) node.Left = Insert(node.Left, value);
        else node.Right = Insert(node.Right, value);
        return node;
    }

    public List<int> InOrder()
    {
        var result = new List<int>();
        Walk(_root, result);
        return result;
    }

    private void Walk(Node? node, List<int> result)
    {
        if (node is null) return;
        Walk(node.Left, result);
        result.Add(node.Value);
        Walk(node.Right, result);
    }
}`,
    },
  ],
};
