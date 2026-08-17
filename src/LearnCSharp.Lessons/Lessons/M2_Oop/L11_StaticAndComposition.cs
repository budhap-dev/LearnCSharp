using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.11.md
public sealed class L11_StaticAndComposition : LessonBase
{
    public override string Id => "2.11";
    public override string Title => "Static members, composition and good design";

    public override void Run()
    {
        Section("Static members belong to the class, not to any object");

        Counter first = new Counter();
        Counter second = new Counter();
        Counter third = new Counter();

        Out("first.Id", first.Id);
        Out("second.Id", second.Id);
        Out("third.Id", third.Id);
        Out("Counter.Created (shared by all)", Counter.Created);

        Section("A static class is a bag of helper methods");

        Out("MathHelper.IsEven(10)", MathHelper.IsEven(10));
        Out("MathHelper.Factorial(5)", MathHelper.Factorial(5));
        Out("MathHelper.Gcd(48, 18)", MathHelper.Gcd(48, 18));
        // new MathHelper();   <- will not compile: a static class cannot be instantiated

        Section("A static constructor runs once, before first use");

        Out("Settings.Version", Settings.Version);
        Out("Settings.LoadedAt", Settings.LoadedAt);
        Out("Settings.Version again (not reloaded)", Settings.Version);

        Section("Composition: HAS-A instead of IS-A");

        // A Car HAS AN Engine and HAS wheels. It is not a kind of engine.
        Car car = new Car("Mini", new Engine(1300));
        Out("car.Start()", car.Start());
        Out("car.Describe()", car.Describe());

        Section("Composition lets you swap a part at runtime");

        Car electric = new Car("Leaf", new ElectricEngine(150));
        Out("electric.Start()", electric.Start());
        Out("electric.Describe()", electric.Describe());

        Section("Why composition often beats inheritance");

        Out("inheritance", "locked in at compile time, one base only, tight coupling");
        Out("composition", "swap parts at run time, mix any number, easy to test");
        Note("The usual advice is 'favour composition over inheritance'. Use inheritance when the "
           + "child genuinely IS A kind of the parent AND you want its behaviour.");

        Section("Dependency injection - composition made explicit");

        // The Game does not build its own logger; it is handed one. That makes it testable.
        Game consoleGame = new Game(new ConsoleLogger());
        Out("Game with ConsoleLogger", consoleGame.Play());

        MemoryLogger memory = new MemoryLogger();
        Game silentGame = new Game(memory);
        silentGame.Play();
        Out("Game with MemoryLogger, captured", string.Join(" | ", memory.Messages));

        Section("The SOLID principles, briefly");

        Out("S - Single responsibility", "one class, one reason to change");
        Out("O - Open/closed", "open to extend (new Shape), closed to modify");
        Out("L - Liskov substitution", "a child must work anywhere its parent does");
        Out("I - Interface segregation", "many small interfaces beat one huge one");
        Out("D - Dependency inversion", "depend on ILogger, not on ConsoleLogger");
    }
}

public class Counter
{
    // One shared field for the whole class.
    private static int _created;

    public Counter()
    {
        _created++;
        Id = _created;
    }

    public int Id { get; }                       // per object
    public static int Created => _created;       // per class
}

/// <summary>static class: no state, no instances, just functions.</summary>
public static class MathHelper
{
    public static bool IsEven(int n) => n % 2 == 0;

    public static long Factorial(int n) => n <= 1 ? 1 : n * Factorial(n - 1);

    /// <summary>Euclid's algorithm for the greatest common divisor.</summary>
    public static int Gcd(int a, int b)
    {
        while (b != 0) (a, b) = (b, a % b);
        return a;
    }
}

public static class Settings
{
    // A static constructor runs automatically, exactly once, before anything else is touched.
    static Settings()
    {
        Version = "1.0.3";
        LoadedAt = "loaded once at first use";
    }

    public static string Version { get; }
    public static string LoadedAt { get; }
}

public class Engine
{
    public Engine(int capacityCc) => CapacityCc = capacityCc;

    public int CapacityCc { get; }

    public virtual string Start() => $"{CapacityCc}cc petrol engine turns over.";
}

public class ElectricEngine : Engine
{
    public ElectricEngine(int kilowatts) : base(0) => Kilowatts = kilowatts;

    public int Kilowatts { get; }

    public override string Start() => $"{Kilowatts}kW motor hums into life.";
}

/// <summary>A Car HAS AN Engine. That is composition.</summary>
public class Car
{
    private readonly Engine _engine;             // the part it is built from

    public Car(string model, Engine engine)
    {
        Model = model;
        _engine = engine;
    }

    public string Model { get; }

    public string Start() => _engine.Start();
    public string Describe() => $"{Model} powered by a {_engine.GetType().Name}";
}

public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"        [console] {message}");
}

public class MemoryLogger : ILogger
{
    public List<string> Messages { get; } = new();

    public void Log(string message) => Messages.Add(message);
}

public class Game
{
    private readonly ILogger _logger;            // injected, not created here

    public Game(ILogger logger) => _logger = logger;

    public string Play()
    {
        _logger.Log("Game started");
        _logger.Log("Player scored 10");
        return "finished";
    }
}
