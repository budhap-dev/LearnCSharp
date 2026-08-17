using System.Reflection;

namespace LearnCSharp.Core;

/// <summary>
/// Finds every ILesson in this assembly using reflection, so the menu never needs editing.
/// (Reflection itself is taught in lesson 4.10 - this class is a real example of it.)
/// </summary>
public static class LessonRegistry
{
    private static readonly Lazy<IReadOnlyList<ILesson>> _all = new(Discover);

    public static IReadOnlyList<ILesson> All => _all.Value;

    private static IReadOnlyList<ILesson> Discover()
    {
        List<ILesson> lessons = new();

        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            bool isLesson = typeof(ILesson).IsAssignableFrom(type)
                            && !type.IsAbstract
                            && !type.IsInterface
                            && type.GetConstructor(Type.EmptyTypes) is not null;

            if (isLesson)
                lessons.Add((ILesson)Activator.CreateInstance(type)!);
        }

        return lessons.OrderBy(l => VersionKey(l.Id)).ToList();
    }

    /// <summary>Sorts "1.10" after "1.9" (a plain string sort would get that wrong).</summary>
    private static (int, int) VersionKey(string id)
    {
        string[] parts = id.Split('.');
        int.TryParse(parts.ElementAtOrDefault(0), out int major);
        int.TryParse(parts.ElementAtOrDefault(1), out int minor);
        return (major, minor);
    }

    public static Module ModuleOf(ILesson lesson) => (Module)VersionKey(lesson.Id).Item1;

    public static string ModuleName(Module module) => module switch
    {
        Module.Foundations => "Module 1 - Foundations (from GCSE pseudocode to C#)",
        Module.ObjectOriented => "Module 2 - Object Oriented Programming",
        Module.CollectionsAndGenerics => "Module 3 - Collections, Generics and LINQ",
        Module.Advanced => "Module 4 - Advanced C#",
        Module.DataStructuresAndAlgorithms => "Module 5 - Data Structures and Algorithms (AQA topics in C#)",
        Module.ProductionCSharp => "Module 6 - Production C# (concurrency, performance, security)",
        Module.Projects => "Module 7 - Put it all together: mini projects",
        _ => module.ToString(),
    };

    public static ILesson? Find(string id) =>
        All.FirstOrDefault(l => string.Equals(l.Id, id, StringComparison.OrdinalIgnoreCase));
}
