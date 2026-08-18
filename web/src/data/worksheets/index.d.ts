export interface WorksheetTask {
  /** "module.number", e.g. "1.3" - unique across all worksheets. */
  id: string;
  /** The lesson this task practises. */
  lesson: string;
  title: string;
  /** 1 = warm-up, 2 = standard, 3 = challenge. */
  level: 1 | 2 | 3;
  /** What to do, with `backtick` spans for inline code. */
  task: string;
  /** A complete Program.cs (top-level statements) that compiles as-is. */
  starter: string;
  /** Lines to type at the console, if the program reads input. */
  input?: string;
  /** Exact stdout the finished program must print. */
  expected: string;
  hints: string[];
  /** A complete Program.cs that prints `expected`. Verified by scripts/verify-worksheets.mjs. */
  solution: string;
}

export interface Worksheet {
  module: number;
  intro: string;
  tasks: WorksheetTask[];
}

export const WORKSHEETS: Worksheet[];
