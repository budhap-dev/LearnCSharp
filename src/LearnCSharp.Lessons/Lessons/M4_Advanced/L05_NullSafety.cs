using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Advanced;

// Notes: docs/module-4/4.5.md
public sealed class L05_NullSafety : LessonBase
{
    public override string Id => "4.5";
    public override string Title => "Null safety and nullable reference types";

    public override void Run()
    {
        Section("The problem");

        string? missing = null;
        try { _ = missing!.Length; }
        catch (NullReferenceException) { Out("missing.Length", "NullReferenceException"); }

        Note("Its inventor called null his 'billion dollar mistake'. C# 8 added a compiler feature "
           + "to catch it before you run: <Nullable>enable</Nullable> in the .csproj.");

        Section("With nullable enabled, the type says whether null is allowed");

        Out("string name", "must never be null - the compiler enforces it");
        Out("string? name", "may be null - the compiler makes you check");

        // string cannot = null;      <- warning: cannot convert null to a non-nullable
        string always = "always here";
        string? sometimes = null;

        Out("always.Length - no check needed", always.Length);
        Out("sometimes?.Length - checked", sometimes?.Length);

        Section("The null operators");

        Out("?.  null-conditional", sometimes?.ToUpperInvariant());
        Out("??  null-coalescing", sometimes ?? "default value");

        string? user = null;
        user ??= "guest";                         // assign only if null
        Out("??= null-coalescing assignment", user);

        Person? person = null;
        Out("person?.Address?.City - chains safely", person?.Address?.City);
        Out("person?.Address?.City ?? \"unknown\"", person?.Address?.City ?? "unknown");

        int[]? maybeArray = null;
        Out("maybeArray?[0] - works on indexers too", maybeArray?[0]);
        Out("maybeArray?.Length ?? 0", maybeArray?.Length ?? 0);

        Section("Narrowing: once you check, the warning goes away");

        string? input = GetInput(true);

        if (input is not null)
        {
            // Inside this block the compiler KNOWS input is not null, so .Length is fine.
            Out("after 'is not null' check", input.Length);
        }

        if (string.IsNullOrEmpty(input))
            Out("empty branch", "nothing to do");
        else
            Out("IsNullOrEmpty also narrows", input.ToUpperInvariant());

        Section("The ! operator: 'trust me, it is not null'");

        string? definitelyThere = GetInput(true);
        Out("definitelyThere!.Length", definitelyThere!.Length);

        Warn("The ! (null-forgiving) operator switches OFF the compiler's check. It does not make "
           + "anything safe - it just silences the warning. Use it only when you can prove the value "
           + "is there and the compiler cannot see why.");

        Section("Guard clauses at the top of a method");

        try { Greet(null!); }
        catch (ArgumentNullException ex) { Out("Greet(null)", $"ArgumentNullException ({ex.ParamName})"); }

        Out("Greet(\"Ada\")", Greet("Ada"));

        Section("Patterns that work well with null");

        object? value = null;
        Out("value is null", value is null);
        Out("value is not null", value is not null);

        string? maybeName = "Ada";
        if (maybeName is { Length: > 2 } longName)
            Out("property pattern also checks non-null", longName);

        Section("Nullable VALUE types are a different mechanism");

        int? count = null;                        // Nullable<int>: a struct with a HasValue flag
        Out("int? count = null", count);
        Out("count.HasValue", count.HasValue);
        Out("count ?? -1", count ?? -1);
        Out("count.GetValueOrDefault(99)", count.GetValueOrDefault(99));

        count = 5;
        Out("count.Value after assignment", count.Value);

        int? a = 5, b = null;
        Out("5 + null (nullable arithmetic)", a + b);

        Section("Nullable reference types are COMPILE-TIME only");

        Out("int?", "a real, different type at run time (Nullable<int>)");
        Out("string?", "the same type as string at run time - just an annotation for the compiler");
        Note("Because it is only an annotation, data arriving from a file, a database or an API can "
           + "still be null even where the type says otherwise. Validate at the boundary.");

        Section("Practical rules");

        Out("1", "turn Nullable on in every new project (it already is in this one)");
        Out("2", "make your type non-nullable unless null genuinely means something");
        Out("3", "check at the boundary, then work with non-null values inside");
        Out("4", "return an empty list rather than null");
        Out("5", "use ?? to supply a sensible default early");
    }

    private static string? GetInput(bool provide) => provide ? "some text" : null;

    private static string Greet(string name)
    {
        ArgumentNullException.ThrowIfNull(name);   // one line instead of if + throw
        return $"Hello, {name}";
    }
}

public class Person
{
    public Address? Address { get; set; }
}

public class Address
{
    public string? City { get; set; }
}
