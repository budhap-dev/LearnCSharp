import { Suspense, lazy } from 'react';
import { RouterProvider, createHashRouter } from 'react-router-dom';
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

const lazyRoute = (node: React.ReactNode, label: string) => (
  <Suspense fallback={<p className="muted">Loading {label}…</p>}>{node}</Suspense>
);

/**
 * A data router (createHashRouter) rather than the <HashRouter> component - required so that
 * useBlocker works, which is what lets an in-progress exam or quiz warn before you navigate
 * away. HashRouter still keeps deep links working on a static host with no rewrite rules.
 */
const router = createHashRouter([
  {
    element: <Layout />,
    children: [
      { index: true, element: <Home /> },
      { path: 'syllabus', element: <Syllabus /> },
      { path: 'practice', element: <Practice /> },
      { path: 'search', element: <Search /> },
      { path: 'lesson/:id', element: lazyRoute(<Lesson />, 'lesson') },
      { path: 'quiz/:id', element: lazyRoute(<QuizPage />, 'quiz') },
      { path: 'module/:module/exam', element: lazyRoute(<ModuleExam />, 'exam') },
      { path: '*', element: <Home /> },
    ],
  },
]);

export default function App() {
  return <RouterProvider router={router} />;
}
