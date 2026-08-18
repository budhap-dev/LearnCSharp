/**
 * Lesson data is produced by the .NET project (`dotnet run -- capture`) and served as
 * static JSON. The syllabus index is small and loaded once; each lesson's captured
 * output is fetched only when that lesson is opened.
 */

export interface SyllabusEntry {
  id: string;
  title: string;
  /** What this topic is and why it matters - shown before any code. */
  summary: string;
  /** What the student should be able to do afterwards. */
  objectives: string[];
  module: number;
  doc: string;
  /** Section headings available in the captured output. */
  sections: string[];
}

export interface LessonOutputData {
  id: string;
  title: string;
  fullOutput: string;
  sections: Record<string, string>;
}

/** What each module is about, so a student can see the shape of the course at a glance. */
export const MODULE_INFO: Record<number, { name: string; blurb: string; emoji: string }> = {
  1: {
    name: 'Foundations',
    emoji: '🌱',
    blurb:
      'The building blocks every program is made of: variables, decisions, loops, arrays, ' +
      'text and methods. Starts from GCSE pseudocode and ends with you able to write any ' +
      'small console program and debug it.',
  },
  2: {
    name: 'Object-Oriented Programming',
    emoji: '🧩',
    blurb:
      'The big shift: from a list of instructions to a set of objects that own their data ' +
      'and collaborate. Covers the concepts first, then the C# that expresses them, then how ' +
      'to design and critique a model of your own.',
  },
  3: {
    name: 'Collections, Generics and LINQ',
    emoji: '📚',
    blurb:
      'Choosing the right way to store many things - lists, dictionaries, sets, stacks and ' +
      'queues - and then querying them in one readable line instead of a loop.',
  },
  4: {
    name: 'Advanced C#',
    emoji: '🚀',
    blurb:
      'The features that separate a beginner from someone employable: delegates, lambdas, ' +
      'events, null safety, exceptions, files, JSON, async and reflection.',
  },
  5: {
    name: 'Data Structures and Algorithms',
    emoji: '🧠',
    blurb:
      'How to measure an algorithm and how the classic ones actually work. Searching, ' +
      'sorting, recursion, linked lists, trees, graphs and hashing - each built from scratch ' +
      'and timed on real data.',
  },
  6: {
    name: 'Production C#',
    emoji: '🛡️',
    blurb:
      'Writing code other people depend on: threading, performance, security, serialization ' +
      'and dependency injection - the module that turns working code into dependable code.',
  },
  7: {
    name: 'Mini Projects',
    emoji: '🎮',
    blurb:
      'Everything above combined into complete, working programs - a game, a grade manager, ' +
      'a text adventure and a to-do app that saves to disk.',
  },
};

export const MODULE_NAMES: Record<number, string> = {
  1: 'Foundations',
  2: 'Object-Oriented Programming',
  3: 'Collections, Generics and LINQ',
  4: 'Advanced C#',
  5: 'Data Structures and Algorithms',
  6: 'Production C#',
  7: 'Mini Projects',
};

const base = import.meta.env.BASE_URL;

let syllabusPromise: Promise<SyllabusEntry[]> | null = null;

export function loadSyllabus(): Promise<SyllabusEntry[]> {
  syllabusPromise ??= fetch(`${base}data/syllabus.json`)
    .then((r) => r.json())
    .then(
      (
        raw: {
          Id: string;
          Title: string;
          Summary: string;
          Objectives: string[];
          Module: number;
          Doc: string;
          Sections: string[];
        }[],
      ) =>
        raw.map((l) => ({
          id: l.Id,
          title: l.Title,
          summary: l.Summary,
          objectives: l.Objectives,
          module: l.Module,
          doc: l.Doc,
          sections: l.Sections,
        })),
    );

  return syllabusPromise;
}

const outputCache = new Map<string, Promise<LessonOutputData>>();

export function loadOutput(id: string): Promise<LessonOutputData> {
  if (!outputCache.has(id)) {
    outputCache.set(
      id,
      fetch(`${base}data/lessons/${id}.json`)
        .then((r) => {
          if (!r.ok) throw new Error(`No captured output for lesson ${id}`);
          return r.json();
        })
        .then((raw: { Id: string; Title: string; FullOutput: string; Sections: Record<string, string> }) => ({
          id: raw.Id,
          title: raw.Title,
          fullOutput: raw.FullOutput,
          sections: raw.Sections,
        })),
    );
  }

  return outputCache.get(id)!;
}

export function modulesOf(syllabus: SyllabusEntry[]): number[] {
  return [...new Set(syllabus.map((l) => l.module))].sort((a, b) => a - b);
}

export function lessonsIn(syllabus: SyllabusEntry[], module: number): SyllabusEntry[] {
  return syllabus.filter((l) => l.module === module);
}

/** Markdown notes, bundled but split per lesson so only what is opened is downloaded. */
const notes = import.meta.glob('../content/lessons/*.md', { query: '?raw', import: 'default' });

export async function loadNotes(id: string): Promise<string | null> {
  const load = notes[`../content/lessons/${id}.md`];
  return load ? ((await load()) as string) : null;
}

/** Strips the leading --- frontmatter block; only the body gets rendered. */
export function stripFrontmatter(markdown: string): string {
  if (!markdown.startsWith('---')) return markdown;
  const end = markdown.indexOf('\n---', 3);
  return end === -1 ? markdown : markdown.slice(markdown.indexOf('\n', end + 1) + 1);
}
