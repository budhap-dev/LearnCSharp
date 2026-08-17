using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.7.md
public sealed class L07_Interfaces : LessonBase
{
    public override string Id => "2.7";
    public override string Title => "Interfaces: contracts without implementation";

    public override void Run()
    {
        Section("One interface, several unrelated implementations");

        IStorage[] stores = [new MemoryStorage(), new FileStorage("save.txt"), new CloudStorage()];

        foreach (IStorage store in stores)
        {
            store.Save("player", "level 7");
            Out(store.GetType().Name, store.Load("player") ?? "not found");
        }

        Section("Code against the interface, not the concrete class");

        // SaveGame does not know or care which storage it was handed.
        Out("SaveGame with memory", SaveGame(new MemoryStorage(), "Ada"));
        Out("SaveGame with cloud", SaveGame(new CloudStorage(), "Ada"));

        Section("A class may implement MANY interfaces");

        Robot robot = new Robot("R2");
        Out("robot is IMovable", robot is IMovable);
        Out("robot is ISpeaker", robot is ISpeaker);
        Out("robot.Move()", robot.Move());
        Out("robot.Speak()", robot.Speak());

        Section("Sorting with a built-in interface: IComparable");

        List<Student> students =
        [
            new Student("Ben", 72),
            new Student("Ada", 91),
            new Student("Cara", 65),
        ];

        students.Sort();                       // uses Student.CompareTo
        foreach (Student student in students) Out("sorted", student.ToString());

        Section("Default interface methods (C# 8+)");

        // A default method lives on the INTERFACE, so you reach it through an interface reference.
        Out("((ISpeaker)robot).Introduce()", ((ISpeaker)robot).Introduce());

        Section("Explicit implementation resolves name clashes");

        Duck duck = new Duck();
        Out("duck.Fly() - the class's own", duck.Fly());
        Out("((IFlyer)duck).Fly()", ((IFlyer)duck).Fly());

        Section("Interface vs abstract class");

        Out("interface", "a contract. No state. A class may implement many.");
        Out("abstract class", "a partly-built base. Can hold fields and code. Only ONE allowed.");
        Note("Rule of thumb: use an interface for a capability (ICanBeSaved), an abstract class when "
           + "several classes genuinely share a base and some implementation.");
    }

    // The parameter type is the INTERFACE, so any implementation fits.
    private static string SaveGame(IStorage storage, string player)
    {
        storage.Save("current", player);
        return $"saved via {storage.GetType().Name}";
    }
}

/// <summary>A contract: what a storage does, never how.</summary>
public interface IStorage
{
    void Save(string key, string value);
    string? Load(string key);
}

public class MemoryStorage : IStorage
{
    private readonly Dictionary<string, string> _data = new();

    public void Save(string key, string value) => _data[key] = value;
    public string? Load(string key) => _data.GetValueOrDefault(key);
}

public class FileStorage : IStorage
{
    // Pretend file storage - the real version is in lesson 4.8.
    private readonly Dictionary<string, string> _pretendFile = new();
    private readonly string _path;

    public FileStorage(string path) => _path = path;

    public void Save(string key, string value) => _pretendFile[$"{_path}:{key}"] = value;
    public string? Load(string key) => _pretendFile.GetValueOrDefault($"{_path}:{key}");
}

public class CloudStorage : IStorage
{
    private readonly Dictionary<string, string> _remote = new();

    public void Save(string key, string value) => _remote[key] = value.ToUpperInvariant();
    public string? Load(string key) => _remote.GetValueOrDefault(key);
}

public interface IMovable
{
    string Move();
}

public interface ISpeaker
{
    string Speak();

    // A default implementation: implementers get this free unless they override it.
    string Introduce() => $"I am a {GetType().Name} and I can speak.";
}

// One class, two contracts.
public class Robot : IMovable, ISpeaker
{
    public Robot(string name) => Name = name;

    public string Name { get; }

    public string Move() => $"{Name} rolls forward.";
    public string Speak() => $"{Name} beeps.";
}

// IComparable<T> is how you teach the framework to sort YOUR type.
public class Student : IComparable<Student>
{
    public Student(string name, int mark)
    {
        Name = name;
        Mark = mark;
    }

    public string Name { get; }
    public int Mark { get; }

    // Negative = I come first, 0 = equal, positive = I come later.
    public int CompareTo(Student? other) => other is null ? 1 : other.Mark.CompareTo(Mark);

    public override string ToString() => $"{Name} ({Mark})";
}

public interface IFlyer
{
    string Fly();
}

public class Duck : IFlyer
{
    public string Fly() => "Duck flaps its wings.";

    // Explicit implementation: only reachable through the interface.
    string IFlyer.Fly() => "IFlyer.Fly on a Duck.";
}
