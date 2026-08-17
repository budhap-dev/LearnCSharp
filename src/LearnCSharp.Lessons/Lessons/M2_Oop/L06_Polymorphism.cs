using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.6.md
public sealed class L06_Polymorphism : LessonBase
{
    public override string Id => "2.6";
    public override string Title => "Polymorphism: virtual, override and abstract";

    public override void Run()
    {
        Section("One variable, many behaviours");

        // Every item is declared as Shape, but each runs its OWN Area code.
        Shape[] shapes =
        [
            new Circle(3),
            new Rectangle(4, 5),
            new Square(6),
            new Triangle(3, 4, 5),
        ];

        foreach (Shape shape in shapes)
            Out(shape.Name, $"area {shape.Area():F2}, perimeter {shape.Perimeter():F2}");

        Section("This is what makes polymorphism useful");

        // TotalArea never needs changing when you invent a new shape.
        Out("TotalArea(shapes)", TotalArea(shapes).ToString("F2"));
        Out("largest shape", shapes.MaxBy(s => s.Area())!.Name);

        Section("virtual + override: a default the child may replace");

        Notifier[] notifiers = [new Notifier(), new EmailNotifier(), new SmsNotifier()];
        foreach (Notifier notifier in notifiers)
            Out(notifier.GetType().Name, notifier.Send("Hello"));

        Section("base.Method() calls the parent's version");

        Out("LoudEmailNotifier", new LoudEmailNotifier().Send("Hello"));

        Section("abstract members have no body and MUST be overridden");

        Out("Shape is abstract", typeof(Shape).IsAbstract);
        // Shape s = new Shape();   <- will not compile: you cannot instantiate an abstract class

        Section("Overriding ToString gives every object a readable form");

        Out("shapes[0].ToString()", shapes[0].ToString());
        Out("string interpolation uses it too", $"{shapes[1]}");

        Section("new vs override - a classic trap");

        Base asBase = new DerivedWithOverride();
        Out("override: Base variable calls", asBase.Speak());

        Base asBaseHidden = new DerivedWithNew();
        Out("new (hiding): Base variable calls", asBaseHidden.Speak());
        Out("new (hiding): real type calls", new DerivedWithNew().Speak());

        Warn("'new' HIDES the method rather than replacing it, so which one runs depends on the "
           + "variable's declared type. Almost always a bug - use override.");
    }

    // Works for any Shape, including ones written years from now.
    private static double TotalArea(IEnumerable<Shape> shapes) => shapes.Sum(s => s.Area());
}

/// <summary>An abstract base: it defines WHAT every shape does, not HOW.</summary>
public abstract class Shape
{
    public abstract string Name { get; }

    // abstract = no body here, every concrete child must supply one.
    public abstract double Area();
    public abstract double Perimeter();

    // virtual = there IS a default, and children may replace it.
    public virtual string Summary() => $"{Name}: area {Area():F2}";

    public override string ToString() => Summary();
}

public class Circle : Shape
{
    private readonly double _radius;
    public Circle(double radius) => _radius = radius;

    public override string Name => "Circle";
    public override double Area() => Math.PI * _radius * _radius;
    public override double Perimeter() => 2 * Math.PI * _radius;
}

public class Rectangle : Shape
{
    protected readonly double _width, _height;

    public Rectangle(double width, double height)
    {
        _width = width;
        _height = height;
    }

    public override string Name => "Rectangle";
    public override double Area() => _width * _height;
    public override double Perimeter() => 2 * (_width + _height);
}

// A Square IS a Rectangle with equal sides - inheritance one level deeper.
public class Square : Rectangle
{
    public Square(double side) : base(side, side) { }

    public override string Name => "Square";
    public override string Summary() => $"{Name} of side {_width}: area {Area():F2}";
}

public class Triangle : Shape
{
    private readonly double _a, _b, _c;

    public Triangle(double a, double b, double c)
    {
        _a = a; _b = b; _c = c;
    }

    public override string Name => "Triangle";
    public override double Perimeter() => _a + _b + _c;

    // Heron's formula
    public override double Area()
    {
        double s = Perimeter() / 2;
        return Math.Sqrt(s * (s - _a) * (s - _b) * (s - _c));
    }
}

public class Notifier
{
    public virtual string Send(string message) => $"[log] {message}";
}

public class EmailNotifier : Notifier
{
    public override string Send(string message) => $"[email] {message}";
}

public class SmsNotifier : Notifier
{
    public override string Send(string message) => $"[sms] {message.ToUpperInvariant()}";
}

public class LoudEmailNotifier : EmailNotifier
{
    // base.Send calls the version one level up, then we add to it.
    public override string Send(string message) => base.Send(message) + " !!!";
}

public class Base
{
    public virtual string Speak() => "Base";
}

public class DerivedWithOverride : Base
{
    public override string Speak() => "Derived (override)";
}

public class DerivedWithNew : Base
{
    public new string Speak() => "Derived (new - hides, does not replace)";
}
