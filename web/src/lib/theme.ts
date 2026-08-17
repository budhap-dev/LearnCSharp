/**
 * Theme handling. The chosen theme is a data-theme attribute on <html>, which the
 * stylesheet keys off; "system" removes the attribute and lets prefers-color-scheme
 * decide. The choice persists in localStorage and is applied before first paint by a
 * tiny inline script in index.html, so there is no flash of the wrong theme.
 */

export const THEMES = [
  { id: 'system', label: 'System', hint: 'follow the OS' },
  { id: 'light', label: 'Light', hint: 'clean and bright' },
  { id: 'dark', label: 'Dark', hint: 'easy on the eyes' },
  { id: 'slate', label: 'Slate', hint: 'muted blue-grey' },
  { id: 'midnight', label: 'Midnight', hint: 'deep navy dark' },
  { id: 'paper', label: 'Paper', hint: 'warm, book-like' },
] as const;

export type ThemeId = (typeof THEMES)[number]['id'];

const KEY = 'learncsharp.theme';

export function currentTheme(): ThemeId {
  try {
    const stored = localStorage.getItem(KEY);
    if (stored && THEMES.some((t) => t.id === stored)) return stored as ThemeId;
  } catch {
    /* storage unavailable */
  }
  return 'system';
}

export function applyTheme(theme: ThemeId): void {
  if (theme === 'system') {
    document.documentElement.removeAttribute('data-theme');
  } else {
    document.documentElement.setAttribute('data-theme', theme);
  }

  try {
    if (theme === 'system') localStorage.removeItem(KEY);
    else localStorage.setItem(KEY, theme);
  } catch {
    /* the theme still applies for this visit */
  }
}
