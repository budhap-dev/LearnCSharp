using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.1.md
public sealed class L01_WhatIsOop : LessonBase
{
    public override string Id => "2.1";
    public override string Title => "What is object-oriented programming?";

    public override string Summary =>
        "Objects bundle data together with the rules that protect it. See one problem solved "
        + "procedurally and then with objects, and learn to find the classes hiding in a "
        + "written brief.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Explain why OOP exists and what problem it solves",
        "Turn a written requirement into candidate classes and methods",
        "Name the four pillars, and recognise when OOP is the wrong tool",
    ];

    public override void Run()
    {
        Section("The same problem, solved twice");

        Line("A library needs to lend books. Here it is the PROCEDURAL way:");
        Line();

        // Procedural: the data is loose, and nothing links the parallel arrays together.
        string[] titles = ["The Hobbit", "Dune", "Emma"];
        bool[] isOnLoan = [false, false, false];
        string[] borrowers = ["", "", ""];

        LendProcedural(titles, isOnLoan, borrowers, 0, "Ada");
        LendProcedural(titles, isOnLoan, borrowers, 1, "Ben");

        for (int i = 0; i < titles.Length; i++)
            Out(titles[i], isOnLoan[i] ? $"on loan to {borrowers[i]}" : "on the shelf");

        Warn("Three arrays that must stay the same length, in the same order, forever. "
           + "Sort one and the data silently corrupts. Nothing stops you lending a book twice.");

        Section("The same thing, object-oriented");

        LibraryBook hobbit = new("The Hobbit", "Tolkien");
        LibraryBook dune = new("Dune", "Herbert");
        LibraryBook emma = new("Emma", "Austen");

        Out("hobbit.Lend(\"Ada\")", hobbit.Lend("Ada"));
        Out("dune.Lend(\"Ben\")", dune.Lend("Ben"));
        Out("hobbit.Lend(\"Cara\") - already out", hobbit.Lend("Cara"));

        foreach (LibraryBook book in new[] { hobbit, dune, emma })
            Out(book.Title, book.Status);

        Note("Each book carries its own data AND the rules about that data. The object cannot be "
           + "put into an impossible state, because the only way in is through its own methods.");

        Section("Finding the objects: nouns and verbs");

        Line("Read the requirement and underline the words:");
        Line();
        Line("  \"A LIBRARY lends BOOKS to MEMBERS. A member can BORROW up to 3 books.");
        Line("   Each LOAN has a due date. Overdue loans generate a FINE.\"");
        Line();
        Out("nouns -> candidate classes", "Library, Book, Member, Loan, Fine");
        Out("verbs -> candidate methods", "Lend, Borrow, Return, CalculateFine");
        Out("adjectives/values -> properties", "dueDate, isOverdue, maxBooks");

        Note("This is not a rule, it is a starting point. Some nouns turn out to be properties "
           + "(a due date is not a class), and some turn out to be missing entirely.");

        Section("The four pillars");

        Out("1. Abstraction", "expose WHAT it does, hide HOW  (lesson 2.3)");
        Out("2. Encapsulation", "keep data private, guard it with methods  (lesson 2.4)");
        Out("3. Inheritance", "a new class built on an existing one  (lesson 2.5)");
        Out("4. Polymorphism", "one call, many behaviours  (lesson 2.6)");

        Line();
        Line("  Our LibraryBook already shows two of them:");
        Line();
        Line("    ABSTRACTION   you call book.Lend(\"Ada\") and do not care how it records that");
        Line("    ENCAPSULATION _borrower is private, so nobody can forge a loan");

        Section("Why bother?");

        Out("manages complexity", "10 classes of 50 lines beat 1 file of 500");
        Out("localises change", "change how fines work -> only Loan changes");
        Out("reuse", "one Book class, ten thousand book objects");
        Out("testability", "test a Book on its own, with no library and no database");
        Out("models reality", "the code reads like the problem it solves");

        Section("When NOT to use OOP");

        Out("a 20-line script", "a class adds ceremony and no value");
        Out("pure calculation", "a static method is simpler than an object");
        Out("data with no rules", "use a record - lesson 2.9");
        Warn("Creating a class called Manager, Helper, Processor or Data usually means you have "
           + "found a verb pretending to be a noun. Look again for the real object.");

        Section("Other paradigms exist");

        Out("procedural", "sequence of instructions on shared data - C, and your Module 1 code");
        Out("object-oriented", "objects owning data and behaviour - C#, Java");
        Out("functional", "pure functions, no shared mutable state - F#, Haskell");
        Note("C# is genuinely multi-paradigm. LINQ (module 3) is functional, records are "
           + "functional-ish, and a static helper class is procedural. Use whichever fits.");
    }

    // The procedural version: the caller has to remember all the rules.
    private static void LendProcedural(string[] titles, bool[] onLoan, string[] borrowers, int index, string who)
    {
        if (onLoan[index]) return;
        onLoan[index] = true;
        borrowers[index] = who;
    }
}

/// <summary>The object-oriented version: the book owns its data AND its rules.</summary>
public class LibraryBook
{
    private string? _borrower;          // private: nobody outside can forge a loan

    public LibraryBook(string title, string author)
    {
        Title = title;
        Author = author;
    }

    public string Title { get; }
    public string Author { get; }

    public bool IsOnLoan => _borrower is not null;

    public string Status => IsOnLoan ? $"on loan to {_borrower}" : "on the shelf";

    /// <summary>The rule "a book can only be lent once" lives WITH the book.</summary>
    public string Lend(string borrower)
    {
        if (IsOnLoan) return $"refused - already with {_borrower}";

        _borrower = borrower;
        return $"lent to {borrower}";
    }

    public string Return()
    {
        if (!IsOnLoan) return "it was not on loan";

        string was = _borrower!;
        _borrower = null;
        return $"returned by {was}";
    }
}
