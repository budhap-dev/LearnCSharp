/**
 * Lesson data is produced by the .NET project (`dotnet run -- capture`) and served as
 * static JSON. The syllabus index is small and loaded once; each lesson's captured
 * output is fetched only when that lesson is opened.
 */

export interface SyllabusEntry {
  id: string;
  title: string;
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
    .then((raw: { Id: string; Title: string; Module: number; Doc: string; Sections: string[] }[]) =>
      raw.map((l) => ({
        id: l.Id,
        title: l.Title,
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
