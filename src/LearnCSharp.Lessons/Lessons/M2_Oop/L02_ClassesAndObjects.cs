using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.2.md
public sealed class L02_ClassesAndObjects : LessonBase
{
    public override string Id => "2.2";
    public override string Title => "Classes, objects, fields and constructors";

    public override string Summary =>
        "A class is a blueprint; an object is one thing built from it. Fields hold state, "
        + "constructors establish it, and methods are the only way to change it.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Write a class with fields, properties, constructors and methods",
        "Explain the difference between a class and an object",
        "Describe what happens when you assign one object variable to another",
    ];

    public override void Run()
    {
        Section("A class is a blueprint; an object is one thing built from it");

        Book hobbit = new Book("The Hobbit", "Tolkien", 310);
        Book dune = new Book("Dune", "Herbert", 412);

        Out("hobbit.Title", hobbit.Title);
        Out("dune.Title", dune.Title);
        Out("hobbit and dune are separate objects", !ReferenceEquals(hobbit, dune));

        Section("Objects hold their own state");

        hobbit.Read(50);
        hobbit.Read(30);
        dune.Read(10);

        Out("hobbit progress", hobbit.Progress());
        Out("dune progress", dune.Progress());

        Section("Constructor overloads and chaining");

        Book unknown = new Book("Untitled");          // uses the shorter constructor
        Out("Book(\"Untitled\")", unknown.Describe());
        Out("full constructor", dune.Describe());

        Section("Object initialiser syntax");

        // Sets public properties after the constructor runs - handy when most values are optional.
        Book note = new Book("Notebook") { Author = "Me", Pages = 100 };
        Out("object initialiser", note.Describe());

        Section("Reference semantics: two names, one object");

        Book alias = hobbit;
        alias.Read(100);
        Out("hobbit.Progress() after alias.Read(100)", hobbit.Progress());
        Out("alias == hobbit (same reference)", ReferenceEquals(alias, hobbit));

        Section("null means 'no object at all'");

        Book? missing = null;
        Out("missing?.Title", missing?.Title);
        Out("missing is null", missing is null);

        try { _ = missing!.Title; }
        catch (NullReferenceException) { Out("missing!.Title", "NullReferenceException"); }

        Section("this, and how many objects exist");

        Out("Book.Count (a static counter)", Book.Count);
    }
}

/// <summary>A minimal class: fields, properties, constructors and methods.</summary>
public class Book
{
    // A private field: internal state, invisible from outside the class.
    private int _pagesRead;

    // Auto-properties: the compiler writes the hidden field for you.
    public string Title { get; }                 // get only - set once in the constructor
    public string Author { get; set; }
    public int Pages { get; set; }

    // A static field belongs to the CLASS, shared by every object.
    public static int Count { get; private set; }

    /// <summary>The full constructor. Runs when you write 'new Book(...)'.</summary>
    public Book(string title, string author, int pages)
    {
        Title = title;
        Author = author;
        Pages = pages;
        Count++;
    }

    /// <summary>A second constructor that chains to the first with ': this(...)'.</summary>
    public Book(string title) : this(title, "Unknown", 0)
    {
    }

    public void Read(int pages)
    {
        // 'this' means "the object this method was called on". Optional unless names clash.
        this._pagesRead = Math.Min(_pagesRead + pages, Pages);
    }

    public string Progress() => Pages == 0 ? "unknown length" : $"{_pagesRead}/{Pages} pages";

    public string Describe() => $"{Title} by {Author} ({Pages} pages)";
}
