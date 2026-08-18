import { type ReactNode } from 'react';
import { CopyButton } from './CopyButton';

interface Props {
  /** The raw source text, used for the clipboard. */
  source: string;
  children: ReactNode;
}

/**
 * A code card with a copy-to-clipboard button. The notes' snippets are self-contained plain
 * C#, so a student can copy one straight into their own console app and run it.
 */
export function CodeBlock({ source, children }: Props) {
  return (
    <div className="code-card">
      <CopyButton source={source} />
      {children}
    </div>
  );
}
