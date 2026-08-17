import { Link } from 'react-router-dom';
import { MODULE_INFO, lessonsIn, modulesOf } from '../lib/lessons';
import { useSyllabus } from '../lib/useSyllabus';
import { read } from '../lib/progress';

export function Home() {
  const syllabus = useSyllabus();

  if (!syllabus) return <p className="muted">Loading the syllabus…</p>;

  const progress = read();
  const done = Object.values(progress.lessons).filter((s) => s === 'done').length;
  const next = syllabus.find((l) => progress.lessons[l.id] !== 'done') ?? syllabus[0];

  return (
    <>
      <section className="hero">
        <h1>Learn C#</h1>
        <p className="lede">
          From your first <code>Console.WriteLine</code> to threads, LINQ and Dijkstra’s
          algorithm. Built for GCSE and A-Level students, and for developers arriving from
          another language.
        </p>

        <div className="cta">
          <Link className="button" to={`/lesson/${next.id}`}>
            {done === 0 ? 'Start at lesson 1.1' : `Continue — ${next.id} ${next.title}`}
          </Link>
          <Link className="button ghost" to="/syllabus">
            Browse all {syllabus.length} lessons
          </Link>
        </div>

        {done > 0 && (
          <p className="progress-line">
            {done} of {syllabus.length} lessons complete
          </p>
        )}
      </section>

      <section>
        <h2>The course</h2>
        <div className="module-grid">
          {modulesOf(syllabus).map((m) => {
            const inModule = lessonsIn(syllabus, m);
            return (
              <article key={m} className="module-card">
                <h3>
                  <Link to={`/syllabus#m${m}`} className="module-link">
                    <span className="num">{m}</span> {MODULE_INFO[m]?.name}
                  </Link>
                </h3>
                <p className="blurb">{MODULE_INFO[m]?.blurb}</p>
                <p className="count">{inModule.length} lessons</p>
                <ul>
                  {inModule.slice(0, 3).map((l) => (
                    <li key={l.id}>
                      <Link to={`/lesson/${l.id}`}>
                        {l.id} {l.title}
                      </Link>
                    </li>
                  ))}
                  {inModule.length > 3 && (
                    <li className="more">
                      <Link to={`/syllabus#m${m}`}>…and {inModule.length - 3} more</Link>
                    </li>
                  )}
                </ul>
              </article>
            );
          })}
        </div>
      </section>
    </>
  );
}
