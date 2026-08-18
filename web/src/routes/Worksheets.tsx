import { useEffect, useMemo, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { WORKSHEETS, type WorksheetTask } from '../data/worksheets/index.js';
import { MODULE_INFO } from '../lib/lessons';
import { Rich } from '../components/Rich';
import { CopyButton } from '../components/CopyButton';
import { toggleWorksheet, worksheetDone } from '../lib/progress';

const LEVEL = {
  1: { label: 'Warm-up', cls: 'lvl-1' },
  2: { label: 'Standard', cls: 'lvl-2' },
  3: { label: 'Challenge', cls: 'lvl-3' },
} as const;

const modules = WORKSHEETS.map((w) => w.module);

/**
 * Programming worksheets: tasks a student solves in their own IDE. Each gives a runnable
 * starter to copy, the exact output to aim for, staged hints, and a checked solution. All
 * solutions are verified by scripts/verify-worksheets.mjs, so the target output is real.
 */
export function Worksheets() {
  const [params, setParams] = useSearchParams();
  const paramModule = Number(params.get('module'));
  const active = modules.includes(paramModule) ? paramModule : modules[0];
  const worksheet = WORKSHEETS.find((w) => w.module === active)!;

  const [done, setDone] = useState<Set<string>>(new Set());
  useEffect(() => {
    const sync = () =>
      setDone(new Set(WORKSHEETS.flatMap((w) => w.tasks).filter((t) => worksheetDone(t.id)).map((t) => t.id)));
    sync();
    window.addEventListener('progress-changed', sync);
    return () => window.removeEventListener('progress-changed', sync);
  }, []);

  function setModule(m: number) {
    setParams(m === modules[0] ? {} : { module: String(m) }, { replace: true });
  }

  const doneCount = useMemo(() => worksheet.tasks.filter((t) => done.has(t.id)).length, [worksheet, done]);

  return (
    <div className="worksheets">
      <h1>Worksheets</h1>
      <p className="lede">
        Practice you do in your own editor. Copy a starter into a C# console app, make it print
        the target output, and check your answer.
      </p>

      <details className="ws-how">
        <summary>How to run these</summary>
        <ol>
          <li>
            Install the <a href="https://dotnet.microsoft.com/download" target="_blank" rel="noreferrer">.NET SDK</a>{' '}
            and open a terminal.
          </li>
          <li>
            Create a project once: <code>dotnet new console -o practice</code> then{' '}
            <code>cd practice</code>.
          </li>
          <li>
            Copy a task&apos;s <strong>starter</strong> into <code>Program.cs</code>, replacing what is
            there.
          </li>
          <li>
            Run it with <code>dotnet run</code>. Edit until the output matches the{' '}
            <strong>target</strong> exactly.
          </li>
          <li>Stuck? Reveal a hint. Finished? Reveal the solution and compare.</li>
        </ol>
        <p className="muted">
          No editor to hand? Paste into an online runner such as{' '}
          <a href="https://dotnetfiddle.net" target="_blank" rel="noreferrer">.NET Fiddle</a> — but typing it
          into a real IDE is the point.
        </p>
      </details>

      <nav className="ws-modules" aria-label="Choose a module">
        {modules.map((m) => (
          <button
            key={m}
            type="button"
            className={m === active ? 'active' : ''}
            style={{ ['--mc' as string]: `var(--m${m})` }}
            onClick={() => setModule(m)}
          >
            <span className="module-emoji">{MODULE_INFO[m]?.emoji}</span> Module {m}
          </button>
        ))}
      </nav>

      <header className="ws-head" style={{ ['--mc' as string]: `var(--m${active})` }}>
        <h2>
          {MODULE_INFO[active]?.emoji} Module {active} — {MODULE_INFO[active]?.name}
        </h2>
        <span className="ws-progress">
          {doneCount} / {worksheet.tasks.length} done
        </span>
      </header>
      <p className="muted ws-intro">{worksheet.intro}</p>

      <ol className="ws-list">
        {worksheet.tasks.map((t) => (
          <Task key={t.id} task={t} done={done.has(t.id)} onToggle={() => toggleWorksheet(t.id)} />
        ))}
      </ol>
    </div>
  );
}

function Task({ task, done, onToggle }: { task: WorksheetTask; done: boolean; onToggle: () => void }) {
  const [hintsShown, setHintsShown] = useState(0);
  const [showSolution, setShowSolution] = useState(false);
  const level = LEVEL[task.level];

  return (
    <li className={`ws-task ${done ? 'is-done' : ''}`}>
      <div className="ws-task-head">
        <span className="ws-num">{task.id}</span>
        <h3>{task.title}</h3>
        <span className={`ws-level ${level.cls}`}>{level.label}</span>
        <Link className="ws-lesson" to={`/lesson/${task.lesson}`}>
          Lesson {task.lesson}
        </Link>
      </div>

      <p className="ws-brief">
        <Rich text={task.task} />
      </p>

      <div className="ws-block">
        <div className="ws-block-head">
          <span>Starter — copy into Program.cs</span>
          <CopyButton source={task.starter} />
        </div>
        <pre className="ws-code">
          <code>{task.starter}</code>
        </pre>
      </div>

      {task.input !== undefined && (
        <div className="ws-block">
          <div className="ws-block-head">
            <span>Type this in when it runs</span>
            <CopyButton source={task.input} />
          </div>
          <pre className="ws-io">
            <code>{task.input.replace(/\n$/, '')}</code>
          </pre>
        </div>
      )}

      <div className="ws-block">
        <div className="ws-block-head">
          <span>Target output — match it exactly</span>
        </div>
        <pre className="ws-io ws-expected">
          <code>{task.expected}</code>
        </pre>
      </div>

      <div className="ws-actions">
        {hintsShown < task.hints.length && (
          <button type="button" className="ws-ghost" onClick={() => setHintsShown((n) => n + 1)}>
            {hintsShown === 0 ? 'Show a hint' : 'Next hint'} ({hintsShown}/{task.hints.length})
          </button>
        )}
        <button type="button" className="ws-ghost" onClick={() => setShowSolution((s) => !s)}>
          {showSolution ? 'Hide solution' : 'Show solution'}
        </button>
        <label className="ws-check">
          <input type="checkbox" checked={done} onChange={onToggle} />
          Mark done
        </label>
      </div>

      {hintsShown > 0 && (
        <ol className="ws-hints">
          {task.hints.slice(0, hintsShown).map((h, i) => (
            <li key={i}>
              <Rich text={h} />
            </li>
          ))}
        </ol>
      )}

      {showSolution && (
        <div className="ws-block ws-solution">
          <div className="ws-block-head">
            <span>One worked solution</span>
            <CopyButton source={task.solution} />
          </div>
          <pre className="ws-code">
            <code>{task.solution}</code>
          </pre>
        </div>
      )}
    </li>
  );
}
