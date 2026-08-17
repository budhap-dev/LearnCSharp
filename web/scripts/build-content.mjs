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

const syllabus = Object.fromEntries(
  JSON.parse(await readFile(SYLLABUS, 'utf8')).map((l) => [l.Id, l]),
);

await rm(OUT, { recursive: true, force: true });
await mkdir(OUT, { recursive: true });

let written = 0;
const skipped = [];

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
  }
}

console.log(`Notes: ${written} lesson(s) copied from docs/ -> src/content/lessons/`);
if (skipped.length) console.log(`  skipped (no matching lesson): ${skipped.join(', ')}`);
