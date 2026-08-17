/**
 * Progress lives in localStorage only - no accounts, no backend, nothing personal
 * ever leaves the device. Versioned so the shape can change later without
 * throwing away a student's history.
 */
const KEY = 'learncsharp.progress.v1';

export type LessonState = 'not-started' | 'in-progress' | 'needs-review' | 'done';

export interface QuizAttempt {
  score: number;
  outOf: number;
  at: string;
}

export interface Progress {
  version: 1;
  lessons: Record<string, LessonState>;
  quizzes: Record<string, QuizAttempt[]>;
}

const empty: Progress = { version: 1, lessons: {}, quizzes: {} };

export function read(): Progress {
  try {
    const stored = localStorage.getItem(KEY);
    if (!stored) return { ...empty };
    const parsed = JSON.parse(stored) as Progress;
    return parsed.version === 1 ? parsed : { ...empty };
  } catch {
    // Private browsing, full storage, or corrupt data must never break the site.
    return { ...empty };
  }
}

function write(progress: Progress): void {
  try {
    localStorage.setItem(KEY, JSON.stringify(progress));
    window.dispatchEvent(new Event('progress-changed'));
  } catch {
    /* storage unavailable - the site still works, it just cannot remember */
  }
}

export function setLessonState(id: string, state: LessonState): void {
  const progress = read();
  progress.lessons[id] = state;
  write(progress);
}

export function recordQuiz(id: string, score: number, outOf: number): void {
  const progress = read();
  const attempts = progress.quizzes[id] ?? [];
  attempts.push({ score, outOf, at: new Date().toISOString() });
  progress.quizzes[id] = attempts;

  // 80% or better counts as learned; below that it is flagged for another look.
  progress.lessons[id] = score / outOf >= 0.8 ? 'done' : 'needs-review';
  write(progress);
}

export function bestScore(id: string): QuizAttempt | null {
  const attempts = read().quizzes[id] ?? [];
  if (attempts.length === 0) return null;
  return attempts.reduce((best, a) => (a.score > best.score ? a : best));
}

export function reset(): void {
  try {
    localStorage.removeItem(KEY);
    window.dispatchEvent(new Event('progress-changed'));
  } catch {
    /* nothing to do */
  }
}

export function exportJson(): string {
  return JSON.stringify(read(), null, 2);
}
