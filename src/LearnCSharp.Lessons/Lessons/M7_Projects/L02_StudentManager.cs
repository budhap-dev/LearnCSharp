using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Projects;

// Notes: docs/module-7/7.2.md
public sealed class L02_StudentManager : LessonBase
{
    public override string Id => "7.2";
    public override string Title => "Project: student grade manager";

    public override void Run()
    {
        Section("What this project uses");

        Out("module 2", "classes, encapsulation, records, computed properties");
        Out("module 3", "List, Dictionary, LINQ grouping and aggregation");
        Out("module 4", "nullable handling, exceptions, extension methods");

        Section("Setting up");

        SchoolRegister register = new();

        register.Enrol(new Student(1, "Ada Lovelace", 10));
        register.Enrol(new Student(2, "Alan Turing", 10));
        register.Enrol(new Student(3, "Grace Hopper", 11));
        register.Enrol(new Student(4, "Edsger Dijkstra", 11));

        register.RecordMark(1, "Maths", 91);
        register.RecordMark(1, "Science", 88);
        register.RecordMark(1, "English", 72);
        register.RecordMark(2, "Maths", 78);
        register.RecordMark(2, "Science", 95);
        register.RecordMark(2, "English", 54);
        register.RecordMark(3, "Maths", 65);
        register.RecordMark(3, "Science", 71);
        register.RecordMark(3, "English", 89);
        register.RecordMark(4, "Maths", 45);
        register.RecordMark(4, "Science", 58);
        register.RecordMark(4, "English", 61);

        Out("students enrolled", register.Count);
        Out("marks recorded", register.MarkCount);

        Section("Validation refuses bad data");

        try { register.RecordMark(1, "Maths", 150); }
        catch (ArgumentOutOfRangeException) { Out("mark of 150", "rejected"); }

        try { register.RecordMark(99, "Maths", 50); }
        catch (KeyNotFoundException) { Out("unknown student id 99", "rejected"); }

        try { register.Enrol(new Student(1, "Duplicate", 10)); }
        catch (InvalidOperationException) { Out("duplicate id", "rejected"); }

        Section("Individual reports");

        Line();
        foreach (Student student in register.AllStudents)
            Line(register.ReportFor(student.Id));

        Section("Class statistics");

        Out("class average", register.ClassAverage().ToString("F2"));
        Out("highest overall", register.TopStudent()?.Name);
        Out("lowest overall", register.BottomStudent()?.Name);
        Out("pass rate (>= 50 in every subject)", $"{register.PassRate():P0}");

        Section("Per subject");

        Line();
        Line($"{"subject",-10}{"average",10}{"best",8}{"worst",8}{"passed",8}");
        foreach (SubjectStats stats in register.SubjectBreakdown())
            Line($"{stats.Subject,-10}{stats.Average,10:F1}{stats.Highest,8}{stats.Lowest,8}{stats.Passed,8}");

        Section("Per year group");

        foreach (IGrouping<int, Student> group in register.ByYear())
            Out($"year {group.Key}", $"{group.Count()} students, average {group.Average(s => s.Average):F1}");

        Section("Ranking");

        Line();
        int position = 1;
        foreach (Student student in register.Ranked())
            Line($"{position++}. {student.Name,-20} {student.Average,6:F1}  grade {student.Grade}");

        Section("Searching and filtering");

        Out("find by id 3", register.Find(3)?.Name);
        Out("find by id 99", register.Find(99)?.Name);
        Out("search 'a'", string.Join(", ", register.Search("a").Select(s => s.Name)));
        Out("grade A students", string.Join(", ", register.WithGrade('A').Select(s => s.Name)));
        Out("needs support (avg < 60)", string.Join(", ",
            register.AllStudents.Where(s => s.Average < 60).Select(s => s.Name)));

        Section("Grade boundaries as a switch expression");

        foreach (int mark in new[] { 95, 82, 71, 55, 42 })
            Out($"average {mark}", Student.GradeFor(mark));

        Section("Extend it yourself");

        Out("1", "save and load the register as JSON (lesson 4.9)");
        Out("2", "add a menu loop so a teacher can add students and marks live");
        Out("3", "add weighted subjects, where Maths counts double");
        Out("4", "track marks over time and show whether each student is improving");
        Out("5", "write unit tests for GradeFor and ClassAverage");
    }
}

public class Student
{
    private readonly Dictionary<string, int> _marks = new();

    public Student(int id, string name, int year)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("A student must have a name.", nameof(name));

        Id = id;
        Name = name;
        Year = year;
    }

    public int Id { get; }
    public string Name { get; }
    public int Year { get; }

    // Read-only view: outsiders can look but cannot add marks behind the register's back.
    public IReadOnlyDictionary<string, int> Marks => _marks;

    public double Average => _marks.Count == 0 ? 0 : _marks.Values.Average();

    public char Grade => GradeFor(Average);

    public bool PassedEverything => _marks.Count > 0 && _marks.Values.All(m => m >= 50);

    internal void SetMark(string subject, int mark)
    {
        if (mark is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(mark), "Marks run from 0 to 100.");

        _marks[subject] = mark;
    }

    public static char GradeFor(double average) => average switch
    {
        >= 90 => 'A',
        >= 80 => 'B',
        >= 70 => 'C',
        >= 60 => 'D',
        >= 50 => 'E',
        _ => 'U',
    };

    public override string ToString() => $"{Name} (year {Year}, average {Average:F1})";
}

public record SubjectStats(string Subject, double Average, int Highest, int Lowest, int Passed);

/// <summary>Owns the collection of students and every rule about changing it.</summary>
public class SchoolRegister
{
    private readonly Dictionary<int, Student> _students = new();

    public int Count => _students.Count;

    public int MarkCount => _students.Values.Sum(s => s.Marks.Count);

    public IEnumerable<Student> AllStudents => _students.Values;

    public void Enrol(Student student)
    {
        if (!_students.TryAdd(student.Id, student))
            throw new InvalidOperationException($"Student id {student.Id} is already enrolled.");
    }

    public void RecordMark(int studentId, string subject, int mark)
    {
        if (!_students.TryGetValue(studentId, out Student? student))
            throw new KeyNotFoundException($"No student with id {studentId}.");

        student.SetMark(subject, mark);
    }

    public Student? Find(int id) => _students.GetValueOrDefault(id);

    public IEnumerable<Student> Search(string text) =>
        _students.Values.Where(s => s.Name.Contains(text, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Student> WithGrade(char grade) => _students.Values.Where(s => s.Grade == grade);

    public IEnumerable<Student> Ranked() => _students.Values.OrderByDescending(s => s.Average);

    public double ClassAverage() =>
        _students.Values.Count == 0 ? 0 : _students.Values.Average(s => s.Average);

    public Student? TopStudent() => _students.Values.MaxBy(s => s.Average);

    public Student? BottomStudent() => _students.Values.MinBy(s => s.Average);

    public double PassRate() =>
        _students.Count == 0 ? 0 : (double)_students.Values.Count(s => s.PassedEverything) / _students.Count;

    public IEnumerable<IGrouping<int, Student>> ByYear() =>
        _students.Values.GroupBy(s => s.Year).OrderBy(g => g.Key);

    /// <summary>Flatten every student's marks, then group them the other way - by subject.</summary>
    public IEnumerable<SubjectStats> SubjectBreakdown() =>
        _students.Values
            .SelectMany(s => s.Marks, (student, mark) => new { mark.Key, mark.Value })
            .GroupBy(entry => entry.Key)
            .Select(g => new SubjectStats(
                g.Key,
                g.Average(e => e.Value),
                g.Max(e => e.Value),
                g.Min(e => e.Value),
                g.Count(e => e.Value >= 50)))
            .OrderBy(s => s.Subject);

    public string ReportFor(int studentId)
    {
        Student student = _students.TryGetValue(studentId, out Student? found)
            ? found
            : throw new KeyNotFoundException($"No student with id {studentId}.");

        string marks = string.Join(", ", student.Marks.Select(m => $"{m.Key} {m.Value}"));
        return $"{student.Name,-20} {marks,-45} avg {student.Average,5:F1}  grade {student.Grade}";
    }
}
