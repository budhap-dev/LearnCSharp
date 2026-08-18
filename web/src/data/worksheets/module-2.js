// Module 2 - Object-Oriented Programming. Classes go after the top-level statements.

/** @type {import('./index').Worksheet} */
export default {
  module: 2,
  intro:
    'Classes, encapsulation, inheritance, polymorphism, interfaces, records and enums. Put your classes below the top-level statements, as the starters do.',
  tasks: [
    {
      id: '2.1',
      lesson: '2.2',
      title: 'A Student class',
      level: 1,
      task: 'Complete the `Student` class: a constructor that takes a name and a year group, two read-only properties, and a method `Describe()` that returns "Ada (Year 12)". Then create the two students and print each description.',
      starter: `var students = new List<Student>
{
    new Student("Ada", 12),
    new Student("Alan", 13),
};

foreach (Student s in students)
{
    Console.WriteLine(s.Describe());
}

class Student
{
    // TODO: properties Name and Year, a constructor, and Describe()
    public Student(string name, int year)
    {
    }

    public string Describe() => "";
}`,
      expected: `Ada (Year 12)
Alan (Year 13)`,
      hints: [
        'public string Name { get; } is a read-only auto-property - set it in the constructor.',
        'Describe can be an expression-bodied method: => $"{Name} (Year {Year})".',
      ],
      solution: `var students = new List<Student>
{
    new Student("Ada", 12),
    new Student("Alan", 13),
};

foreach (Student s in students)
{
    Console.WriteLine(s.Describe());
}

class Student
{
    public string Name { get; }
    public int Year { get; }

    public Student(string name, int year)
    {
        Name = name;
        Year = year;
    }

    public string Describe() => $"{Name} (Year {Year})";
}`,
    },
    {
      id: '2.2',
      lesson: '2.4',
      title: 'Encapsulated bank account',
      level: 2,
      task: 'Make `BankAccount` protect its balance: the field must be private, `Balance` read-only from outside, `Deposit` must reject amounts of zero or less, and `Withdraw` must refuse to go overdrawn - printing "Insufficient funds" and leaving the balance alone. Return `true`/`false` from both methods to say whether they succeeded.',
      starter: `var account = new BankAccount("Ada");
account.Deposit(100);
account.Withdraw(30);
account.Withdraw(500);
account.Deposit(-5);
Console.WriteLine($"{account.Owner}: £{account.Balance:F2}");

class BankAccount
{
    public string Owner;
    public decimal Balance;

    public BankAccount(string owner) => Owner = owner;

    // TODO: make Balance private-set, validate Deposit and Withdraw, return bool
    public void Deposit(decimal amount) => Balance += amount;
    public void Withdraw(decimal amount) => Balance -= amount;
}`,
      expected: `Insufficient funds
Ada: £70.00`,
      hints: [
        'public decimal Balance { get; private set; } - readable everywhere, writable only inside the class.',
        'if (amount <= 0) return false; guards Deposit. if (amount > Balance) { Console.WriteLine("Insufficient funds"); return false; } guards Withdraw.',
      ],
      solution: `var account = new BankAccount("Ada");
account.Deposit(100);
account.Withdraw(30);
account.Withdraw(500);
account.Deposit(-5);
Console.WriteLine($"{account.Owner}: £{account.Balance:F2}");

class BankAccount
{
    public string Owner { get; }
    public decimal Balance { get; private set; }

    public BankAccount(string owner) => Owner = owner;

    public bool Deposit(decimal amount)
    {
        if (amount <= 0) return false;
        Balance += amount;
        return true;
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0) return false;
        if (amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
            return false;
        }
        Balance -= amount;
        return true;
    }
}`,
    },
    {
      id: '2.3',
      lesson: '2.6',
      title: 'Shapes and polymorphism',
      level: 2,
      task: 'Make `Shape` abstract with an abstract `Area()` method, then implement `Circle`, `Rectangle` and `Triangle` (½ × base × height). The loop must print each shape\'s name and area to two decimal places, then the total - without any `if` or `switch` on the type.',
      starter: `List<Shape> shapes =
[
    new Circle(1),
    new Rectangle(3, 4),
    new Triangle(6, 2),
];

double total = 0;
foreach (Shape shape in shapes)
{
    Console.WriteLine($"{shape.GetType().Name,-10} {shape.Area(),6:F2}");
    total += shape.Area();
}
Console.WriteLine($"{"Total",-10} {total,6:F2}");

// TODO: make Shape abstract, and give every subclass its own Area
class Shape
{
    public double Area() => 0;
}

class Circle : Shape
{
    public Circle(double radius) { }
}

class Rectangle : Shape
{
    public Rectangle(double width, double height) { }
}

class Triangle : Shape
{
    public Triangle(double b, double height) { }
}`,
      expected: `Circle       3.14
Rectangle   12.00
Triangle     6.00
Total       21.14`,
      hints: [
        'abstract class Shape { public abstract double Area(); } - no body, subclasses must override.',
        'Each subclass stores its dimensions in private readonly fields set by the constructor.',
        'public override double Area() => Math.PI * _radius * _radius;',
      ],
      solution: `List<Shape> shapes =
[
    new Circle(1),
    new Rectangle(3, 4),
    new Triangle(6, 2),
];

double total = 0;
foreach (Shape shape in shapes)
{
    Console.WriteLine($"{shape.GetType().Name,-10} {shape.Area(),6:F2}");
    total += shape.Area();
}
Console.WriteLine($"{"Total",-10} {total,6:F2}");

abstract class Shape
{
    public abstract double Area();
}

class Circle : Shape
{
    private readonly double _radius;
    public Circle(double radius) => _radius = radius;
    public override double Area() => Math.PI * _radius * _radius;
}

class Rectangle : Shape
{
    private readonly double _width, _height;
    public Rectangle(double width, double height) => (_width, _height) = (width, height);
    public override double Area() => _width * _height;
}

class Triangle : Shape
{
    private readonly double _base, _height;
    public Triangle(double b, double height) => (_base, _height) = (b, height);
    public override double Area() => 0.5 * _base * _height;
}`,
    },
    {
      id: '2.4',
      lesson: '2.7',
      title: 'Payroll through an interface',
      level: 2,
      task: 'The interface `IPayable` has one member, `decimal MonthlyPay()`. Make `Employee` (annual salary ÷ 12) and `Contractor` (hours × rate) implement it, and let the loop total the payroll without knowing which is which.',
      starter: `List<IPayable> staff =
[
    new Employee("Ada", 36000m),
    new Contractor("Alan", 120, 45m),
];

decimal total = 0;
foreach (IPayable p in staff)
{
    total += p.MonthlyPay();
}
Console.WriteLine($"Payroll: {total:F2}");

// TODO: implement MonthlyPay properly in both classes (store the constructor arguments!)
interface IPayable
{
    decimal MonthlyPay();
}

class Employee : IPayable
{
    public Employee(string name, decimal salary) { }
    public decimal MonthlyPay() => 0;
}

class Contractor : IPayable
{
    public Contractor(string name, int hours, decimal rate) { }
    public decimal MonthlyPay() => 0;
}`,
      expected: `Payroll: 8400.00`,
      hints: [
        'An interface member has no body: decimal MonthlyPay();',
        'Store the constructor arguments in fields or properties, then use them in MonthlyPay.',
        '36000 / 12 = 3000; 120 * 45 = 5400.',
      ],
      solution: `List<IPayable> staff =
[
    new Employee("Ada", 36000m),
    new Contractor("Alan", 120, 45m),
];

decimal total = 0;
foreach (IPayable p in staff)
{
    total += p.MonthlyPay();
}
Console.WriteLine($"Payroll: {total:F2}");

interface IPayable
{
    decimal MonthlyPay();
}

class Employee : IPayable
{
    public string Name { get; }
    public decimal Salary { get; }
    public Employee(string name, decimal salary) => (Name, Salary) = (name, salary);
    public decimal MonthlyPay() => Salary / 12;
}

class Contractor : IPayable
{
    public string Name { get; }
    public int Hours { get; }
    public decimal Rate { get; }
    public Contractor(string name, int hours, decimal rate) => (Name, Hours, Rate) = (name, hours, rate);
    public decimal MonthlyPay() => Hours * Rate;
}`,
    },
    {
      id: '2.5',
      lesson: '2.8',
      title: 'Enum and switch expression',
      level: 1,
      task: 'Declare an enum `TrafficLight` with `Red`, `RedAmber`, `Green`, `Amber`, and write `Next(TrafficLight)` returning the following light in the UK sequence Red → RedAmber → Green → Amber → Red. Print five steps starting from Red.',
      starter: `TrafficLight light = TrafficLight.Red;
for (int i = 0; i < 5; i++)
{
    Console.WriteLine(light);
    light = Next(light);
}

TrafficLight Next(TrafficLight current)
{
    // TODO: switch expression
    return current;
}

enum TrafficLight { Red }`,
      expected: `Red
RedAmber
Green
Amber
Red`,
      hints: [
        'enum TrafficLight { Red, RedAmber, Green, Amber }',
        'current switch { TrafficLight.Red => TrafficLight.RedAmber, ... , _ => TrafficLight.Red }',
        'Console.WriteLine of an enum prints its name.',
      ],
      solution: `TrafficLight light = TrafficLight.Red;
for (int i = 0; i < 5; i++)
{
    Console.WriteLine(light);
    light = Next(light);
}

TrafficLight Next(TrafficLight current) => current switch
{
    TrafficLight.Red => TrafficLight.RedAmber,
    TrafficLight.RedAmber => TrafficLight.Green,
    TrafficLight.Green => TrafficLight.Amber,
    _ => TrafficLight.Red,
};

enum TrafficLight { Red, RedAmber, Green, Amber }`,
    },
    {
      id: '2.6',
      lesson: '2.9',
      title: 'Records and value equality',
      level: 2,
      task: 'Declare a positional record `Point(int X, int Y)`. Show that two points with the same values are equal, use a `with` expression to move a point right by 3, and add a method `DistanceTo(Point other)` returning the straight-line distance.',
      starter: `var a = new Point(1, 2);
var b = new Point(1, 2);
Console.WriteLine(a == b);
Console.WriteLine(a);

// TODO: c is a moved 3 to the right using 'with'
var c = a;
Console.WriteLine(c);
Console.WriteLine(a.DistanceTo(new Point(4, 6)));

record Point(int X, int Y)
{
    public double DistanceTo(Point other) => 0;
}`,
      expected: `True
Point { X = 1, Y = 2 }
Point { X = 4, Y = 2 }
5`,
      hints: [
        'Records get value equality and a readable ToString for free - that is what the first two lines rely on.',
        'var c = a with { X = a.X + 3 };',
        'Math.Sqrt(dx * dx + dy * dy) where dx = other.X - X.',
      ],
      solution: `var a = new Point(1, 2);
var b = new Point(1, 2);
Console.WriteLine(a == b);
Console.WriteLine(a);

var c = a with { X = a.X + 3 };
Console.WriteLine(c);
Console.WriteLine(a.DistanceTo(new Point(4, 6)));

record Point(int X, int Y)
{
    public double DistanceTo(Point other)
    {
        int dx = other.X - X;
        int dy = other.Y - Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}`,
    },
    {
      id: '2.7',
      lesson: '2.10',
      title: 'ToString and a static counter',
      level: 2,
      task: 'Give `Book` a `ToString` override that returns "Title by Author (Year)", and a static property `Count` that tracks how many books have been created. Print each book and then the count.',
      starter: `var books = new List<Book>
{
    new Book("Dune", "Frank Herbert", 1965),
    new Book("Neuromancer", "William Gibson", 1984),
    new Book("The Hobbit", "J. R. R. Tolkien", 1937),
};

foreach (Book b in books) Console.WriteLine(b);
Console.WriteLine($"{Book.Count} books");

class Book
{
    public string Title { get; }
    public string Author { get; }
    public int Year { get; }
    public static int Count => 0; // TODO: count instances

    public Book(string title, string author, int year)
    {
        Title = title;
        Author = author;
        Year = year;
    }

    // TODO: override ToString
}`,
      expected: `Dune by Frank Herbert (1965)
Neuromancer by William Gibson (1984)
The Hobbit by J. R. R. Tolkien (1937)
3 books`,
      hints: [
        'public static int Count { get; private set; } and Count++ inside the constructor.',
        'public override string ToString() => $"{Title} by {Author} ({Year})";',
        'Console.WriteLine(b) calls ToString for you.',
      ],
      solution: `var books = new List<Book>
{
    new Book("Dune", "Frank Herbert", 1965),
    new Book("Neuromancer", "William Gibson", 1984),
    new Book("The Hobbit", "J. R. R. Tolkien", 1937),
};

foreach (Book b in books) Console.WriteLine(b);
Console.WriteLine($"{Book.Count} books");

class Book
{
    public string Title { get; }
    public string Author { get; }
    public int Year { get; }
    public static int Count { get; private set; }

    public Book(string title, string author, int year)
    {
        Title = title;
        Author = author;
        Year = year;
        Count++;
    }

    public override string ToString() => $"{Title} by {Author} ({Year})";
}`,
    },
    {
      id: '2.8',
      lesson: '2.14',
      title: 'Strategy pattern',
      level: 3,
      task: 'A checkout applies a discount strategy. `IDiscount` has `decimal Apply(decimal total)`; implement `NoDiscount`, `PercentOff(int percent)` and `BuyOverSaveTen` (£10 off totals over £50), and make `Checkout` take a strategy in its constructor. The main program must not change.',
      starter: `decimal basket = 80m;
IDiscount[] strategies = [new NoDiscount(), new PercentOff(25), new BuyOverSaveTen()];

foreach (IDiscount d in strategies)
{
    var checkout = new Checkout(d);
    Console.WriteLine($"{d.GetType().Name,-15} {checkout.Total(basket):F2}");
}

// TODO: implement Apply in each strategy, and make Checkout use whichever it is given
interface IDiscount
{
    decimal Apply(decimal total);
}
class NoDiscount : IDiscount { public decimal Apply(decimal total) => total; }
class PercentOff : IDiscount { public PercentOff(int percent) { } public decimal Apply(decimal total) => total; }
class BuyOverSaveTen : IDiscount { public decimal Apply(decimal total) => total; }

class Checkout
{
    public Checkout(IDiscount discount) { }
    public decimal Total(decimal basket) => basket;
}`,
      expected: `NoDiscount      80.00
PercentOff      60.00
BuyOverSaveTen  70.00`,
      hints: [
        'Checkout stores the IDiscount it is given and calls _discount.Apply(basket) in Total.',
        'PercentOff: total * (100 - percent) / 100. BuyOverSaveTen: total > 50 ? total - 10 : total.',
        'The point of the pattern: adding a fourth discount needs no change to Checkout.',
      ],
      solution: `decimal basket = 80m;
IDiscount[] strategies = [new NoDiscount(), new PercentOff(25), new BuyOverSaveTen()];

foreach (IDiscount d in strategies)
{
    var checkout = new Checkout(d);
    Console.WriteLine($"{d.GetType().Name,-15} {checkout.Total(basket):F2}");
}

interface IDiscount
{
    decimal Apply(decimal total);
}

class NoDiscount : IDiscount
{
    public decimal Apply(decimal total) => total;
}

class PercentOff : IDiscount
{
    private readonly int _percent;
    public PercentOff(int percent) => _percent = percent;
    public decimal Apply(decimal total) => total * (100 - _percent) / 100;
}

class BuyOverSaveTen : IDiscount
{
    public decimal Apply(decimal total) => total > 50 ? total - 10 : total;
}

class Checkout
{
    private readonly IDiscount _discount;
    public Checkout(IDiscount discount) => _discount = discount;
    public decimal Total(decimal basket) => _discount.Apply(basket);
}`,
    },
  ],
};
