import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import rehypeHighlight from 'rehype-highlight';
import type { PluggableList } from 'unified';
import csharp from 'highlight.js/lib/languages/csharp';
import { loadNotes, stripFrontmatter, MODULE_NAMES } from '../lib/lessons';
import { useSyllabus } from '../lib/useSyllabus';
import { LessonOutput } from '../components/LessonOutput';
import { Quiz, type Question } from '../components/Quiz';
import { bestScore, setLessonState } from '../lib/progress';

const quizzes = import.meta.glob('../data/quizzes/*.json', { import: 'default' });

// Only C# is registered, so highlight.js does not drag in 190 other grammars.
const rehypePlugins: PluggableList = [[rehypeHighlight, { languages: { csharp }, detect: false }]];

export function Lesson() {
  const { id } = useParams();
  const syllabus = useSyllabus();
  const lesson = syllabus?.find((l) => l.id === id);

  const [notes, setNotes] = useState<string | null>(null);
  const [questions, setQuestions] = useState<Question[] | null>(null);
  const [showQuiz, setShowQuiz] = useState(false);

  useEffect(() => {
    if (!id) return;

    setNotes(null);
    setQuestions(null);
    setShowQuiz(false);

    loadNotes(id).then((md) => setNotes(md ? stripFrontmatter(md) : null));

    const loadQuiz = quizzes[`../data/quizzes/${id}.json`];
    if (loadQuiz) loadQuiz().then((q) => setQuestions(q as Question[]));

    setLessonState(id, 'in-progress');
  }, [id]);

  if (!syllabus) return <p className="muted">Loading…</p>;

  if (!lesson) {
    return (
      <>
        <h1>Lesson not found</h1>
        <Link to="/syllabus">Back to the syllabus</Link>
      </>
    );
  }

  const position = syllabus.indexOf(lesson);
  const previous = syllabus[position - 1];
  const next = syllabus[position + 1];
  const best = bestScore(lesson.id);

  return (
    <article className="lesson">
      <nav className="crumbs">
        <Link to="/syllabus">Syllabus</Link> <span>/</span>{' '}
        <span>
          Module {lesson.module} — {MODULE_NAMES[lesson.module]}
        </span>
      </nav>

      <h1>
        <span className="lid big">{lesson.id}</span>
        {lesson.title}
      </h1>

      {notes ? (
        <div className="prose">
          <ReactMarkdown remarkPlugins={[remarkGfm]} rehypePlugins={rehypePlugins}>
            {notes}
          </ReactMarkdown>
        </div>
      ) : (
        <p className="callout">
          Written notes for this lesson are still to come. The verified output from the
          worked example is below — it shows exactly what the code produces.
        </p>
      )}

      <h2>What the code prints</h2>
      <p className="muted">
        Captured by running the real lesson, so it can never drift out of date.
      </p>
      <LessonOutput lesson={lesson.id} />

      <section className="quiz-panel">
        <h2>Check yourself</h2>

        {questions === null ? (
          <p className="muted">No quiz for this lesson yet.</p>
        ) : showQuiz ? (
          <Quiz lessonId={lesson.id} questions={questions} />
        ) : (
          <>
            <p>
              {questions.length} questions.
              {best && ` Your best so far: ${best.score}/${best.outOf}.`}
            </p>
            <button onClick={() => setShowQuiz(true)}>
              {best ? 'Try again' : 'Start the quiz'}
            </button>
          </>
        )}
      </section>

      <nav className="pager">
        {previous ? (
          <Link to={`/lesson/${previous.id}`}>
            ← {previous.id} {previous.title}
          </Link>
        ) : (
          <span />
        )}
        {next && (
          <Link to={`/lesson/${next.id}`}>
            {next.id} {next.title} →
          </Link>
        )}
      </nav>
    </article>
  );
}
