/**
 * Helpers over the worksheet data. The worksheets themselves live as plain .js under
 * src/data/worksheets/ (so the .NET verifier can read them too); this module gives the
 * React pages typed access and the lesson -> task lookup.
 */
import { WORKSHEETS, type Worksheet, type WorksheetTask } from '../data/worksheets/index.js';

export type { Worksheet, WorksheetTask };

/** The module a worksheet task belongs to, from the "module.number" id. */
export function moduleOfTask(task: WorksheetTask): number {
  return Number(task.id.split('.')[0]);
}

/** Every task that practises a given lesson, in worksheet order. */
export function tasksForLesson(lessonId: string): WorksheetTask[] {
  const out: WorksheetTask[] = [];
  for (const sheet of WORKSHEETS) {
    for (const task of sheet.tasks) {
      if (task.lesson === lessonId) out.push(task);
    }
  }
  return out;
}
