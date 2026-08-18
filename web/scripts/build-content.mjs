/**
 * Copies the lesson notes from docs/ into the site, adding frontmatter from the captured
 * syllabus. docs/ is the single source of truth - these copies are generated on every
 * build and gitignored, so the two can never drift apart.
 *
 * Run automatically by `npm run build`. Requires `npm run capture` to have run first.
 */
import { readFile, writeFile, mkdir, readdir, rm } from 'node:fs/promises';
import { join, basename } from 'node:path';

const DOCS = new URL('../../docs/', import.meta.url).pathname;
const OUT = new URL('../src/content/lessons/', import.meta.url).pathname;
const SYLLABUS = new URL('../public/data/syllabus.json', import.meta.url).pathname;
const SEARCH = new URL('../public/data/search-index.json', import.meta.url).pathname;

/** Reduce Markdown to searchable plaintext: drop code fences, links to their text, and syntax. */
function toPlainText(markdown) {
  return markdown
    .replace(/```[\s\S]*?```/g, ' ')        // fenced code blocks
    .replace(/`[^`]*`/g, ' ')                // inline code
    .replace(/!\[[^\]]*\]\([^)]*\)/g, ' ') // images
    .replace(/\[([^\]]*)\]\([^)]*\)/g, '$1') // links -> their text
    .replace(/[#>*_|-]+/g, ' ')              // markdown punctuation
    .replace(/\s+/g, ' ')
    .trim();
}

const syllabus = Object.fromEntries(
  JSON.parse(await readFile(SYLLABUS, 'utf8')).map((l) => [l.Id, l]),
);

await rm(OUT, { recursive: true, force: true });
await mkdir(OUT, { recursive: true });

let written = 0;
const skipped = [];
const searchIndex = [];

for (const dir of await readdir(DOCS, { withFileTypes: true })) {
  if (!dir.isDirectory()) continue;

  for (const file of await readdir(join(DOCS, dir.name))) {
    if (!file.endsWith('.md')) continue;

    const id = basename(file, '.md');
    const entry = syllabus[id];
    if (!entry) {
      skipped.push(`${dir.name}/${file}`);
      continue;
    }

    const raw = await readFile(join(DOCS, dir.name, file), 'utf8');

    // Strip the docs-only header (title + run/code links) and the prev/next footer:
    // the web page renders both from metadata instead.
    let body = raw;
    const header = body.match(/^---\n\n/m);
    if (header) body = body.slice(header.index + header[0].length);
    body = body.split(/\n---\n\n(?:Previous|Next):/)[0].trimEnd();

    const frontmatter = [
      '---',
      `id: "${id}"`,
      `module: ${entry.Module}`,
      `title: ${JSON.stringify(entry.Title)}`,
      `notes: "docs/${dir.name}/${file}"`,
      '---',
      '',
      '',
    ].join('\n');

    await writeFile(join(OUT, `${id}.md`), frontmatter + body + '\n');
    written++;

    // One search record per lesson. The body is truncated - enough to match on, not to bloat.
    const plain = toPlainText(body);
    searchIndex.push({
      id,
      title: entry.Title,
      module: entry.Module,
      summary: entry.Summary,
      objectives: entry.Objectives ?? [],
      sections: entry.Sections ?? [],
      text: plain.slice(0, 4000),
    });
  }
}

searchIndex.sort((a, b) => a.id.localeCompare(b.id, undefined, { numeric: true }));
await writeFile(SEARCH, JSON.stringify(searchIndex));

console.log(`Notes: ${written} lesson(s) copied from docs/ -> src/content/lessons/`);
console.log(`Search: index of ${searchIndex.length} lessons -> public/data/search-index.json`);
if (skipped.length) console.log(`  skipped (no matching lesson): ${skipped.join(', ')}`);
