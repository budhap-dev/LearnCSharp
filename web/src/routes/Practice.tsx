import { useState } from 'react';
import { Link } from 'react-router-dom';
import { MODULE_INFO, lessonsIn, modulesOf } from '../lib/lessons';
import { useSyllabus } from '../lib/useSyllabus';
import { read, examHistory } from '../lib/progress';

type Tab = 'exams' | 'quizzes';

/**
 * One launchpad for both assessment modes, split across two tabs: a marked exam per module,
 * and every lesson quiz. Quizzes still live on the lesson pages too (theory a scroll away);
 * this tab is the direct way in.
 */
export function Practice() {
  const syllabus = useSyllabus();
  const [tab, setTab] = useState<Tab>('exams');

  if (!syllabus) return <p className="muted">Loading…</p>;

  const progress = read();
  const modules = modulesOf(syllabus);

  return (
    <>
      <h1>Practice</h1>
      <p className="lede">
        A <strong>quiz</strong> teaches, with an explanation after every answer; an{' '}
        <strong>exam</strong> measures, with marks and a grade.
      </p>

      <div className="tablist" role="tablist" aria-label="Practice mode">
        <button
          role="tab"
          id="tab-exams"
          aria-selected={tab === 'exams'}
          aria-controls="panel-exams"
          className={tab === 'exams' ? 'tab active' : 'tab'}
          onClick={() => setTab('exams')}
        >
          Module exams
        </button>
        <button
          role="tab"
          id="tab-quizzes"
          aria-selected={tab === 'quizzes'}
          aria-controls="panel-quizzes"
          className={tab === 'quizzes' ? 'tab active' : 'tab'}
          onClick={() => setTab('quizzes')}
        >
          Lesson quizzes
        </button>
      </div>

      {tab === 'exams' && (
        <section id="panel-exams" role="tabpanel" aria-labelledby="tab-exams">
          <p className="muted">
            Three levels each — Foundation, Standard, Challenge — drawn from the whole module's
            question bank.
          </p>
          <div className="exam-launch-grid">
            {modules.map((m) => {
              const attempts = examHistory(m);
              const best = attempts.length
                ? attempts.reduce((a, b) =>
                    b.marks / b.totalMarks > a.marks / a.totalMarks ? b : a,
                  )
                : null;
              return (
                <Link
                  key={m}
                  to={`/module/${m}/exam`}
                  state={{ from: 'practice' }}
                  className="exam-launch"
                >
                  <span className="num">{m}</span>
                  <span className="el-body">
                    <strong>{MODULE_INFO[m]?.name}</strong>
                    {best ? (
                      <span className="el-best">
                        best: {best.marks}/{best.totalMarks}{' '}
                        <span
                          className={`badge grade-${best.grade.toLowerCase().replace(/\s+/g, '-')}`}
                        >
                          {best.grade}
                        </span>
                      </span>
                    ) : (
                      <span className="el-best muted">not attempted</span>
                    )}
                  </span>
                </Link>
              );
            })}
          </div>
        </section>
      )}

      {tab === 'quizzes' && (
        <section id="panel-quizzes" role="tabpanel" aria-labelledby="tab-quizzes">
          <p className="muted">Ten questions each, with an explanation after every answer.</p>
          {modules.map((m) => (
            <details key={m} className="module-accordion">
              <summary>
                <span className="num">{m}</span>
                <span className="module-title">
                  <strong>{MODULE_INFO[m]?.name}</strong>
                  <span className="module-meta">{lessonsIn(syllabus, m).length} quizzes</span>
                </span>
                <span className="chevron" aria-hidden="true" />
              </summary>
              <div className="module-body">
                <ol className="lesson-list">
                  {lessonsIn(syllabus, m).map((l) => {
                    const best = progress.quizzes[l.id]?.reduce(
                      (a, b) => (b.score > a.score ? b : a),
                      { score: -1, outOf: 0, at: '' },
                    );
                    return (
                      <li key={l.id}>
                        <Link to={`/quiz/${l.id}`}>
                          <span className="lid">{l.id}</span>
                          <span className="ltitle">
                            <strong>{l.title}</strong>
                          </span>
                          {best && best.score >= 0 && (
                            <span className="badge">
                              best {best.score}/{best.outOf}
                            </span>
                          )}
                        </Link>
                      </li>
                    );
                  })}
                </ol>
              </div>
            </details>
          ))}
        </section>
      )}
    </>
  );
}
