# Course notes

Read the notes for a topic, then run the matching lesson and read its source.

Every page follows the same shape: **the idea → the syntax → worked detail → common mistakes → exercises.**

---

## Module 1 — Foundations

From GCSE pseudocode to real C#. By the end you can write any small console program.

| # | Topic | Notes | Code |
|---|---|---|---|
| 1.1 | Hello World and the shape of a C# program | [notes](module-1/1.1.md) | [code](../src/LearnCSharp.Console/Lessons/M1_Foundations/L01_HelloWorld.cs) |
| 1.2 | Variables, data types and constants | [notes](module-1/1.2.md) | [code](../src/LearnCSharp.Console/Lessons/M1_Foundations/L02_Variables.cs) |
| 1.3 | Console input, output and formatting | [notes](module-1/1.3.md) | [code](../src/LearnCSharp.Console/Lessons/M1_Foundations/L03_InputOutput.cs) |
| 1.4 | Operators and expressions | [notes](module-1/1.4.md) | [code](../src/LearnCSharp.Console/Lessons/M1_Foundations/L04_Operators.cs) |
| 1.5 | Selection: if, switch, pattern matching | [notes](module-1/1.5.md) | [code](../src/LearnCSharp.Console/Lessons/M1_Foundations/L05_Selection.cs) |
| 1.6 | Iteration: for, while, do-while, foreach | [notes](module-1/1.6.md) | [code](../src/LearnCSharp.Console/Lessons/M1_Foundations/L06_Iteration.cs) |
| 1.7 | Arrays: 1D, 2D and jagged | [notes](module-1/1.7.md) | [code](../src/LearnCSharp.Console/Lessons/M1_Foundations/L07_Arrays.cs) |
| 1.8 | Strings and text handling | [notes](module-1/1.8.md) | [code](../src/LearnCSharp.Console/Lessons/M1_Foundations/L08_Strings.cs) |
| 1.9 | Methods: parameters, return values, overloading | [notes](module-1/1.9.md) | [code](../src/LearnCSharp.Console/Lessons/M1_Foundations/L09_Methods.cs) |
| 1.10 | Type conversion, casting, Math and Random | [notes](module-1/1.10.md) | [code](../src/LearnCSharp.Console/Lessons/M1_Foundations/L10_TypeConversion.cs) |
| 1.11 | Errors, exceptions and debugging | [notes](module-1/1.11.md) | [code](../src/LearnCSharp.Console/Lessons/M1_Foundations/L11_ErrorsAndDebugging.cs) |

## Module 2 — Object-oriented programming

The big shift: from "a list of instructions" to "a set of objects that collaborate". Starts with
the *concepts*, then the C# that expresses them, then how to design with them.

| # | Topic | Notes | Code |
|---|---|---|---|
| 2.1 | What is object-oriented programming? | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L01_WhatIsOop.cs) |
| 2.2 | Classes, objects, fields and constructors | [notes](module-2/2.2.md) | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L02_ClassesAndObjects.cs) |
| 2.3 | Abstraction: showing what, hiding how | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L03_Abstraction.cs) |
| 2.4 | Encapsulation, properties, access modifiers | [notes](module-2/2.4.md) | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L04_Encapsulation.cs) |
| 2.5 | Inheritance and the object hierarchy | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L05_Inheritance.cs) |
| 2.6 | Polymorphism: virtual, override, abstract | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L06_Polymorphism.cs) |
| 2.7 | Interfaces | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L07_Interfaces.cs) |
| 2.8 | Value vs reference types, structs, enums | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L08_ValueVsReference.cs) |
| 2.9 | Records and immutable data | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L09_Records.cs) |
| 2.10 | ToString, Equals, GetHashCode, operators | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L10_ObjectMembers.cs) |
| 2.11 | Static members, composition, good design | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L11_StaticAndComposition.cs) |
| 2.12 | Class relationships and UML class diagrams | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L12_Relationships.cs) |
| 2.13 | Coupling, cohesion and SOLID | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L13_SolidPrinciples.cs) |
| 2.14 | Design patterns you will actually use | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L14_DesignPatterns.cs) |
| 2.15 | Workshop: modelling a real system | _to write_ | [code](../src/LearnCSharp.Console/Lessons/M2_Oop/L15_ModellingWorkshop.cs) |

## Module 3 — Collections, generics and LINQ

Choosing the right data structure, and querying it in one readable line.

| # | Topic | Notes | Code |
|---|---|---|---|
| 3.1 | List&lt;T&gt; | [notes](module-3/3.1.md) | [code](../src/LearnCSharp.Console/Lessons/M3_Collections/L01_Lists.cs) |
| 3.2 | Dictionary and HashSet | [notes](module-3/3.2.md) | [code](../src/LearnCSharp.Console/Lessons/M3_Collections/L02_Dictionaries.cs) |
| 3.3 | Stack, Queue and LinkedList | [notes](module-3/3.3.md) | [code](../src/LearnCSharp.Console/Lessons/M3_Collections/L03_StacksQueues.cs) |
| 3.4 | Generics | [notes](module-3/3.4.md) | [code](../src/LearnCSharp.Console/Lessons/M3_Collections/L04_Generics.cs) |
| 3.5 | IEnumerable, iterators and yield | [notes](module-3/3.5.md) | [code](../src/LearnCSharp.Console/Lessons/M3_Collections/L05_Enumerables.cs) |
| 3.6 | LINQ part 1: filter, project, order | [notes](module-3/3.6.md) | [code](../src/LearnCSharp.Console/Lessons/M3_Collections/L06_Linq.cs) |
| 3.7 | LINQ part 2: group, join, aggregate | [notes](module-3/3.7.md) | [code](../src/LearnCSharp.Console/Lessons/M3_Collections/L07_LinqAdvanced.cs) |

## Module 4 — Advanced C#

The features that separate a beginner from someone employable.

| # | Topic | Notes | Code |
|---|---|---|---|
| 4.1 | Delegates | [notes](module-4/4.1.md) | [code](../src/LearnCSharp.Console/Lessons/M4_Advanced/L01_Delegates.cs) |
| 4.2 | Lambdas and closures | [notes](module-4/4.2.md) | [code](../src/LearnCSharp.Console/Lessons/M4_Advanced/L02_Lambdas.cs) |
| 4.3 | Events | [notes](module-4/4.3.md) | [code](../src/LearnCSharp.Console/Lessons/M4_Advanced/L03_Events.cs) |
| 4.4 | Extension methods | [notes](module-4/4.4.md) | [code](../src/LearnCSharp.Console/Lessons/M4_Advanced/L04_ExtensionMethods.cs) |
| 4.5 | Null safety and nullable reference types | [notes](module-4/4.5.md) | [code](../src/LearnCSharp.Console/Lessons/M4_Advanced/L05_NullSafety.cs) |
| 4.6 | Tuples, deconstruction, pattern matching | [notes](module-4/4.6.md) | [code](../src/LearnCSharp.Console/Lessons/M4_Advanced/L06_TuplesPatterns.cs) |
| 4.7 | Exceptions in depth | [notes](module-4/4.7.md) | [code](../src/LearnCSharp.Console/Lessons/M4_Advanced/L07_Exceptions.cs) |
| 4.8 | IDisposable, using, memory and the GC | [notes](module-4/4.8.md) | [code](../src/LearnCSharp.Console/Lessons/M4_Advanced/L08_DisposableAndMemory.cs) |
| 4.9 | Files, directories and JSON | [notes](module-4/4.9.md) | [code](../src/LearnCSharp.Console/Lessons/M4_Advanced/L09_FilesAndJson.cs) |
| 4.10 | Async, await and Task | [notes](module-4/4.10.md) | [code](../src/LearnCSharp.Console/Lessons/M4_Advanced/L10_Async.cs) |
| 4.11 | Attributes and reflection | [notes](module-4/4.11.md) | [code](../src/LearnCSharp.Console/Lessons/M4_Advanced/L11_Reflection.cs) |

## Module 5 — Data structures and algorithms

The AQA algorithms topics, implemented properly in C# and measured for real.

| # | Topic | Notes | Code |
|---|---|---|---|
| 5.1 | Complexity and Big-O | [notes](module-5/5.1.md) | [code](../src/LearnCSharp.Console/Lessons/M5_DataStructures/L01_BigO.cs) |
| 5.2 | Searching: linear and binary | [notes](module-5/5.2.md) | [code](../src/LearnCSharp.Console/Lessons/M5_DataStructures/L02_Searching.cs) |
| 5.3 | Sorting: bubble → quicksort | [notes](module-5/5.3.md) | [code](../src/LearnCSharp.Console/Lessons/M5_DataStructures/L03_Sorting.cs) |
| 5.4 | Recursion | [notes](module-5/5.4.md) | [code](../src/LearnCSharp.Console/Lessons/M5_DataStructures/L04_Recursion.cs) |
| 5.5 | Linked lists from scratch | [notes](module-5/5.5.md) | [code](../src/LearnCSharp.Console/Lessons/M5_DataStructures/L05_LinkedLists.cs) |
| 5.6 | Stacks and queues from scratch | [notes](module-5/5.6.md) | [code](../src/LearnCSharp.Console/Lessons/M5_DataStructures/L06_StacksQueuesFromScratch.cs) |
| 5.7 | Binary search trees and traversals | [notes](module-5/5.7.md) | [code](../src/LearnCSharp.Console/Lessons/M5_DataStructures/L07_Trees.cs) |
| 5.8 | Graphs, BFS, DFS and Dijkstra | [notes](module-5/5.8.md) | [code](../src/LearnCSharp.Console/Lessons/M5_DataStructures/L08_Graphs.cs) |
| 5.9 | Hashing and hash tables | [notes](module-5/5.9.md) | [code](../src/LearnCSharp.Console/Lessons/M5_DataStructures/L09_Hashing.cs) |

## Module 6 — Production C#

Concurrency, performance, security, serialization and dependency injection — the topics that
separate "it works on my machine" from code other people depend on. *(Lessons to be written.)*

| # | Topic | Notes | Code |
|---|---|---|---|
| 6.1 | Threading and concurrency | _to write_ | _to write_ |
| 6.2 | Performance and benchmarking | _to write_ | _to write_ |
| 6.3 | Security essentials | _to write_ | _to write_ |
| 6.4 | Serialization in depth | _to write_ | _to write_ |
| 6.5 | Dependency injection and architecture | _to write_ | _to write_ |

## Module 7 — Mini projects

Everything above, combined into complete programs.

| # | Project | Notes | Code |
|---|---|---|---|
| 7.1 | Number guessing game | [notes](module-7/7.1.md) | [code](../src/LearnCSharp.Console/Lessons/M7_Projects/L01_NumberGuessing.cs) |
| 7.2 | Student grade manager | [notes](module-7/7.2.md) | [code](../src/LearnCSharp.Console/Lessons/M7_Projects/L02_StudentManager.cs) |
| 7.3 | Text adventure | [notes](module-7/7.3.md) | [code](../src/LearnCSharp.Console/Lessons/M7_Projects/L03_TextAdventure.cs) |
| 7.4 | To-do list with file storage | [notes](module-7/7.4.md) | [code](../src/LearnCSharp.Console/Lessons/M7_Projects/L04_TodoApp.cs) |

---

## From GCSE pseudocode to C#

| AQA pseudocode | C# |
|---|---|
| `x ← 5` | `int x = 5;` |
| `OUTPUT x` | `Console.WriteLine(x);` |
| `x ← USERINPUT` | `string? x = Console.ReadLine();` |
| `IF a > b THEN … ENDIF` | `if (a > b) { … }` |
| `FOR i ← 1 TO 10 … ENDFOR` | `for (int i = 1; i <= 10; i++) { … }` |
| `WHILE a > b … ENDWHILE` | `while (a > b) { … }` |
| `REPEAT … UNTIL a > b` | `do { … } while (a <= b);` |
| `LEN(word)` | `word.Length` |
| `POSITION(word, 'x')` | `word.IndexOf('x')` |
| `SUBSTRING(0, 3, word)` | `word.Substring(0, 4)` |
| `a MOD b` | `a % b` |
| `a DIV b` | `a / b` (for two ints) |
| `SUBROUTINE f(x) … ENDSUBROUTINE` | `void F(int x) { … }` |
