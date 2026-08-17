using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Advanced;

// Notes: docs/module-4/4.6.md
public sealed class L06_TuplesPatterns : LessonBase
{
    public override string Id => "4.6";
    public override string Title => "Tuples, deconstruction and advanced pattern matching";

    public override string Summary =>
        "Return several values without declaring a type, and match on shape, range and "
        + "structure rather than just equality.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Return and deconstruct tuples",
        "Use property, relational and list patterns",
        "Choose appropriately between a tuple and a record",
    ];

    public override void Run()
    {
        Section("A tuple groups values without declaring a type");

        (string, int) pair = ("Ada", 36);
        Out("unnamed tuple .Item1", pair.Item1);
        Out("unnamed tuple .Item2", pair.Item2);

        (string Name, int Age) named = ("Ada", 36);
        Out("named tuple .Name", named.Name);
        Out("named tuple .Age", named.Age);
        Out("the whole tuple", named);

        Section("Returning several values from a method");

        (int min, int max, double average) stats = Analyse([4, 8, 15, 16, 23, 42]);
        Out("min", stats.min);
        Out("max", stats.max);
        Out("average", Math.Round(stats.average, 2));

        // Or deconstruct straight into separate variables:
        (int low, int high, _) = Analyse([1, 2, 3]);
        Out("deconstructed low", low);
        Out("deconstructed high", high);

        Note("Tuples are ideal for a private helper returning two or three values. For something "
           + "public and long-lived, a record reads better because the parts get real names.");

        Section("Swapping without a temporary variable");

        int a = 1, b = 2;
        (a, b) = (b, a);
        Out("after (a, b) = (b, a)", $"a = {a}, b = {b}");

        Section("Tuples compare by value");

        Out("(1, \"x\") == (1, \"x\")", (1, "x") == (1, "x"));
        Out("(1, \"x\") == (2, \"x\")", (1, "x") == (2, "x"));

        Section("Deconstructing your own types");

        Point3D point = new(3, 4, 5);
        (double x, double y, double z) = point;    // uses the Deconstruct method
        Out("x, y, z", $"{x}, {y}, {z}");

        // Records get a Deconstruct for free.
        Book book = new("Dune", "Herbert", 412);
        (string title, string author, _) = book;
        Out("record deconstruction", $"{title} by {author}");

        Section("Switching on a tuple");

        foreach ((string p1, string p2) in new[] { ("rock", "scissors"), ("paper", "scissors"), ("rock", "rock") })
            Out($"{p1} vs {p2}", Judge(p1, p2));

        Section("Property patterns");

        Order[] orders =
        [
            new Order("book", 5.00m, "UK"),
            new Order("laptop", 950.00m, "UK"),
            new Order("book", 5.00m, "US"),
        ];

        foreach (Order order in orders)
            Out($"{order.Item} to {order.Country}", Postage(order));

        Section("Relational and logical patterns");

        foreach (int score in new[] { -5, 0, 45, 75, 101 })
            Out($"score {score}", Band(score));

        Section("List patterns (C# 11)");

        foreach (int[] values in new[] { new[] { 1 }, [1, 2], [1, 2, 3], [1, 2, 3, 4, 5] })
            Out($"[{string.Join(",", values)}]", DescribeList(values));

        Section("Positional patterns on records");

        Shape[] shapes = [new Circle(2), new Rect(3, 3), new Rect(4, 2)];
        foreach (Shape shape in shapes)
            Out(shape.GetType().Name, ClassifyShape(shape));

        Section("var and discard patterns");

        Out("_ matches anything and ignores it", Judge("lizard", "spock"));
    }

    private static (int min, int max, double average) Analyse(int[] values) =>
        (values.Min(), values.Max(), values.Average());

    private static string Judge(string first, string second) => (first, second) switch
    {
        ("rock", "scissors") or ("paper", "rock") or ("scissors", "paper") => "player 1 wins",
        (var x, var y) when x == y => "draw",
        ("rock" or "paper" or "scissors", "rock" or "paper" or "scissors") => "player 2 wins",
        _ => "unknown move",
    };

    // A property pattern matches on the values of an object's properties.
    private static string Postage(Order order) => order switch
    {
        { Country: "UK", Amount: > 50 } => "free UK delivery",
        { Country: "UK" } => "3.99 UK delivery",
        { Country: not "UK", Amount: > 100 } => "15.00 international",
        _ => "9.99 international",
    };

    private static string Band(int score) => score switch
    {
        < 0 or > 100 => "invalid",
        0 => "zero",
        > 0 and < 50 => "fail",
        >= 50 and < 70 => "pass",
        _ => "distinction",
    };

    private static string DescribeList(int[] values) => values switch
    {
        [] => "empty",
        [var only] => $"one item: {only}",
        [var first, var second] => $"two items: {first} and {second}",
        [1, ..] => "starts with 1",
        [.., var last] => $"ends with {last}",
    };

    private static string ClassifyShape(Shape shape) => shape switch
    {
        Circle(var r) when r > 5 => "a big circle",
        Circle(var r) => $"a circle of radius {r}",
        Rect(var w, var h) when w == h => $"a square of side {w}",
        Rect(var w, var h) => $"a {w} by {h} rectangle",
        _ => "unknown shape",
    };
}

public class Point3D
{
    public Point3D(double x, double y, double z)
    {
        X = x; Y = y; Z = z;
    }

    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    // Write a Deconstruct method and your type supports (a, b, c) = obj.
    public void Deconstruct(out double x, out double y, out double z)
    {
        x = X; y = Y; z = Z;
    }
}

public record Book(string Title, string Author, int Pages);
public record Order(string Item, decimal Amount, string Country);

public abstract record Shape;
public record Circle(double Radius) : Shape;
public record Rect(double Width, double Height) : Shape;
