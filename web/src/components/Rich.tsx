import type { ReactNode } from 'react';

/** Wraps every occurrence of a query word in <mark>. */
export function highlight(text: string, query: string): ReactNode {
  const words = query
    .toLowerCase()
    .split(/\s+/)
    .map((t) => t.trim())
    .filter((t) => t.length > 1);
  if (words.length === 0) return text;
  const pattern = new RegExp(
    `(${words.map((t) => t.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')).join('|')})`,
    'gi',
  );
  return text.split(pattern).map((part, i) =>
    words.includes(part.toLowerCase()) ? <mark key={i}>{part}</mark> : part,
  );
}

interface Props {
  /** Plain text with `backtick` spans for inline code - the glossary's one bit of markup. */
  text: string;
  /** Optional search query whose words are highlighted, inside and outside code. */
  query?: string;
}

/**
 * Renders glossary text: `code` spans become <code>, everything else is plain, and an
 * optional query is highlighted throughout. Deliberately not a Markdown renderer - the
 * definitions only ever need inline code.
 */
export function Rich({ text, query = '' }: Props) {
  const parts = text.split('`');
  return (
    <>
      {parts.map((part, i) =>
        i % 2 === 1 ? <code key={i}>{highlight(part, query)}</code> : <span key={i}>{highlight(part, query)}</span>,
      )}
    </>
  );
}
