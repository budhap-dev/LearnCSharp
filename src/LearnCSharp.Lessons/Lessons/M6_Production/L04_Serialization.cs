using System.Text.Json;
using System.Text.Json.Serialization;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Production;

// Notes: docs/module-6/6.4.md
public sealed class L04_Serialization : LessonBase
{
    public override string Id => "6.4";
    public override string Title => "Serialization in depth";

    public override string Summary =>
        "Lesson 4.9 round-tripped simple objects; real data is messier. Naming policies, "
        + "custom converters, polymorphic payloads and - the part everyone learns the hard "
        + "way - keeping old saved files loadable after the schema changes.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Control JSON shape with JsonSerializerOptions and attributes",
        "Serialise a class hierarchy polymorphically and read it back intact",
        "Version a schema so old files keep loading after the model changes",
    ];

    public override void Run()
    {
        Section("Naming policies: JSON convention vs C# convention");

        SaveGame save = new() { PlayerName = "Ada", Level = 7, LastPlayed = new DateOnly(2026, 8, 17) };

        Out("default", JsonSerializer.Serialize(save));

        JsonSerializerOptions camel = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        Out("camelCase policy", JsonSerializer.Serialize(save, camel));

        Note("JSON convention is camelCase; C# convention is PascalCase. Set the policy once "
           + "in options rather than renaming your C# properties. Deserialisation honours the "
           + "same policy - and PropertyNameCaseInsensitive = true forgives either.");

        Section("Attributes: per-property control");

        Configured configured = new() { Name = "Ada", Secret = "s3cret", Score = 0, Nickname = null };
        Out("with attributes", JsonSerializer.Serialize(configured));

        Out("[JsonPropertyName(\"display_name\")]", "renames one property only");
        Out("[JsonIgnore]", "Secret never reaches the file - passwords, caches, computed state");
        Out("WhenWritingNull", "nulls omitted, so files stay small and diffs stay readable");

        Section("Custom converters: types JSON does not know");

        // Money as "19.99 GBP" instead of an object - a custom converter owns the format.
        JsonSerializerOptions withConverter = new() { Converters = { new MoneyConverter() } };

        Purchase purchase = new("Keyboard", new Money(49.99m, "GBP"));
        string json = JsonSerializer.Serialize(purchase, withConverter);
        Out("with MoneyConverter", json);

        Purchase? back = JsonSerializer.Deserialize<Purchase>(json, withConverter);
        Out("round-trips", back?.Price.Amount == 49.99m && back.Price.Currency == "GBP");

        Note("A converter is ~15 lines: Read parses your format, Write emits it. Reach for one "
           + "when the default shape is wrong for a type - dates, money, units, legacy formats.");

        Section("Polymorphism: a list of different shapes");

        // Serialising List<Shape> loses the subtype... unless the base declares its children.
        List<Shape> shapes = [new Circle(3), new Rect(4, 5)];
        string shapesJson = JsonSerializer.Serialize(shapes);
        Out("polymorphic JSON", shapesJson);

        List<Shape>? loaded = JsonSerializer.Deserialize<List<Shape>>(shapesJson);
        Out("loaded[0] is Circle", loaded?[0] is Circle);
        Out("loaded[1] is Rect", loaded?[1] is Rect);

        Note("[JsonDerivedType] on the base writes a \"$type\" discriminator into the JSON and "
           + "uses it to rebuild the right subclass. Without it you get a Shape-shaped nothing "
           + "- or an exception. The 4.6 shape hierarchy, made saveable.");

        Section("Schema versioning: old files must keep loading");

        // Version 1 of the app saved this:
        string v1File = """{ "PlayerName": "Ada", "Level": 7 }""";

        // Version 2 added Health and renamed nothing. Old file, new model:
        SaveGameV2? migrated = JsonSerializer.Deserialize<SaveGameV2>(v1File);
        Out("v1 file into v2 model", $"{migrated?.PlayerName}, level {migrated?.Level}, health {migrated?.Health}");

        Out("rule 1: ADD with defaults", "new properties get defaults when absent - free back-compat");
        Out("rule 2: never RENAME", "old files still carry the old name - keep it, or map it");
        Out("rule 3: never REPURPOSE", "changing a field's meaning corrupts silently");
        Out("rule 4: version explicitly", "a SchemaVersion property makes real migration possible");

        // A real migration: v1 stored full name in one field; v3 wants two.
        string versioned = """{ "SchemaVersion": 1, "FullName": "Ada Lovelace" }""";
        using JsonDocument document = JsonDocument.Parse(versioned);
        int fileVersion = document.RootElement.GetProperty("SchemaVersion").GetInt32();
        Out("file declares version", fileVersion);
        Out("so the loader", "runs Migrate1To2, then 2To3... - each step small and testable");

        Section("Reading without a class: JsonDocument");

        string unknown = """{ "sensor": "temp-1", "readings": [19.5, 20.1, 20.7] }""";
        using JsonDocument doc = JsonDocument.Parse(unknown);
        double first = doc.RootElement.GetProperty("readings")[0].GetDouble();
        Out("dip into unknown JSON", $"sensor {doc.RootElement.GetProperty("sensor")}, first reading {first}");
        Note("For exploring JSON whose shape you do not control - or only need one field of - "
           + "JsonDocument avoids declaring a class at all.");

        Section("Other formats, honestly");

        Out("JSON", "the default: human-readable, universal, good tooling");
        Out("XML", "XmlSerializer exists; you will meet it in old systems and config files");
        Out("CSV", "tables for spreadsheets - you hand-rolled it in 4.11");
        Out("binary (protobuf etc.)", "smaller and faster when both ends are yours");
        Warn("BinaryFormatter - .NET's old binary serialiser - is REMOVED in modern .NET: "
           + "deserialising hostile bytes could execute code. If you meet it in old code, "
           + "that is a security bug (6.3), not a convenience.");

        Section("The habits");

        Out("1", "options are set once, shared everywhere - not rebuilt per call");
        Out("2", "validate after deserialising: the annotations do not survive the wire (4.5)");
        Out("3", "add, never rename or repurpose - and version the schema from day one");
        Out("4", "[JsonIgnore] anything secret before it ever reaches a file");
    }
}

public class SaveGame
{
    public string PlayerName { get; set; } = "";
    public int Level { get; set; }
    public DateOnly LastPlayed { get; set; }
}

public class Configured
{
    [JsonPropertyName("display_name")]
    public string Name { get; set; } = "";

    [JsonIgnore]
    public string Secret { get; set; } = "";

    public int Score { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Nickname { get; set; }
}

public record Money(decimal Amount, string Currency);

public record Purchase(string Item, Money Price);

/// <summary>Writes Money as "49.99 GBP" and reads it back - the whole converter.</summary>
public class MoneyConverter : JsonConverter<Money>
{
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string[] parts = reader.GetString()!.Split(' ');
        return new Money(decimal.Parse(parts[0]), parts[1]);
    }

    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
        => writer.WriteStringValue($"{value.Amount} {value.Currency}");
}

// The base class declares its children; $type in the JSON picks the subclass on the way back.
[JsonDerivedType(typeof(Circle), "circle")]
[JsonDerivedType(typeof(Rect), "rect")]
public abstract record Shape;
public record Circle(double Radius) : Shape;
public record Rect(double Width, double Height) : Shape;

public class SaveGameV2
{
    public string PlayerName { get; set; } = "";
    public int Level { get; set; }
    public int Health { get; set; } = 100;      // added in v2 - old files get the default
}
