using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.12.md
public sealed class L12_Relationships : LessonBase
{
    public override string Id => "2.12";
    public override string Title => "Class relationships and UML class diagrams";

    public override void Run()
    {
        Section("The five relationships between classes");

        Out("IS-A      inheritance", "a Dog IS AN Animal");
        Out("HAS-A     composition", "a House HAS Rooms - and they die with it");
        Out("HAS-A     aggregation", "a Team HAS Players - who outlive the team");
        Out("USES-A    dependency", "a Printer USES a Document, briefly");
        Out("KNOWS-A   association", "a Student KNOWS their Teacher, and vice versa");

        Section("Composition: the part cannot exist without the whole");

        House house = new("12 Elm Street");
        house.AddRoom("Kitchen", 14);
        house.AddRoom("Bedroom", 11);

        Out("house", house.Describe());
        Out("rooms", house.RoomCount);
        Note("The House CREATES its own Rooms. Demolish the house and the rooms are gone. "
           + "In UML this is a FILLED diamond on the House end.");

        Section("Aggregation: the part exists independently");

        TeamPlayer ada = new("Ada");
        TeamPlayer ben = new("Ben");

        Team lions = new("Lions");
        lions.Sign(ada);
        lions.Sign(ben);
        Out("lions", lions.Describe());

        lions.Release(ada);
        Out("after releasing Ada", lions.Describe());
        Out("but Ada still exists", ada.Name);

        Team tigers = new("Tigers");
        tigers.Sign(ada);                       // the same player joins another team
        Out("Ada joins the Tigers", tigers.Describe());

        Note("The Team is GIVEN its Players; it does not create them. Disband the team and the "
           + "players carry on. In UML this is a HOLLOW diamond.");

        Section("Dependency: a passing acquaintance");

        Printer printer = new();
        Out("printer.Print(document)", printer.Print(new Document("Essay", 3)));
        Note("The Printer never STORES a Document - it just receives one as a parameter and uses "
           + "it. This is the weakest coupling there is, and usually the best.");

        Section("Association: they know each other");

        Teacher teacher = new("Mrs Hopper");
        SchoolStudent student = new("Alex");
        student.AssignTutor(teacher);

        Out("student.Tutor", student.Tutor?.Name);
        Out("teacher's tutees", teacher.TuteeCount);

        Section("Reading a UML class diagram");

        Line();
        Line("  +---------------------+          The box has three compartments:");
        Line("  |       Account       |            1. name");
        Line("  +---------------------+            2. attributes");
        Line("  | - balance: decimal  |            3. operations");
        Line("  | + Owner: string     |");
        Line("  +---------------------+          Visibility:");
        Line("  | + Deposit(d): void  |            -  private");
        Line("  | + Withdraw(d): bool |            +  public");
        Line("  | # Log(m): void      |            #  protected");
        Line("  +---------------------+            _  static (underlined)");

        Section("The arrows");

        Line();
        Line("  Dog ------|>  Animal        inheritance      (hollow triangle)");
        Line("  Dog ------|>  IPet          realization      (dashed + hollow triangle)");
        Line("  House *----   Room          composition      (filled diamond)");
        Line("  Team  o----   TeamPlayer        aggregation      (hollow diamond)");
        Line("  Student ----  Teacher       association      (plain line)");
        Line("  Printer <---  Document      dependency       (dashed arrow)");

        Section("Multiplicity");

        Line();
        Line("  Team  o---- 1..* TeamPlayer        a team has one or more players");
        Line("  House *---- 1..* Room          a house has one or more rooms");
        Line("  Student ---- 0..1 Teacher      a student has at most one tutor");
        Line("  Order  *---- 1..* OrderLine    an order has at least one line");
        Line();
        Line("     1      exactly one          0..1   none or one");
        Line("     *      any number           1..*   at least one");
        Line("     2..5   between 2 and 5");

        Section("From diagram to code");

        Line();
        Line("  Team o---- 1..* TeamPlayer          becomes");
        Line();
        Line("      class Team");
        Line("      {");
        Line("          private readonly List<TeamPlayer> _players = new();   // 1..*");
        Line("          public void Sign(TeamPlayer p) => _players.Add(p);    // GIVEN, not created");
        Line("      }");
        Line();
        Line("  House *---- 1..* Room           becomes");
        Line();
        Line("      class House");
        Line("      {");
        Line("          private readonly List<Room> _rooms = new();");
        Line("          public void AddRoom(string n, double a)");
        Line("              => _rooms.Add(new Room(n, a));                // CREATED inside");
        Line("      }");

        Section("Choosing the right relationship");

        Out("ask: IS the child genuinely a KIND of the parent?", "if yes, inheritance");
        Out("ask: can the part exist alone?", "yes -> aggregation, no -> composition");
        Out("ask: do I only need it for one call?", "dependency - pass it as a parameter");
        Warn("When in doubt, prefer composition or dependency over inheritance. They are looser, "
           + "easier to change and far easier to test. See lesson 2.13.");
    }
}

// --- composition: House owns its Rooms ---
public class Room
{
    public Room(string name, double areaM2)
    {
        Name = name;
        AreaM2 = areaM2;
    }

    public string Name { get; }
    public double AreaM2 { get; }
}

public class House
{
    private readonly List<Room> _rooms = new();

    public House(string address) => Address = address;

    public string Address { get; }
    public int RoomCount => _rooms.Count;

    /// <summary>The House CREATES the Room. That is composition.</summary>
    public void AddRoom(string name, double areaM2) => _rooms.Add(new Room(name, areaM2));

    public string Describe() => $"{Address}: {_rooms.Count} rooms, {_rooms.Sum(r => r.AreaM2)} m2";
}

// --- aggregation: Team refers to Players it did not create ---
public class TeamPlayer
{
    public TeamPlayer(string name) => Name = name;

    public string Name { get; }
}

public class Team
{
    private readonly List<TeamPlayer> _players = new();

    public Team(string name) => Name = name;

    public string Name { get; }

    /// <summary>The Team is GIVEN a TeamPlayer. That is aggregation.</summary>
    public void Sign(TeamPlayer player) => _players.Add(player);

    public void Release(TeamPlayer player) => _players.Remove(player);

    public string Describe() => _players.Count == 0
        ? $"{Name}: no players"
        : $"{Name}: {string.Join(", ", _players.Select(p => p.Name))}";
}

// --- dependency: Printer uses a Document without keeping it ---
public class Document
{
    public Document(string title, int pages)
    {
        Title = title;
        Pages = pages;
    }

    public string Title { get; }
    public int Pages { get; }
}

public class Printer
{
    /// <summary>Receives a Document, uses it, forgets it. No field, no ownership.</summary>
    public string Print(Document document) => $"printing \"{document.Title}\", {document.Pages} pages";
}

// --- association: two classes that know each other ---
public class Teacher
{
    private readonly List<SchoolStudent> _tutees = new();

    public Teacher(string name) => Name = name;

    public string Name { get; }
    public int TuteeCount => _tutees.Count;

    internal void AddTutee(SchoolStudent student) => _tutees.Add(student);
}

public class SchoolStudent
{
    public SchoolStudent(string name) => Name = name;

    public string Name { get; }
    public Teacher? Tutor { get; private set; }

    public void AssignTutor(Teacher teacher)
    {
        Tutor = teacher;
        teacher.AddTutee(this);          // keep both ends of the association in step
    }
}
