import { useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { MODULE_INFO, lessonsIn, modulesOf } from '../lib/lessons';
import { useSyllabus } from '../lib/useSyllabus';
import { read } from '../lib/progress';

const LABEL: Record<string, string> = {
  done: 'done',
  'needs-review': 'needs review',
  'in-progress': 'in progress',
};

export function Syllabus() {
  const syllabus = useSyllabus();
  const { hash } = useLocation();

  // A "#m3" hash (from the home page's course cards) scrolls to that module.
  useEffect(() => {
    if (!hash || !syllabus) return;
    document.getElementById(hash.slice(1))?.scrollIntoView({ block: 'start' });
  }, [hash, syllabus]);

  if (!syllabus) return <p className="muted">Loading the syllabus…</p>;

  const progress = read();
  const modules = modulesOf(syllabus);

  // Which module starts open: one deep-linked from a course card, otherwise the one
  // holding the student's next incomplete lesson.
  const linked = /^#m(\d+)$/.exec(hash);
  const next = syllabus.find((l) => progress.lessons[l.id] !== 'done') ?? syllabus[0];
  const currentModule = linked ? Number(linked[1]) : next.module;

  return (
    <>
      <h1>Syllabus</h1>
      <p className="lede syllabus-lede">
        {syllabus.length} lessons across {modules.length} modules — your current module is open.
      </p>

      {modules.map((m) => {
        const inModule = lessonsIn(syllabus, m);
        const done = inModule.filter((l) => progress.lessons[l.id] === 'done').length;

        return (
          // <details> gives us an accessible accordion with zero JS: keyboard,
          // screen-reader and no-JS behaviour all come from the browser.
          <details key={m} id={`m${m}`} className="module-accordion" open={m === currentModule}>
            <summary>
              <span className="num">{m}</span>
              <span className="module-title">
                <strong>{MODULE_INFO[m]?.name}</strong>
                <span className="module-meta">
                  {done > 0 ? `${done} of ${inModule.length} lessons done` : `${inModule.length} lessons`}
                </span>
              </span>
              {done === inModule.length && done > 0 && <span className="badge done">complete</span>}
              <span className="chevron" aria-hidden="true" />
            </summary>

            <div className="module-body">
              <p className="blurb">{MODULE_INFO[m]?.blurb}</p>

              <Link className="exam-link" to={`/module/${m}/exam`}>
                Take the Module {m} exam →
              </Link>

              {done > 0 && (
                <div
                  className="module-progress"
                  role="progressbar"
                  aria-valuenow={done}
                  aria-valuemin={0}
                  aria-valuemax={inModule.length}
                  aria-label={`${done} of ${inModule.length} lessons complete`}
                >
                  <span style={{ width: `${(done / inModule.length) * 100}%` }} />
                </div>
              )}

              <ol className="lesson-list">
                {inModule.map((l) => {
                  const state = progress.lessons[l.id];
                  return (
                    <li key={l.id}>
                      <Link to={`/lesson/${l.id}`}>
                        <span className="lid">{l.id}</span>
                        <span className="ltitle">
                          <strong>{l.title}</strong>
                          <span className="lsummary">{l.summary}</span>
                        </span>
                        {state && <span className={`badge ${state}`}>{LABEL[state] ?? state}</span>}
                      </Link>
                    </li>
                  );
                })}
              </ol>
            </div>
          </details>
        );
      })}
    </>
  );
}
