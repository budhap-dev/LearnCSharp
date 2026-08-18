// Module 7 - Mini projects. Real programs, kept deterministic for checking: the guessing
// game uses a fixed secret and reads guesses from input; the to-do list writes to a file in
// the current directory and reads it back.

/** @type {import('./index').Worksheet} */
export default {
  module: 7,
  intro:
    'Small complete programs that pull the course together. A couple read input or use a file - the task says which. Build them a piece at a time and run often.',
  tasks: [
    {
      id: '7.1',
      lesson: '7.1',
      title: 'Number guessing game',
      level: 2,
      task: 'The secret number is fixed at 42. Read guesses from the console one per line; for each, print "Too low", "Too high", or "Correct!" and stop. At the end print how many guesses it took. (Real games pick the secret with Random - here it is fixed so the output can be checked.)',
      input: `50
25
40
42
`,
      starter: `const int secret = 42;
int guesses = 0;

while (true)
{
    string? line = Console.ReadLine();
    if (line is null) break;
    int guess = int.Parse(line);
    guesses++;

    // TODO: compare guess to secret, print the hint, break when correct
}

Console.WriteLine($"Guesses: {guesses}");`,
      expected: `Too high
Too low
Too low
Correct!
Guesses: 4`,
      hints: [
        'if (guess < secret) Console.WriteLine("Too low"); else if (guess > secret) Console.WriteLine("Too high");',
        'else { Console.WriteLine("Correct!"); break; } - the break leaves the loop.',
        'guesses++ happens for every attempt, including the winning one.',
      ],
      solution: `const int secret = 42;
int guesses = 0;

while (true)
{
    string? line = Console.ReadLine();
    if (line is null) break;
    int guess = int.Parse(line);
    guesses++;

    if (guess < secret)
    {
        Console.WriteLine("Too low");
    }
    else if (guess > secret)
    {
        Console.WriteLine("Too high");
    }
    else
    {
        Console.WriteLine("Correct!");
        break;
    }
}

Console.WriteLine($"Guesses: {guesses}");`,
    },
    {
      id: '7.2',
      lesson: '7.2',
      title: 'Grade manager',
      level: 2,
      task: 'Model students with a name and a list of marks. Print each student\'s name and average (one decimal place), then the name of the top student by average. Use a class and a little LINQ.',
      starter: `var students = new List<Student>
{
    new Student("Ada", [80, 90, 100]),
    new Student("Alan", [60, 70, 65]),
    new Student("Grace", [95, 85, 90]),
};

foreach (Student s in students)
{
    Console.WriteLine($"{s.Name}: {s.Average():F1}");
}

// TODO: print "Top: <name>" - the student with the highest average

class Student
{
    public string Name { get; }
    public List<int> Marks { get; }
    public Student(string name, List<int> marks) => (Name, Marks) = (name, marks);

    // TODO: Average() returns the mean of Marks
    public double Average() => 0;
}`,
      expected: `Ada: 90.0
Alan: 65.0
Grace: 90.0
Top: Ada`,
      hints: [
        'Average can lean on LINQ: Marks.Average() already returns a double.',
        'Top student: students.OrderByDescending(s => s.Average()).First().Name.',
        'Ada and Grace tie on 90.0; OrderByDescending keeps the first-seen (Ada) on a tie.',
      ],
      solution: `var students = new List<Student>
{
    new Student("Ada", [80, 90, 100]),
    new Student("Alan", [60, 70, 65]),
    new Student("Grace", [95, 85, 90]),
};

foreach (Student s in students)
{
    Console.WriteLine($"{s.Name}: {s.Average():F1}");
}

Student top = students.OrderByDescending(s => s.Average()).First();
Console.WriteLine($"Top: {top.Name}");

class Student
{
    public string Name { get; }
    public List<int> Marks { get; }
    public Student(string name, List<int> marks) => (Name, Marks) = (name, marks);

    public double Average() => Marks.Average();
}`,
    },
    {
      id: '7.3',
      lesson: '7.3',
      title: 'Text adventure',
      level: 3,
      task: 'A tiny two-room adventure. The player is in the "Hall". Read commands from the console: "look" prints the current room, "go north" moves to the "Garden", "go south" returns to the "Hall", and "quit" ends with "Bye". An unknown command prints "Huh?".',
      input: `look
go north
look
go south
look
quit
`,
      starter: `string room = "Hall";

while (true)
{
    string? command = Console.ReadLine();
    if (command is null) break;

    // TODO: handle look, go north, go south, quit, and anything else
}`,
      expected: `You are in the Hall
You are in the Garden
You are in the Hall
Bye`,
      hints: [
        'A switch on command: "look" => Console.WriteLine($"You are in the {room}");',
        '"go north" sets room = "Garden"; "go south" sets room = "Hall". Neither prints anything on its own here.',
        '"quit" prints "Bye" then break; the default case prints "Huh?".',
      ],
      solution: `string room = "Hall";

while (true)
{
    string? command = Console.ReadLine();
    if (command is null) break;

    switch (command)
    {
        case "look":
            Console.WriteLine($"You are in the {room}");
            break;
        case "go north":
            room = "Garden";
            break;
        case "go south":
            room = "Hall";
            break;
        case "quit":
            Console.WriteLine("Bye");
            return;
        default:
            Console.WriteLine("Huh?");
            break;
    }
}`,
    },
    {
      id: '7.4',
      lesson: '7.4',
      title: 'To-do list with a file',
      level: 3,
      task: 'Save three to-do items to a file called "todo.txt" (one per line), then read the file back and print each item numbered "1. ...". This writes a real file in the current folder, so run it in your practice project. Use `File.WriteAllLines` and `File.ReadAllLines`.',
      starter: `string[] items = ["Buy milk", "Email Ada", "Practice C#"];

// TODO: write items to todo.txt, then read them back and print numbered
`,
      expected: `1. Buy milk
2. Email Ada
3. Practice C#`,
      hints: [
        'File.WriteAllLines("todo.txt", items); writes one item per line.',
        'string[] loaded = File.ReadAllLines("todo.txt"); reads them back.',
        'A for loop gives you the index: Console.WriteLine($"{i + 1}. {loaded[i]}");',
      ],
      solution: `string[] items = ["Buy milk", "Email Ada", "Practice C#"];

File.WriteAllLines("todo.txt", items);

string[] loaded = File.ReadAllLines("todo.txt");
for (int i = 0; i < loaded.Length; i++)
{
    Console.WriteLine($"{i + 1}. {loaded[i]}");
}`,
    },
  ],
};
