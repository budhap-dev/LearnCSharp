using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.4.md
public sealed class L04_Encapsulation : LessonBase
{
    public override string Id => "2.4";
    public override string Title => "Encapsulation, properties and access modifiers";

    public override string Summary =>
        "Keep data private and force every change through a method that checks it, so an "
        + "object can never be put into an impossible state.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Replace public fields with validated properties",
        "Choose the right access modifier for each member",
        "Decide when to throw an exception and when to return false",
    ];

    public override void Run()
    {
        Section("Why a public field is dangerous");

        BadAccount bad = new BadAccount();
        bad.Balance = -5000;                      // nothing stops this
        Out("bad.Balance after setting it to -5000", bad.Balance);
        Warn("A public field lets any code put the object into an impossible state.");

        Section("The same thing done properly");

        BankAccount account = new BankAccount("Ada", 100);
        Out("opening balance", account.Balance);

        account.Deposit(50);
        Out("after Deposit(50)", account.Balance);

        Out("Withdraw(30) succeeded", account.Withdraw(30));
        Out("balance", account.Balance);

        Out("Withdraw(9999) succeeded", account.Withdraw(9999));
        Out("balance unchanged", account.Balance);

        try { account.Deposit(-10); }
        catch (ArgumentOutOfRangeException) { Out("Deposit(-10)", "ArgumentOutOfRangeException"); }

        // account.Balance = 1_000_000;   <- will not compile: the setter is private

        Section("Computed (read-only) properties");

        Out("account.IsOverdrawn", account.IsOverdrawn);
        Out("account.Summary", account.Summary);

        Section("Validation inside a property setter");

        Player player = new Player();
        player.Health = 150;
        Out("set Health = 150, clamped to", player.Health);
        player.Health = -20;
        Out("set Health = -20, clamped to", player.Health);
        Out("player.IsAlive", player.IsAlive);

        Section("Access modifiers");

        Out("public", "everyone");
        Out("private", "this class only (the default for members)");
        Out("protected", "this class and anything that inherits it");
        Out("internal", "anything in this project/assembly (default for classes)");
        Out("protected internal", "either of the above two");
        Out("private protected", "inheritors inside this assembly only");

        Section("init-only properties: set at creation, then frozen");

        Config config = new Config { Host = "localhost", Port = 8080 };
        Out("config", $"{config.Host}:{config.Port}");
        // config.Port = 9090;   <- will not compile: init-only
    }
}

/// <summary>What NOT to do: state with no protection.</summary>
public class BadAccount
{
    public decimal Balance;      // a public field - anyone can write anything
}

public class BankAccount
{
    private decimal _balance;                   // the real state, hidden

    public BankAccount(string owner, decimal opening)
    {
        Owner = owner;
        _balance = opening;
    }

    public string Owner { get; }

    // Public to read, impossible to write from outside: all changes go through the methods.
    public decimal Balance => _balance;

    // Computed properties: no stored value, worked out on demand.
    public bool IsOverdrawn => _balance < 0;
    public string Summary => $"{Owner}: {_balance:F2}";

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit must be positive.");
        _balance += amount;
    }

    /// <summary>Returns false rather than throwing - a refused withdrawal is expected, not exceptional.</summary>
    public bool Withdraw(decimal amount)
    {
        if (amount <= 0 || amount > _balance) return false;
        _balance -= amount;
        return true;
    }
}

public class Player
{
    private int _health = 100;

    // A full property with a backing field, so the setter can validate.
    public int Health
    {
        get => _health;
        set => _health = Math.Clamp(value, 0, 100);   // 'value' is the incoming assignment
    }

    public bool IsAlive => _health > 0;
}

public class Config
{
    public required string Host { get; init; }   // required: the compiler insists you set it
    public int Port { get; init; }               // init: assignable only while constructing
}
