import { useState } from 'react';
import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom';
import { ThemePicker } from './ThemePicker';

export function Layout() {
  const navigate = useNavigate();
  const [q, setQ] = useState('');

  function onSearch(e: React.FormEvent) {
    e.preventDefault();
    if (q.trim()) navigate(`/search?q=${encodeURIComponent(q.trim())}`);
  }

  return (
    <div className="shell">
      <header>
        <Link to="/" className="brand">
          Learn<span>C#</span>
        </Link>
        <nav>
          <NavLink to="/">Home</NavLink>
          <NavLink to="/syllabus">Syllabus</NavLink>
          <NavLink to="/practice">Practice</NavLink>
        </nav>
        <div className="header-right">
          <form className="header-search" onSubmit={onSearch} role="search">
            <input
              type="search"
              placeholder="Search topics…"
              value={q}
              onChange={(e) => setQ(e.target.value)}
              aria-label="Search topics"
            />
          </form>
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
