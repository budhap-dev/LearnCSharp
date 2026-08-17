import { Link } from 'react-router-dom';
import type { AnchorHTMLAttributes } from 'react';

/**
 * Notes are authored as files in docs/, so they link to each other as "2.4.md" or
 * "../module-3/3.1.md". Those paths mean nothing to the router, so they are rewritten
 * into real in-app routes here. Anything else is left alone and opened as a normal link.
 */
const LESSON_LINK = /(?:\.\.\/module-\d+\/)?(\d+\.\d+)\.md(#.*)?$/;

export function MarkdownLink({ href, children, ...rest }: AnchorHTMLAttributes<HTMLAnchorElement>) {
  const match = href?.match(LESSON_LINK);

  if (match) {
    return <Link to={`/lesson/${match[1]}`}>{children}</Link>;
  }

  const isExternal = href?.startsWith('http');

  return (
    <a href={href} {...rest} {...(isExternal ? { target: '_blank', rel: 'noreferrer' } : {})}>
      {children}
    </a>
  );
}
