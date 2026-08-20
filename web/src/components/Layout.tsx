import { useEffect, useState } from 'react';
import { Link, NavLink, Outlet, useLocation } from 'react-router-dom';
import { ThemePicker } from './ThemePicker';
import { SearchBox } from './SearchBox';
import { ThemeDecor } from './ThemeDecor';

// Each link gets a friendly emoji and its own colour, reusing the module palette so the
// menu feels like the rest of the (deliberately playful) site.
const LINKS = [
  { to: '/', label: 'Home', emoji: '🏠', color: 'var(--m1)', end: true },
  { to: '/syllabus', label: 'Syllabus', emoji: '🗺️', color: 'var(--m3)' },
  { to: '/practice', label: 'Practice', emoji: '🎯', color: 'var(--m4)' },
  { to: '/worksheets', label: 'Worksheets', emoji: '💻', color: 'var(--m5)' },
  { to: '/glossary', label: 'Glossary', emoji: '📖', color: 'var(--m2)' },
];

export function Layout() {
  const [menuOpen, setMenuOpen] = useState(false);
  const location = useLocation();

  // Close the drawer whenever the route changes (a link was followed).
  useEffect(() => setMenuOpen(false), [location.pathname]);

  // Escape closes it; lock body scroll while the drawer covers the page.
  useEffect(() => {
    if (!menuOpen) return;
    const onKey = (e: KeyboardEvent) => e.key === 'Escape' && setMenuOpen(false);
    window.addEventListener('keydown', onKey);
    document.body.classList.add('nav-locked');
    return () => {
      window.removeEventListener('keydown', onKey);
      document.body.classList.remove('nav-locked');
    };
  }, [menuOpen]);

  return (
    <div className="shell">
      <ThemeDecor />
      <header>
        <button
          type="button"
          className={`nav-toggle ${menuOpen ? 'is-open' : ''}`}
          aria-label={menuOpen ? 'Close menu' : 'Open menu'}
          aria-expanded={menuOpen}
          aria-controls="main-nav"
          onClick={() => setMenuOpen((o) => !o)}
        >
          <span className="nav-toggle-bars" aria-hidden="true">
            <span />
            <span />
            <span />
          </span>
        </button>

        <Link to="/" className="brand">
          Learn<span>C#</span>
        </Link>

        <nav id="main-nav" className={menuOpen ? 'is-open' : ''}>
          {LINKS.map((l) => (
            <NavLink key={l.to} to={l.to} end={l.end} style={{ ['--nav-c' as string]: l.color }}>
              <span className="nav-emoji" aria-hidden="true">
                {l.emoji}
              </span>
              {l.label}
            </NavLink>
          ))}
        </nav>

        <div className="header-right">
          <SearchBox />
          <ThemePicker />
        </div>

        <div
          className={`nav-backdrop ${menuOpen ? 'is-open' : ''}`}
          onClick={() => setMenuOpen(false)}
          aria-hidden="true"
        />
      </header>

      <main>
        <Outlet />
      </main>

      <footer>
        <p>
          Every lesson on this site is backed by a C# program that really runs.{' '}
          <a href="https://github.com">Source</a>
        </p>
      </footer>
    </div>
  );
}
