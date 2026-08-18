/**
 * Client-side full-text search over a build-time index (public/data/search-index.json).
 * No server, no dependency: a small weighted scorer good enough for 62 lessons.
 */
const base = import.meta.env.BASE_URL;

export interface SearchRecord {
  id: string;
  title: string;
  module: number;
  summary: string;
  objectives: string[];
  sections: string[];
  text: string;
}

export interface SearchHit {
  record: SearchRecord;
  score: number;
  /** A short excerpt of the body around the first match, for context. */
  snippet: string;
}

let indexPromise: Promise<SearchRecord[]> | null = null;

export function loadIndex(): Promise<SearchRecord[]> {
  indexPromise ??= fetch(`${base}data/search-index.json`).then((r) => r.json());
  return indexPromise;
}

/** Builds a highlighted snippet of `text` around the first occurrence of any term. */
function makeSnippet(text: string, terms: string[]): string {
  const lower = text.toLowerCase();
  let at = -1;
  for (const term of terms) {
    const found = lower.indexOf(term);
    if (found !== -1 && (at === -1 || found < at)) at = found;
  }
  if (at === -1) return text.slice(0, 140) + (text.length > 140 ? '…' : '');

  const start = Math.max(0, at - 50);
  const end = Math.min(text.length, at + 100);
  return (start > 0 ? '…' : '') + text.slice(start, end).trim() + (end < text.length ? '…' : '');
}

export function search(records: SearchRecord[], query: string): SearchHit[] {
  const terms = query
    .toLowerCase()
    .split(/\s+/)
    .map((t) => t.trim())
    .filter((t) => t.length > 1);

  if (terms.length === 0) return [];

  const hits: SearchHit[] = [];

  for (const record of records) {
    const title = record.title.toLowerCase();
    const summary = record.summary.toLowerCase();
    const objectives = record.objectives.join(' ').toLowerCase();
    const sections = record.sections.join(' ').toLowerCase();
    const body = record.text.toLowerCase();

    let score = 0;
    let matchedAll = true;

    for (const term of terms) {
      let termScore = 0;
      // Weight matches by where they appear - a title hit is worth far more than a body hit.
      if (title.includes(term)) termScore += 10;
      if (title.split(/\s+/).includes(term)) termScore += 6; // whole-word title match
      if (summary.includes(term)) termScore += 5;
      if (objectives.includes(term)) termScore += 3;
      if (sections.includes(term)) termScore += 3;
      if (body.includes(term)) termScore += 1;

      if (termScore === 0) matchedAll = false;
      score += termScore;
    }

    // Require every term to appear somewhere - AND semantics beat noisy OR for a small corpus.
    if (matchedAll && score > 0) {
      hits.push({
        record,
        score,
        snippet: makeSnippet(record.text, terms),
      });
    }
  }

  return hits.sort((a, b) => b.score - a.score || a.record.id.localeCompare(b.record.id, undefined, { numeric: true }));
}
