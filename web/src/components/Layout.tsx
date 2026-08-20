import { useEffect, useState } from 'react';
import { Link, NavLink, Outlet, useLocation } from 'react-router-dom';
import { ThemePicker } from './ThemePicker';
import { SearchBox } from './SearchBox';
import { ThemeDecor } from './ThemeDecor';

const LINKS = [
  { to: '/', label: 'Home', end: true },
  { to: '/syllabus', label: 'Syllabus' },
  { to: '/practice', label: 'Practice' },
  { to: '/worksheets', label: 'Worksheets' },
  { to: '/glossary', label: 'Glossary' },
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
            <NavLink key={l.to} to={l.to} end={l.end}>
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
