import { useEffect, useState } from 'react';

/**
 * Subtle, decorative motifs that appear only for the playful themes - chess pieces for
 * Grandmaster, webs for Web Slinger, and so on. Original emoji/glyphs only (no character art
 * or logos), rendered behind the content with pointer-events off, so they decorate without
 * getting in the way. The plain/professional themes stay clean.
 */
const DECOR: Record<string, string[]> = {
  grandmaster: ['♞', '♜', '♛', '♚', '♝', '♟', '♘', '♖'],
  webslinger: ['🕸️', '🕷️', '🕸️', '🕷️', '🕸️', '🕸️'],
  hero: ['⚡', '⭐', '💥', '🌟', '⚡', '💫'],
  cube: ['🟥', '🟩', '🟦', '🟨', '🟧', '⬜', '🟥', '🟦'],
  fairytale: ['✨', '🏰', '🌟', '🦄', '🧚', '✨', '🌙', '💫'],
  matchday: ['⚽', '🏆', '🥅', '⚽', '🏟️', '⚽'],
};

// Fixed spots around the viewport edges - enough for up to eight motifs.
const SPOTS = [
  'decor-tl', 'decor-tr', 'decor-bl', 'decor-br',
  'decor-ml', 'decor-mr', 'decor-tc', 'decor-bc',
];

export function ThemeDecor() {
  const [theme, setTheme] = useState<string | null>(
    () => document.documentElement.getAttribute('data-theme'),
  );

  // React to theme changes - the picker sets data-theme on <html>, so watch that.
  useEffect(() => {
    const observer = new MutationObserver(() =>
      setTheme(document.documentElement.getAttribute('data-theme')),
    );
    observer.observe(document.documentElement, { attributes: true, attributeFilter: ['data-theme'] });
    return () => observer.disconnect();
  }, []);

  const motifs = theme ? DECOR[theme] : undefined;
  if (!motifs) return null;

  return (
    <div className="theme-decor" aria-hidden="true">
      {motifs.slice(0, SPOTS.length).map((emoji, i) => (
        <span key={i} className={`decor ${SPOTS[i]}`}>
          {emoji}
        </span>
      ))}
    </div>
  );
}
