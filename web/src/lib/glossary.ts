/**
 * The glossary: a hand-written list of C# and computer-science terms in src/data/glossary.json.
 * Each entry links to the lesson(s) that teach it, and to related terms. It ships in the main
 * bundle (it is small text) so the page and the header search can use it without a fetch.
 */
import raw from '../data/glossary.json';

export interface GlossaryEntry {
  term: string;
  /** URL-safe id, derived from the term. */
  slug: string;
  aliases: string[];
  definition: string;
  example?: string;
  /** Lesson ids that teach the term, most relevant first. */
  lessons: string[];
  related: string[];
}

interface RawEntry {
  term: string;
  aliases?: string[];
  definition: string;
  example?: string;
  lessons: string[];
  related?: string[];
}

export function slugOf(term: string): string {
  return term
    .toLowerCase()
    .replace(/<t>/g, '-t')
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '');
}

export const GLOSSARY: GlossaryEntry[] = (raw as RawEntry[])
  .map((e) => ({
    term: e.term,
    slug: slugOf(e.term),
    aliases: e.aliases ?? [],
    definition: e.definition,
    example: e.example,
    lessons: e.lessons,
    related: e.related ?? [],
  }))
  .sort((a, b) => a.term.localeCompare(b.term, 'en', { sensitivity: 'base' }));

const bySlug = new Map(GLOSSARY.map((e) => [e.slug, e]));

export function entryBySlug(slug: string): GlossaryEntry | undefined {
  return bySlug.get(slug);
}

/** The letter a term files under - digits and symbols go under '#'. */
export function letterOf(entry: GlossaryEntry): string {
  const first = entry.term[0].toUpperCase();
  return /[A-Z]/.test(first) ? first : '#';
}

/** Which module (1-7) an entry belongs to, taken from its first lesson. */
export function moduleOf(entry: GlossaryEntry): number {
  return Number(entry.lessons[0]?.split('.')[0] ?? 0);
}

export interface GlossaryHit {
  entry: GlossaryEntry;
  score: number;
}

function terms(query: string): string[] {
  return query
    .toLowerCase()
    .split(/\s+/)
    .map((t) => t.trim())
    .filter(Boolean);
}

/**
 * Ranks entries against a query. Every term must match somewhere; a hit on the name or an
 * alias outranks one buried in the definition, and an exact name match comes first of all.
 */
export function searchGlossary(query: string, entries: GlossaryEntry[] = GLOSSARY): GlossaryHit[] {
  const q = query.trim().toLowerCase();
  const words = terms(q);
  if (words.length === 0) return [];

  const hits: GlossaryHit[] = [];

  for (const entry of entries) {
    const name = entry.term.toLowerCase();
    const aliases = entry.aliases.map((a) => a.toLowerCase());
    const definition = entry.definition.toLowerCase();
    const example = entry.example?.toLowerCase() ?? '';

    let score = 0;
    if (name === q || aliases.includes(q)) score += 100;
    else if (name.startsWith(q) || aliases.some((a) => a.startsWith(q))) score += 40;

    let matchedAll = true;
    for (const w of words) {
      let s = 0;
      if (name.includes(w)) s += 20;
      if (aliases.some((a) => a.includes(w))) s += 12;
      if (definition.includes(w)) s += 3;
      if (example.includes(w)) s += 2;
      if (s === 0) matchedAll = false;
      score += s;
    }

    if (matchedAll) hits.push({ entry, score });
  }

  return hits.sort((a, b) => b.score - a.score || a.entry.term.localeCompare(b.entry.term));
}

/** Terms taught by a lesson, primary ones (listed first in `lessons`) before secondary. */
export function termsForLesson(lessonId: string): GlossaryEntry[] {
  return GLOSSARY.filter((e) => e.lessons.includes(lessonId)).sort((a, b) => {
    const pa = a.lessons.indexOf(lessonId);
    const pb = b.lessons.indexOf(lessonId);
    return pa - pb || a.term.localeCompare(b.term);
  });
}
