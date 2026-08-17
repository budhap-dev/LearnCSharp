import { Suspense, lazy } from 'react';
import { HashRouter, Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './routes/Home';
import { Syllabus } from './routes/Syllabus';

// The lesson page pulls in the markdown renderer and syntax highlighter, which are by far
// the heaviest dependencies. Loading it lazily keeps the home and syllabus pages small.
const Lesson = lazy(() => import('./routes/Lesson').then((m) => ({ default: m.Lesson })));

/**
 * HashRouter keeps deep links working on any static host with no rewrite rules,
 * which is exactly what we want on Azure Static Web Apps' free tier.
 */
export default function App() {
  return (
    <HashRouter>
      <Routes>
        <Route element={<Layout />}>
          <Route index element={<Home />} />
          <Route path="syllabus" element={<Syllabus />} />
          <Route
            path="lesson/:id"
            element={
              <Suspense fallback={<p className="muted">Loading lesson…</p>}>
                <Lesson />
              </Suspense>
            }
          />
          <Route path="*" element={<Home />} />
        </Route>
      </Routes>
    </HashRouter>
  );
}
