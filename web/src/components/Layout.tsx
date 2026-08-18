import { Link, NavLink, Outlet } from 'react-router-dom';
import { ThemePicker } from './ThemePicker';
import { SearchBox } from './SearchBox';
import { ThemeDecor } from './ThemeDecor';

export function Layout() {
  return (
    <div className="shell">
      <ThemeDecor />
      <header>
        <Link to="/" className="brand">
          Learn<span>C#</span>
        </Link>
        <nav>
          <NavLink to="/">Home</NavLink>
          <NavLink to="/syllabus">Syllabus</NavLink>
          <NavLink to="/practice">Practice</NavLink>
          <NavLink to="/worksheets">Worksheets</NavLink>
          <NavLink to="/glossary">Glossary</NavLink>
        </nav>
        <div className="header-right">
          <SearchBox />
          <ThemePicker />
        </div>
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
