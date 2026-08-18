// Module 1 - Foundations. Every task is a complete console program: paste the starter into
// Program.cs of a `dotnet new console` project, make it print exactly the expected output.
// Verified by scripts/verify-worksheets.mjs - the solutions really run.

/** @type {import('./index').Worksheet} */
export default {
  module: 1,
  intro:
    'Variables, decisions, loops, arrays, strings and methods. Every program here runs with no input unless the task says otherwise.',
  tasks: [
    {
      id: '1.1',
      lesson: '1.1',
      title: 'Three lines',
      level: 1,
      task: 'Change the program so it prints exactly three lines: a greeting, your favourite number as a sentence, and a line of ten dashes. Watch the spelling and punctuation - the output must match character for character.',
      starter: `Console.WriteLine("Hello, world!");
// TODO: print two more lines`,
      expected: `Hello, world!
My favourite number is 7.
----------`,
      hints: [
        'Each Console.WriteLine call prints one line and moves to the next.',
        'A string can contain digits and dashes - "----------" is just text.',
      ],
      solution: `Console.WriteLine("Hello, world!");
Console.WriteLine("My favourite number is 7.");
Console.WriteLine("----------");`,
    },
    {
      id: '1.2',
      lesson: '1.2',
      title: 'Temperature converter',
      level: 1,
      task: 'Declare a `double` for a temperature in Celsius (start with 21.5), convert it to Fahrenheit with `F = C × 9 / 5 + 32`, and print both to one decimal place using string interpolation. Then change the Celsius value to 100 and print again.',
      starter: `double celsius = 21.5;
// TODO: work out fahrenheit and print both

// TODO: do it again for 100 degrees`,
      expected: `21.5°C is 70.7°F
100.0°C is 212.0°F`,
      hints: [
        'The formula is celsius * 9 / 5 + 32 - the multiplication and division happen before the addition.',
        'Format a double to one decimal place with :F1 inside the braces: {celsius:F1}.',
        'Reassign the variable (celsius = 100;) rather than declaring a new one with the same name.',
      ],
      solution: `double celsius = 21.5;
double fahrenheit = celsius * 9 / 5 + 32;
Console.WriteLine($"{celsius:F1}°C is {fahrenheit:F1}°F");

celsius = 100;
fahrenheit = celsius * 9 / 5 + 32;
Console.WriteLine($"{celsius:F1}°C is {fahrenheit:F1}°F");`,
    },
    {
      id: '1.3',
      lesson: '1.4',
      title: 'Integer arithmetic',
      level: 1,
      task: 'Given `int a = 17` and `int b = 5`, print the results of division, remainder, and the "real" division as a decimal. Then print whether `a` is even using the `%` operator and a `bool`.',
      starter: `int a = 17;
int b = 5;
// TODO: print a / b, a % b, and a divided by b as a decimal
// TODO: print whether a is even`,
      expected: `17 / 5 = 3
17 % 5 = 2
17 / 5.0 = 3.4
17 is even: False`,
      hints: [
        'int / int throws away the remainder. To get 3.4, make one side a double: a / (double)b or a / 5.0.',
        'a % 2 == 0 is a bool expression - you can print it directly.',
      ],
      solution: `int a = 17;
int b = 5;
Console.WriteLine($"{a} / {b} = {a / b}");
Console.WriteLine($"{a} % {b} = {a % b}");
Console.WriteLine($"{a} / {b}.0 = {a / (double)b}");
bool isEven = a % 2 == 0;
Console.WriteLine($"{a} is even: {isEven}");`,
    },
    {
      id: '1.4',
      lesson: '1.5',
      title: 'Grade boundaries',
      level: 1,
      task: 'Write a method `string Grade(int mark)` that returns "A" for 70 and above, "B" for 60-69, "C" for 50-59, "D" for 40-49 and "U" below 40. Print the grade for each mark in the array.',
      starter: `int[] marks = [95, 70, 69, 55, 40, 39, 0];

foreach (int mark in marks)
{
    Console.WriteLine($"{mark,3} -> {Grade(mark)}");
}

string Grade(int mark)
{
    // TODO: return the right letter
    return "?";
}`,
      expected: ` 95 -> A
 70 -> A
 69 -> B
 55 -> C
 40 -> D
 39 -> U
  0 -> U`,
      hints: [
        'Test the highest boundary first: if (mark >= 70) return "A"; then else if (mark >= 60) ...',
        'A switch expression with relational patterns is neat: mark switch { >= 70 => "A", >= 60 => "B", ... _ => "U" }.',
      ],
      solution: `int[] marks = [95, 70, 69, 55, 40, 39, 0];

foreach (int mark in marks)
{
    Console.WriteLine($"{mark,3} -> {Grade(mark)}");
}

string Grade(int mark) => mark switch
{
    >= 70 => "A",
    >= 60 => "B",
    >= 50 => "C",
    >= 40 => "D",
    _ => "U",
};`,
    },
    {
      id: '1.5',
      lesson: '1.6',
      title: 'FizzBuzz',
      level: 2,
      task: 'Print the numbers 1 to 15, one per line - but for multiples of 3 print "Fizz", for multiples of 5 print "Buzz", and for multiples of both print "FizzBuzz".',
      starter: `for (int i = 1; i <= 15; i++)
{
    // TODO: Fizz, Buzz, FizzBuzz or the number
    Console.WriteLine(i);
}`,
      expected: `1
2
Fizz
4
Buzz
Fizz
7
8
Fizz
Buzz
11
Fizz
13
14
FizzBuzz`,
      hints: [
        'Test for "both" first (i % 15 == 0, or i % 3 == 0 && i % 5 == 0) - otherwise 15 prints Fizz.',
        'The order of the if / else if branches is the whole puzzle.',
      ],
      solution: `for (int i = 1; i <= 15; i++)
{
    if (i % 15 == 0) Console.WriteLine("FizzBuzz");
    else if (i % 3 == 0) Console.WriteLine("Fizz");
    else if (i % 5 == 0) Console.WriteLine("Buzz");
    else Console.WriteLine(i);
}`,
    },
    {
      id: '1.6',
      lesson: '1.6',
      title: 'Times-table grid',
      level: 2,
      task: 'Use nested `for` loops to print a 5 × 5 multiplication grid. Every number is right-aligned in a column 4 characters wide, so the columns line up.',
      starter: `// TODO: nested loops - rows 1..5, columns 1..5
// Use Console.Write for the numbers and Console.WriteLine at the end of each row
for (int row = 1; row <= 5; row++)
{
    Console.WriteLine(row);
}`,
      expected: `   1   2   3   4   5
   2   4   6   8  10
   3   6   9  12  15
   4   8  12  16  20
   5  10  15  20  25`,
      hints: [
        'The outer loop is the row, the inner loop is the column; print row * col.',
        '{value,4} inside an interpolated string right-aligns the value in 4 characters.',
        'Console.Write does not move to a new line - call Console.WriteLine() once after the inner loop.',
      ],
      solution: `for (int row = 1; row <= 5; row++)
{
    for (int col = 1; col <= 5; col++)
    {
        Console.Write($"{row * col,4}");
    }
    Console.WriteLine();
}`,
    },
    {
      id: '1.7',
      lesson: '1.7',
      title: 'Score statistics',
      level: 2,
      task: 'From the array of scores, work out and print the highest, the lowest, the average (to two decimal places) and how many scores are above the average. Do it with loops - no LINQ yet.',
      starter: `int[] scores = [72, 45, 88, 91, 60, 55, 79];

// TODO: highest, lowest, average, count above average
`,
      expected: `Highest: 91
Lowest: 45
Average: 70.00
Above average: 4`,
      hints: [
        'Start highest at scores[0] (not 0!) and lowest at scores[0], then loop from index 1.',
        'Sum as an int, then divide by (double)scores.Length for the average.',
        'A second loop counts scores greater than the average.',
      ],
      solution: `int[] scores = [72, 45, 88, 91, 60, 55, 79];

int highest = scores[0];
int lowest = scores[0];
int sum = 0;

foreach (int s in scores)
{
    if (s > highest) highest = s;
    if (s < lowest) lowest = s;
    sum += s;
}

double average = sum / (double)scores.Length;

int above = 0;
foreach (int s in scores)
{
    if (s > average) above++;
}

Console.WriteLine($"Highest: {highest}");
Console.WriteLine($"Lowest: {lowest}");
Console.WriteLine($"Average: {average:F2}");
Console.WriteLine($"Above average: {above}");`,
    },
    {
      id: '1.8',
      lesson: '1.8',
      title: 'Palindrome checker',
      level: 2,
      task: 'Complete `IsPalindrome` so it ignores case, spaces and punctuation: "A man, a plan, a canal: Panama" is a palindrome. Only letters and digits count.',
      starter: `string[] phrases = ["racecar", "Never odd or even", "A man, a plan, a canal: Panama", "hello"];

foreach (string p in phrases)
{
    Console.WriteLine($"\\"{p}\\" -> {IsPalindrome(p)}");
}

bool IsPalindrome(string text)
{
    // TODO: keep only letters and digits, lower-case them, compare with the reverse
    return false;
}`,
      expected: `"racecar" -> True
"Never odd or even" -> True
"A man, a plan, a canal: Panama" -> True
"hello" -> False`,
      hints: [
        'char.IsLetterOrDigit(c) tells you whether to keep a character; char.ToLower(c) lowers it.',
        'Build the cleaned string with a StringBuilder or a loop, then compare index i with index Length - 1 - i.',
        'Alternatively reverse the cleaned string: new string(cleaned.Reverse().ToArray()).',
      ],
      solution: `string[] phrases = ["racecar", "Never odd or even", "A man, a plan, a canal: Panama", "hello"];

foreach (string p in phrases)
{
    Console.WriteLine($"\\"{p}\\" -> {IsPalindrome(p)}");
}

bool IsPalindrome(string text)
{
    var cleaned = new System.Text.StringBuilder();
    foreach (char c in text)
    {
        if (char.IsLetterOrDigit(c)) cleaned.Append(char.ToLower(c));
    }

    string s = cleaned.ToString();
    for (int i = 0; i < s.Length / 2; i++)
    {
        if (s[i] != s[s.Length - 1 - i]) return false;
    }
    return true;
}`,
    },
    {
      id: '1.9',
      lesson: '1.9',
      title: 'Overloaded Area',
      level: 2,
      task: 'In the static class `Geometry`, write three overloads of a method called `Area`, using exactly these signatures: `Area(double radius)` for a circle, `Area(double width, double height)` for a rectangle, and `Area(double a, double b, double c)` for a triangle by Heron\'s formula. Print each result to two decimal places. (Overloads live in a class - top-level local functions cannot be overloaded.)',
      starter: `Console.WriteLine($"Circle r=2: {Geometry.Area(2):F2}");
Console.WriteLine($"Rectangle 3x4: {Geometry.Area(3, 4):F2}");
Console.WriteLine($"Triangle 3,4,5: {Geometry.Area(3, 4, 5):F2}");

static class Geometry
{
    // TODO: three methods, all called Area - fill in the bodies
    public static double Area(double radius) => 0;
    public static double Area(double width, double height) => 0;
    public static double Area(double a, double b, double c) => 0;
}`,
      expected: `Circle r=2: 12.57
Rectangle 3x4: 12.00
Triangle 3,4,5: 6.00`,
      hints: [
        'Overloads share a name but must differ in parameter count or types - here 1, 2 and 3 doubles. They must be methods of a class, not local functions.',
        'Circle: Math.PI * radius * radius. Heron: s = (a+b+c)/2, area = Math.Sqrt(s(s-a)(s-b)(s-c)).',
      ],
      solution: `Console.WriteLine($"Circle r=2: {Geometry.Area(2):F2}");
Console.WriteLine($"Rectangle 3x4: {Geometry.Area(3, 4):F2}");
Console.WriteLine($"Triangle 3,4,5: {Geometry.Area(3, 4, 5):F2}");

static class Geometry
{
    public static double Area(double radius) => Math.PI * radius * radius;

    public static double Area(double width, double height) => width * height;

    public static double Area(double a, double b, double c)
    {
        double s = (a + b + c) / 2;
        return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
    }
}`,
    },
    {
      id: '1.10',
      lesson: '1.10',
      title: 'Robust number reader',
      level: 3,
      task: 'Read lines from the console until the user types "done". Add up every line that is a valid whole number, ignore (but count) the ones that are not, and print a summary. Use `int.TryParse` - the program must never crash. Type the input lines shown, one per line.',
      input: `10
20
abc
5.5
-3
done
`,
      starter: `int total = 0;
int invalid = 0;

// TODO: loop reading lines until "done"
//   - valid whole numbers add to total
//   - anything else counts as invalid

Console.WriteLine($"Total: {total}");
Console.WriteLine($"Invalid entries: {invalid}");`,
      expected: `Total: 27
Invalid entries: 2`,
      hints: [
        'while (true) { string? line = Console.ReadLine(); if (line == null || line == "done") break; ... }',
        'if (int.TryParse(line, out int n)) total += n; else invalid++;',
        '"5.5" is not a whole number, so TryParse returns false and it counts as invalid.',
      ],
      solution: `int total = 0;
int invalid = 0;

while (true)
{
    string? line = Console.ReadLine();
    if (line == null || line.Trim() == "done") break;

    if (int.TryParse(line, out int n)) total += n;
    else invalid++;
}

Console.WriteLine($"Total: {total}");
Console.WriteLine($"Invalid entries: {invalid}");`,
    },
    {
      id: '1.11',
      lesson: '1.11',
      title: 'Catch it',
      level: 3,
      task: 'The starter crashes. Wrap the risky code so that each item in the list is processed in turn: a valid item prints its result, a bad one prints a message, and the program always prints "Finished" at the end. Use `try` / `catch` / `finally` and catch the specific exception types shown in the messages.',
      starter: `string[] inputs = ["12", "0", "seven", "3"];

foreach (string s in inputs)
{
    int n = int.Parse(s);
    Console.WriteLine($"100 / {n} = {100 / n}");
}

Console.WriteLine("Finished");`,
      expected: `100 / 12 = 8
Cannot divide by zero: 0
Not a number: seven
100 / 3 = 33
Finished`,
      hints: [
        'Put the two risky lines in a try block inside the loop, so one failure does not stop the others.',
        'catch (DivideByZeroException) and catch (FormatException) as two separate catch blocks.',
        'A finally block after the loop, or a plain WriteLine, prints "Finished".',
      ],
      solution: `string[] inputs = ["12", "0", "seven", "3"];

try
{
    foreach (string s in inputs)
    {
        try
        {
            int n = int.Parse(s);
            Console.WriteLine($"100 / {n} = {100 / n}");
        }
        catch (DivideByZeroException)
        {
            Console.WriteLine($"Cannot divide by zero: {s}");
        }
        catch (FormatException)
        {
            Console.WriteLine($"Not a number: {s}");
        }
    }
}
finally
{
    Console.WriteLine("Finished");
}`,
    },
  ],
};
