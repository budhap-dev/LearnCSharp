using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.3.md
public sealed class L03_Abstraction : LessonBase
{
    public override string Id => "2.3";
    public override string Title => "Abstraction: showing what, hiding how";

    public override void Run()
    {
        Section("You already rely on abstraction constantly");

        Out("Console.WriteLine(\"hi\")", "you have no idea how text reaches the screen");
        Out("list.Sort()", "which algorithm? you did not need to ask");
        Out("driving a car", "a steering wheel hides the entire steering mechanism");
        Out("a light switch", "two states exposed; the National Grid hidden behind them");

        Section("A driver's view of a car");

        FamilyCar car = new();
        Out("car.Start()", car.Start());
        Out("car.Drive(30)", car.Drive(30));
        Out("car.FuelGauge", car.FuelGauge);

        Note("Three simple members. Behind them the class ran the fuel pump, the ignition, the "
           + "starter motor and the transmission - and you did not have to know.");

        Line();
        Line("what the driver sees          what actually happens");
        Line("--------------------          ---------------------");
        Line("car.Start()             ->    PrimeFuelPump()");
        Line("                              EngageStarterMotor()");
        Line("                              Ignite()");
        Line("car.Drive(30)           ->    SelectGear(30)");
        Line("                              BurnFuel(30)");

        Section("Abstraction vs encapsulation - the classic confusion");

        Out("ABSTRACTION", "a DESIGN idea: decide what to expose and what to ignore");
        Out("ENCAPSULATION", "the MECHANISM: private fields, public methods, validation");
        Out("in one line", "abstraction chooses the simple view; encapsulation enforces it");

        Note("Abstraction is about the OUTSIDE - what the user of your class sees. Encapsulation "
           + "is about the INSIDE - protecting the data that makes it work.");

        Section("Levels of abstraction");

        Line("  your game code          player.Attack(goblin)");
        Line("  game engine             sprite.Animate(\"swing\")");
        Line("  graphics library        DrawTriangle(v1, v2, v3)");
        Line("  driver                  write to GPU register 0x40");
        Line("  hardware                voltages");
        Note("Each layer offers a simpler view of the layer below. That is how anything as big as "
           + "a modern computer stays comprehensible.");

        Section("abstract classes: a partly-finished blueprint");

        // You cannot 'new' a PaymentMethod - it defines WHAT, not HOW.
        PaymentMethod[] methods = [new CardPayment("4242"), new CashPayment(), new BankTransfer("Ada")];

        foreach (PaymentMethod method in methods)
            Out(method.Name, method.Pay(25.00m));

        Out("PaymentMethod is abstract", typeof(PaymentMethod).IsAbstract);
        Out("all share the same receipt code", methods[0].Receipt(25m));

        Line();
        Line("  abstract class PaymentMethod");
        Line("      abstract Pay()        <- WHAT: every method must do this, somehow");
        Line("      Receipt()             <- HOW: written once, shared by all");

        Section("Interfaces: abstraction with no implementation at all");

        Out("abstract class", "some 'what', some 'how'. One per class.");
        Out("interface", "pure 'what'. A class may implement many. (lesson 2.7)");

        Section("Choosing the right level of abstraction");

        Line("Too little - the caller must know everything:");
        Line("    engine.PrimeFuelPump(); engine.EngageStarter(); engine.Ignite();");
        Line();
        Line("Too much - it does not say enough to be useful:");
        Line("    car.DoThing(\"start\");");
        Line();
        Line("Right - one clear intention:");
        Line("    car.Start();");

        Section("Leaky abstractions");

        Warn("An abstraction 'leaks' when the details it hid start to matter anyway. "
           + "list.Sort() hides the algorithm - until your list is 10 million items and you have "
           + "to care. No abstraction is perfect; the goal is to be right almost all the time.");

        Section("Designing an abstraction - questions to ask");

        Out("1", "Who uses this, and what do they actually want to achieve?");
        Out("2", "What is the smallest set of members that lets them do it?");
        Out("3", "What could I change later without breaking them?");
        Out("4", "Does every public member express an INTENTION, not a step?");
    }
}

/// <summary>Public surface: three simple members. Everything else is hidden.</summary>
public class FamilyCar
{
    private double _fuel = 50;
    private bool _running;
    private int _gear;

    public string FuelGauge => $"{_fuel:F1} litres";

    public string Start()
    {
        if (_running) return "already running";

        PrimeFuelPump();
        EngageStarterMotor();
        Ignite();
        _running = true;
        return "engine started";
    }

    public string Drive(int speed)
    {
        if (!_running) return "start the engine first";

        SelectGear(speed);
        BurnFuel(speed);
        return $"driving at {speed} mph in gear {_gear}";
    }

    // --- everything below is the "how", and none of it is any of the driver's business ---
    private void PrimeFuelPump() { }
    private void EngageStarterMotor() { }
    private void Ignite() { }
    private void SelectGear(int speed) => _gear = Math.Clamp(speed / 15 + 1, 1, 5);
    private void BurnFuel(int speed) => _fuel -= speed * 0.05;
}

/// <summary>abstract = defines WHAT every payment method does, and shares what it can.</summary>
public abstract class PaymentMethod
{
    public abstract string Name { get; }

    /// <summary>No body: every concrete payment method must supply its own.</summary>
    public abstract string Pay(decimal amount);

    /// <summary>Written once here, inherited by all of them.</summary>
    public string Receipt(decimal amount) => $"Receipt: {amount:C} paid by {Name}";
}

public class CardPayment : PaymentMethod
{
    private readonly string _lastFour;

    public CardPayment(string lastFour) => _lastFour = lastFour;

    public override string Name => "card";

    public override string Pay(decimal amount) => $"charged {amount:C} to card ending {_lastFour}";
}

public class CashPayment : PaymentMethod
{
    public override string Name => "cash";

    public override string Pay(decimal amount) => $"took {amount:C} in notes and coins";
}

public class BankTransfer : PaymentMethod
{
    private readonly string _accountName;

    public BankTransfer(string accountName) => _accountName = accountName;

    public override string Name => "bank transfer";

    public override string Pay(decimal amount) => $"transferred {amount:C} from {_accountName}";
}
