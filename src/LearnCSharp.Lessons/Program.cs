using LearnCSharp.Core;

// ---------------------------------------------------------------------------
//  LearnCSharp - a self-contained C# course for a GCSE Computer Science student.
//
//  Usage:
//     dotnet run                 -> interactive menu
//     dotnet run -- 2.4          -> run one lesson
//     dotnet run -- module 3     -> run every lesson in module 3
//     dotnet run -- all          -> run the whole course top to bottom
//     dotnet run -- list         -> print the syllabus
// ---------------------------------------------------------------------------

if (args.Length > 0)
{
    return RunFromCommandLine(args);
}

RunInteractiveMenu();
return 0;

static int RunFromCommandLine(string[] args)
{
    string command = args[0].ToLowerInvariant();

    switch (command)
    {
        case "list":
        case "syllabus":
            PrintSyllabus();
            return 0;

        case "all":
            foreach (ILesson lesson in LessonRegistry.All) RunLesson(lesson);
            return 0;

        case "module":
            if (args.Length < 2 || !int.TryParse(args[1], out int moduleNumber))
            {
                Console.Error.WriteLine("Usage: dotnet run -- module <1-7>");
                return 1;
            }
            List<ILesson> inModule = LessonRegistry.All
                .Where(l => (int)LessonRegistry.ModuleOf(l) == moduleNumber)
                .ToList();
            if (inModule.Count == 0)
            {
                Console.Error.WriteLine($"No module {moduleNumber}. Try 1 to 7.");
                return 1;
            }
            foreach (ILesson lesson in inModule) RunLesson(lesson);
            return 0;

        default:
            ILesson? found = LessonRegistry.Find(args[0]);
            if (found is null)
            {
                Console.Error.WriteLine($"No lesson '{args[0]}'. Run 'dotnet run -- list' to see them all.");
                return 1;
            }
            RunLesson(found);
            return 0;
    }
}

static void RunLesson(ILesson lesson)
{
    Ui.Title(lesson.Id, lesson.Title, lesson.Doc);

    try
    {
        lesson.Run();
    }
    catch (Exception ex)
    {
        // A lesson blowing up should not kill a "run all" sweep.
        Ui.Warn($"This lesson threw {ex.GetType().Name}: {ex.Message}");
    }

    Console.WriteLine();
}

static void PrintSyllabus()
{
    Module? current = null;
    foreach (ILesson lesson in LessonRegistry.All)
    {
        Module module = LessonRegistry.ModuleOf(lesson);
        if (module != current)
        {
            current = module;
            Console.WriteLine();
            Ui.Section(LessonRegistry.ModuleName(module));
        }
        Console.WriteLine($"  {lesson.Id,-5} {lesson.Title}");
    }
    Console.WriteLine();
    Console.WriteLine($"  {LessonRegistry.All.Count} lessons in total.");
    Console.WriteLine();
}

static void RunInteractiveMenu()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine();
        Ui.Title("C#", "Learn C# - from GCSE to advanced");
        PrintSyllabus();
        Console.WriteLine("  Type a lesson id (e.g. 1.2), 'm3' for a whole module, 'all', or 'q' to quit.");
        Console.Write("  > ");

        string? input = Console.ReadLine()?.Trim();
        if (input is null or "q" or "quit" or "exit") return;
        if (input.Length == 0) continue;

        List<ILesson> toRun = new();

        if (input is "all")
        {
            toRun.AddRange(LessonRegistry.All);
        }
        else if (input.StartsWith('m') && int.TryParse(input[1..], out int moduleNumber))
        {
            toRun.AddRange(LessonRegistry.All.Where(l => (int)LessonRegistry.ModuleOf(l) == moduleNumber));
        }
        else if (LessonRegistry.Find(input) is { } lesson)
        {
            toRun.Add(lesson);
        }

        if (toRun.Count == 0)
        {
            Console.WriteLine("  Not found - press Enter to try again.");
            Console.ReadLine();
            continue;
        }

        Console.Clear();
        foreach (ILesson lesson in toRun) RunLesson(lesson);

        Console.WriteLine();
        Console.WriteLine("  Press Enter to go back to the menu.");
        Console.ReadLine();
    }
}
