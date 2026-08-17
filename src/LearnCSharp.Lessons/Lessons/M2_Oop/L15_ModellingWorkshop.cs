using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.15.md
public sealed class L15_ModellingWorkshop : LessonBase
{
    public override string Id => "2.15";
    public override string Title => "Workshop: modelling a real system";

    public override string Summary =>
        "Everything in this module applied end to end: a written brief becomes a working "
        + "class model, which is then critiqued and improved.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Turn a written brief into a class model",
        "Decide which class each rule belongs to",
        "Critique a design and identify its next refactor",
    ];

    public override void Run()
    {
        Section("The brief");

        Line();
        Line("  \"A school library lends books to members. A member may borrow up to");
        Line("   3 books at a time, for 14 days. Borrowing an unavailable book must be");
        Line("   refused. Late returns are fined 10p per day. Staff need a report of");
        Line("   everything currently overdue.\"");

        Section("Step 1 - find the nouns");

        Out("library", "the whole system - a class that coordinates");
        Out("book", "a class");
        Out("member", "a class");
        Out("loan", "a class - it holds the dates, which belong to neither book nor member");
        Out("fine", "NOT a class - it is a calculation on a Loan");
        Out("3 books, 14 days, 10p", "NOT classes - they are constants/rules");

        Note("'Loan' is the interesting one. Beginners put a DueDate on Book, then discover a "
           + "book that has been lent twice has nowhere to store two histories. When a fact "
           + "belongs to a RELATIONSHIP rather than a thing, it wants its own class.");

        Section("Step 2 - find the verbs");

        Out("borrow / lend", "Library.Borrow(member, book)");
        Out("return", "Library.Return(loan)");
        Out("calculate fine", "Loan.FineOn(date)");
        Out("report overdue", "Library.OverdueReport(date)");

        Section("Step 3 - sketch the relationships");

        Line();
        Line("  +----------+          +---------+          +----------+");
        Line("  |  Member  |1      * |  Loan   | *      1 |   Book   |");
        Line("  +----------+----------+---------+----------+----------+");
        Line("  | Name     |          | Due     |          | Title    |");
        Line("  | MaxLoans |          | Returned|          | Author   |");
        Line("  +----------+          +---------+          +----------+");
        Line();
        Line("  Library  *---- Book        composition: the library owns its stock");
        Line("  Library  *---- Member      composition: it owns its membership list");
        Line("  Library  *---- Loan        composition: a loan means nothing without it");
        Line("  Loan     ----> Book        association: a loan refers to a book");
        Line("  Loan     ----> Member      association: and to a member");

        Section("Step 4 - decide where each rule lives");

        Out("\"up to 3 books\"", "Library - only it can see all of a member's loans");
        Out("\"14 days\"", "Loan - it owns the dates");
        Out("\"10p per day late\"", "Loan.FineOn - a calculation on its own data");
        Out("\"refuse if unavailable\"", "Library.Borrow - it decides, Book just reports state");

        Note("The guiding question is always: which object OWNS the data this rule needs? Put the "
           + "rule there. A rule sitting far from its data is the main cause of tangled code.");

        Section("Step 5 - build it and try it");

        DateOnly today = new(2026, 8, 17);
        SchoolLibrary library = new();

        library.AddBook(1, "Dune", "Herbert");
        library.AddBook(2, "Emma", "Austen");
        library.AddBook(3, "The Hobbit", "Tolkien");
        library.AddBook(4, "Hamlet", "Shakespeare");
        library.AddMember(100, "Ada");
        library.AddMember(101, "Ben");

        Out("stock", library.BookCount);
        Out("members", library.MemberCount);

        Line();
        Out("Ada borrows Dune", library.Borrow(100, 1, today));
        Out("Ada borrows Emma", library.Borrow(100, 2, today));
        Out("Ben borrows Dune - already out", library.Borrow(101, 1, today));
        Out("Ben borrows The Hobbit", library.Borrow(101, 3, today));

        Section("The rules do their job");

        Out("Ada borrows Hamlet (3rd book)", library.Borrow(100, 4, today));
        Out("Ada tries a 4th - over the limit", library.Borrow(100, 3, today));
        Out("unknown member", library.Borrow(999, 1, today));

        Section("Time passes - fines and reports");

        DateOnly threeWeeksLater = today.AddDays(21);

        Line();
        foreach (string line in library.OverdueReport(threeWeeksLater))
            Line(line);

        Out("total fines owed", library.TotalFines(threeWeeksLater).ToString("C"));

        Line();
        Out("Ada returns Dune", library.Return(100, 1, threeWeeksLater));
        Out("Dune available again", library.Borrow(101, 1, threeWeeksLater));

        Section("Step 6 - critique your own design");

        Out("Single responsibility?", "Loan does dates+fines; Library does policy. Reasonable.");
        Out("Would a fine change be easy?", "yes - one constant in Loan");
        Out("Would a 'reserve a book' feature fit?", "yes - a Reservation class beside Loan");
        Out("What is still weak?", "Library is growing - it holds stock, members AND policy");
        Out("Next refactor", "extract a LendingPolicy class (max loans, loan length, fine rate)");

        Note("A first design is never the final one. The measure of a good model is not that it "
           + "was right first time, but that changing it later is cheap.");

        Section("Your turn");

        Out("model this", "a cinema: films, screens, showings, seats, bookings");
        Out("ask", "which facts belong to a Showing rather than a Film?");
        Out("then", "a car park, a quiz app, a bank, a train timetable");
        Out("always", "nouns -> classes, verbs -> methods, relationship facts -> their own class");
    }
}

public class StockBook
{
    public StockBook(int id, string title, string author)
    {
        Id = id;
        Title = title;
        Author = author;
    }

    public int Id { get; }
    public string Title { get; }
    public string Author { get; }

    /// <summary>The book reports its state; the Library decides what to do about it.</summary>
    public bool IsAvailable { get; private set; } = true;

    internal void MarkLent() => IsAvailable = false;
    internal void MarkReturned() => IsAvailable = true;
}

public class Member
{
    public Member(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }
    public string Name { get; }
}

/// <summary>
/// The relationship between a Member and a Book, over time. The due date belongs to
/// neither of them individually, which is exactly why Loan has to exist.
/// </summary>
public class Loan
{
    public const int LoanDays = 14;
    public const decimal FinePerDay = 0.10m;

    public Loan(Member member, StockBook book, DateOnly borrowedOn)
    {
        Member = member;
        Book = book;
        BorrowedOn = borrowedOn;
        DueOn = borrowedOn.AddDays(LoanDays);
    }

    public Member Member { get; }
    public StockBook Book { get; }
    public DateOnly BorrowedOn { get; }
    public DateOnly DueOn { get; }
    public DateOnly? ReturnedOn { get; private set; }

    public bool IsOpen => ReturnedOn is null;

    public bool IsOverdueOn(DateOnly date) => IsOpen && date > DueOn;

    public int DaysLateOn(DateOnly date) =>
        IsOverdueOn(date) ? date.DayNumber - DueOn.DayNumber : 0;

    /// <summary>The fine rule lives with the dates it depends on.</summary>
    public decimal FineOn(DateOnly date) => DaysLateOn(date) * FinePerDay;

    internal void Close(DateOnly date) => ReturnedOn = date;
}

/// <summary>Owns the stock, the membership and the lending policy.</summary>
public class SchoolLibrary
{
    private const int MaxLoansPerMember = 3;

    private readonly Dictionary<int, StockBook> _books = new();
    private readonly Dictionary<int, Member> _members = new();
    private readonly List<Loan> _loans = new();

    public int BookCount => _books.Count;
    public int MemberCount => _members.Count;

    public void AddBook(int id, string title, string author) =>
        _books[id] = new StockBook(id, title, author);

    public void AddMember(int id, string name) => _members[id] = new Member(id, name);

    public string Borrow(int memberId, int bookId, DateOnly today)
    {
        if (!_members.TryGetValue(memberId, out Member? member)) return "refused - unknown member";
        if (!_books.TryGetValue(bookId, out StockBook? book)) return "refused - unknown book";
        if (!book.IsAvailable) return $"refused - \"{book.Title}\" is already on loan";

        int openLoans = _loans.Count(l => l.IsOpen && l.Member.Id == memberId);
        if (openLoans >= MaxLoansPerMember)
            return $"refused - {member.Name} already has {openLoans} books";

        Loan loan = new(member, book, today);
        _loans.Add(loan);
        book.MarkLent();

        return $"lent \"{book.Title}\" to {member.Name}, due {loan.DueOn:dd MMM}";
    }

    public string Return(int memberId, int bookId, DateOnly today)
    {
        Loan? loan = _loans.FirstOrDefault(l => l.IsOpen && l.Member.Id == memberId && l.Book.Id == bookId);
        if (loan is null) return "no open loan found";

        // Read BOTH values before closing: once ReturnedOn is set the loan is no longer
        // open, so DaysLateOn and FineOn correctly report 0 from then on.
        int daysLate = loan.DaysLateOn(today);
        decimal fine = loan.FineOn(today);

        loan.Close(today);
        loan.Book.MarkReturned();

        return fine > 0
            ? $"returned \"{loan.Book.Title}\" {daysLate} days late - fine {fine:C}"
            : $"returned \"{loan.Book.Title}\" on time";
    }

    public List<string> OverdueReport(DateOnly today)
    {
        List<Loan> overdue = _loans.Where(l => l.IsOverdueOn(today)).ToList();

        if (overdue.Count == 0) return ["nothing overdue"];

        List<string> lines = [$"OVERDUE REPORT as at {today:dd MMM yyyy}", ""];

        lines.AddRange(overdue
            .OrderByDescending(l => l.DaysLateOn(today))
            .Select(l => $"  {l.Member.Name,-6} {l.Book.Title,-12} due {l.DueOn:dd MMM} "
                       + $"({l.DaysLateOn(today)} days late, {l.FineOn(today):C})"));

        return lines;
    }

    public decimal TotalFines(DateOnly today) => _loans.Sum(l => l.FineOn(today));
}
