import { Suspense, lazy } from 'react';
import { HashRouter, Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './routes/Home';
import { Syllabus } from './routes/Syllabus';
import { Search } from './routes/Search';
import { Practice } from './routes/Practice';

// The lesson page pulls in the markdown renderer and syntax highlighter, which are by far
// the heaviest dependencies. Loading it lazily keeps the home and syllabus pages small.
const Lesson = lazy(() => import('./routes/Lesson').then((m) => ({ default: m.Lesson })));
const ModuleExam = lazy(() =>
  import('./routes/ModuleExam').then((m) => ({ default: m.ModuleExam })),
);
const QuizPage = lazy(() => import('./routes/QuizPage').then((m) => ({ default: m.QuizPage })));

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
          <Route path="practice" element={<Practice />} />
          <Route path="search" element={<Search />} />
          <Route
            path="lesson/:id"
            element={
              <Suspense fallback={<p className="muted">Loading lesson…</p>}>
                <Lesson />
              </Suspense>
            }
          />
          <Route
            path="quiz/:id"
            element={
              <Suspense fallback={<p className="muted">Loading quiz…</p>}>
                <QuizPage />
              </Suspense>
            }
          />
          <Route
            path="module/:module/exam"
            element={
              <Suspense fallback={<p className="muted">Loading exam…</p>}>
                <ModuleExam />
              </Suspense>
            }
          />
          <Route path="*" element={<Home />} />
        </Route>
      </Routes>
    </HashRouter>
  );
}
