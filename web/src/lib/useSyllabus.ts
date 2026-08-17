import { useEffect, useState } from 'react';
import { loadSyllabus, type SyllabusEntry } from './lessons';

/** Loads the syllabus index once and shares it across every route. */
export function useSyllabus(): SyllabusEntry[] | null {
  const [syllabus, setSyllabus] = useState<SyllabusEntry[] | null>(null);

  useEffect(() => {
    let live = true;
    loadSyllabus().then((s) => {
      if (live) setSyllabus(s);
    });
    return () => {
      live = false;
    };
  }, []);

  return syllabus;
}
