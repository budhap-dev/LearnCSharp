// Module 3 - Collections, Generics and LINQ.

/** @type {import('./index').Worksheet} */
export default {
  module: 3,
  intro:
    'Lists, dictionaries and sets, your own generic method, iterators with yield, and LINQ for filtering, projecting, grouping and aggregating.',
  tasks: [
    {
      id: '3.1',
      lesson: '3.1',
      title: 'Growing a list',
      level: 1,
      task: 'Start from the list of names. Add "Grace", remove "Alan", insert "Ada" at the front, and print the final count and the names joined by commas.',
      starter: `var names = new List<string> { "Alan", "Katherine", "Edsger" };

// TODO: add Grace, remove Alan, insert Ada at index 0

Console.WriteLine($"{names.Count} names");
Console.WriteLine(string.Join(", ", names));`,
      expected: `4 names
Ada, Katherine, Edsger, Grace`,
      hints: [
        'names.Add("Grace"); names.Remove("Alan"); names.Insert(0, "Ada");',
        'string.Join(", ", names) turns the list into one comma-separated string.',
      ],
      solution: `var names = new List<string> { "Alan", "Katherine", "Edsger" };

names.Add("Grace");
names.Remove("Alan");
names.Insert(0, "Ada");

Console.WriteLine($"{names.Count} names");
Console.WriteLine(string.Join(", ", names));`,
    },
    {
      id: '3.2',
      lesson: '3.2',
      title: 'Word frequency',
      level: 2,
      task: 'Count how many times each word appears in the sentence, then print each distinct word and its count in the order the words first appear. Use a `Dictionary<string, int>`.',
      starter: `string sentence = "the cat sat on the mat and the cat ran";

var counts = new Dictionary<string, int>();

// TODO: split into words, count each one

foreach (var pair in counts)
{
    Console.WriteLine($"{pair.Key}: {pair.Value}");
}`,
      expected: `the: 3
cat: 2
sat: 1
on: 1
mat: 1
and: 1
ran: 1`,
      hints: [
        'sentence.Split(\" \") gives the words.',
        'counts.TryGetValue(word, out int n) then counts[word] = n + 1; - or use counts[word] = counts.GetValueOrDefault(word) + 1;',
        'A Dictionary preserves insertion order when you only ever add, which is why the output is in first-seen order.',
      ],
      solution: `string sentence = "the cat sat on the mat and the cat ran";

var counts = new Dictionary<string, int>();

foreach (string word in sentence.Split(' '))
{
    counts[word] = counts.GetValueOrDefault(word) + 1;
}

foreach (var pair in counts)
{
    Console.WriteLine($"{pair.Key}: {pair.Value}");
}`,
    },
    {
      id: '3.3',
      lesson: '3.2',
      title: 'Unique in order',
      level: 1,
      task: 'Print the numbers keeping only the first time each value appears, in order. Use a `HashSet<int>` to remember what you have already seen.',
      starter: `int[] numbers = [3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5];

var seen = new HashSet<int>();

// TODO: print each number only the first time it appears
`,
      expected: `3 1 4 5 9 2 6`,
      hints: [
        'seen.Add(n) returns false if n was already in the set.',
        'Collect the fresh numbers, then string.Join(\" \", result) to print them on one line.',
      ],
      solution: `int[] numbers = [3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5];

var seen = new HashSet<int>();
var unique = new List<int>();

foreach (int n in numbers)
{
    if (seen.Add(n)) unique.Add(n);
}

Console.WriteLine(string.Join(" ", unique));`,
    },
    {
      id: '3.4',
      lesson: '3.4',
      title: 'A generic Swap and Middle',
      level: 2,
      task: 'Write a generic method `T Middle<T>(T[] items)` that returns the middle element of an odd-length array, and `void Swap<T>(ref T a, ref T b)`. Show both working on an int array and two strings.',
      starter: `int[] xs = [10, 20, 30, 40, 50];
Console.WriteLine(Middle(xs));

string first = "left", second = "right";
Swap(ref first, ref second);
Console.WriteLine($"{first} {second}");

// TODO: generic Middle and Swap
T Middle<T>(T[] items) => items[0];
void Swap<T>(ref T a, ref T b) { }`,
      expected: `30
right left`,
      hints: [
        'The middle index of an odd-length array is items.Length / 2 (integer division): 5 / 2 == 2.',
        'Swap needs a temporary: T temp = a; a = b; b = temp;',
        'One generic definition works for both int and string - that is the point of <T>.',
      ],
      solution: `int[] xs = [10, 20, 30, 40, 50];
Console.WriteLine(Middle(xs));

string first = "left", second = "right";
Swap(ref first, ref second);
Console.WriteLine($"{first} {second}");

T Middle<T>(T[] items) => items[items.Length / 2];

void Swap<T>(ref T a, ref T b)
{
    T temp = a;
    a = b;
    b = temp;
}`,
    },
    {
      id: '3.5',
      lesson: '3.5',
      title: 'An iterator with yield',
      level: 2,
      task: 'Write an iterator `IEnumerable<int> Fibonacci(int count)` that yields the first `count` Fibonacci numbers starting 0, 1. Print the first ten, space-separated.',
      starter: `Console.WriteLine(string.Join(" ", Fibonacci(10)));

IEnumerable<int> Fibonacci(int count)
{
    // TODO: yield return each number in turn
    yield break;
}`,
      expected: `0 1 1 2 3 5 8 13 21 34`,
      hints: [
        'Keep two variables a = 0, b = 1. Each step: yield return a; then (a, b) = (b, a + b);',
        'A for loop that runs count times controls how many you produce.',
        'yield return hands out one value and pauses until the next is asked for.',
      ],
      solution: `Console.WriteLine(string.Join(" ", Fibonacci(10)));

IEnumerable<int> Fibonacci(int count)
{
    int a = 0, b = 1;
    for (int i = 0; i < count; i++)
    {
        yield return a;
        (a, b) = (b, a + b);
    }
}`,
    },
    {
      id: '3.6',
      lesson: '3.6',
      title: 'LINQ: filter, sort, project',
      level: 2,
      task: 'From the list of people, use LINQ to find everyone aged 18 or over, ordered by age then name, and print "Name (age)" for each. Do it as one query chain - no loops for the logic.',
      starter: `var people = new[]
{
    (Name: "Ada", Age: 17),
    (Name: "Alan", Age: 41),
    (Name: "Grace", Age: 41),
    (Name: "Edsger", Age: 20),
    (Name: "Katherine", Age: 16),
};

// TODO: Where age >= 18, OrderBy age then name, Select "Name (age)"
IEnumerable<string> result = [];

foreach (string line in result) Console.WriteLine(line);`,
      expected: `Edsger (20)
Alan (41)
Grace (41)`,
      hints: [
        'people.Where(p => p.Age >= 18)',
        'Chain .OrderBy(p => p.Age).ThenBy(p => p.Name) for the tie-break.',
        '.Select(p => $"{p.Name} ({p.Age})") turns each person into the string.',
      ],
      solution: `var people = new[]
{
    (Name: "Ada", Age: 17),
    (Name: "Alan", Age: 41),
    (Name: "Grace", Age: 41),
    (Name: "Edsger", Age: 20),
    (Name: "Katherine", Age: 16),
};

IEnumerable<string> result = people
    .Where(p => p.Age >= 18)
    .OrderBy(p => p.Age)
    .ThenBy(p => p.Name)
    .Select(p => $"{p.Name} ({p.Age})");

foreach (string line in result) Console.WriteLine(line);`,
    },
    {
      id: '3.7',
      lesson: '3.7',
      title: 'LINQ: group and aggregate',
      level: 3,
      task: 'Group the sales by product, and for each product print its name and total quantity, ordered by total descending. Use `GroupBy` and `Sum`.',
      starter: `var sales = new[]
{
    (Product: "Pen", Qty: 3),
    (Product: "Book", Qty: 1),
    (Product: "Pen", Qty: 5),
    (Product: "Mug", Qty: 2),
    (Product: "Book", Qty: 4),
};

// TODO: GroupBy Product, Sum Qty, OrderByDescending the total
`,
      expected: `Pen: 8
Book: 5
Mug: 2`,
      hints: [
        'sales.GroupBy(s => s.Product) gives groups; each group has a Key and its items.',
        '.Select(g => (Product: g.Key, Total: g.Sum(s => s.Qty)))',
        'Finish with .OrderByDescending(x => x.Total) then a foreach to print.',
      ],
      solution: `var sales = new[]
{
    (Product: "Pen", Qty: 3),
    (Product: "Book", Qty: 1),
    (Product: "Pen", Qty: 5),
    (Product: "Mug", Qty: 2),
    (Product: "Book", Qty: 4),
};

var totals = sales
    .GroupBy(s => s.Product)
    .Select(g => (Product: g.Key, Total: g.Sum(s => s.Qty)))
    .OrderByDescending(x => x.Total);

foreach (var t in totals)
{
    Console.WriteLine($"{t.Product}: {t.Total}");
}`,
    },
  ],
};
