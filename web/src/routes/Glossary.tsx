import { useEffect, useMemo, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { Link, useSearchParams } from 'react-router-dom';
import {
  GLOSSARY,
  letterOf,
  moduleOf,
  searchGlossary,
  slugOf,
  entryBySlug,
  type GlossaryEntry,
} from '../lib/glossary';
import { MODULE_INFO } from '../lib/lessons';
import { useSyllabus } from '../lib/useSyllabus';
import { Rich, highlight } from '../components/Rich';

const LETTERS = ['#', ...'ABCDEFGHIJKLMNOPQRSTUVWXYZ'];

/**
 * A searchable A-Z of every term the course uses. Each entry links to the lesson that
 * teaches it and to related terms. `?q=` searches; `?term=slug` deep-links to one entry.
 */
export function Glossary() {
  const [params, setParams] = useSearchParams();
  const [query, setQuery] = useState(params.get('q') ?? '');
  const [module, setModule] = useState<number>(0);
  const [showTop, setShowTop] = useState(false);
  const focusSlug = params.get('term');
  const syllabus = useSyllabus();
  const inputRef = useRef<HTMLInputElement>(null);

  const titles = useMemo(
    () => new Map((syllabus ?? []).map((l) => [l.id, l.title])),
    [syllabus],
  );

  useEffect(() => {
    if (!focusSlug) inputRef.current?.focus();
  }, [focusSlug]);

  // Keep ?q= in the URL so a search is shareable and survives back/forward.
  useEffect(() => {
    const trimmed = query.trim();
    const next: Record<string, string> = {};
    if (trimmed) next.q = trimmed;
    else if (focusSlug) next.term = focusSlug;
    setParams(next, { replace: true });
  }, [query, focusSlug, setParams]);

  // Deep link: scroll the requested term into view and flash it.
  useEffect(() => {
    if (!focusSlug || query) return;
    const el = document.getElementById(`term-${focusSlug}`);
    if (el) {
      el.scrollIntoView({ block: 'start', behavior: 'smooth' });
      el.classList.add('flash');
      const t = setTimeout(() => el.classList.remove('flash'), 1800);
      return () => clearTimeout(t);
    }
  }, [focusSlug, query]);

  // Floating "back to top" once the reader has scrolled past the controls.
  useEffect(() => {
    function onScroll() {
      setShowTop(window.scrollY > 600);
    }
    onScroll();
    window.addEventListener('scroll', onScroll, { passive: true });
    return () => window.removeEventListener('scroll', onScroll);
  }, []);

  const filtered = useMemo(
    () => (module ? GLOSSARY.filter((e) => moduleOf(e) === module) : GLOSSARY),
    [module],
  );

  const searching = query.trim().length > 0;
  const results = useMemo(
    () => (searching ? searchGlossary(query, filtered).map((h) => h.entry) : filtered),
    [searching, query, filtered],
  );

  // Letters that have at least one entry in the current view, for the jump bar.
  const present = useMemo(() => new Set(results.map(letterOf)), [results]);

  const modules = useMemo(
    () => [...new Set(GLOSSARY.map(moduleOf))].filter(Boolean).sort((a, b) => a - b),
    [],
  );

  function jump(letter: string) {
    document.getElementById(`letter-${letter}`)?.scrollIntoView({ block: 'start', behavior: 'smooth' });
  }

  function toTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    inputRef.current?.focus({ preventScroll: true });
  }

  function focusTerm(entry: GlossaryEntry) {
    setQuery('');
    setModule(0);
    setParams({ term: entry.slug }, { replace: false });
  }

  return (
    <div className="glossary">
      <h1>Glossary</h1>
      <p className="lede">
        Every term the course uses, in plain words, with the lesson that teaches it.{' '}
        {GLOSSARY.length} entries — search by name, alias or meaning.
      </p>

      <div className="glossary-controls">
        <input
          ref={inputRef}
          className="search-input-big"
          type="search"
          placeholder="e.g. polymorphism, ??, big-o, yield…"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          aria-label="Search the glossary"
        />
        <label className="glossary-module">
          <span className="visually-hidden">Module</span>
          <select value={module} onChange={(e) => setModule(Number(e.target.value))} aria-label="Filter by module">
            <option value={0}>All modules</option>
            {modules.map((m) => (
              <option key={m} value={m}>
                Module {m} · {MODULE_INFO[m]?.name}
              </option>
            ))}
          </select>
        </label>
      </div>

      {!searching && (
        <nav className="letter-bar" aria-label="Jump to letter">
          {LETTERS.map((l) => (
            <button
              key={l}
              type="button"
              disabled={!present.has(l)}
              onClick={() => jump(l)}
            >
              {l}
            </button>
          ))}
        </nav>
      )}

      {searching && (
        <p className="muted result-count">
          {results.length} {results.length === 1 ? 'term matches' : 'terms match'} “{query.trim()}”.
        </p>
      )}

      {searching ? (
        <dl className="glossary-list">
          {results.map((e) => (
            <Entry key={e.slug} entry={e} query={query} titles={titles} onRelated={focusTerm} />
          ))}
        </dl>
      ) : (
        LETTERS.filter((l) => present.has(l)).map((letter) => (
          <section key={letter} className="glossary-letter" id={`letter-${letter}`}>
            <h2>{letter}</h2>
            <dl className="glossary-list">
              {results
                .filter((e) => letterOf(e) === letter)
                .map((e) => (
                  <Entry key={e.slug} entry={e} query="" titles={titles} onRelated={focusTerm} />
                ))}
            </dl>
          </section>
        ))
      )}

      {/* Portalled to <body>: `main > *` carries a transform (the page fade-in), which
          would otherwise turn position: fixed into "fixed to this div". */}
      {showTop &&
        createPortal(
          <button type="button" className="to-top" onClick={toTop} aria-label="Back to top">
            ↑ Top
          </button>,
          document.body,
        )}

      {searching && results.length === 0 && (
        <p className="callout">
          No term matches. Try a shorter word, or{' '}
          <Link to={`/search?q=${encodeURIComponent(query.trim())}`}>search the lesson notes</Link>{' '}
          instead.
        </p>
      )}
    </div>
  );
}

interface EntryProps {
  entry: GlossaryEntry;
  query: string;
  titles: Map<string, string>;
  onRelated: (entry: GlossaryEntry) => void;
}

function Entry({ entry, query, titles, onRelated }: EntryProps) {
  return (
    <div className="glossary-entry" id={`term-${entry.slug}`}>
      <dt>
        <Link
          to={`/glossary?term=${entry.slug}`}
          className="term-anchor"
          title="Link to this term"
          onClick={() => onRelated(entry)}
        >
          {highlight(entry.term, query)}
        </Link>
        {entry.aliases.length > 0 && (
          <span className="aliases">
            also: {entry.aliases.map((a, i) => (
              <span key={a}>
                {i > 0 && ', '}
                <code>{highlight(a, query)}</code>
              </span>
            ))}
          </span>
        )}
      </dt>
      <dd>
        <p><Rich text={entry.definition} query={query} /></p>
        {entry.example && (
          <pre className="glossary-example">
            <code>{entry.example}</code>
          </pre>
        )}
        <p className="entry-meta">
          <span className="taught">
            Taught in{' '}
            {entry.lessons.map((id, i) => (
              <span key={id}>
                {i > 0 && ', '}
                <Link to={`/lesson/${id}`}>
                  <span className="lid">{id}</span> {titles.get(id) ?? ''}
                </Link>
              </span>
            ))}
          </span>
          {entry.related.length > 0 && (
            <span className="see-also">
              See also{' '}
              {entry.related.map((r) => {
                const target = entryBySlug(slugOf(r));
                return target ? (
                  <button
                    key={r}
                    type="button"
                    className="term-chip"
                    onClick={() => onRelated(target)}
                  >
                    {r}
                  </button>
                ) : null;
              })}
            </span>
          )}
        </p>
      </dd>
    </div>
  );
}
