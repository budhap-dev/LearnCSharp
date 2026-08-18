import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import rehypeHighlight from 'rehype-highlight';
import type { PluggableList } from 'unified';
import csharp from 'highlight.js/lib/languages/csharp';
import { loadNotes, stripFrontmatter, MODULE_INFO } from '../lib/lessons';
import { useSyllabus } from '../lib/useSyllabus';
import { MarkdownLink } from '../components/MarkdownLink';
import { Diagram } from '../components/Diagram';
import { CodeBlock } from '../components/CodeBlock';
import { termsForLesson } from '../lib/glossary';
import { Rich } from '../components/Rich';
import { Quiz, type Question } from '../components/Quiz';
import { bestScore, setLessonState } from '../lib/progress';

const quizzes = import.meta.glob('../data/quizzes/*.json', { import: 'default' });

// A ```diagram fence names a themed SVG from the registry; everything else is a
// normal code block. We intercept at <pre> so the diagram is not nested inside one.
function PreBlock(props: React.ComponentProps<'pre'>) {
  const child = props.children as React.ReactElement<{ className?: string; children?: React.ReactNode }> | undefined;
  const className = child?.props?.className ?? '';

  if (/language-diagram/.test(className)) {
    return <Diagram name={String(child?.props?.children).trim()} />;
  }

  // Recover the raw text for the clipboard. After highlighting, children are nested spans,
  // so walk them and concatenate their text.
  const source = textOf(child?.props?.children);
  return (
    <CodeBlock source={source}>
      <pre {...props} />
    </CodeBlock>
  );
}

function textOf(node: React.ReactNode): string {
  if (node == null || typeof node === 'boolean') return '';
  if (typeof node === 'string' || typeof node === 'number') return String(node);
  if (Array.isArray(node)) return node.map(textOf).join('');
  if (typeof node === 'object' && 'props' in node) {
    return textOf((node as React.ReactElement<{ children?: React.ReactNode }>).props.children);
  }
  return '';
}

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
    <article className="lesson" style={{ ['--mc' as string]: `var(--m${lesson.module})` }}>
      <nav className="crumbs">
        <Link to="/syllabus">Syllabus</Link> <span>/</span>{' '}
        <span>
          {MODULE_INFO[lesson.module]?.emoji} Module {lesson.module} — {MODULE_INFO[lesson.module]?.name}
        </span>
      </nav>

      <h1>
        <span className="lid big">{lesson.id}</span>
        {lesson.title}
      </h1>

      <p className="summary">{lesson.summary}</p>

      {lesson.objectives.length > 0 && (
        <section className="objectives">
          <h2>By the end of this lesson you will be able to</h2>
          <ul>
            {lesson.objectives.map((o) => (
              <li key={o}>{o}</li>
            ))}
          </ul>
        </section>
      )}

      {notes ? (
        <div className="prose">
          <ReactMarkdown
            remarkPlugins={[remarkGfm]}
            rehypePlugins={rehypePlugins}
            components={{ a: MarkdownLink, pre: PreBlock }}
          >
            {notes}
          </ReactMarkdown>
        </div>
      ) : (
        <p className="callout">
          <strong>The written notes for this lesson are still being prepared.</strong> The
          worked example is complete and runnable — open the source file listed at the foot of
          this page to read the fully commented version.
        </p>
      )}

      <KeyTerms lessonId={lesson.id} />

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

      <p className="source-note">
        Every snippet above is plain C# you can copy into your own console app. To run the full
        worked example — verified in CI on every change — clone the repo and run{' '}
        <code>dotnet run --project src/LearnCSharp.Lessons -- {lesson.id}</code>
      </p>

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

/** The glossary entries this lesson teaches, as a quick recap before the quiz. */
function KeyTerms({ lessonId }: { lessonId: string }) {
  const terms = termsForLesson(lessonId);
  if (terms.length === 0) return null;
  return (
    <section className="key-terms">
      <h2>Key terms</h2>
      <dl>
        {terms.map((t) => (
          <div key={t.slug}>
            <dt>
              <Link to={`/glossary?term=${t.slug}`}>{t.term}</Link>
            </dt>
            <dd><Rich text={t.definition} /></dd>
          </div>
        ))}
      </dl>
      <p className="muted">
        <Link to="/glossary">Browse the full glossary →</Link>
      </p>
    </section>
  );
}
