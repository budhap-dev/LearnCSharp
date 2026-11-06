import { lazy, Suspense, useEffect, useState, useSyncExternalStore } from 'react';
import type { WorksheetTask } from '../data/worksheets/index.js';
import { compareOutput, getRunnerState, runProgram, subscribeRunner, type RunResult } from '../lib/runner';

const CodeEditor = lazy(() => import('./CodeEditor'));

const draftKey = (id: string) => `lcs-worksheet-draft-${id}`;

function readDraft(id: string): string | null {
  try {
    return localStorage.getItem(draftKey(id));
  } catch {
    return null;
  }
}

function saveDraft(id: string, code: string, starter: string) {
  try {
    if (code === starter) localStorage.removeItem(draftKey(id));
    else localStorage.setItem(draftKey(id), code);
  } catch {
    // Private browsing or a full disk: the code still works for this visit.
  }
}

interface Props {
  task: WorksheetTask;
  /** Called when the program's output matches the task's target. */
  onPass: () => void;
}

/** Edit, run and check a worksheet task in the browser. */
export function TaskRunner({ task, onPass }: Props) {
  const [code, setCode] = useState(() => readDraft(task.id) ?? task.starter);
  const [input, setInput] = useState(task.input?.replace(/\n$/, '') ?? '');
  const [result, setResult] = useState<RunResult | null>(null);
  const [running, setRunning] = useState(false);
  const runner = useSyncExternalStore(subscribeRunner, getRunnerState);

  useEffect(() => saveDraft(task.id, code, task.starter), [task.id, code, task.starter]);

  async function run() {
    if (getRunnerState().busy) return;
    setRunning(true);
    setResult(null);
    const outcome = await runProgram(code, input);
    setRunning(false);
    setResult(outcome);
    if (outcome.kind === 'ran' && passed(outcome, task)) onPass();
  }

  function reset() {
    if (code !== task.starter && !window.confirm('Replace your code with the starter?')) return;
    setCode(task.starter);
    setResult(null);
  }

  return (
    <div className="runner">
      <div className="ws-block">
        <div className="ws-block-head">
          <span>Your code</span>
          <button type="button" className="ws-ghost runner-reset" onClick={reset} disabled={code === task.starter}>
            Reset to starter
          </button>
        </div>
        <Suspense fallback={<pre className="ws-code">{code}</pre>}>
          <CodeEditor value={code} onChange={setCode} onRun={run} label={`Code for task ${task.id}`} />
        </Suspense>
      </div>

      {task.input !== undefined && (
        <div className="ws-block">
          <div className="ws-block-head">
            <label htmlFor={`input-${task.id}`}>Input, one line per Console.ReadLine</label>
          </div>
          <textarea
            id={`input-${task.id}`}
            className="runner-input"
            rows={Math.max(2, input.split('\n').length)}
            spellCheck={false}
            value={input}
            onChange={(event) => setInput(event.target.value)}
          />
        </div>
      )}

      <div className="runner-actions">
        <button type="button" className="runner-run" onClick={run} disabled={runner.busy}>
          ▶ Run
        </button>
        <span className="runner-tip muted">Ctrl or ⌘ + Enter</span>
      </div>

      {running && <Progress loading={runner.loading} />}
      {result && <Result result={result} task={task} />}
    </div>
  );
}

function passed(result: Extract<RunResult, { kind: 'ran' }>, task: WorksheetTask): boolean {
  return !result.exception && !result.truncated && compareOutput(result.output, task.expected).matches;
}

function Progress({ loading }: { loading: { done: number; total: number } | null }) {
  const text = loading
    ? `Starting the C# compiler. The first time takes a little longer${loading.total ? ` (${loading.done} of ${loading.total} files)` : ''}…`
    : 'Running…';
  return (
    <div className="runner-banner busy" role="status">
      <span className="runner-spinner" aria-hidden="true" />
      {text}
    </div>
  );
}

function Banner({ tone, title, detail }: { tone: 'ok' | 'warn' | 'err'; title: string; detail?: string }) {
  return (
    <div className={`runner-banner ${tone}`} role="status">
      <span>{title}</span>
      {detail && <small>{detail}</small>}
    </div>
  );
}

function Result({ result, task }: { result: RunResult; task: WorksheetTask }) {
  switch (result.kind) {
    case 'compile-error': {
      const count = result.errors.length;
      return (
        <>
          <Banner tone="err" title={`✗ Didn't compile: ${count} error${count === 1 ? '' : 's'}`} />
          <pre className="runner-term err">
            {result.errors.map((error) => (
              <span className="bad" key={error}>
                {error}
              </span>
            ))}
          </pre>
        </>
      );
    }
    case 'timeout':
      return (
        <>
          <Banner tone="warn" title="⏱ Stopped: it ran for too long" />
          <pre className="runner-term warn">
            <span className="note">
              Your program did not finish within {result.seconds} seconds, which usually means an infinite loop.
            </span>
          </pre>
        </>
      );
    case 'crashed':
      return (
        <>
          <Banner tone="err" title="✗ Your program crashed the runner" />
          <pre className="runner-term err">
            <span className="bad">This usually means a stack overflow: a method that keeps calling itself.</span>
          </pre>
        </>
      );
    case 'unavailable':
      return (
        <div className="runner-banner err standalone" role="alert">
          <span>
            ✗ The C# compiler could not be loaded on this device. Try again, or paste your code into{' '}
            <a href="https://dotnetfiddle.net" target="_blank" rel="noreferrer">
              .NET Fiddle
            </a>
            .
          </span>
          <small>{result.message}</small>
        </div>
      );
    case 'ran':
      return <RanResult result={result} task={task} />;
  }
}

function RanResult({ result, task }: { result: Extract<RunResult, { kind: 'ran' }>; task: WorksheetTask }) {
  const timing = `Compile ${result.compileMs} ms · Run ${result.runMs} ms`;
  const output = result.output || (result.exception ? '' : '(no output)');
  // The messages below start on their own line, so a final newline here would leave a gap.
  const outputBeforeMessage = output.replace(/\n$/, '');

  if (result.exception) {
    const unsupported = result.exception.type === 'System.PlatformNotSupportedException';
    return (
      <>
        <Banner tone="err" title="✗ Crashed while running" detail={timing} />
        <pre className="runner-term err">
          {outputBeforeMessage}
          <span className="bad">
            {result.exception.type}: {result.exception.message}
          </span>
          {unsupported && (
            <span className="note">
              This feature does not work in a browser, so this is not your mistake. Run it with `dotnet run` on your
              own computer instead.
            </span>
          )}
        </pre>
      </>
    );
  }

  if (result.truncated) {
    return (
      <>
        <Banner tone="warn" title="⏱ Stopped: it printed too much" detail={timing} />
        <pre className="runner-term warn">
          {outputBeforeMessage}
          <span className="note">Stopped here. A loop that never ends is the usual cause.</span>
        </pre>
      </>
    );
  }

  const comparison = compareOutput(result.output, task.expected);
  if (comparison.matches) {
    return (
      <>
        <Banner tone="ok" title="✓ Matches the target. Marked as done" detail={timing} />
        <pre className="runner-term ok">{output}</pre>
      </>
    );
  }
  return (
    <>
      <Banner tone="warn" title="It ran, but doesn't match the target yet" detail={timing} />
      <pre className="runner-term warn">{output}</pre>
      <dl className="runner-diff">
        <dt>First difference, on line {comparison.line}</dt>
        <dd>
          <span className="runner-diff-label">Target</span>
          <code>{comparison.target || '(nothing)'}</code>
        </dd>
        <dd>
          <span className="runner-diff-label">Yours</span>
          <code>{comparison.actual || '(nothing)'}</code>
        </dd>
      </dl>
    </>
  );
}
