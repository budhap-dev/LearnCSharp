import { useRef, useState } from 'react';
import { recordQuiz } from '../lib/progress';
import { useCountdown, formatTime } from '../lib/useCountdown';
import { useUnsavedWarning } from '../lib/useUnsavedWarning';

const QUIZ_SECONDS_PER_QUESTION = 60;

export interface Question {
  id: string;
  topic: string;
  stem: string;
  code?: string;
  options: string[];
  answer: number;
  explanation: string;
}

interface Props {
  lessonId: string;
  questions: Question[];
}

/**
 * One question at a time, with the explanation shown the moment an answer is given -
 * a misconception is corrected while it is still fresh.
 */
export function Quiz({ lessonId, questions }: Props) {
  const [index, setIndex] = useState(0);
  const [picked, setPicked] = useState<number | null>(null);
  const [score, setScore] = useState(0);
  const [finished, setFinished] = useState(false);
  const [wrong, setWrong] = useState<Question[]>([]);
  const [timerOn, setTimerOn] = useState(false);
  const scoreRef = useRef(0);
  scoreRef.current = score;

  const question = questions[index];

  // Optional, student-initiated timer. On expiry the quiz ends and records the score so far.
  const remaining = useCountdown(
    questions.length * QUIZ_SECONDS_PER_QUESTION,
    timerOn && !finished,
    () => {
      recordQuiz(lessonId, scoreRef.current, questions.length);
      setFinished(true);
    },
  );

  // Warn before leaving once the quiz is under way and not finished.
  useUnsavedWarning(!finished && (index > 0 || picked !== null));

  function choose(option: number) {
    if (picked !== null) return; // already answered

    setPicked(option);
    if (option === question.answer) {
      setScore((s) => s + 1);
    } else {
      setWrong((w) => [...w, question]);
    }
  }

  function next() {
    if (index + 1 < questions.length) {
      setIndex(index + 1);
      setPicked(null);
      return;
    }

    const finalScore = score;
    recordQuiz(lessonId, finalScore, questions.length);
    setFinished(true);
  }

  function restart() {
    setIndex(0);
    setPicked(null);
    setScore(0);
    setWrong([]);
    setFinished(false);
  }

  if (finished) {
    const percent = Math.round((score / questions.length) * 100);
    return (
      <section className="quiz" aria-live="polite">
        <h3>
          {score} / {questions.length} — {percent}%
        </h3>
        <p>
          {percent >= 80
            ? 'Lesson marked complete. Well done.'
            : 'Marked as “needs review” — worth another read before moving on.'}
        </p>

        {wrong.length > 0 && (
          <>
            <h4>Worth revisiting</h4>
            <ul className="review">
              {wrong.map((q) => (
                <li key={q.id}>
                  <strong>{q.topic}</strong> — {q.explanation}
                </li>
              ))}
            </ul>
          </>
        )}

        <button onClick={restart}>Try again</button>
      </section>
    );
  }

  return (
    <section className="quiz">
      <div className="quiz-top">
        <p className="progress-line">
          Question {index + 1} of {questions.length}
        </p>
        {timerOn ? (
          <span className={`exam-timer ${remaining <= 30 ? 'low' : ''}`} role="timer">
            ⏱ {formatTime(remaining)}
          </span>
        ) : (
          <button className="timer-start" onClick={() => setTimerOn(true)}>
            ⏱ Start timer ({formatTime(questions.length * QUIZ_SECONDS_PER_QUESTION)})
          </button>
        )}
      </div>

      <h3>{question.stem}</h3>

      {question.code && (
        <pre className="quiz-code">
          <code>{question.code}</code>
        </pre>
      )}

      <ul className="options">
        {question.options.map((option, i) => {
          const isAnswer = i === question.answer;
          const state =
            picked === null ? '' : isAnswer ? 'correct' : i === picked ? 'wrong' : 'muted';

          return (
            <li key={i}>
              <button
                className={`option ${state}`}
                onClick={() => choose(i)}
                disabled={picked !== null}
              >
                <span className="letter">{String.fromCharCode(65 + i)}</span>
                <code>{option}</code>
              </button>
            </li>
          );
        })}
      </ul>

      {picked !== null && (
        <div className="explanation" aria-live="polite">
          <p>
            <strong>{picked === question.answer ? 'Correct.' : 'Not quite.'}</strong>{' '}
            {question.explanation}
          </p>
          <button onClick={next}>
            {index + 1 < questions.length ? 'Next question' : 'See results'}
          </button>
        </div>
      )}
    </section>
  );
}
