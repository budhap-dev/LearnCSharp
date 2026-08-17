import { useEffect, useState } from 'react';
import { loadOutput, type LessonOutputData } from '../lib/lessons';

interface Props {
  lesson: string;
  /** A section heading from the lesson. Omit to show the whole thing. */
  section?: string;
}

/**
 * Renders output captured from a real run of the .NET lesson.
 * Nothing here is hand-typed, so it cannot drift away from the code.
 */
export function LessonOutput({ lesson, section }: Props) {
  const [data, setData] = useState<LessonOutputData | null>(null);
  const [failed, setFailed] = useState(false);

  useEffect(() => {
    let live = true;
    setData(null);
    setFailed(false);

    loadOutput(lesson).then(
      (d) => live && setData(d),
      () => live && setFailed(true),
    );

    return () => {
      live = false;
    };
  }, [lesson]);

  if (failed) return <p className="missing">No captured output for lesson {lesson}.</p>;
  if (!data) return <p className="muted">Loading output…</p>;

  const text = section ? data.sections[section] : data.fullOutput;

  if (text === undefined) {
    return (
      <p className="missing">
        Lesson {lesson} has no section called “{section}”.
      </p>
    );
  }

  return (
    <figure className="output">
      <figcaption>
        <span className="dot" aria-hidden="true" />
        output — verified by running lesson {lesson}
      </figcaption>
      <pre>
        <code>{text}</code>
      </pre>
    </figure>
  );
}
