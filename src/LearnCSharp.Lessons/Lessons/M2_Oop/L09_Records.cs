using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.9.md
public sealed class L09_Records : LessonBase
{
    public override string Id => "2.9";
    public override string Title => "Records and immutable data";

    public override string Summary =>
        "One line gives you an entire immutable data type, with equality, ToString and "
        + "copying generated for you.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Declare positional and full records",
        "Explain value equality and how it differs from reference equality",
        "Use with-expressions to copy and modify",
    ];

    public override void Run()
    {
        Section("One line gives you a whole data class");

        PersonRecord ada = new PersonRecord("Ada", "Lovelace", 36);
        Out("ada", ada);                          // ToString is generated for you
        Out("ada.FirstName", ada.FirstName);

        Section("Records compare by VALUE, classes compare by identity");

        PersonRecord sameData = new PersonRecord("Ada", "Lovelace", 36);
        Out("ada == sameData", ada == sameData);
        Out("ReferenceEquals(ada, sameData)", ReferenceEquals(ada, sameData));

        PersonClass classA = new PersonClass("Ada", "Lovelace", 36);
        PersonClass classB = new PersonClass("Ada", "Lovelace", 36);
        Out("classA == classB", classA == classB);
        Note("A record generates Equals, GetHashCode, ToString and a deconstructor for you.");

        Section("'with' makes a modified copy");

        PersonRecord older = ada with { Age = 37 };
        Out("ada", ada);
        Out("ada with { Age = 37 }", older);
        Out("original untouched", ada.Age);

        Section("Deconstruction");

        (string first, string last, int age) = ada;
        Out("first", first);
        Out("last", last);
        Out("age", age);

        Section("Records can have extra members and validation");

        Temperature warm = new Temperature(25);
        Out("warm", warm);
        Out("warm.Fahrenheit", warm.Fahrenheit);
        Out("warm.Description", warm.Description);

        try { _ = new Temperature(-500); }
        catch (ArgumentOutOfRangeException) { Out("new Temperature(-500)", "ArgumentOutOfRangeException"); }

        Section("Records support inheritance");

        Employee dev = new Employee("Grace", "Hopper", 45, "Engineering");
        Out("dev", dev);
        Out("dev is PersonRecord", dev is PersonRecord);

        Section("record struct - a value type record");

        Coordinate origin = new Coordinate(0, 0);
        Coordinate alsoOrigin = new Coordinate(0, 0);
        Out("origin", origin);
        Out("origin == alsoOrigin", origin == alsoOrigin);

        Section("Records in a collection - value equality is very handy");

        HashSet<Coordinate> visited = [new Coordinate(0, 0), new Coordinate(1, 1), new Coordinate(0, 0)];
        Out("HashSet of 3 coords, 2 identical -> count", visited.Count);
        Out("visited.Contains(new Coordinate(1, 1))", visited.Contains(new Coordinate(1, 1)));

        Section("When to use what");
        Out("record", "data that is defined by its values: DTOs, settings, coordinates, events");
        Out("class", "something with identity and changing state: BankAccount, Player, Game");
        Out("struct", "a small value you copy a lot: Point, Money, Colour");
    }
}

// A positional record: the parameters become init-only public properties automatically.
public record PersonRecord(string FirstName, string LastName, int Age);

// The same idea written as a class - note how much more code it takes.
public class PersonClass
{
    public PersonClass(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public int Age { get; }
}

// Records can have a body: validation, computed properties, extra methods.
public record Temperature
{
    public Temperature(double celsius)
    {
        if (celsius < -273.15)
            throw new ArgumentOutOfRangeException(nameof(celsius), "Below absolute zero.");
        Celsius = celsius;
    }

    public double Celsius { get; init; }

    public double Fahrenheit => Celsius * 9 / 5 + 32;

    public string Description => Celsius switch
    {
        < 0 => "freezing",
        < 15 => "cold",
        < 25 => "mild",
        _ => "warm",
    };
}

public record Employee(string FirstName, string LastName, int Age, string Department)
    : PersonRecord(FirstName, LastName, Age);

// record struct = value semantics AND generated members. Great for small data.
public readonly record struct Coordinate(int X, int Y);
