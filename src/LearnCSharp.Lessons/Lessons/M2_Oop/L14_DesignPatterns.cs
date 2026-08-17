using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.14.md
public sealed class L14_DesignPatterns : LessonBase
{
    public override string Id => "2.14";
    public override string Title => "Design patterns you will actually use";

    public override string Summary =>
        "Named, proven solutions to problems that keep coming back - and a shared vocabulary, "
        + "so that 'use a factory here' replaces a paragraph of explanation.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Recognise and implement Strategy, Factory, Observer, Repository, Adapter and Decorator",
        "Explain why Singleton is usually a mistake",
        "Identify the patterns you already use without naming them",
    ];

    public override void Run()
    {
        Section("What a design pattern is");

        Out("a pattern", "a named, proven solution to a problem that keeps recurring");
        Out("not", "a library, a framework, or code you copy and paste");
        Out("the real benefit", "a shared vocabulary - \"use a factory here\" replaces a paragraph");

        Section("STRATEGY - swap the algorithm at run time");

        // Same context, three interchangeable behaviours.
        Basket basket = new(100m);
        Out("no discount", basket.Checkout(new NoDiscount()));
        Out("10% off", basket.Checkout(new PercentageDiscount(10)));
        Out("student card", basket.Checkout(new StudentDiscount()));

        Line();
        Line("  Basket ---> IDiscount <|--- NoDiscount");
        Line("                        <|--- PercentageDiscount");
        Line("                        <|--- StudentDiscount");
        Note("You met this already: List.Sort(comparison) and LINQ's Where(predicate) are both "
           + "Strategy. It is the most useful pattern there is.");

        Section("FACTORY - centralise how objects get created");

        foreach (string kind in new[] { "goblin", "orc", "dragon" })
        {
            Enemy enemy = EnemyFactory.Create(kind);
            Out($"Create(\"{kind}\")", $"{enemy.Name}, {enemy.Health} hp, {enemy.Damage} dmg");
        }

        Note("The game code says Create(\"dragon\") and never uses 'new Dragon()'. Change how a "
           + "dragon is built, or add a Lich, and only the factory is touched.");

        Section("OBSERVER - tell everyone who cares");

        Newsletter newsletter = new();
        newsletter.Subscribe(new EmailSubscriber("ada@school.uk"));
        newsletter.Subscribe(new SmsSubscriber("07700 900000"));
        newsletter.Subscribe(new LogSubscriber());

        Line();
        newsletter.Publish("New C# course released");

        Out("subscribers", newsletter.Count);
        Note("C# has this built into the language: 'event' IS the Observer pattern. See 4.3.");

        Section("SINGLETON - exactly one instance, ever");

        GameSettings first = GameSettings.Instance;
        GameSettings second = GameSettings.Instance;

        first.Volume = 7;
        Out("second.Volume after first.Volume = 7", second.Volume);
        Out("same object?", ReferenceEquals(first, second));

        Warn("Singleton is the most OVERUSED pattern. It is global state in disguise: it makes "
           + "testing hard, hides dependencies and causes threading bugs. Prefer creating one "
           + "instance and injecting it (lesson 2.13, D). Learn to recognise it, use it rarely.");

        Section("REPOSITORY - hide where the data lives");

        IBookRepository repository = new InMemoryBookRepository();
        repository.Add(new StoredBook(1, "Dune"));
        repository.Add(new StoredBook(2, "Emma"));

        Out("GetById(2)", repository.GetById(2)?.Title);
        Out("GetAll().Count", repository.GetAll().Count);
        Out("GetById(99)", repository.GetById(99)?.Title);

        Note("Swap InMemoryBookRepository for SqlBookRepository or JsonBookRepository and nothing "
           + "else changes. You used exactly this in project 7.4 with ITodoStore.");

        Section("ADAPTER - make an incompatible thing fit");

        // Our code speaks ILogger. The third-party library does not. The adapter translates.
        ILogTarget target = new LegacyLoggerAdapter(new LegacyLogger());
        Out("adapted legacy logger", target.Write("system started"));

        Note("Use an Adapter when you cannot change the other class - a NuGet package, an old "
           + "system, someone else's API.");

        Section("DECORATOR - add behaviour by wrapping");

        ICoffee coffee = new SimpleCoffee();
        Out("plain", $"{coffee.Description} = {coffee.Cost:C}");

        coffee = new WithMilk(coffee);
        coffee = new WithSyrup(coffee);
        coffee = new WithExtraShot(coffee);

        Out("decorated", $"{coffee.Description} = {coffee.Cost:C}");
        Note("Each decorator wraps the previous one. Inheritance could never give you every "
           + "combination - you would need MilkSyrupExtraShotCoffee and dozens more classes.");

        Section("The pattern families");

        Out("creational", "how objects are made: Factory, Builder, Singleton, Prototype");
        Out("structural", "how objects are composed: Adapter, Decorator, Facade, Composite, Proxy");
        Out("behavioural", "how objects interact: Strategy, Observer, Command, State, Iterator");

        Section("Patterns you have already used without knowing");

        Out("Iterator", "foreach and IEnumerable - lesson 3.5");
        Out("Observer", "events - lesson 4.3");
        Out("Strategy", "passing a Func or a Comparison - lesson 4.1");
        Out("Decorator", "Stream wrapping Stream in file I/O - lesson 4.9");
        Out("Composite", "a tree of folders containing folders - lesson 5.4");

        Section("The health warning");

        Warn("Patterns are a vocabulary for problems you HAVE, not a shopping list. Forcing five "
           + "patterns into a small program produces code that is harder to read, not easier. "
           + "Write the simple version first; reach for a pattern when the simple version starts "
           + "to hurt.");
    }
}

// --- STRATEGY ---
public interface IDiscount
{
    string Name { get; }
    decimal Apply(decimal total);
}

public class NoDiscount : IDiscount
{
    public string Name => "none";

    public decimal Apply(decimal total) => total;
}

public class PercentageDiscount : IDiscount
{
    private readonly decimal _percent;

    public PercentageDiscount(decimal percent) => _percent = percent;

    public string Name => $"{_percent}%";

    public decimal Apply(decimal total) => total * (1 - _percent / 100);
}

public class StudentDiscount : IDiscount
{
    public string Name => "student";

    public decimal Apply(decimal total) => total > 50 ? total - 15 : total * 0.9m;
}

public class Basket
{
    private readonly decimal _total;

    public Basket(decimal total) => _total = total;

    /// <summary>The Basket does not know or care which discount it was handed.</summary>
    public string Checkout(IDiscount discount) => $"{discount.Apply(_total):C} ({discount.Name})";
}

// --- FACTORY ---
public class Enemy
{
    public Enemy(string name, int health, int damage)
    {
        Name = name;
        Health = health;
        Damage = damage;
    }

    public string Name { get; }
    public int Health { get; }
    public int Damage { get; }
}

public static class EnemyFactory
{
    /// <summary>One place that knows how every enemy is built.</summary>
    public static Enemy Create(string kind) => kind switch
    {
        "goblin" => new Enemy("Goblin", 30, 5),
        "orc" => new Enemy("Orc", 60, 12),
        "dragon" => new Enemy("Dragon", 500, 90),
        _ => throw new ArgumentException($"Unknown enemy: {kind}", nameof(kind)),
    };
}

// --- OBSERVER ---
public interface ISubscriber
{
    void Receive(string story);
}

public class EmailSubscriber : ISubscriber
{
    private readonly string _address;

    public EmailSubscriber(string address) => _address = address;

    public void Receive(string story) => Console.WriteLine($"      email to {_address}: {story}");
}

public class SmsSubscriber : ISubscriber
{
    private readonly string _number;

    public SmsSubscriber(string number) => _number = number;

    public void Receive(string story) => Console.WriteLine($"      sms to {_number}: {story}");
}

public class LogSubscriber : ISubscriber
{
    public void Receive(string story) => Console.WriteLine($"      log: published \"{story}\"");
}

public class Newsletter
{
    private readonly List<ISubscriber> _subscribers = new();

    public int Count => _subscribers.Count;

    public void Subscribe(ISubscriber subscriber) => _subscribers.Add(subscriber);

    public void Unsubscribe(ISubscriber subscriber) => _subscribers.Remove(subscriber);

    /// <summary>The publisher does not know what any subscriber will do with the story.</summary>
    public void Publish(string story)
    {
        foreach (ISubscriber subscriber in _subscribers) subscriber.Receive(story);
    }
}

// --- SINGLETON ---
public sealed class GameSettings
{
    // Lazy<T> makes this thread-safe with no locking code of your own.
    private static readonly Lazy<GameSettings> _instance = new(() => new GameSettings());

    private GameSettings() { }        // private constructor: nobody else can call 'new'

    public static GameSettings Instance => _instance.Value;

    public int Volume { get; set; } = 5;
}

// --- REPOSITORY ---
public record StoredBook(int Id, string Title);

public interface IBookRepository
{
    void Add(StoredBook book);
    StoredBook? GetById(int id);
    List<StoredBook> GetAll();
}

public class InMemoryBookRepository : IBookRepository
{
    private readonly Dictionary<int, StoredBook> _books = new();

    public void Add(StoredBook book) => _books[book.Id] = book;

    public StoredBook? GetById(int id) => _books.GetValueOrDefault(id);

    public List<StoredBook> GetAll() => _books.Values.ToList();
}

// --- ADAPTER ---
/// <summary>Pretend third-party class we cannot change.</summary>
public class LegacyLogger
{
    public string WriteEntry(int severity, string text) => $"[legacy sev={severity}] {text}";
}

public interface ILogTarget
{
    string Write(string message);
}

/// <summary>Translates our interface into the shape the old class expects.</summary>
public class LegacyLoggerAdapter : ILogTarget
{
    private readonly LegacyLogger _legacy;

    public LegacyLoggerAdapter(LegacyLogger legacy) => _legacy = legacy;

    public string Write(string message) => _legacy.WriteEntry(1, message);
}

// --- DECORATOR ---
public interface ICoffee
{
    string Description { get; }
    decimal Cost { get; }
}

public class SimpleCoffee : ICoffee
{
    public string Description => "coffee";
    public decimal Cost => 2.00m;
}

/// <summary>Base decorator: wraps another ICoffee and adds to it.</summary>
public abstract class CoffeeDecorator : ICoffee
{
    protected readonly ICoffee _inner;

    protected CoffeeDecorator(ICoffee inner) => _inner = inner;

    public abstract string Description { get; }
    public abstract decimal Cost { get; }
}

public class WithMilk : CoffeeDecorator
{
    public WithMilk(ICoffee inner) : base(inner) { }

    public override string Description => _inner.Description + " + milk";
    public override decimal Cost => _inner.Cost + 0.40m;
}

public class WithSyrup : CoffeeDecorator
{
    public WithSyrup(ICoffee inner) : base(inner) { }

    public override string Description => _inner.Description + " + syrup";
    public override decimal Cost => _inner.Cost + 0.60m;
}

public class WithExtraShot : CoffeeDecorator
{
    public WithExtraShot(ICoffee inner) : base(inner) { }

    public override string Description => _inner.Description + " + extra shot";
    public override decimal Cost => _inner.Cost + 0.80m;
}
