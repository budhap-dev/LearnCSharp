using Microsoft.Extensions.DependencyInjection;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Production;

// Notes: docs/module-6/6.5.md
public sealed class L05_DependencyInjection : LessonBase
{
    public override string Id => "6.5";
    public override string Title => "Dependency injection and architecture";

    public override string Summary =>
        "Lesson 2.13 taught injecting dependencies by hand; at scale the hand-wiring becomes "
        + "the problem. A DI container builds the object graph for you - meet "
        + "IServiceCollection, the three lifetimes, and when a container is overkill.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Register services in an IServiceCollection and resolve a whole object graph",
        "Choose between singleton, scoped and transient lifetimes",
        "Explain the composition root, and when a container is not worth it",
    ];

    public override void Run()
    {
        Section("The problem: hand-wiring grows quadratically");

        // 2.13 taught this - and it is right, at small scale:
        var handWired = new ReportService(
            new SqlOrderStore(new ConnectionFactory("server=prod")),
            new PdfRenderer(),
            new SmtpMailer(new ConnectionFactory("server=prod")));

        Out("hand-wired", handWired.Describe());
        Warn("Four objects already need careful ordering and a shared ConnectionFactory. Real "
           + "apps have hundreds - and every new constructor parameter ripples through every "
           + "place that builds the object. The WIRING becomes the maintenance burden.");

        Section("The container: describe once, resolve anywhere");

        ServiceCollection services = new();

        // Registration: match each abstraction to its implementation. THIS is the app's recipe.
        services.AddSingleton<ConnectionFactory>(_ => new ConnectionFactory("server=prod"));
        services.AddSingleton<IOrderStore, SqlOrderStore>();
        services.AddSingleton<IRenderer, PdfRenderer>();
        services.AddSingleton<IMailer, SmtpMailer>();
        services.AddTransient<ReportService>();

        using ServiceProvider provider = services.BuildServiceProvider();

        // Resolution: the container reads ReportService's constructor, sees what it needs,
        // builds the dependencies (and THEIR dependencies), and hands back the finished graph.
        ReportService fromContainer = provider.GetRequiredService<ReportService>();
        Out("container-built", fromContainer.Describe());

        Note("No factory code was written. Add a fifth constructor parameter tomorrow and "
           + "NOTHING else changes - the container reads the new constructor and supplies it. "
           + "That is the quadratic wiring cost gone.");

        Section("The three lifetimes");

        ServiceCollection demo = new();
        demo.AddSingleton<SingletonCounter>();
        demo.AddScoped<ScopedCounter>();
        demo.AddTransient<TransientCounter>();

        using ServiceProvider p = demo.BuildServiceProvider();

        using (IServiceScope scope1 = p.CreateScope())
        {
            var s1a = scope1.ServiceProvider.GetRequiredService<SingletonCounter>();
            var s1b = scope1.ServiceProvider.GetRequiredService<SingletonCounter>();
            var sc1a = scope1.ServiceProvider.GetRequiredService<ScopedCounter>();
            var sc1b = scope1.ServiceProvider.GetRequiredService<ScopedCounter>();
            var t1a = scope1.ServiceProvider.GetRequiredService<TransientCounter>();
            var t1b = scope1.ServiceProvider.GetRequiredService<TransientCounter>();

            Out("singleton, asked twice in scope 1", $"instance #{s1a.Id} then #{s1b.Id}");
            Out("scoped, asked twice in scope 1", $"instance #{sc1a.Id} then #{sc1b.Id}");
            Out("transient, asked twice in scope 1", $"instance #{t1a.Id} then #{t1b.Id}");
        }

        using (IServiceScope scope2 = p.CreateScope())
        {
            Out("singleton, in a NEW scope", $"instance #{scope2.ServiceProvider.GetRequiredService<SingletonCounter>().Id}");
            Out("scoped, in a NEW scope", $"instance #{scope2.ServiceProvider.GetRequiredService<ScopedCounter>().Id}");
        }

        Out("singleton", "ONE instance for the whole application - config, caches");
        Out("scoped", "one per scope - in a web app, a scope IS one request");
        Out("transient", "new every time - cheap, stateless services");

        Section("The composition root");

        Out("the rule", "registration happens in ONE place, at startup - the composition root");
        Out("everywhere else", "classes just declare constructor parameters; nobody calls new");
        Out("anti-pattern", "passing the provider around and calling GetService inside classes");
        Note("A class that asks the container for its dependencies has hidden them again - "
           + "the 'service locator' anti-pattern. Constructor parameters keep dependencies "
           + "visible in the signature, which is the whole point.");

        Section("Testing: the payoff, again");

        // The same ReportService, with fakes - no SQL, no SMTP, no container even needed:
        var testable = new ReportService(new FakeOrderStore(), new PdfRenderer(), new RecordingMailer());
        Out("under test", testable.Describe());
        Note("DI the PRINCIPLE (2.13) is what makes this possible; the CONTAINER is only "
           + "convenience at scale. Tests usually skip the container and pass fakes directly.");

        Section("When a container is overkill");

        Out("a console tool with 5 classes", "new them up in Main - done");
        Out("this course's lessons", "no container anywhere; none needed");
        Out("an ASP.NET web app", "the framework IS built on this container - you meet it day one");
        Out("the skill transfer", "AddSingleton/AddScoped in any ASP.NET tutorial is THIS lesson");

        Section("The habits");

        Out("1", "depend on interfaces; register implementations (2.13 D)");
        Out("2", "all registration in the composition root, nowhere else");
        Out("3", "lifetimes: default transient, scoped for per-request state, singleton for shared");
        Out("4", "never inject the provider itself - that is a hidden dependency");
        Out("5", "a singleton must be thread-safe: every request shares it (6.1)");
    }
}

// --- the object graph ---

public class ConnectionFactory
{
    public ConnectionFactory(string server) => Server = server;
    public string Server { get; }
}

public interface IOrderStore { int Count(); }
public interface IRenderer { string Render(int orders); }
public interface IMailer { string Send(string document); }

public class SqlOrderStore : IOrderStore
{
    private readonly ConnectionFactory _connections;
    public SqlOrderStore(ConnectionFactory connections) => _connections = connections;
    public int Count() => 42;
}

public class PdfRenderer : IRenderer
{
    public string Render(int orders) => $"PDF of {orders} orders";
}

public class SmtpMailer : IMailer
{
    private readonly ConnectionFactory _connections;
    public SmtpMailer(ConnectionFactory connections) => _connections = connections;
    public string Send(string document) => $"mailed \"{document}\"";
}

/// <summary>Top of the graph: declares what it needs, builds nothing itself.</summary>
public class ReportService
{
    private readonly IOrderStore _store;
    private readonly IRenderer _renderer;
    private readonly IMailer _mailer;

    public ReportService(IOrderStore store, IRenderer renderer, IMailer mailer)
    {
        _store = store;
        _renderer = renderer;
        _mailer = mailer;
    }

    public string Describe() => _mailer.Send(_renderer.Render(_store.Count()));
}

// --- test doubles ---

public class FakeOrderStore : IOrderStore
{
    public int Count() => 3;
}

public class RecordingMailer : IMailer
{
    public List<string> Sent { get; } = new();
    public string Send(string document) { Sent.Add(document); return $"recorded \"{document}\""; }
}

// --- lifetime demo counters: each counts its own constructions ---

public class SingletonCounter { private static int _n; public int Id { get; } = ++_n; }
public class ScopedCounter { private static int _n; public int Id { get; } = ++_n; }
public class TransientCounter { private static int _n; public int Id { get; } = ++_n; }
