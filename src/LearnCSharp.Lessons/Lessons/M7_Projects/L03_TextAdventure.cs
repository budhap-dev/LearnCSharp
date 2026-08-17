using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Projects;

// Notes: docs/module-7/7.3.md
public sealed class L03_TextAdventure : LessonBase
{
    public override string Id => "7.3";
    public override string Title => "Project: text adventure (inheritance and interfaces)";

    public override void Run()
    {
        Section("What this project uses");

        Out("module 2", "abstract classes, interfaces, polymorphism, composition");
        Out("module 3", "Dictionary for the map, List for the inventory");
        Out("module 4", "events, pattern matching, delegates for the command table");
        Out("module 5", "the world map is a GRAPH; the solver is a breadth-first search");

        Section("The world");

        World world = BuildWorld();
        Line();
        world.Describe();

        Section("Polymorphism: every item is used differently");

        Line();
        Adventurer player = new("Ada");
        player.Take(new Key("brass key"));
        player.Take(new Potion("healing potion", heals: 30));
        player.Take(new Sword("iron sword", damage: 12));

        foreach (IItem item in player.Inventory)
            Line($"{item.Name,-18} {item.Describe()}");

        Section("Interfaces let unrelated things share a capability");

        foreach (IItem item in player.Inventory)
        {
            string effect = item switch
            {
                IUsable usable => usable.Use(player),
                _ => "nothing happens",
            };
            Out(item.Name, effect);
        }

        Out("player health", player.Health);

        Section("Events: the world reacts to what the player does");

        Line();
        player.HealthChanged += (_, e) => Line($"  [event] health {e.Before} -> {e.After}");
        player.Died += (_, _) => Line("  [event] you have died!");

        player.TakeDamage(40);
        player.TakeDamage(80);

        Section("Moving around the map");

        Adventurer explorer = new("Ben");
        Line();

        foreach (string direction in new[] { "north", "east", "east", "south", "west" })
        {
            MoveResult result = world.Move(direction);
            Line($"go {direction,-6} -> {result.Message}");
        }

        Section("The map is a graph, so a search can solve it");

        Out("rooms", world.RoomCount);
        Out("shortest route hall -> treasury", string.Join(" -> ", world.ShortestRoute("Hall", "Treasury")));
        Out("rooms reachable from Hall", world.Reachable("Hall").Count);

        Section("A command table built from delegates");

        Dictionary<string, Func<string, string>> commands = new(StringComparer.OrdinalIgnoreCase)
        {
            ["look"] = _ => world.Current.Description,
            ["go"] = direction => world.Move(direction).Message,
            ["inventory"] = _ => string.Join(", ", player.Inventory.Select(i => i.Name)),
            ["help"] = _ => "commands: look, go <direction>, inventory, help, quit",
        };

        Line();
        foreach (string input in new[] { "look", "go north", "inventory", "help", "dance" })
        {
            string[] parts = input.Split(' ', 2);
            string output = commands.TryGetValue(parts[0], out Func<string, string>? command)
                ? command(parts.ElementAtOrDefault(1) ?? "")
                : $"I do not know how to '{parts[0]}'.";

            Line($"> {input,-12} {output}");
        }

        Section("The real game loop");

        Line();
        Line("while (playing)");
        Line("{");
        Line("    Console.Write(\"> \");");
        Line("    string[] parts = (Console.ReadLine() ?? \"\").Trim().Split(' ', 2);");
        Line();
        Line("    if (parts[0] == \"quit\") break;");
        Line();
        Line("    Console.WriteLine(commands.TryGetValue(parts[0], out var command)");
        Line("        ? command(parts.ElementAtOrDefault(1) ?? \"\")");
        Line("        : \"I do not understand.\");");
        Line("}");

        Section("Extend it yourself");

        Out("1", "add locked doors that need the right Key in your inventory");
        Out("2", "add monsters with an ICombatant interface and a turn-based fight");
        Out("3", "save and load the game state as JSON");
        Out("4", "load the map from a text file so you can design levels without recompiling");
        Out("5", "add a 'hint' command that runs the BFS and tells you which way to go");
    }

    private static World BuildWorld()
    {
        World world = new();

        world.AddRoom("Hall", "A dusty hall. Portraits watch you.");
        world.AddRoom("Library", "Shelves of rotting books.");
        world.AddRoom("Kitchen", "It smells of old soup.");
        world.AddRoom("Cellar", "Dark, damp and cold.");
        world.AddRoom("Treasury", "Gold glitters in the torchlight.");

        world.Connect("Hall", "north", "Library");
        world.Connect("Hall", "east", "Kitchen");
        world.Connect("Library", "east", "Cellar");
        world.Connect("Kitchen", "north", "Cellar");
        world.Connect("Cellar", "east", "Treasury");

        world.Enter("Hall");
        return world;
    }
}

// ---- items: an interface for identity, a second one for a capability ----

public interface IItem
{
    string Name { get; }
    string Describe();
}

public interface IUsable
{
    string Use(Adventurer player);
}

public class Key : IItem
{
    public Key(string name) => Name = name;

    public string Name { get; }

    public string Describe() => "opens something, somewhere";
}

// A potion is an item AND usable - two interfaces on one class.
public class Potion : IItem, IUsable
{
    private readonly int _heals;

    public Potion(string name, int heals)
    {
        Name = name;
        _heals = heals;
    }

    public string Name { get; }

    public string Describe() => $"restores {_heals} health";

    public string Use(Adventurer player)
    {
        player.Heal(_heals);
        return $"you drink it and recover {_heals} health";
    }
}

public class Sword : IItem, IUsable
{
    private readonly int _damage;

    public Sword(string name, int damage)
    {
        Name = name;
        _damage = damage;
    }

    public string Name { get; }

    public string Describe() => $"deals {_damage} damage";

    public string Use(Adventurer player) => $"you swing it, dealing {_damage} damage";
}

// ---- the player ----

public class HealthEventArgs : EventArgs
{
    public HealthEventArgs(int before, int after)
    {
        Before = before;
        After = after;
    }

    public int Before { get; }
    public int After { get; }
}

public class Adventurer
{
    private readonly List<IItem> _inventory = new();

    public Adventurer(string name) => Name = name;

    public event EventHandler<HealthEventArgs>? HealthChanged;
    public event EventHandler? Died;

    public string Name { get; }
    public int Health { get; private set; } = 100;
    public bool IsAlive => Health > 0;

    public IReadOnlyList<IItem> Inventory => _inventory;

    public void Take(IItem item) => _inventory.Add(item);

    public void Heal(int amount) => SetHealth(Math.Min(100, Health + amount));

    public void TakeDamage(int amount) => SetHealth(Math.Max(0, Health - amount));

    private void SetHealth(int value)
    {
        int before = Health;
        Health = value;

        HealthChanged?.Invoke(this, new HealthEventArgs(before, Health));

        if (before > 0 && Health == 0) Died?.Invoke(this, EventArgs.Empty);
    }
}

// ---- the world: a graph of rooms ----

public record MoveResult(bool Moved, string Message);

public class Room
{
    public Room(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; }
    public string Description { get; }

    /// <summary>direction -> the room it leads to.</summary>
    public Dictionary<string, string> Exits { get; } = new(StringComparer.OrdinalIgnoreCase);
}

public class World
{
    private readonly Dictionary<string, Room> _rooms = new(StringComparer.OrdinalIgnoreCase);

    public Room Current { get; private set; } = null!;

    public int RoomCount => _rooms.Count;

    public void AddRoom(string name, string description) => _rooms[name] = new Room(name, description);

    /// <summary>Adds the exit both ways, so every corridor works in reverse.</summary>
    public void Connect(string from, string direction, string to)
    {
        _rooms[from].Exits[direction] = to;
        _rooms[to].Exits[Opposite(direction)] = from;
    }

    public void Enter(string room) => Current = _rooms[room];

    public MoveResult Move(string direction)
    {
        if (!Current.Exits.TryGetValue(direction, out string? destination))
            return new MoveResult(false, $"you cannot go {direction} from the {Current.Name}");

        Current = _rooms[destination];
        return new MoveResult(true, $"you are in the {Current.Name}. {Current.Description}");
    }

    /// <summary>Breadth-first search over the rooms - exactly lesson 5.8, applied.</summary>
    public List<string> ShortestRoute(string from, string to)
    {
        Dictionary<string, string?> cameFrom = new(StringComparer.OrdinalIgnoreCase) { [from] = null };
        Queue<string> pending = new([from]);

        while (pending.Count > 0)
        {
            string room = pending.Dequeue();
            if (string.Equals(room, to, StringComparison.OrdinalIgnoreCase)) break;

            foreach (string next in _rooms[room].Exits.Values)
            {
                if (cameFrom.ContainsKey(next)) continue;
                cameFrom[next] = room;
                pending.Enqueue(next);
            }
        }

        if (!cameFrom.ContainsKey(to)) return [];

        List<string> route = new();
        for (string? at = to; at is not null; at = cameFrom[at]) route.Add(at);
        route.Reverse();
        return route;
    }

    public HashSet<string> Reachable(string from)
    {
        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase) { from };
        Queue<string> pending = new([from]);

        while (pending.Count > 0)
            foreach (string next in _rooms[pending.Dequeue()].Exits.Values)
                if (seen.Add(next)) pending.Enqueue(next);

        return seen;
    }

    public void Describe()
    {
        foreach (Room room in _rooms.Values)
            Console.WriteLine($"      {room.Name,-10} exits: {string.Join(", ", room.Exits.Select(e => $"{e.Key}->{e.Value}"))}");
    }

    private static string Opposite(string direction) => direction.ToLowerInvariant() switch
    {
        "north" => "south",
        "south" => "north",
        "east" => "west",
        "west" => "east",
        "up" => "down",
        "down" => "up",
        _ => direction,
    };
}
