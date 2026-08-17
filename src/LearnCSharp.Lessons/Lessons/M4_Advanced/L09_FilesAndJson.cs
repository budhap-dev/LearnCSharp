using System.Text.Json;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Advanced;

// Notes: docs/module-4/4.9.md
public sealed class L09_FilesAndJson : LessonBase
{
    public override string Id => "4.9";
    public override string Title => "Files, directories and JSON";

    public override void Run()
    {
        // Everything happens in a temporary folder that is deleted at the end,
        // so running this lesson never leaves anything behind.
        string folder = Path.Combine(Path.GetTempPath(), "learncsharp-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(folder);

        try
        {
            Section("Paths - always build them with Path.Combine");

            string file = Path.Combine(folder, "scores.txt");
            Out("Path.Combine", file.Replace(Path.GetTempPath(), "<temp>/"));
            Out("GetFileName", Path.GetFileName(file));
            Out("GetFileNameWithoutExtension", Path.GetFileNameWithoutExtension(file));
            Out("GetExtension", Path.GetExtension(file));
            Out("DirectorySeparatorChar", Path.DirectorySeparatorChar);

            Note("Never write \"folder\\file.txt\" by hand - that breaks on macOS and Linux. "
               + "Path.Combine uses the right separator for the machine it runs on.");

            Section("Writing a whole file at once");

            File.WriteAllText(file, "Ada,91\n");
            File.AppendAllText(file, "Ben,64\n");
            File.AppendAllLines(file, ["Cara,78", "Dev,55"]);

            Out("File.Exists", File.Exists(file));
            Out("file size in bytes", new FileInfo(file).Length);

            Section("Reading a whole file at once");

            string all = File.ReadAllText(file);
            string[] lines = File.ReadAllLines(file);

            Out("ReadAllText length", all.Length);
            Out("ReadAllLines count", lines.Length);
            Out("first line", lines[0]);

            Section("Parsing the lines into objects");

            List<Score> scores = File.ReadAllLines(file)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split(','))
                .Select(parts => new Score(parts[0], int.Parse(parts[1])))
                .ToList();

            foreach (Score score in scores) Out("parsed", score);
            Out("average mark", Math.Round(scores.Average(s => s.Mark), 2));
            Out("top scorer", scores.MaxBy(s => s.Mark)?.Name);

            Section("Streaming a large file line by line");

            // ReadLines is lazy: it never holds the whole file in memory.
            int counted = 0;
            foreach (string line in File.ReadLines(file)) counted++;
            Out("File.ReadLines counted", counted);

            using (StreamWriter writer = new StreamWriter(Path.Combine(folder, "log.txt")))
            {
                writer.WriteLine("line one");
                writer.WriteLine("line two");
            }   // the using guarantees the file is flushed and closed

            using (StreamReader reader = new StreamReader(Path.Combine(folder, "log.txt")))
            {
                string? first = reader.ReadLine();
                Out("StreamReader first line", first);
            }

            Section("Handling the things that go wrong");

            try
            {
                File.ReadAllText(Path.Combine(folder, "does-not-exist.txt"));
            }
            catch (FileNotFoundException)
            {
                Out("reading a missing file", "FileNotFoundException");
            }

            Out("safe check first", File.Exists(Path.Combine(folder, "nope.txt")));

            Section("Directories");

            string sub = Path.Combine(folder, "saves");
            Directory.CreateDirectory(sub);
            File.WriteAllText(Path.Combine(sub, "slot1.sav"), "level 3");
            File.WriteAllText(Path.Combine(sub, "slot2.sav"), "level 7");

            Out("Directory.Exists", Directory.Exists(sub));
            Out("files in saves", string.Join(", ",
                Directory.GetFiles(sub).Select(Path.GetFileName)));
            Out("search pattern *.sav", Directory.GetFiles(sub, "*.sav").Length);
            Out("directories in root", string.Join(", ",
                Directory.GetDirectories(folder).Select(Path.GetFileName)));

            Section("Serialising objects to JSON");

            Player player = new Player("Ada", 7, 1250, ["sword", "shield", "potion"]);

            JsonSerializerOptions options = new() { WriteIndented = true };
            string json = JsonSerializer.Serialize(player, options);

            Line();
            foreach (string line in json.Split('\n')) Line(line);

            Section("Deserialising JSON back into objects");

            string savePath = Path.Combine(folder, "player.json");
            File.WriteAllText(savePath, json);

            Player? loaded = JsonSerializer.Deserialize<Player>(File.ReadAllText(savePath));

            Out("loaded.Name", loaded?.Name);
            Out("loaded.Level", loaded?.Level);
            Out("loaded.Inventory", string.Join(", ", loaded?.Inventory ?? []));
            Out("loaded == player", loaded == player);
            Note("false, and that is not a bug. A record compares each member, and List<string> "
               + "compares by REFERENCE, not by contents. Two lists holding the same strings are "
               + "still two different lists.");
            Out("comparing the contents instead",
                loaded?.Name == player.Name && (loaded?.Inventory.SequenceEqual(player.Inventory) ?? false));

            Section("JSON for a whole collection");

            List<Score> table = [new Score("Ada", 91), new Score("Ben", 64)];
            string tableJson = JsonSerializer.Serialize(table);
            Out("serialised list", tableJson);

            List<Score>? back = JsonSerializer.Deserialize<List<Score>>(tableJson);
            Out("deserialised count", back?.Count);

            Section("Handling bad JSON");

            try
            {
                JsonSerializer.Deserialize<Player>("{ this is not json }");
            }
            catch (JsonException)
            {
                Out("malformed JSON", "JsonException");
            }

            Section("Which method should I use?");

            Out("File.ReadAllText", "small file, want it as one string");
            Out("File.ReadAllLines", "small file, want an array of lines");
            Out("File.ReadLines", "large file - streams, low memory");
            Out("StreamReader/Writer", "fine-grained control, or writing as you go");
            Out("JsonSerializer", "saving and loading structured objects");
        }
        finally
        {
            // Always tidy up, whatever happened.
            Directory.Delete(folder, recursive: true);
            Out("temp folder deleted", !Directory.Exists(folder));
        }
    }
}

public record Score(string Name, int Mark);

public record Player(string Name, int Level, int Points, List<string> Inventory);
