using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Advanced;

// Notes: docs/module-4/4.8.md
public sealed class L08_DisposableAndMemory : LessonBase
{
    public override string Id => "4.8";
    public override string Title => "IDisposable, using, and how memory is managed";

    public override void Run()
    {
        Section("Stack and heap");

        Out("value type local", "stack - freed the instant the method returns");
        Out("object", "heap - freed later, by the garbage collector");
        Out("a reference variable", "the arrow lives on the stack, the object on the heap");
        Out("fields of an object", "heap, alongside the object that owns them");

        Section("The garbage collector");

        long before = GC.GetTotalMemory(forceFullCollection: false);

        // Make a lot of rubbish nobody keeps a reference to.
        for (int i = 0; i < 100_000; i++)
        {
            _ = new byte[64];
        }

        long afterAllocating = GC.GetTotalMemory(false);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        long afterCollect = GC.GetTotalMemory(true);

        Out("heap before (KB)", before / 1024);
        Out("after allocating 100k arrays (KB)", afterAllocating / 1024);
        Out("after GC.Collect() (KB)", afterCollect / 1024);
        Out("collections in generation 0", GC.CollectionCount(0));

        Note("The GC uses generations: new objects start in gen 0, which is collected often and "
           + "cheaply. Survivors move to gen 1 and then gen 2, which are collected rarely. "
           + "Most objects die young, which is exactly what this design is tuned for.");

        Warn("Never call GC.Collect() in real code. It is used here only to demonstrate. The runtime "
           + "decides better than you can.");

        Section("The GC does NOT handle file handles, sockets or connections");

        Out("managed memory", "the GC frees it automatically");
        Out("unmanaged resources", "file handles, network sockets, database connections, graphics");
        Out("the fix", "implement IDisposable and release them yourself");

        Section("Without using: you must remember to Dispose");

        FakeFile file = new FakeFile("notes.txt");
        file.Write("hello");
        file.Dispose();                            // if an exception happened above, this is skipped
        Out("manual Dispose", file.IsDisposed);

        Section("With using: Dispose is guaranteed");

        Line();
        using (FakeFile safe = new FakeFile("safe.txt"))
        {
            safe.Write("this is written inside the using block");
            // Dispose runs when the block ends - even if an exception is thrown.
        }

        Section("using survives an exception");

        Line();
        try
        {
            using FakeFile risky = new FakeFile("risky.txt");
            risky.Write("about to fail");
            throw new InvalidOperationException("something went wrong");
        }
        catch (InvalidOperationException)
        {
            Out("after the exception", "Dispose still ran - see the message above");
        }

        Section("using declarations (no braces needed)");

        Out("using var f = new FakeFile(...)", "disposed at the end of the enclosing block");

        Section("Nesting several disposables");

        Line();
        using (FakeFile outer = new FakeFile("outer.txt"))
        using (FakeFile inner = new FakeFile("inner.txt"))
        {
            outer.Write("a");
            inner.Write("b");
        }
        Note("They are disposed in reverse order: inner first, then outer.");

        Section("Real disposable types you will meet");

        Out("StreamReader / StreamWriter", "files");
        Out("FileStream", "raw file access");
        Out("HttpClient", "web requests (but keep ONE for the whole app)");
        Out("SqlConnection", "databases");
        Out("Bitmap, Graphics", "images");

        Section("Common memory leaks in managed code");

        Out("event handlers", "a subscriber stays alive as long as the publisher holds it - use -=");
        Out("static collections", "anything you add to a static list lives forever");
        Out("captured variables", "a lambda holds its captured objects alive");
        Out("undisposed resources", "the file handle stays open even after the object is collected");

        Section("Value types can dodge the heap entirely");

        Out("struct in a local variable", "stack, no allocation, no GC work");
        Out("class", "always a heap allocation");
        Out("boxing", "putting a struct into an object variable copies it ONTO the heap");

        object boxed = 42;                         // boxing: the int is copied to the heap
        int unboxed = (int)boxed;                  // unboxing: copied back
        Out("boxed int", boxed);
        Out("unboxed", unboxed);
        Note("Generics exist largely to avoid boxing: List<int> stores real ints, whereas the old "
           + "ArrayList boxed every single one.");
    }
}

/// <summary>Pretend unmanaged resource so the pattern is visible without touching the disk.</summary>
public sealed class FakeFile : IDisposable
{
    private readonly string _name;

    public FakeFile(string name)
    {
        _name = name;
        Console.WriteLine($"      opened {_name}");
    }

    public bool IsDisposed { get; private set; }

    public void Write(string text)
    {
        // A disposed object must refuse to work rather than misbehave.
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        Console.WriteLine($"      wrote \"{text}\" to {_name}");
    }

    public void Dispose()
    {
        if (IsDisposed) return;                    // Dispose must be safe to call twice
        IsDisposed = true;
        Console.WriteLine($"      closed {_name}");
    }
}
