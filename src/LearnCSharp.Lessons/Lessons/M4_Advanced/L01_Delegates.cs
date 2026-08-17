using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Advanced;

// Notes: docs/module-4/4.1.md
public sealed class L01_Delegates : LessonBase
{
    public override string Id => "4.1";
    public override string Title => "Delegates: variables that hold methods";

    public override void Run()
    {
        Section("A delegate is a type-safe reference to a method");

        // Declare the variable, then point it at a method by name.
        MathOperation operation = Add;
        Out("operation = Add;  operation(3, 4)", operation(3, 4));

        operation = Multiply;                    // point it somewhere else
        Out("operation = Multiply; operation(3, 4)", operation(3, 4));

        Section("Passing behaviour into a method");

        Out("Apply(10, 5, Add)", Apply(10, 5, Add));
        Out("Apply(10, 5, Subtract)", Apply(10, 5, Subtract));
        Out("Apply(10, 5, (a, b) => a % b)", Apply(10, 5, (a, b) => a % b));

        Section("The built-in delegates: Func, Action, Predicate");

        // Func<..., TResult>: the LAST type parameter is the return type.
        Func<int, int, int> add = (a, b) => a + b;
        Func<double, double> square = x => x * x;
        Func<string> now = () => "no arguments, returns a string";

        Out("Func<int,int,int> add(3, 4)", add(3, 4));
        Out("Func<double,double> square(5)", square(5));
        Out("Func<string> now()", now());

        // Action: returns nothing.
        Action<string> shout = message => Console.WriteLine($"      {message.ToUpperInvariant()}!");
        Line();
        shout("hello");

        // Predicate<T>: takes a T, returns bool. Same as Func<T, bool>.
        Predicate<int> isEven = n => n % 2 == 0;
        Out("Predicate<int> isEven(10)", isEven(10));

        Section("Where the framework already uses them");

        List<int> numbers = [5, 12, 8, 130, 44];
        Out("List.Find takes a Predicate", numbers.Find(n => n > 10));
        Out("List.RemoveAll takes a Predicate", new List<int>(numbers).RemoveAll(n => n > 10));
        Out("Where takes a Func<T,bool>", string.Join(", ", numbers.Where(n => n < 50)));
        Out("Select takes a Func<T,TResult>", string.Join(", ", numbers.Select(n => n * 2)));
        Out("OrderBy takes a Func<T,TKey>", string.Join(", ", numbers.OrderBy(n => n)));

        Section("Strategy pattern: pick the algorithm at run time");

        int[] data = [5, 3, 9, 1, 7];

        Out("sorted ascending", string.Join(", ", SortBy(data, (a, b) => a.CompareTo(b))));
        Out("sorted descending", string.Join(", ", SortBy(data, (a, b) => b.CompareTo(a))));
        Out("sorted by distance from 5", string.Join(", ",
            SortBy(data, (a, b) => Math.Abs(a - 5).CompareTo(Math.Abs(b - 5)))));

        Section("Multicast delegates: one call, several methods");

        // Declared nullable, because removing the last method leaves the delegate null.
        Action<string>? pipeline = LogToConsole;
        pipeline += LogWithTimestamp;             // += adds another method to the chain
        pipeline += message => Console.WriteLine($"      [lambda] {message}");

        Line();
        pipeline("system started");               // all three run, in the order they were added

        pipeline -= LogWithTimestamp;             // -= removes one
        Line();
        Line("after removing the timestamp logger:");
        pipeline?.Invoke("second message");

        Out("methods in the chain", pipeline?.GetInvocationList().Length ?? 0);

        Section("A delegate can be null");

        Action? nothing = null;
        nothing?.Invoke();                        // ?. means "only if it is not null"
        Out("nothing?.Invoke()", "did not crash");

        Section("Returning a function from a function");

        Func<int, int> timesThree = Multiplier(3);
        Func<int, int> timesTen = Multiplier(10);
        Out("Multiplier(3)(7)", timesThree(7));
        Out("Multiplier(10)(7)", timesTen(7));

        Section("Composing functions");

        Func<int, int> addTwo = n => n + 2;
        Func<int, int> doubler = n => n * 2;
        Func<int, int> addThenDouble = n => doubler(addTwo(n));

        Out("addThenDouble(5)", addThenDouble(5));
    }

    // A delegate declaration: "any method taking two ints and returning an int".
    private delegate int MathOperation(int a, int b);

    private static int Add(int a, int b) => a + b;
    private static int Subtract(int a, int b) => a - b;
    private static int Multiply(int a, int b) => a * b;

    private static int Apply(int a, int b, MathOperation operation) => operation(a, b);

    private static int[] SortBy(int[] source, Comparison<int> comparison)
    {
        int[] copy = (int[])source.Clone();
        Array.Sort(copy, comparison);
        return copy;
    }

    private static void LogToConsole(string message) => Console.WriteLine($"      [console] {message}");

    private static void LogWithTimestamp(string message) => Console.WriteLine($"      [12:00] {message}");

    // Returns a new function that remembers 'factor'. That is a closure - lesson 4.2.
    private static Func<int, int> Multiplier(int factor) => n => n * factor;
}
