using System.Reflection;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Advanced;

// Notes: docs/module-4/4.11.md
public sealed class L11_Reflection : LessonBase
{
    public override string Id => "4.11";
    public override string Title => "Attributes and reflection";

    public override void Run()
    {
        Section("Reflection: a program inspecting itself");

        Type type = typeof(Monster);

        Out("Name", type.Name);
        Out("FullName", type.FullName);
        Out("Namespace", type.Namespace);
        Out("BaseType", type.BaseType?.Name);
        Out("IsClass", type.IsClass);
        Out("IsSealed", type.IsSealed);

        Section("Listing members");

        Out("properties", string.Join(", ",
            type.GetProperties().Select(p => $"{p.PropertyType.Name} {p.Name}")));

        Out("public methods (declared here)", string.Join(", ",
            type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => m.Name)));

        Out("constructors", type.GetConstructors().Length);

        Section("Getting a type from an object");

        Monster goblin = new Monster("Goblin", 30, 5);
        Out("goblin.GetType().Name", goblin.GetType().Name);
        Out("typeof(Monster) == goblin.GetType()", typeof(Monster) == goblin.GetType());

        Section("Reading property values by name");

        foreach (PropertyInfo property in type.GetProperties())
            Out($"  {property.Name}", property.GetValue(goblin));

        Section("Creating objects and calling methods dynamically");

        object? created = Activator.CreateInstance(typeof(Monster), "Dragon", 500, 90);
        Out("Activator.CreateInstance", created);

        MethodInfo? attack = type.GetMethod(nameof(Monster.Attack));
        object? result = attack?.Invoke(goblin, [10]);
        Out("invoked Attack(10)", result);

        Section("Attributes: metadata attached to your code");

        // [Obsolete], [Serializable] and the test-framework [Fact]/[Test] are all attributes.
        Out("what they are", "extra information the compiler stores in the assembly");
        Out("how you read them", "with reflection, at run time");

        Section("Reading a custom attribute");

        foreach (PropertyInfo property in typeof(SaveGame).GetProperties())
        {
            ColumnAttribute? column = property.GetCustomAttribute<ColumnAttribute>();
            Out(property.Name, column is null ? "(not saved)" : $"column \"{column.Name}\", order {column.Order}");
        }

        Section("Using attributes to build something generic");

        SaveGame save = new SaveGame
        {
            PlayerName = "Ada",
            Level = 7,
            Score = 1250,
            TempCacheValue = "should not be saved",
        };

        Out("generated header", ToCsvHeader<SaveGame>());
        Out("generated row", ToCsvRow(save));

        Note("That CSV writer works for ANY class marked with [Column]. Serialisers, ORMs and test "
           + "runners all work exactly like this.");

        Section("Enum reflection");

        Out("names", string.Join(", ", Enum.GetNames<Element>()));
        Out("values", string.Join(", ", Enum.GetValues<Element>().Select(e => (int)e)));
        Out("descriptions", string.Join(", ",
            Enum.GetValues<Element>().Select(DescriptionOf)));

        Section("How this course uses reflection");

        // LessonRegistry finds every ILesson in the assembly - no manual list to maintain.
        Out("lessons discovered by reflection", Core.LessonRegistry.All.Count);
        Out("the code that does it", "src/LearnCSharp.Lessons/Core/LessonRegistry.cs");

        Section("The trade-offs");

        Out("powerful", "plugins, serialisers, test runners, dependency injection");
        Out("slow", "roughly 10-100x slower than a direct call - cache MethodInfo if you use it often");
        Out("unsafe", "typos become run-time errors, not compile errors - use nameof()");
        Out("advice", "wonderful for frameworks, rarely the right tool in everyday code");
    }

    private static string ToCsvHeader<T>() =>
        string.Join(",", typeof(T).GetProperties()
            .Select(p => new { p, attr = p.GetCustomAttribute<ColumnAttribute>() })
            .Where(x => x.attr is not null)
            .OrderBy(x => x.attr!.Order)
            .Select(x => x.attr!.Name));

    private static string ToCsvRow<T>(T item) =>
        string.Join(",", typeof(T).GetProperties()
            .Select(p => new { p, attr = p.GetCustomAttribute<ColumnAttribute>() })
            .Where(x => x.attr is not null)
            .OrderBy(x => x.attr!.Order)
            .Select(x => x.p.GetValue(item)?.ToString() ?? ""));

    private static string DescriptionOf(Element element)
    {
        DescriptionAttribute? description = typeof(Element)
            .GetField(element.ToString())
            ?.GetCustomAttribute<DescriptionAttribute>();

        return description?.Text ?? element.ToString();
    }
}

public class Monster
{
    public Monster(string name, int health, int attackPower)
    {
        Name = name;
        Health = health;
        AttackPower = attackPower;
    }

    public string Name { get; }
    public int Health { get; private set; }
    public int AttackPower { get; }

    public string Attack(int damage)
    {
        Health -= damage;
        return $"{Name} takes {damage} damage, {Health} health left";
    }

    public override string ToString() => $"{Name} ({Health} hp)";
}

/// <summary>A custom attribute. Inherit Attribute; the "Attribute" suffix is dropped when you use it.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ColumnAttribute : Attribute
{
    public ColumnAttribute(string name, int order)
    {
        Name = name;
        Order = order;
    }

    public string Name { get; }
    public int Order { get; }
}

[AttributeUsage(AttributeTargets.Field)]
public sealed class DescriptionAttribute : Attribute
{
    public DescriptionAttribute(string text) => Text = text;

    public string Text { get; }
}

public class SaveGame
{
    [Column("player", 1)]
    public string PlayerName { get; set; } = "";

    [Column("lvl", 2)]
    public int Level { get; set; }

    [Column("score", 3)]
    public int Score { get; set; }

    // No attribute, so the generic CSV writer skips it.
    public string TempCacheValue { get; set; } = "";
}

public enum Element
{
    [Description("burns")] Fire,
    [Description("soaks")] Water,
    [Description("grows")] Earth,
}
