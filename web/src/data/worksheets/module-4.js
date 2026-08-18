// Module 4 - Advanced C#: delegates, lambdas, events, extension methods, null safety,
// tuples/patterns, custom exceptions, IDisposable and async.

/** @type {import('./index').Worksheet} */
export default {
  module: 4,
  intro:
    'Methods as values, functional style, null safety, pattern matching, custom exceptions, deterministic clean-up and async/await.',
  tasks: [
    {
      id: '4.1',
      lesson: '4.1',
      title: 'Func, Action, Predicate',
      level: 1,
      task: 'Fill in three delegate variables: a `Func<int, int>` that squares, an `Action<string>` that prints "Log: " and its argument, and a `Predicate<int>` that is true for even numbers. Use each once.',
      starter: `Func<int, int> square = null!;
Action<string> log = null!;
Predicate<int> isEven = null!;

// TODO: assign a lambda to each of the three above

Console.WriteLine(square(6));
log("started");
Console.WriteLine(isEven(10));`,
      expected: `36
Log: started
True`,
      hints: [
        'Func<int,int> takes an int and returns an int: n => n * n.',
        'Action<string> returns nothing: s => Console.WriteLine($"Log: {s}").',
        'Predicate<int> returns a bool: n => n % 2 == 0.',
      ],
      solution: `Func<int, int> square = n => n * n;
Action<string> log = s => Console.WriteLine($"Log: {s}");
Predicate<int> isEven = n => n % 2 == 0;

Console.WriteLine(square(6));
log("started");
Console.WriteLine(isEven(10));`,
    },
    {
      id: '4.2',
      lesson: '4.2',
      title: 'A reusable filter',
      level: 2,
      task: 'Write `List<int> Filter(List<int> items, Func<int, bool> keep)` that returns only the items for which `keep` is true. Use it twice: once for even numbers, once for numbers greater than 4.',
      starter: `var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };

Console.WriteLine(string.Join(" ", Filter(numbers, n => n % 2 == 0)));
Console.WriteLine(string.Join(" ", Filter(numbers, n => n > 4)));

List<int> Filter(List<int> items, Func<int, bool> keep)
{
    // TODO: build and return a new list of items where keep(item) is true
    return new List<int>();
}`,
      expected: `2 4 6 8
5 6 7 8`,
      hints: [
        'Loop over items; if (keep(item)) result.Add(item);',
        'The same Filter works for any test because the test is a parameter - this is what LINQ\'s Where does.',
      ],
      solution: `var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };

Console.WriteLine(string.Join(" ", Filter(numbers, n => n % 2 == 0)));
Console.WriteLine(string.Join(" ", Filter(numbers, n => n > 4)));

List<int> Filter(List<int> items, Func<int, bool> keep)
{
    var result = new List<int>();
    foreach (int item in items)
    {
        if (keep(item)) result.Add(item);
    }
    return result;
}`,
    },
    {
      id: '4.3',
      lesson: '4.4',
      title: 'An extension method',
      level: 2,
      task: 'Add an extension method `bool IsPalindrome(this string text)` on `string`, then use it as if it were built in. Extension methods must live in a static class.',
      starter: `foreach (string word in new[] { "level", "hello", "noon" })
{
    Console.WriteLine($"{word}: {word.IsPalindrome()}");
}

// TODO: make IsPalindrome actually test the string
static class StringExtensions
{
    public static bool IsPalindrome(this string text) => false;
}`,
      expected: `level: True
hello: False
noon: True`,
      hints: [
        'public static bool IsPalindrome(this string text) - the "this" makes it callable as text.IsPalindrome().',
        'Compare character i with character text.Length - 1 - i for the first half.',
        'The class and the method must both be static.',
      ],
      solution: `foreach (string word in new[] { "level", "hello", "noon" })
{
    Console.WriteLine($"{word}: {word.IsPalindrome()}");
}

static class StringExtensions
{
    public static bool IsPalindrome(this string text)
    {
        for (int i = 0; i < text.Length / 2; i++)
        {
            if (text[i] != text[text.Length - 1 - i]) return false;
        }
        return true;
    }
}`,
    },
    {
      id: '4.4',
      lesson: '4.5',
      title: 'Null safety',
      level: 2,
      task: 'Complete `Greeting` so a `null` name becomes "Guest", using the null-coalescing operator. Then print the length of each name safely with the null-conditional operator, printing -1 when it is null.',
      starter: `string?[] names = ["Ada", null, "Grace"];

foreach (string? name in names)
{
    Console.WriteLine(Greeting(name));
    Console.WriteLine(name?.Length ?? -1);
}

string Greeting(string? name)
{
    // TODO: use ?? so null becomes "Guest"
    return $"Hello, {name}!";
}`,
      expected: `Hello, Ada!
3
Hello, Guest!
-1
Hello, Grace!
5`,
      hints: [
        'name ?? "Guest" gives "Guest" only when name is null.',
        'name?.Length is null when name is null; ?? -1 supplies the fallback.',
      ],
      solution: `string?[] names = ["Ada", null, "Grace"];

foreach (string? name in names)
{
    Console.WriteLine(Greeting(name));
    Console.WriteLine(name?.Length ?? -1);
}

string Greeting(string? name) => $"Hello, {name ?? "Guest"}!";`,
    },
    {
      id: '4.5',
      lesson: '4.6',
      title: 'Tuples and pattern matching',
      level: 2,
      task: 'Write `string Quadrant((int X, int Y) p)` that returns "Origin", "On axis", or "Q1".."Q4" using a `switch` expression with `when` guards or property patterns. Print the result for each point.',
      starter: `(int, int)[] points = [(0, 0), (3, 0), (2, 5), (-1, 4), (-3, -2), (6, -1)];

foreach (var p in points)
{
    Console.WriteLine($"({p.Item1},{p.Item2}) -> {Quadrant(p)}");
}

string Quadrant((int X, int Y) p)
{
    // TODO: switch expression
    return "?";
}`,
      expected: `(0,0) -> Origin
(3,0) -> On axis
(2,5) -> Q1
(-1,4) -> Q2
(-3,-2) -> Q3
(6,-1) -> Q4`,
      hints: [
        'p switch { (0, 0) => "Origin", ... }',
        'Use guards: { X: 0 } or { Y: 0 } => "On axis"; var (x, y) when x > 0 && y > 0 => "Q1".',
        'Order matters - the Origin and axis cases must come before the quadrant cases.',
      ],
      solution: `(int, int)[] points = [(0, 0), (3, 0), (2, 5), (-1, 4), (-3, -2), (6, -1)];

foreach (var p in points)
{
    Console.WriteLine($"({p.Item1},{p.Item2}) -> {Quadrant(p)}");
}

string Quadrant((int X, int Y) p) => p switch
{
    (0, 0) => "Origin",
    (_, 0) or (0, _) => "On axis",
    var (x, y) when x > 0 && y > 0 => "Q1",
    var (x, y) when x < 0 && y > 0 => "Q2",
    var (x, y) when x < 0 && y < 0 => "Q3",
    _ => "Q4",
};`,
    },
    {
      id: '4.6',
      lesson: '4.7',
      title: 'A custom exception',
      level: 3,
      task: 'Define `WithdrawalException : Exception` and make `Account.Withdraw` throw it (with a helpful message) when there are insufficient funds. The caller catches it and prints the message; a valid withdrawal prints the new balance.',
      starter: `var account = new Account(100);

foreach (decimal amount in new decimal[] { 30, 200 })
{
    try
    {
        account.Withdraw(amount);
        Console.WriteLine($"Balance: {account.Balance}");
    }
    catch (WithdrawalException ex)
    {
        Console.WriteLine($"Refused: {ex.Message}");
    }
}

// TODO: give Withdraw a guard that throws WithdrawalException when funds are short
class WithdrawalException : Exception
{
    public WithdrawalException(string message) : base(message) { }
}

class Account
{
    public decimal Balance { get; private set; }
    public Account(decimal opening) => Balance = opening;

    public void Withdraw(decimal amount)
    {
        Balance -= amount;
    }
}`,
      expected: `Balance: 70
Refused: Cannot withdraw 200, balance is 70`,
      hints: [
        'class WithdrawalException : Exception { public WithdrawalException(string message) : base(message) { } }',
        'In Withdraw, check first: if (amount > Balance) throw new WithdrawalException($"Cannot withdraw {amount}, balance is {Balance}");',
        'Only subtract after the check passes, so a refused withdrawal leaves the balance alone.',
      ],
      solution: `var account = new Account(100);

foreach (decimal amount in new decimal[] { 30, 200 })
{
    try
    {
        account.Withdraw(amount);
        Console.WriteLine($"Balance: {account.Balance}");
    }
    catch (WithdrawalException ex)
    {
        Console.WriteLine($"Refused: {ex.Message}");
    }
}

class WithdrawalException : Exception
{
    public WithdrawalException(string message) : base(message) { }
}

class Account
{
    public decimal Balance { get; private set; }
    public Account(decimal opening) => Balance = opening;

    public void Withdraw(decimal amount)
    {
        if (amount > Balance)
        {
            throw new WithdrawalException($"Cannot withdraw {amount}, balance is {Balance}");
        }
        Balance -= amount;
    }
}`,
    },
    {
      id: '4.7',
      lesson: '4.8',
      title: 'IDisposable and using',
      level: 2,
      task: 'Make `Timer` implement `IDisposable`: its constructor prints "open", `Dispose` prints "close". A `using` block must print "open", the work, then "close" automatically - even though nothing calls Dispose explicitly.',
      starter: `Console.WriteLine("before");
using (var t = new Timer())
{
    Console.WriteLine("working");
}
Console.WriteLine("after");

// TODO: make the constructor print "open" and Dispose print "close"
class Timer : IDisposable
{
    public Timer() { }
    public void Dispose() { }
}`,
      expected: `before
open
working
close
after`,
      hints: [
        'class Timer : IDisposable { ... }',
        'The constructor prints "open"; public void Dispose() prints "close".',
        'The using block calls Dispose for you when it ends - that is the whole point.',
      ],
      solution: `Console.WriteLine("before");
using (var t = new Timer())
{
    Console.WriteLine("working");
}
Console.WriteLine("after");

class Timer : IDisposable
{
    public Timer() => Console.WriteLine("open");
    public void Dispose() => Console.WriteLine("close");
}`,
    },
    {
      id: '4.8',
      lesson: '4.10',
      title: 'async and await',
      level: 3,
      task: 'Write an async method `Task<int> SlowSquare(int n)` that awaits a tiny delay then returns n². Await three of them and print each result. (Use `await Task.Delay(1)` to stand in for slow work.)',
      starter: `Console.WriteLine(await SlowSquare(2));
Console.WriteLine(await SlowSquare(5));
Console.WriteLine(await SlowSquare(9));

async Task<int> SlowSquare(int n)
{
    // TODO: await a small delay, then return n squared
    return 0;
}`,
      expected: `4
25
81`,
      hints: [
        'await Task.Delay(1); pauses without blocking the thread.',
        'After the delay, return n * n; the method\'s Task<int> carries the result.',
        'Top-level statements can use await directly - the file becomes an async Main.',
      ],
      solution: `Console.WriteLine(await SlowSquare(2));
Console.WriteLine(await SlowSquare(5));
Console.WriteLine(await SlowSquare(9));

async Task<int> SlowSquare(int n)
{
    await Task.Delay(1);
    return n * n;
}`,
    },
  ],
};
