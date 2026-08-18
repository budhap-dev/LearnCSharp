import { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import type { ExamPaper } from '../lib/exam';
import { gradeFor } from '../lib/exam';
import { recordExam } from '../lib/progress';
import { examSeconds } from '../lib/exam';
import { useCountdown, formatTime } from '../lib/useCountdown';
import { useUnsavedWarning } from '../lib/useUnsavedWarning';
import { confetti, examMessage } from '../lib/celebrate';

interface Props {
  paper: ExamPaper;
  timed: boolean;
  onRetry: () => void;
}

/**
 * Exam conditions: answerable in any order, NO explanations until submission, an answer-sheet
 * overview, then a full marked review. Distinct from the teaching Quiz on purpose.
 */
export function Exam({ paper, timed, onRetry }: Props) {
  const [index, setIndex] = useState(0);
  const [answers, setAnswers] = useState<(number | null)[]>(() => paper.questions.map(() => null));
  const [submitted, setSubmitted] = useState(false);

  // The countdown runs only when timed and not yet submitted; at zero it submits what is answered.
  const remaining = useCountdown(examSeconds(paper), timed && !submitted, () => submit());

  // Warn before leaving once any answer has been made and the exam is not yet submitted.
  useUnsavedWarning(!submitted && answers.some((a) => a !== null));

  const answeredCount = answers.filter((a) => a !== null).length;

  const result = useMemo(() => {
    if (!submitted) return null;
    let marks = 0;
    const perLesson: Record<string, { got: number; total: number }> = {};
    paper.questions.forEach((q, i) => {
      const row = (perLesson[q.lesson] ??= { got: 0, total: 0 });
      row.total += q.marks;
      if (answers[i] === q.answer) {
        marks += q.marks;
        row.got += q.marks;
      }
    });
    const percent = Math.round((marks / paper.totalMarks) * 100);
    return { marks, percent, grade: gradeFor(percent), perLesson };
  }, [submitted, answers, paper]);

  function choose(option: number) {
    setAnswers((prev) => {
      const next = [...prev];
      next[index] = option;
      return next;
    });
  }

  function submit() {
    setSubmitted(true);
    // result is memoised off `submitted`; recompute inline for the record.
    let marks = 0;
    paper.questions.forEach((q, i) => {
      if (answers[i] === q.answer) marks += q.marks;
    });
    const percent = Math.round((marks / paper.totalMarks) * 100);
    recordExam({
      module: paper.module,
      level: paper.level,
      marks,
      totalMarks: paper.totalMarks,
      grade: gradeFor(percent),
      at: new Date().toISOString(),
    });
  }

  if (submitted && result) {
    if (result.grade === 'Distinction' || result.grade === 'Merit') confetti();
    return (
      <section className="exam-results" aria-live="polite">
        <div className={`result-headline grade-${result.grade.toLowerCase().replace(/\s+/g, '-')}`}>
          <span className="big-mark">
            {result.marks} / {paper.totalMarks}
          </span>
          <span className="big-percent">{result.percent}%</span>
          <span className="big-grade">{result.grade}</span>
        </div>
        <p className="exam-cheer">{examMessage(result.grade)}</p>

        <h3>By lesson</h3>
        <ul className="per-lesson">
          {Object.entries(result.perLesson)
            .sort(([a], [b]) => a.localeCompare(b, undefined, { numeric: true }))
            .map(([lesson, row]) => (
              <li key={lesson}>
                <Link to={`/lesson/${lesson}`}>Lesson {lesson}</Link>
                <span className={row.got === row.total ? 'full' : row.got === 0 ? 'none' : ''}>
                  {row.got} / {row.total}
                </span>
              </li>
            ))}
        </ul>

        <h3>Review every question</h3>
        <ol className="review-list">
          {paper.questions.map((q, i) => {
            const correct = answers[i] === q.answer;
            return (
              <li key={q.id} className={correct ? 'correct' : 'wrong'}>
                <p className="review-stem">
                  <span className="marks-chip">{q.marks} {q.marks === 1 ? 'mark' : 'marks'}</span>
                  {q.stem}
                </p>
                {q.code && (
                  <pre className="quiz-code">
                    <code>{q.code}</code>
                  </pre>
                )}
                <p className="review-answer">
                  {answers[i] === null ? (
                    <em>You left this blank.</em>
                  ) : (
                    <>
                      Your answer: <code>{q.options[answers[i]!]}</code>
                      {!correct && (
                        <>
                          {' '}· Correct: <code>{q.options[q.answer]}</code>
                        </>
                      )}
                    </>
                  )}
                </p>
                <p className="review-why">{q.explanation}</p>
              </li>
            );
          })}
        </ol>

        <div className="exam-actions">
          <button onClick={onRetry}>New paper</button>
          <Link className="button ghost" to="/syllabus">
            Back to syllabus
          </Link>
        </div>
      </section>
    );
  }

  const q = paper.questions[index];
  return (
    <section className="exam">
      <div className="exam-bar">
        <span>
          Question {index + 1} of {paper.questions.length}
        </span>
        <span className="marks-chip">{q.marks} {q.marks === 1 ? 'mark' : 'marks'}</span>
        <span className="answered">
          {answeredCount} / {paper.questions.length} answered
        </span>
        {timed && (
          <span className={`exam-timer ${remaining <= 60 ? 'low' : ''}`} role="timer" aria-live="off">
            ⏱ {formatTime(remaining)}
          </span>
        )}
      </div>

      <h3>{q.stem}</h3>
      {q.code && (
        <pre className="quiz-code">
          <code>{q.code}</code>
        </pre>
      )}

      <ul className="options">
        {q.options.map((option, i) => (
          <li key={i}>
            <button
              className={`option ${answers[index] === i ? 'chosen' : ''}`}
              onClick={() => choose(i)}
              aria-pressed={answers[index] === i}
            >
              <span className="letter">{String.fromCharCode(65 + i)}</span>
              <code>{option}</code>
            </button>
          </li>
        ))}
      </ul>

      <div className="exam-nav">
        <button className="ghost" onClick={() => setIndex((i) => i - 1)} disabled={index === 0}>
          ← Previous
        </button>
        {index < paper.questions.length - 1 ? (
          <button onClick={() => setIndex((i) => i + 1)}>Next →</button>
        ) : (
          <button onClick={submit}>Submit exam</button>
        )}
      </div>

      {/* An answer-sheet overview: jump anywhere, see what is left. */}
      <div className="answer-sheet" role="group" aria-label="Answer sheet">
        {paper.questions.map((_, i) => (
          <button
            key={i}
            className={`sheet-dot ${answers[i] !== null ? 'done' : ''} ${i === index ? 'here' : ''}`}
            onClick={() => setIndex(i)}
            aria-label={`Question ${i + 1}${answers[i] !== null ? ', answered' : ', unanswered'}`}
          >
            {i + 1}
          </button>
        ))}
      </div>

      {answeredCount < paper.questions.length && index === paper.questions.length - 1 && (
        <p className="muted">
          {paper.questions.length - answeredCount} question(s) still blank — you can submit
          anyway, or use the grid above to fill them in.
        </p>
      )}
    </section>
  );
}
