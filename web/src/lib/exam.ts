/**
 * Topic exams (US-407). Unlike a lesson quiz - which teaches, with instant explanations - an
 * exam MEASURES: marks, no help until the end, a grade band.
 *
 * Difficulty is not tagged on every question, but a real signal already exists in the data:
 * a question with a `code` block is predict-output / spot-the-bug, genuinely harder than a
 * plain conceptual one. We use that to build the three sets and to weight marks - a code
 * question is worth 2 marks, a conceptual one 1.
 */
import type { Question } from '../components/Quiz';

export type Level = 'foundation' | 'standard' | 'challenge';

export interface LevelInfo {
  id: Level;
  label: string;
  blurb: string;
  /** Roughly how many questions the paper aims for. */
  size: number;
  /** Percentage needed for the lowest pass band. */
  pass: number;
}

export const LEVELS: LevelInfo[] = [
  {
    id: 'foundation',
    label: 'Foundation',
    blurb: 'A short paper — a dozen questions sampled across the module. A confidence check.',
    size: 12,
    pass: 50,
  },
  {
    id: 'standard',
    label: 'Standard',
    blurb: 'A fuller paper of twenty questions, and a higher pass mark. Exam breadth.',
    size: 20,
    pass: 60,
  },
  {
    id: 'challenge',
    label: 'Challenge',
    blurb: 'Every question in the module, code-and-traps first, and the strictest bar.',
    size: 0, // 0 = the whole module
    pass: 70,
  },
];

/** One question on a paper, with the marks it is worth. */
export interface ExamQuestion extends Question {
  marks: number;
  lesson: string;
}

export interface ExamPaper {
  module: number;
  level: Level;
  questions: ExamQuestion[];
  totalMarks: number;
}

export interface GradeBand {
  name: string;
  min: number;
}

// Highest first: the first band whose min is met wins.
export const GRADE_BANDS: GradeBand[] = [
  { name: 'Distinction', min: 80 },
  { name: 'Merit', min: 70 },
  { name: 'Pass', min: 55 },
  { name: 'Not yet', min: 0 },
];

export function gradeFor(percent: number): string {
  return (GRADE_BANDS.find((b) => percent >= b.min) ?? GRADE_BANDS.at(-1)!).name;
}

const isHard = (q: Question): boolean => typeof q.code === 'string' && q.code.length > 0;

export const marksFor = (q: Question): number => (isHard(q) ? 2 : 1);

/**
 * Deterministic-enough shuffle. Math.random is unavailable in some sandboxes and makes exams
 * unrepeatable; a seeded generator lets "retry the same paper" mean something.
 */
function seededShuffle<T>(items: T[], seed: number): T[] {
  const copy = [...items];
  let state = seed || 1;
  for (let i = copy.length - 1; i > 0; i--) {
    state = (state * 1103515245 + 12345) & 0x7fffffff;
    const j = state % (i + 1);
    [copy[i], copy[j]] = [copy[j], copy[i]];
  }
  return copy;
}

interface Tagged {
  question: Question;
  lesson: string;
}

/**
 * Builds a paper for one module at one level. Draws across every lesson in the module, biased
 * towards or away from harder (code) questions by level, and caps the length.
 */
export function buildPaper(
  module: number,
  level: Level,
  byLesson: Record<string, Question[]>,
  seed: number,
): ExamPaper {
  const info = LEVELS.find((l) => l.id === level)!;

  const all: Tagged[] = Object.entries(byLesson)
    .filter(([id]) => Number(id.split('.')[0]) === module)
    .flatMap(([lesson, qs]) => qs.map((question) => ({ question, lesson })));

  const hard = all.filter((t) => isHard(t.question));
  const easy = all.filter((t) => !isHard(t.question));

  let chosen: Tagged[];
  if (info.size === 0) {
    // Challenge: the whole module, harder (code) questions presented first.
    chosen = [...seededShuffle(hard, seed), ...seededShuffle(easy, seed + 1)];
  } else {
    // Foundation/Standard: a sample of the requested length, code questions kept in
    // proportion to how many the module actually has (most are conceptual).
    const hardShare = hard.length / Math.max(1, all.length);
    const wantHard = Math.min(hard.length, Math.round(info.size * hardShare));
    const wantEasy = Math.min(easy.length, info.size - wantHard);
    chosen = seededShuffle(
      [
        ...seededShuffle(hard, seed).slice(0, wantHard),
        ...seededShuffle(easy, seed + 1).slice(0, wantEasy),
      ],
      seed + 2,
    );
  }

  const questions: ExamQuestion[] = chosen.map((t) => ({
    ...t.question,
    marks: marksFor(t.question),
    lesson: t.lesson,
  }));

  return {
    module,
    level,
    questions,
    totalMarks: questions.reduce((sum, q) => sum + q.marks, 0),
  };
}
