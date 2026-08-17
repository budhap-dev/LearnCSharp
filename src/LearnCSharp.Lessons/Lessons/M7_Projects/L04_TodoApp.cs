using System.Text.Json;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Projects;

// Notes: docs/module-7/7.4.md
public sealed class L04_TodoApp : LessonBase
{
    public override string Id => "7.4";
    public override string Title => "Project: to-do list with file storage";

    public override void Run()
    {
        RunAsync().GetAwaiter().GetResult();
    }

    private static async Task RunAsync()
    {
        Section("What this project uses");

        Out("module 2", "records, enums, interfaces");
        Out("module 3", "List, LINQ filtering, grouping and ordering");
        Out("module 4", "async/await, JSON, file I/O, dependency injection");

        string folder = Path.Combine(Path.GetTempPath(), "todo-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, "todo.json");

        try
        {
            Section("The repository is behind an interface, so storage is swappable");

            ITodoStore store = new JsonTodoStore(path);
            TodoService service = new(store);

            Out("store type", store.GetType().Name);
            Out("items on a fresh start", (await service.LoadAsync()).Count);

            Section("Adding tasks");

            await service.AddAsync("Finish C# module 1", Priority.High, new DateOnly(2026, 8, 20));
            await service.AddAsync("Read about recursion", Priority.Medium, new DateOnly(2026, 8, 25));
            await service.AddAsync("Tidy the desk", Priority.Low, null);
            await service.AddAsync("Revise binary search", Priority.High, new DateOnly(2026, 8, 18));

            List<TodoItem> items = await service.LoadAsync();
            Out("items now", items.Count);

            Line();
            foreach (TodoItem item in items) Line(Format(item));

            Section("It really is on disk");

            Out("file exists", File.Exists(path));
            Out("file size (bytes)", new FileInfo(path).Length);
            Line();
            foreach (string line in (await File.ReadAllTextAsync(path)).Split('\n').Take(8))
                Line(line);

            Section("Completing tasks");

            await service.CompleteAsync(1);
            await service.CompleteAsync(4);

            Line();
            foreach (TodoItem item in await service.LoadAsync()) Line(Format(item));

            Out("completing an unknown id", await service.CompleteAsync(99));

            Section("Querying with LINQ");

            items = await service.LoadAsync();

            Out("outstanding", items.Count(i => !i.IsDone));
            Out("done", items.Count(i => i.IsDone));
            Out("high priority outstanding", string.Join(", ",
                items.Where(i => !i.IsDone && i.Priority == Priority.High).Select(i => i.Title)));
            Out("due before 22 Aug", string.Join(", ",
                items.Where(i => i.Due is not null && i.Due < new DateOnly(2026, 8, 22)).Select(i => i.Title)));
            Out("no deadline", string.Join(", ", items.Where(i => i.Due is null).Select(i => i.Title)));

            Line();
            foreach (IGrouping<Priority, TodoItem> group in items.GroupBy(i => i.Priority).OrderByDescending(g => g.Key))
                Line($"{group.Key,-8} {group.Count()} tasks, {group.Count(i => i.IsDone)} done");

            Section("Sorted the way a human wants to see it");

            Line();
            foreach (TodoItem item in items
                         .OrderBy(i => i.IsDone)                             // outstanding first
                         .ThenByDescending(i => i.Priority)                  // then most urgent
                         .ThenBy(i => i.Due ?? DateOnly.MaxValue))           // then soonest deadline
                Line(Format(item));

            Section("Persistence survives a restart");

            // A brand new service reading the same file - just like reopening the app.
            TodoService reopened = new(new JsonTodoStore(path));
            List<TodoItem> reloaded = await reopened.LoadAsync();

            Out("items after 'restart'", reloaded.Count);
            Out("completed states kept", reloaded.Count(i => i.IsDone));

            Section("Testing is easy because the store is an interface");

            // No file, no disk, no cleanup - the same service logic, a fake store.
            TodoService inMemory = new(new InMemoryTodoStore());
            await inMemory.AddAsync("A test task", Priority.Low, null);
            Out("in-memory store works too", (await inMemory.LoadAsync()).Count);

            Section("Extend it yourself");

            Out("1", "add tags, and filter by them");
            Out("2", "add a Console menu loop: add / list / done / delete / quit");
            Out("3", "highlight overdue tasks in red with Console.ForegroundColor");
            Out("4", "add recurring tasks that reappear when completed");
            Out("5", "swap JsonTodoStore for a CSV or SQLite store - nothing else needs to change");
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
            Line();
            Out("temp folder cleaned up", !Directory.Exists(folder));
        }
    }

    private static string Format(TodoItem item) =>
        $"[{(item.IsDone ? "x" : " ")}] #{item.Id} {item.Title,-28} {item.Priority,-7} "
        + $"due {(item.Due?.ToString("dd MMM") ?? "-")}";
}

public enum Priority
{
    Low,
    Medium,
    High,
}

public record TodoItem(int Id, string Title, Priority Priority, DateOnly? Due, bool IsDone);

/// <summary>The storage contract. The service depends on THIS, never on a file.</summary>
public interface ITodoStore
{
    Task<List<TodoItem>> LoadAsync();
    Task SaveAsync(List<TodoItem> items);
}

public class JsonTodoStore : ITodoStore
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    private readonly string _path;

    public JsonTodoStore(string path) => _path = path;

    public async Task<List<TodoItem>> LoadAsync()
    {
        if (!File.Exists(_path)) return [];        // a missing file just means "nothing saved yet"

        try
        {
            string json = await File.ReadAllTextAsync(_path);
            return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? [];
        }
        catch (JsonException)
        {
            // A corrupt file should not crash the app on start-up.
            return [];
        }
    }

    public async Task SaveAsync(List<TodoItem> items) =>
        await File.WriteAllTextAsync(_path, JsonSerializer.Serialize(items, Options));
}

/// <summary>A store that keeps everything in memory - ideal for tests.</summary>
public class InMemoryTodoStore : ITodoStore
{
    private List<TodoItem> _items = [];

    public Task<List<TodoItem>> LoadAsync() => Task.FromResult(_items.ToList());

    public Task SaveAsync(List<TodoItem> items)
    {
        _items = items.ToList();
        return Task.CompletedTask;
    }
}

/// <summary>All the rules. It has no idea whether the data lives in a file, a database or memory.</summary>
public class TodoService
{
    private readonly ITodoStore _store;

    public TodoService(ITodoStore store) => _store = store;

    public Task<List<TodoItem>> LoadAsync() => _store.LoadAsync();

    public async Task<TodoItem> AddAsync(string title, Priority priority, DateOnly? due)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("A task needs a title.", nameof(title));

        List<TodoItem> items = await _store.LoadAsync();

        int nextId = items.Count == 0 ? 1 : items.Max(i => i.Id) + 1;
        TodoItem item = new(nextId, title.Trim(), priority, due, false);

        items.Add(item);
        await _store.SaveAsync(items);

        return item;
    }

    public async Task<bool> CompleteAsync(int id)
    {
        List<TodoItem> items = await _store.LoadAsync();
        int index = items.FindIndex(i => i.Id == id);

        if (index < 0) return false;

        items[index] = items[index] with { IsDone = true };    // records make this a one-liner
        await _store.SaveAsync(items);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        List<TodoItem> items = await _store.LoadAsync();

        if (items.RemoveAll(i => i.Id == id) == 0) return false;

        await _store.SaveAsync(items);
        return true;
    }
}
