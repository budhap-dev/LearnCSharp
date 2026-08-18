import { useEffect, useMemo, useState } from 'react';
import { Link, useLocation, useParams } from 'react-router-dom';
import type { Question } from '../components/Quiz';
import { Exam } from '../components/Exam';
import { LEVELS, EXAM_SECONDS_PER_QUESTION, buildPaper, type ExamPaper, type Level } from '../lib/exam';
import { MODULE_INFO } from '../lib/lessons';
import { examHistory } from '../lib/progress';

// All quiz files, bundled and split; loaded on demand when an exam starts.
const quizFiles = import.meta.glob('../data/quizzes/*.json', { import: 'default' });

async function loadModuleQuestions(module: number): Promise<Record<string, Question[]>> {
  const byLesson: Record<string, Question[]> = {};
  await Promise.all(
    Object.entries(quizFiles).map(async ([path, load]) => {
      const id = path.match(/([\d.]+)\.json$/)?.[1];
      if (!id || Number(id.split('.')[0]) !== module) return;
      byLesson[id] = (await load()) as Question[];
    }),
  );
  return byLesson;
}

export function ModuleExam() {
  const { module } = useParams();
  const { state } = useLocation() as { state?: { from?: string } };
  const moduleNum = Number(module);
  const info = MODULE_INFO[moduleNum];

  // Reflect where the student came from; default to the syllabus on a fresh load.
  const origin =
    state?.from === 'practice'
      ? { to: '/practice', label: 'Practice' }
      : { to: '/syllabus', label: 'Syllabus' };

  const [byLesson, setByLesson] = useState<Record<string, Question[]> | null>(null);
  const [paper, setPaper] = useState<ExamPaper | null>(null);
  const [seed, setSeed] = useState(1);
  const [timed, setTimed] = useState(false);

  useEffect(() => {
    setByLesson(null);
    setPaper(null);
    loadModuleQuestions(moduleNum).then(setByLesson);
  }, [moduleNum]);

  const history = useMemo(() => examHistory(moduleNum), [moduleNum, paper]);

  if (!info) {
    return (
      <>
        <h1>No such module</h1>
        <Link to="/syllabus">Back to the syllabus</Link>
      </>
    );
  }

  function start(level: Level) {
    if (!byLesson) return;
    // A fresh seed each start so "New paper" genuinely reshuffles.
    const s = seed + Date.now() % 100000;
    setSeed(s);
    setPaper(buildPaper(moduleNum, level, byLesson, s));
  }

  if (paper) {
    return (
      <article className="exam-page">
        <nav className="crumbs">
          <Link to={origin.to}>{origin.label}</Link> <span>/</span>{' '}
          <button className="linkish" onClick={() => setPaper(null)}>
            Module {moduleNum} exam
          </button>{' '}
          <span>/</span> {LEVELS.find((l) => l.id === paper.level)!.label}
        </nav>
        <h1>
          Module {moduleNum} exam — {LEVELS.find((l) => l.id === paper.level)!.label}
        </h1>
        <p className="muted">
          {paper.questions.length} questions · {paper.totalMarks} marks ·{' '}
          {timed ? `${Math.round((paper.questions.length * EXAM_SECONDS_PER_QUESTION) / 60)} min limit` : 'untimed'} · no
          feedback until you submit.
        </p>
        <Exam paper={paper} timed={timed} onRetry={() => setPaper(null)} />
      </article>
    );
  }

  const available = byLesson
    ? Object.entries(byLesson).filter(([id]) => Number(id.split('.')[0]) === moduleNum).length
    : 0;

  return (
    <article className="exam-page">
      <nav className="crumbs">
        <Link to={origin.to}>{origin.label}</Link> <span>/</span> Module {moduleNum} exam
      </nav>

      <h1>
        <span className="num">{moduleNum}</span> {info.name} — exam
      </h1>
      <p className="lede">
        An exam measures rather than teaches: marks, and no explanations until you submit. Pick a
        level.
      </p>

      {byLesson === null ? (
        <p className="muted">Loading the question bank…</p>
      ) : available === 0 ? (
        <p className="callout">No questions for this module yet.</p>
      ) : (
        <>
          <label className="timer-toggle">
            <input type="checkbox" checked={timed} onChange={(e) => setTimed(e.target.checked)} />
            <span>
              Timed exam
              <span className="muted"> — a time limit is set from the paper's length</span>
            </span>
          </label>

          <div className="level-grid">
            {LEVELS.map((level) => {
              const count = level.size === 0 ? available : level.size;
              const mins = Math.round((count * EXAM_SECONDS_PER_QUESTION) / 60);
              return (
                <article key={level.id} className="level-card">
                  <h2>{level.label}</h2>
                  <p>{level.blurb}</p>
                  <p className="level-meta">
                    {level.size === 0 ? 'the whole module' : `~${level.size} questions`} · pass{' '}
                    {level.pass}%{timed ? ` · ${mins} min` : ''}
                  </p>
                  <button onClick={() => start(level.id)}>Start {level.label}</button>
                </article>
              );
            })}
          </div>
        </>
      )}

      {history.length > 0 && (
        <section className="exam-history">
          <h2>Your attempts</h2>
          <ul>
            {[...history].reverse().slice(0, 8).map((a, i) => (
              <li key={i}>
                <span className="hist-level">{a.level}</span>
                <span className="hist-mark">
                  {a.marks}/{a.totalMarks}
                </span>
                <span className={`badge grade-${a.grade.toLowerCase().replace(/\s+/g, '-')}`}>
                  {a.grade}
                </span>
                <span className="hist-date">{new Date(a.at).toLocaleDateString()}</span>
              </li>
            ))}
          </ul>
        </section>
      )}
    </article>
  );
}
