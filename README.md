# Learn C# — from GCSE Computer Science to advanced

A complete, self-teaching C# course for a student who has done **GCSE Computer Science (AQA)**
and wants to go from the very basics to genuinely advanced material.

It is two things at once:

- **`docs/`** — the notes. This is where the concepts are explained. Read these.
- **`src/`** — a runnable .NET console app with **57 worked examples** (62 planned), one per topic.
  Every example prints its results so you can see the idea actually happening.

## How to use it

For each topic:

1. Read the notes in `docs/`.
2. Run the matching lesson and watch the output.
3. Open the `.cs` file and read the code that produced it.
4. Do the exercises at the bottom of the notes.

## Running the lessons

```bash
dotnet run --project src/LearnCSharp.Lessons              # interactive menu
dotnet run --project src/LearnCSharp.Lessons -- 1.4       # one lesson
dotnet run --project src/LearnCSharp.Lessons -- module 3  # a whole module
dotnet run --project src/LearnCSharp.Lessons -- list      # the syllabus
dotnet run --project src/LearnCSharp.Lessons -- all       # everything, start to finish
```

Requires the [.NET SDK](https://dotnet.microsoft.com/download) (10.0 or later).

## The syllabus

| Module | Topic | Lessons |
|---|---|---|
| [1](docs/module-1/) | Foundations — from GCSE pseudocode to C# | 11 |
| [2](docs/module-2/) | Object-oriented programming — concepts, C#, and design | 15 |
| [3](docs/module-3/) | Collections, generics and LINQ | 7 |
| [4](docs/module-4/) | Advanced C# | 11 |
| [5](docs/module-5/) | Data structures and algorithms | 9 |
| [6](docs/module-6/) | Production C# — concurrency, performance, security | 5 |
| [7](docs/module-7/) | Mini projects | 4 |

**Start here: [docs/README.md](docs/README.md)** · Web app plan: **[STORIES.md](STORIES.md)**

## Project layout

```
docs/                      the notes - read these first
  module-1/ ... module-6/
src/LearnCSharp.Lessons/
  Program.cs               the menu and command-line runner
  Core/                    ILesson, the Ui helpers, the reflection-based registry
  Lessons/M1_Foundations/  one .cs file per lesson
  Lessons/M2_Oop/
  Lessons/M3_Collections/
  Lessons/M4_Advanced/
  Lessons/M5_DataStructures/
  Lessons/M6_Production/
  Lessons/M7_Projects/
```

## Suggested pace

Roughly one lesson per study session, two or three a week.

| Weeks | Module | Goal |
|---|---|---|
| 1–4 | 1 | Write small console programs confidently |
| 5–9 | 2 | Think in objects; model a system and design it well |
| 9–11 | 3 | Use the right collection; read and write LINQ |
| 12–16 | 4 | Understand the features professional C# actually uses |
| 17–20 | 5 | Implement and analyse the classic algorithms |
| 21–23 | 6 | Write code other people can depend on |
| 24–26 | 7 | Build complete programs of your own |
