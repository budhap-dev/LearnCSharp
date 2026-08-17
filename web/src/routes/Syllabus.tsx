import { Link } from 'react-router-dom';
import { MODULE_NAMES, lessonsIn, modulesOf } from '../lib/lessons';
import { useSyllabus } from '../lib/useSyllabus';
import { read } from '../lib/progress';

const LABEL: Record<string, string> = {
  done: 'done',
  'needs-review': 'needs review',
  'in-progress': 'in progress',
};

export function Syllabus() {
  const syllabus = useSyllabus();
  if (!syllabus) return <p className="muted">Loading the syllabus…</p>;

  const progress = read();
  const modules = modulesOf(syllabus);

  return (
    <>
      <h1>Syllabus</h1>
      <p className="lede">
        {syllabus.length} lessons across {modules.length} modules.
      </p>

      {modules.map((m) => (
        <section key={m} className="module-block">
          <h2>
            <span className="num">{m}</span> {MODULE_NAMES[m]}
          </h2>
          <ol className="lesson-list">
            {lessonsIn(syllabus, m).map((l) => {
              const state = progress.lessons[l.id];
              return (
                <li key={l.id}>
                  <Link to={`/lesson/${l.id}`}>
                    <span className="lid">{l.id}</span>
                    <span className="ltitle">{l.title}</span>
                    {state && <span className={`badge ${state}`}>{LABEL[state] ?? state}</span>}
                  </Link>
                </li>
              );
            })}
          </ol>
        </section>
      ))}
    </>
  );
}
