namespace LearnCSharp.Core;

/// <summary>
/// Every lesson implements this. The menu finds them automatically with reflection,
/// so adding a lesson is just: create a class, implement ILesson, rebuild.
///
/// The .cs lesson is the WORKED EXAMPLE. The matching .md file in /docs is where the
/// concepts are explained - always read the doc first, then run the code.
/// </summary>
public interface ILesson
{
    /// <summary>Dotted id, e.g. "1.3" - module number, then position in the module.</summary>
    string Id { get; }

    /// <summary>Title shown in the menu.</summary>
    string Title { get; }

    /// <summary>
    /// One or two sentences saying what this topic IS and why it matters. Shown on the
    /// module cards, the syllabus and at the top of the lesson - so a student always knows
    /// what they are about to learn before they see any code.
    /// </summary>
    string Summary { get; }

    /// <summary>What the student should be able to do once they have finished.</summary>
    IReadOnlyList<string> Objectives { get; }

    /// <summary>Path to the markdown notes for this lesson, relative to the repo root.</summary>
    string Doc { get; }

    /// <summary>Runs the demonstration. Must never block on Console.ReadLine.</summary>
    void Run();
}

public enum Module
{
    Foundations = 1,
    ObjectOriented = 2,
    CollectionsAndGenerics = 3,
    Advanced = 4,
    DataStructuresAndAlgorithms = 5,
    ProductionCSharp = 6,
    Projects = 7,
}

/// <summary>Gives lessons the Ui helpers without repeating "Ui." on every line.</summary>
public abstract class LessonBase : ILesson
{
    public abstract string Id { get; }
    public abstract string Title { get; }
    public abstract string Summary { get; }
    public abstract void Run();

    public virtual IReadOnlyList<string> Objectives => [];

    /// <summary>Defaults to the conventional docs path; lessons rarely need to override it.</summary>
    public virtual string Doc => $"docs/module-{Id.Split('.')[0]}/{Id}.md";

    protected static void Section(string title) => Ui.Section(title);
    protected static void Out(string label, object? value) => Ui.Out(label, value);
    protected static void Note(string text) => Ui.Note(text);
    protected static void Warn(string text) => Ui.Warn(text);
    protected static void Line(string text = "") => Console.WriteLine(text.Length == 0 ? "" : "      " + text);
}
