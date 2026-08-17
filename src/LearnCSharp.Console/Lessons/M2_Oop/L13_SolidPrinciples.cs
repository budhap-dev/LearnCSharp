using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Oop;

// Notes: docs/module-2/2.13.md
public sealed class L13_SolidPrinciples : LessonBase
{
    public override string Id => "2.13";
    public override string Title => "Coupling, cohesion and the SOLID principles";

    public override void Run()
    {
        Section("Coupling: how much classes depend on each other");

        Out("tight coupling", "change one class -> several others break");
        Out("loose coupling", "classes talk through small, stable contracts");
        Out("goal", "LOW coupling");

        Line();
        Line("  TIGHT                              LOOSE");
        Line("  class OrderService                 class OrderService");
        Line("  {                                  {");
        Line("      SqlDatabase _db = new();           readonly IRepository _repo;");
        Line("      //  ^ welded to SQL Server         //  ^ any storage will do");
        Line("  }                                  }");

        Section("Cohesion: how focused one class is");

        Out("low cohesion", "a class doing unrelated jobs - the 'Utils' or 'Manager' class");
        Out("high cohesion", "every member serves one clear purpose");
        Out("goal", "HIGH cohesion");

        Line();
        Line("  LOW COHESION                       HIGH COHESION");
        Line("  class Everything                   class Invoice        class EmailMessageSender");
        Line("  {                                  {                    {");
        Line("      CalculateVat()                     CalculateVat()       Send()");
        Line("      SendEmail()                        Total()          }");
        Line("      SaveToDisk()                   }                    class FileStore");
        Line("      ParseCsv()                                          { Save() }");
        Line("  }");

        Note("The target is always the same: high cohesion, low coupling. Every principle below "
           + "is a specific technique for getting there.");

        Section("S - Single Responsibility");

        Line("A class should have ONE reason to change.");
        Line();
        Line("  BAD   class Report { Generate(); SaveToFile(); Email(); }");
        Line("        three reasons to change: report format, file format, mail server");
        Line();
        Line("  GOOD  class Report      { Generate(); }");
        Line("        class FileSaver   { Save(report); }");
        Line("        class Mailer      { Send(report); }");

        Report report = new("Sales Q3");
        Out("report.Generate()", report.Generate());
        Out("new FileSaver().Save(...)", new FileSaver().Save(report));
        Out("new Mailer().Send(...)", new Mailer().Send(report, "head@school.uk"));

        Section("O - Open/Closed");

        Line("Open to EXTENSION, closed to MODIFICATION.");
        Line();
        Line("  BAD   a switch you must edit every time a shape is invented");
        Line("        double Area(Shape s) { switch (s.Type) { case \"circle\": ... } }");
        Line();
        Line("  GOOD  each shape knows its own area; the calculator never changes");

        AreaShape[] shapes = [new AreaCircle(3), new AreaSquare(4), new AreaTriangle(3, 4)];
        foreach (AreaShape shape in shapes)
            Out(shape.GetType().Name, shape.Area().ToString("F2"));

        Out("TotalArea - never needs editing", shapes.Sum(s => s.Area()).ToString("F2"));
        Note("Adding a Pentagon means writing ONE new class. No existing file is touched, so no "
           + "existing behaviour can break.");

        Section("L - Liskov Substitution");

        Line("A subclass must work anywhere its parent works, without surprises.");

        Out("Rectangle(5, 4).Area()", new LskRectangle(5, 4).Area());
        Out("Square(5).Area()", new LskSquare(5).Area());

        Line();
        Line("  The classic violation: making Square inherit Rectangle.");
        Line("      rect.Width = 5; rect.Height = 4;   caller expects area 20");
        Line("      but if rect is really a Square, setting Width also sets Height -> area 16");
        Line("      The subclass BROKE a promise the parent made.");
        Line();
        Line("  Fix: they are siblings, not parent and child. Both implement IShape.");

        Warn("Symptoms of a violation: a subclass that throws NotSupportedException, ignores a "
           + "setter, or needs the caller to check its type first.");

        Section("I - Interface Segregation");

        Line("Many small interfaces beat one large one.");
        Line();
        Line("  BAD   interface IMachine { Print(); Scan(); Fax(); }");
        Line("        an old printer must implement Scan() and Fax() it cannot do");
        Line();
        Line("  GOOD  interface IPrinter { Print(); }");
        Line("        interface IScanner { Scan(); }");
        Line("        class AllInOne : IPrinter, IScanner");

        Out("SimplePrinter implements", "IPrinter only");
        Out("new SimplePrinter().Print()", new SimplePrinter().Print("essay.pdf"));
        Out("AllInOne implements", "IPrinter and IScanner");
        Out("new AllInOne().Scan()", new AllInOne().Scan());

        Section("D - Dependency Inversion");

        Line("Depend on ABSTRACTIONS, not on concrete classes.");
        Line();
        Line("  BAD   class AlertService { EmailMessageSender _s = new(); }   // welded to email");
        Line("  GOOD  class AlertService { IMessageSender _s;      }   // anything that sends");

        // The same AlertService, two completely different behaviours - decided by the caller.
        Out("with email", new AlertService(new EmailMessageSender()).Notify("Homework due"));
        Out("with SMS", new AlertService(new SmsMessageSender()).Notify("Homework due"));

        RecordingSender recording = new();
        new AlertService(recording).Notify("test message");
        Out("with a test double", recording.Sent.Count);

        Note("This is what makes code testable. The AlertService can be tested with a fake sender - "
           + "no mail server, no network, no waiting. Lesson 6.5 takes this further with a "
           + "dependency injection container.");

        Section("Two more rules worth knowing");

        Out("DRY - Don't Repeat Yourself", "the same knowledge should live in exactly one place");
        Out("YAGNI - You Aren't Gonna Need It", "do not build for a future that may never arrive");
        Out("Law of Demeter", "talk to your friends, not your friends' friends:");
        Line("        BAD   order.Customer.Address.Postcode.Trim()");
        Line("        GOOD  order.CustomerPostcode()");

        Section("Applying them sensibly");

        Warn("SOLID is a set of heuristics, not laws. Splitting a 20-line program into nine "
           + "classes and four interfaces is worse, not better. Apply a principle when you feel "
           + "the pain it removes - duplicated changes, untestable code, a class you dread opening.");
    }
}

// --- S: one reason to change, each ---
public class Report
{
    public Report(string title) => Title = title;

    public string Title { get; }

    public string Generate() => $"[{Title}] generated with 3 sections";
}

public class FileSaver
{
    public string Save(Report report) => $"saved \"{report.Title}\" to disk";
}

public class Mailer
{
    public string Send(Report report, string to) => $"emailed \"{report.Title}\" to {to}";
}

// --- O: add a shape without editing anything that exists ---
public abstract class AreaShape
{
    public abstract double Area();
}

public class AreaCircle : AreaShape
{
    private readonly double _radius;

    public AreaCircle(double radius) => _radius = radius;

    public override double Area() => Math.PI * _radius * _radius;
}

public class AreaSquare : AreaShape
{
    private readonly double _side;

    public AreaSquare(double side) => _side = side;

    public override double Area() => _side * _side;
}

public class AreaTriangle : AreaShape
{
    private readonly double _baseLength, _height;

    public AreaTriangle(double baseLength, double height)
    {
        _baseLength = baseLength;
        _height = height;
    }

    public override double Area() => _baseLength * _height / 2;
}

// --- L: siblings, not parent and child, so neither can break the other's promises ---
public interface IAreaShape
{
    double Area();
}

public class LskRectangle : IAreaShape
{
    public LskRectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public double Width { get; }
    public double Height { get; }

    public double Area() => Width * Height;
}

public class LskSquare : IAreaShape
{
    public LskSquare(double side) => Side = side;

    public double Side { get; }

    public double Area() => Side * Side;
}

// --- I: small interfaces, so nobody implements what they cannot do ---
public interface IPrinterDevice
{
    string Print(string file);
}

public interface IScannerDevice
{
    string Scan();
}

public class SimplePrinter : IPrinterDevice
{
    public string Print(string file) => $"printed {file}";
}

public class AllInOne : IPrinterDevice, IScannerDevice
{
    public string Print(string file) => $"printed {file}";

    public string Scan() => "scanned one page";
}

// --- D: depend on the interface, never on the concrete sender ---
public interface IMessageSender
{
    string Send(string message);
}

public class EmailMessageSender : IMessageSender
{
    public string Send(string message) => $"email: {message}";
}

public class SmsMessageSender : IMessageSender
{
    public string Send(string message) => $"sms: {message}";
}

/// <summary>A test double: captures messages instead of sending them anywhere.</summary>
public class RecordingSender : IMessageSender
{
    public List<string> Sent { get; } = new();

    public string Send(string message)
    {
        Sent.Add(message);
        return "recorded";
    }
}

public class AlertService
{
    private readonly IMessageSender _sender;      // the abstraction, not a concrete class

    public AlertService(IMessageSender sender) => _sender = sender;

    public string Notify(string message) => _sender.Send(message);
}
