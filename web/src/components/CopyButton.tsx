import { useState } from 'react';

interface Props {
  /** Text placed on the clipboard when the button is pressed. */
  source: string;
  /** Extra classes for positioning in different layouts. */
  className?: string;
}

/** A small copy-to-clipboard button that shows a tick for a moment after a successful copy. */
export function CopyButton({ source, className = '' }: Props) {
  const [copied, setCopied] = useState(false);

  async function copy() {
    try {
      await navigator.clipboard.writeText(source);
      setCopied(true);
      setTimeout(() => setCopied(false), 1600);
    } catch {
      /* clipboard unavailable (insecure context / permission) - fail quietly */
    }
  }

  return (
    <button
      type="button"
      className={`copy-button ${copied ? 'copied' : ''} ${className}`}
      onClick={copy}
      aria-label={copied ? 'Copied' : 'Copy code'}
      title="Copy code"
    >
      {copied ? '✓ Copied' : 'Copy'}
    </button>
  );
}
