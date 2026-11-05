/**
 * Runs C# in the student's browser, through the worker published from src/LearnCSharp.Runner.
 *
 * The worker is only started by the first Run, because it downloads the whole compiler and most
 * students reading a page never need it. There is one worker for the whole page, running one
 * program at a time, since a program redirects the runtime's single Console.
 */
import { RUNNER_PATH } from './runnerPath.generated';

export type RunResult =
  | { kind: 'compile-error'; errors: string[] }
  | {
      kind: 'ran';
      output: string;
      compileMs: number;
      runMs: number;
      /** The program printed so much that it was stopped. */
      truncated: boolean;
      exception?: { type: string; message: string };
    }
  | { kind: 'timeout'; seconds: number }
  /** The program took the runtime down with it, usually with a stack overflow. */
  | { kind: 'crashed' }
  /** The compiler could not be loaded. */
  | { kind: 'unavailable'; message: string };

export interface RunnerState {
  /** A program is being compiled or run, so no other can start. */
  busy: boolean;
  /** Set while the compiler is loading, with how many of its files have arrived so far. */
  loading: { done: number; total: number } | null;
}

/** How long a program gets, counted from when it has finished compiling. */
const RUN_SECONDS = 10;
/** Compiling is slow on a phone, but a compile that goes on this long has gone wrong. */
const COMPILE_SECONDS = 120;
/** Generous, for a slow connection, but a stalled download should not spin forever. */
const LOAD_SECONDS = 300;

let worker: Worker | null = null;
let starting: Promise<Worker> | null = null;
let nextId = 1;

let state: RunnerState = { busy: false, loading: null };
const listeners = new Set<() => void>();

function setState(change: Partial<RunnerState>) {
  state = { ...state, ...change };
  listeners.forEach((listener) => listener());
}

export function subscribeRunner(listener: () => void): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

export function getRunnerState(): RunnerState {
  return state;
}

function discardWorker() {
  worker?.terminate();
  worker = null;
}

function startWorker(): Promise<Worker> {
  if (worker) return Promise.resolve(worker);
  starting ??= new Promise<Worker>((resolve, reject) => {
    setState({ loading: { done: 0, total: 0 } });
    const candidate = new Worker(`${RUNNER_PATH}/worker.js`, { type: 'module' });
    const fail = (message: string) => {
      clearTimeout(timer);
      candidate.terminate();
      reject(new Error(message));
    };
    const timer = setTimeout(() => fail('It took too long to download.'), LOAD_SECONDS * 1000);
    candidate.onerror = (event) => fail(event.message || 'The compiler could not be loaded.');
    candidate.onmessage = ({ data }) => {
      if (data.type === 'progress') {
        setState({ loading: { done: data.done, total: data.total } });
      } else if (data.type === 'load-failed') {
        fail(data.message);
      } else if (data.type === 'ready') {
        clearTimeout(timer);
        worker = candidate;
        resolve(candidate);
      }
    };
  }).finally(() => {
    starting = null;
    setState({ loading: null });
  });
  return starting;
}

/** Sends a program to the worker. Settles with a result on every path, including a dead worker. */
function execute(target: Worker, code: string, input: string): Promise<RunResult> {
  return new Promise((resolve) => {
    const id = nextId++;
    let compiled: { ms: number } | null = null;
    let timer: ReturnType<typeof setTimeout>;

    const finish = (result: RunResult, workerIsDead = false) => {
      clearTimeout(timer);
      if (workerIsDead) discardWorker();
      resolve(result);
    };
    const giveUpAfter = (seconds: number, result: RunResult) => {
      clearTimeout(timer);
      timer = setTimeout(() => finish(result, true), seconds * 1000);
    };

    target.onerror = () => finish({ kind: 'crashed' }, true);
    target.onmessage = ({ data }) => {
      if (data.id !== id) return;
      if (data.type === 'crashed') {
        finish({ kind: 'crashed' }, true);
      } else if (data.type === 'compiled') {
        if (data.compiled.ok) {
          compiled = data.compiled;
          giveUpAfter(RUN_SECONDS, { kind: 'timeout', seconds: RUN_SECONDS });
        } else {
          finish({ kind: 'compile-error', errors: data.compiled.errors });
        }
      } else if (data.type === 'finished') {
        finish({
          kind: 'ran',
          output: data.run.output,
          compileMs: compiled?.ms ?? 0,
          runMs: data.run.ms,
          truncated: Boolean(data.run.truncated),
          exception: data.run.exception,
        });
      }
    };

    giveUpAfter(COMPILE_SECONDS, { kind: 'timeout', seconds: COMPILE_SECONDS });
    target.postMessage({ id, code, input });
  });
}

export async function runProgram(code: string, input: string): Promise<RunResult> {
  if (state.busy) throw new Error('A program is already running.');
  setState({ busy: true });
  try {
    let target: Worker;
    try {
      target = await startWorker();
    } catch (error) {
      return { kind: 'unavailable', message: error instanceof Error ? error.message : String(error) };
    }
    return await execute(target, code, input);
  } finally {
    setState({ busy: false });
  }
}

export type OutputComparison = { matches: true } | { matches: false; line: number; target: string; actual: string };

/** Compares output the way scripts/verify-worksheets.mjs does: trailing spaces and blank ends do not count. */
export function compareOutput(actual: string, target: string): OutputComparison {
  const lines = (text: string) =>
    text.replace(/\r\n/g, '\n').split('\n').map((line) => line.trimEnd()).join('\n').trim().split('\n');
  const actualLines = lines(actual);
  const targetLines = lines(target);
  for (let i = 0; i < Math.max(actualLines.length, targetLines.length); i++) {
    if (actualLines[i] !== targetLines[i]) {
      return { matches: false, line: i + 1, target: targetLines[i] ?? '', actual: actualLines[i] ?? '' };
    }
  }
  return { matches: true };
}
