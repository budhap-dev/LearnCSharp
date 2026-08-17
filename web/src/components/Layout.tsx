import { Link, NavLink, Outlet } from 'react-router-dom';
import { ThemePicker } from './ThemePicker';

export function Layout() {
  return (
    <div className="shell">
      <header>
        <Link to="/" className="brand">
          Learn<span>C#</span>
        </Link>
        <nav>
          <NavLink to="/">Home</NavLink>
          <NavLink to="/syllabus">Syllabus</NavLink>
        </nav>
        <div className="header-right">
          <ThemePicker />
        </div>
      </header>

      <main>
        <Outlet />
      </main>

      <footer>
        <p>
          Every output block on this site was captured from a C# program that really ran.
          {' '}
          <a href="https://github.com">Source</a>
        </p>
      </footer>
    </div>
  );
}
