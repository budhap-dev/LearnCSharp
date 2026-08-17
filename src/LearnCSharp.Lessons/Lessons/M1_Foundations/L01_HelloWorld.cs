using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Foundations;

// Notes: docs/module-1/1.1.md
public sealed class L01_HelloWorld : LessonBase
{
    public override string Id => "1.1";
    public override string Title => "Hello World and the shape of a C# program";

    public override void Run()
    {
        Section("Output");

        // WriteLine adds a line break at the end. Write does not.
        Console.WriteLine("      Hello, world!");
        Console.Write("      No line break here... ");
        Console.WriteLine("so this continues the same line.");

        Section("Statements, blocks and comments");

        // A statement ends with a semicolon.
        int score = 72;

        // A block groups statements inside { }. Indentation is for humans only.
        if (score > 50)
        {
            Out("score", score);
            Out("verdict", "pass");
        }

        /* This is a block comment.
           It can span several lines. */

        Section("Where the program starts");

        // The classic entry point looks like this:
        //
        //     class Program
        //     {
        //         static void Main(string[] args) { ... }
        //     }
        //
        // Program.cs in this project uses "top-level statements" instead: loose
        // statements that the compiler wraps in a Main method for you. Same result.

        Out("this program's name", Environment.GetCommandLineArgs()[0].Split('/')[^1]);
        Out("running on", Environment.OSVersion.Platform);
        Out(".NET version", Environment.Version);

        Section("The classic beginner bug");

        // A semicolon straight after an if is legal - and does nothing.
        if (score > 1000) ;
        {
            Out("this block always runs", "the ';' ended the if statement early");
        }

        Warn("Always use { } braces, even for one line. It removes a whole family of bugs.");
    }
}
