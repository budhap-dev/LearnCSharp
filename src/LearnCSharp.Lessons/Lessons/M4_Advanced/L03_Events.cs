using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Advanced;

// Notes: docs/module-4/4.3.md
public sealed class L03_Events : LessonBase
{
    public override string Id => "4.3";
    public override string Title => "Events: broadcasting that something happened";

    public override string Summary =>
        "A publisher announces that something happened and any number of subscribers react. "
        + "It is the Observer pattern built directly into the language.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Declare, raise and subscribe to events",
        "Use the standard EventHandler pattern",
        "Explain why an event must be raised with ?.Invoke",
    ];

    public override void Run()
    {
        Section("Subscribing with +=");

        TickTimer timer = new();

        timer.Tick += OnTick;                                 // a named method
        timer.Tick += seconds => Line($"lambda saw tick {seconds}");   // a lambda

        Line();
        timer.RunFor(3);

        Section("Unsubscribing with -=");

        timer.Tick -= OnTick;
        Line();
        Line("after removing OnTick:");
        timer.RunFor(2);

        Warn("To unsubscribe you need the SAME reference you added. You cannot remove a lambda you "
           + "did not store in a variable - a common cause of memory leaks in long-running apps.");

        Section("Events carry data with EventArgs");

        Basket basket = new();
        basket.ItemAdded += (sender, e) => Line($"added {e.Item} - basket total is now {e.Total:F2}");
        basket.LimitExceeded += (sender, e) => Line($"WARNING: over budget by {e.Total - 50:F2}");

        Line();
        basket.Add("book", 12.99m);
        basket.Add("headphones", 29.50m);
        basket.Add("game", 24.99m);

        Section("Why 'event' and not a plain delegate field");

        Out("a public delegate field", "any code can overwrite it, or raise it");
        Out("a public event", "outside code may only += and -=, never clear it or fire it");

        Section("The standard .NET pattern");

        Out("delegate type", "EventHandler<TEventArgs>");
        Out("signature", "(object? sender, TEventArgs e)");
        Out("raising it", "OnSomething?.Invoke(this, args)  - the ?. matters");
        Out("naming", "past tense for 'it happened': Clicked, Saved, ItemAdded");

        Section("Publisher and subscriber are decoupled");

        // The TickTimer has no idea who is listening, and neither listener knows about the other.
        int tickCount = 0;
        TickTimer decoupled = new();
        decoupled.Tick += _ => tickCount++;
        decoupled.RunFor(5);
        Out("ticks counted by an anonymous subscriber", tickCount);

        Section("An event with no subscribers is harmless");

        TickTimer lonely = new();
        lonely.RunFor(2);
        Out("no subscribers", "the ?. before Invoke stopped a NullReferenceException");
    }

    private static void OnTick(int seconds) => Line($"OnTick method saw tick {seconds}");
}

/// <summary>A publisher: it announces ticks and does not care who listens.</summary>
public class TickTimer
{
    // 'event' restricts outside code to += and -= only.
    public event Action<int>? Tick;

    public void RunFor(int seconds)
    {
        for (int second = 1; second <= seconds; second++)
        {
            // ?. is essential: Tick is null when nobody has subscribed.
            Tick?.Invoke(second);
        }
    }
}

/// <summary>Carrying data with an event, the standard way.</summary>
public class BasketEventArgs : EventArgs
{
    public BasketEventArgs(string item, decimal total)
    {
        Item = item;
        Total = total;
    }

    public string Item { get; }
    public decimal Total { get; }
}

public class Basket
{
    private const decimal Limit = 50m;

    public event EventHandler<BasketEventArgs>? ItemAdded;
    public event EventHandler<BasketEventArgs>? LimitExceeded;

    public decimal Total { get; private set; }

    public void Add(string item, decimal price)
    {
        Total += price;

        // Convention: a protected virtual OnX method raises the event, so subclasses can extend it.
        OnItemAdded(new BasketEventArgs(item, Total));

        if (Total > Limit)
            LimitExceeded?.Invoke(this, new BasketEventArgs(item, Total));
    }

    protected virtual void OnItemAdded(BasketEventArgs e) => ItemAdded?.Invoke(this, e);
}
