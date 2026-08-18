import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { Quiz, type Question } from '../components/Quiz';
import { useSyllabus } from '../lib/useSyllabus';

// The quiz JSON files, bundled and code-split, loaded on demand.
const quizzes = import.meta.glob('../data/quizzes/*.json', { import: 'default' });

/**
 * A focused quiz page - just the questions, no theory to scroll past. This is where the
 * Practice tab sends you; the lesson page keeps its own inline quiz for reading in context.
 */
export function QuizPage() {
  const { id } = useParams();
  const syllabus = useSyllabus();
  const lesson = syllabus?.find((l) => l.id === id);

  const [questions, setQuestions] = useState<Question[] | null | undefined>(undefined);

  useEffect(() => {
    if (!id) return;
    setQuestions(undefined);
    const load = quizzes[`../data/quizzes/${id}.json`];
    if (load) load().then((q) => setQuestions(q as Question[]));
    else setQuestions(null);
  }, [id]);

  if (!syllabus) return <p className="muted">Loading…</p>;

  if (!lesson) {
    return (
      <>
        <h1>Quiz not found</h1>
        <Link to="/practice">Back to Practice</Link>
      </>
    );
  }

  return (
    <article className="quiz-page">
      <nav className="crumbs">
        <Link to="/practice">Practice</Link> <span>/</span> Quiz
      </nav>

      <h1>
        <span className="lid big">{lesson.id}</span>
        {lesson.title} — quiz
      </h1>
      <p className="lede">
        Ten questions, with an explanation after each answer.{' '}
        <Link to={`/lesson/${lesson.id}`}>Read the lesson first →</Link>
      </p>

      {questions === undefined && <p className="muted">Loading the quiz…</p>}
      {questions === null && (
        <p className="callout">No quiz exists for this lesson yet.</p>
      )}
      {questions && <Quiz lessonId={lesson.id} questions={questions} />}

      <nav className="pager">
        <Link to={`/lesson/${lesson.id}`}>← Back to the lesson</Link>
        <Link to="/practice">More practice →</Link>
      </nav>
    </article>
  );
}
