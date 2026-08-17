using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.8.md
public sealed class L08_ValueVsReference : LessonBase
{
    public override string Id => "2.8";
    public override string Title => "Value types, reference types, structs and enums";

    public override void Run()
    {
        Section("Assigning a VALUE type copies the value");

        PointStruct a = new PointStruct(1, 2);
        PointStruct b = a;                       // a full copy
        b.X = 99;

        Out("a.X after b.X = 99", a.X);
        Out("b.X", b.X);

        Section("Assigning a REFERENCE type copies the arrow");

        PointClass c = new PointClass(1, 2);
        PointClass d = c;                        // both names point at ONE object
        d.X = 99;

        Out("c.X after d.X = 99", c.X);
        Out("d.X", d.X);
        Out("ReferenceEquals(c, d)", ReferenceEquals(c, d));

        Section("Passing to a method follows the same rule");

        PointStruct s = new PointStruct(1, 1);
        Mutate(s);
        Out("struct after Mutate(s)", s.X);

        PointClass k = new PointClass(1, 1);
        Mutate(k);
        Out("class after Mutate(k)", k.X);

        Section("Equality differs too");

        Out("struct: a == new PointStruct(1,2)", a.Equals(new PointStruct(1, 2)));
        Out("class: c.Equals(new PointClass(99,2))", c.Equals(new PointClass(99, 2)));
        Note("Structs compare field by field. Classes compare identity - is it literally the same "
           + "object? - unless you override Equals (lesson 2.8).");

        Section("Which built-in types are which");

        Out("value types", "int, double, bool, char, decimal, DateTime, all structs and enums");
        Out("reference types", "string, arrays, List<T>, all classes, records, delegates");
        Note("string is a reference type, but it is immutable, so it FEELS like a value type.");

        Section("Where they live: stack and heap");

        Out("value type local", "on the stack - freed automatically when the method returns");
        Out("object", "on the heap - freed later by the garbage collector");
        Out("a value type INSIDE an object", "on the heap, with its owner");

        Section("Nullable value types");

        int? maybe = null;                       // int? is shorthand for Nullable<int>
        Out("int? maybe = null", maybe);
        Out("maybe.HasValue", maybe.HasValue);
        Out("maybe ?? -1", maybe ?? -1);

        maybe = 42;
        Out("after maybe = 42, maybe.Value", maybe.Value);

        Section("Enums: a fixed set of named values");

        Difficulty level = Difficulty.Hard;
        Out("level", level);
        Out("(int)level", (int)level);
        Out("Difficulty.Easy", (int)Difficulty.Easy);
        Out("Enum.Parse", Enum.Parse<Difficulty>("Medium"));
        Out("Enum.TryParse(\"Silly\")", Enum.TryParse("Silly", out Difficulty _));
        Out("all values", string.Join(", ", Enum.GetNames<Difficulty>()));

        // Enums are ideal in a switch: the compiler can see every case.
        Out("lives for Hard", LivesFor(Difficulty.Hard));
        Out("lives for Easy", LivesFor(Difficulty.Easy));

        Section("Flags enums combine with bitwise OR");

        Permissions granted = Permissions.Read | Permissions.Write;
        Out("granted", granted);
        Out("granted.HasFlag(Read)", granted.HasFlag(Permissions.Read));
        Out("granted.HasFlag(Delete)", granted.HasFlag(Permissions.Delete));
        Out("as a number", (int)granted);

        Section("readonly struct - the safe way to write one");

        Money price = new Money(19.99m, "GBP");
        Out("price", price);
        Out("price.Add(5)", price.Add(5m));
        Out("price unchanged", price);
    }

    private static void Mutate(PointStruct point) => point.X = 99;
    private static void Mutate(PointClass point) => point.X = 99;

    private static int LivesFor(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => 5,
        Difficulty.Medium => 3,
        Difficulty.Hard => 1,
        _ => 3,
    };
}

// A struct is a VALUE type: copied on assignment, usually small and immutable.
public struct PointStruct
{
    public PointStruct(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }
}

// A class is a REFERENCE type.
public class PointClass
{
    public PointClass(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }
}

public enum Difficulty
{
    Easy,        // 0 unless you say otherwise
    Medium,      // 1
    Hard,        // 2
}

// [Flags] means the values are meant to be combined - so give them powers of two.
[Flags]
public enum Permissions
{
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    All = Read | Write | Delete,
}

/// <summary>readonly struct: nothing can change after construction. The safest kind of struct.</summary>
public readonly struct Money
{
    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }
    public string Currency { get; }

    // Returns a NEW Money rather than changing this one.
    public Money Add(decimal extra) => new Money(Amount + extra, Currency);

    public override string ToString() => $"{Amount:F2} {Currency}";
}
