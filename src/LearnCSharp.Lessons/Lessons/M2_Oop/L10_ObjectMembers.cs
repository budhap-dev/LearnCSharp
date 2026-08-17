using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.10.md
public sealed class L10_ObjectMembers : LessonBase
{
    public override string Id => "2.10";
    public override string Title => "ToString, Equals, GetHashCode and operator overloading";

    public override void Run()
    {
        Section("Without ToString you get the type name");

        Out("new Plain()", new Plain().ToString());
        Out("new Vector(3, 4)", new Vector(3, 4).ToString());

        Section("Default Equals compares identity");

        Plain p1 = new Plain(), p2 = new Plain();
        Out("p1.Equals(p2)", p1.Equals(p2));
        Out("p1.Equals(p1)", p1.Equals(p1));

        Section("Overridden Equals compares contents");

        Vector v1 = new Vector(3, 4);
        Vector v2 = new Vector(3, 4);
        Out("v1.Equals(v2)", v1.Equals(v2));
        Out("v1 == v2 (operator overloaded)", v1 == v2);
        Out("v1 != new Vector(1, 1)", v1 != new Vector(1, 1));
        Out("ReferenceEquals(v1, v2)", ReferenceEquals(v1, v2));

        Section("Equals and GetHashCode must agree");

        HashSet<Vector> set = [new Vector(3, 4), new Vector(3, 4), new Vector(1, 1)];
        Out("HashSet with two equal vectors -> count", set.Count);
        Out("v1.GetHashCode() == v2.GetHashCode()", v1.GetHashCode() == v2.GetHashCode());

        Warn("If two objects are Equal but return different hash codes, dictionaries and hash sets "
           + "silently lose them. Always override both together.");

        Section("Operator overloading");

        Out("v1 + new Vector(1, 1)", v1 + new Vector(1, 1));
        Out("v1 - new Vector(1, 1)", v1 - new Vector(1, 1));
        Out("v1 * 2", v1 * 2);
        Out("-v1", -v1);
        Out("v1.Length", v1.Length);

        Section("Implicit and explicit conversion operators");

        Vector fromTuple = (5, 12);              // implicit conversion from a tuple
        Out("Vector fromTuple = (5, 12)", fromTuple);
        Out("its length", fromTuple.Length);

        (int x, int y) backToTuple = ((int, int))fromTuple;   // explicit conversion
        Out("back to a tuple", $"({backToTuple.x}, {backToTuple.y})");

        Section("IComparable makes sorting work");

        List<Vector> vectors = [new Vector(3, 4), new Vector(1, 1), new Vector(6, 8)];
        vectors.Sort();
        foreach (Vector v in vectors) Out("sorted by length", $"{v} length {v.Length:F2}");

        Section("What object gives every type");

        Out("ToString()", "text form - override it, always");
        Out("Equals(object)", "are these the same? - override for value-like types");
        Out("GetHashCode()", "a bucket number for hashing - override with Equals");
        Out("GetType()", "runtime type information");
        Out("ReferenceEquals(a, b)", "identity check that ignores any overload");

        Note("A record generates ToString, Equals and GetHashCode for you - which is why records are "
           + "usually the better choice for pure data.");
    }
}

public class Plain
{
}

/// <summary>A value-like class that plays by all the rules.</summary>
public class Vector : IComparable<Vector>
{
    public Vector(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; }
    public double Y { get; }

    public double Length => Math.Sqrt(X * X + Y * Y);

    public override string ToString() => $"({X}, {Y})";

    // 1. Equals: same type and same contents.
    public override bool Equals(object? obj) =>
        obj is Vector other && X.Equals(other.X) && Y.Equals(other.Y);

    // 2. GetHashCode: MUST match Equals. HashCode.Combine does the hard part.
    public override int GetHashCode() => HashCode.Combine(X, Y);

    // 3. == and != must be overloaded as a pair.
    public static bool operator ==(Vector? left, Vector? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Vector? left, Vector? right) => !(left == right);

    // Arithmetic operators.
    public static Vector operator +(Vector left, Vector right) => new(left.X + right.X, left.Y + right.Y);
    public static Vector operator -(Vector left, Vector right) => new(left.X - right.X, left.Y - right.Y);
    public static Vector operator *(Vector vector, double scale) => new(vector.X * scale, vector.Y * scale);
    public static Vector operator -(Vector vector) => new(-vector.X, -vector.Y);

    // Conversions: implicit when it cannot fail, explicit when it might lose information.
    public static implicit operator Vector((int x, int y) tuple) => new(tuple.x, tuple.y);
    public static explicit operator (int, int)(Vector vector) => ((int)vector.X, (int)vector.Y);

    // Ordering, used by List.Sort and Array.Sort.
    public int CompareTo(Vector? other) => other is null ? 1 : Length.CompareTo(other.Length);
}
