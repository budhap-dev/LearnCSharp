using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.5.md
public sealed class L05_Inheritance : LessonBase
{
    public override string Id => "2.5";
    public override string Title => "Inheritance and the object hierarchy";

    public override void Run()
    {
        Section("A derived class gets everything the base class has");

        Dog rex = new Dog("Rex", 3, "Labrador");
        Out("rex.Name (from Animal)", rex.Name);
        Out("rex.Breed (from Dog)", rex.Breed);
        Out("rex.Describe() (from Animal)", rex.Describe());
        Out("rex.Fetch() (Dog only)", rex.Fetch());

        Section("Constructors run base-first");

        Line();
        Cat whiskers = new Cat("Whiskers", 2);
        Out("cat built", whiskers.Describe());

        Section("protected members are visible to children, not to outsiders");

        Out("rex.Sleep()", rex.Sleep());
        // rex._energy;  <- will not compile: protected, and we are not inside Animal

        Section("Every class inherits from object");

        Out("rex is Animal", rex is Animal);
        Out("typeof(Dog) is assignable to object", typeof(Dog).IsAssignableTo(typeof(object)));
        Out("rex.GetType().Name", rex.GetType().Name);
        Out("rex.GetType().BaseType?.Name", rex.GetType().BaseType?.Name);
        Out("typeof(Dog).BaseType.BaseType", typeof(Dog).BaseType?.BaseType?.Name);

        Section("Upcasting is automatic, downcasting must be checked");

        Animal asAnimal = rex;                        // upcast: always safe
        Out("stored as Animal, real type is", asAnimal.GetType().Name);

        if (asAnimal is Dog backToDog)                // safe downcast with a pattern
            Out("pattern downcast worked", backToDog.Fetch());

        Dog? maybe = asAnimal as Dog;                 // 'as' gives null instead of throwing
        Out("as Dog", maybe?.Breed);

        Animal cat = whiskers;
        Out("cat as Dog", cat as Dog);

        try { Dog wrong = (Dog)cat; }                 // a hard cast throws when it is wrong
        catch (InvalidCastException) { Out("(Dog)cat", "InvalidCastException"); }

        Section("sealed stops further inheritance");

        Out("Puppy is sealed", typeof(Puppy).IsSealed);
        // class Chihuahua : Puppy { }   <- will not compile

        Section("C# has SINGLE inheritance only");

        Note("A class may have exactly one base class. To mix in several capabilities, use "
           + "interfaces (lesson 2.5) or composition (lesson 2.9).");
    }
}

public class Animal
{
    // protected: this class AND anything inheriting from it can see it.
    protected int _energy = 100;

    public Animal(string name, int age)
    {
        Name = name;
        Age = age;
        Console.WriteLine($"        Animal constructor ran for {name}");
    }

    public string Name { get; }
    public int Age { get; }

    public string Describe() => $"{Name}, aged {Age}";

    public string Sleep()
    {
        _energy = 100;
        return $"{Name} sleeps. Energy back to {_energy}.";
    }
}

// Dog : Animal means "Dog IS AN Animal and inherits everything public/protected in it".
public class Dog : Animal
{
    // ': base(name, age)' passes arguments up to the Animal constructor, which runs FIRST.
    public Dog(string name, int age, string breed) : base(name, age)
    {
        Breed = breed;
        Console.WriteLine($"        Dog constructor ran for {name}");
    }

    public string Breed { get; }

    public string Fetch()
    {
        _energy -= 10;                        // allowed: _energy is protected
        return $"{Name} fetches the ball. Energy now {_energy}.";
    }
}

public class Cat : Animal
{
    public Cat(string name, int age) : base(name, age)
    {
        Console.WriteLine($"        Cat constructor ran for {name}");
    }
}

/// <summary>sealed = nothing may inherit from this.</summary>
public sealed class Puppy : Dog
{
    public Puppy(string name) : base(name, 0, "unknown") { }
}
