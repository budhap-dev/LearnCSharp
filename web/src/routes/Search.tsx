import { useEffect, useMemo, useRef, useState, type ReactNode } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { loadIndex, search, type SearchRecord } from '../lib/search';
import { MODULE_INFO } from '../lib/lessons';

function highlight(text: string, query: string): ReactNode {
  const terms = query
    .toLowerCase()
    .split(/\s+/)
    .map((t) => t.trim())
    .filter((t) => t.length > 1);
  if (terms.length === 0) return text;
  const pattern = new RegExp(`(${terms.map((t) => t.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')).join('|')})`, 'gi');
  return text.split(pattern).map((part, i) =>
    terms.includes(part.toLowerCase()) ? <mark key={i}>{part}</mark> : part,
  );
}

export function Search() {
  const [params, setParams] = useSearchParams();
  const initial = params.get('q') ?? '';
  const [query, setQuery] = useState(initial);
  const [records, setRecords] = useState<SearchRecord[] | null>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    loadIndex().then(setRecords);
    inputRef.current?.focus();
  }, []);

  // Keep the URL query string in step so results are shareable/back-navigable.
  useEffect(() => {
    const trimmed = query.trim();
    setParams(trimmed ? { q: trimmed } : {}, { replace: true });
  }, [query, setParams]);

  const hits = useMemo(() => (records ? search(records, query) : []), [records, query]);

  return (
    <>
      <h1>Search</h1>
      <p className="lede">Find any topic across all 62 lessons — titles, objectives and the notes.</p>

      <input
        ref={inputRef}
        className="search-input-big"
        type="search"
        placeholder="e.g. dijkstra, boxing, yield, dependency injection…"
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        aria-label="Search lessons"
      />

      {records === null && <p className="muted">Loading the index…</p>}

      {records !== null && query.trim().length > 1 && (
        <p className="muted result-count">
          {hits.length} {hits.length === 1 ? 'lesson' : 'lessons'} match “{query.trim()}”.
        </p>
      )}

      <ol className="search-results">
        {hits.map(({ record, snippet }) => (
          <li key={record.id}>
            <Link to={`/lesson/${record.id}`}>
              <div className="sr-head">
                <span className="lid">{record.id}</span>
                <strong>{highlight(record.title, query)}</strong>
                <span className="sr-module">Module {record.module} · {MODULE_INFO[record.module]?.name}</span>
              </div>
              <p className="sr-snippet">{highlight(snippet, query)}</p>
            </Link>
          </li>
        ))}
      </ol>

      {records !== null && query.trim().length > 1 && hits.length === 0 && (
        <p className="callout">
          Nothing matched. Try a single keyword — a C# term, a method name, or a concept.
        </p>
      )}
    </>
  );
}
