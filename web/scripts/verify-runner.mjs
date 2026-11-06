/**
 * Proves the in-browser runner gives students the right answers. verify-worksheets.mjs checks the
 * worksheets against real `dotnet run`; this runs the same programs through the runtime and compiler
 * the site actually ships (under Node, which uses the same WebAssembly build), so a
 * browser-only break stops the deployment instead of reaching students.
 *
 *   node scripts/verify-runner.mjs
 */
import { readFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath, pathToFileURL } from 'node:url';
import { WORKSHEETS } from '../src/data/worksheets/index.js';

const web = join(dirname(fileURLToPath(import.meta.url)), '..');
const path = readFileSync(join(web, 'src/lib/runnerPath.generated.ts'), 'utf8').match(/'(\/runner\/[^']+)'/)[1];
const root = join(web, 'public', path);

const { dotnet } = await import(pathToFileURL(join(root, '_framework/dotnet.js')).href);
const { getAssemblyExports, getConfig } = await dotnet.create();
const { Runner } = (await getAssemblyExports(getConfig().mainAssemblyName)).LearnCSharp.Runner;
Runner.Initialise(new Uint8Array(readFileSync(join(root, 'refs.zip'))));

async function run(code, input = '') {
  const compiled = JSON.parse(Runner.Compile(code));
  return compiled.ok ? { compiled, ...JSON.parse(await Runner.Execute(input)) } : { compiled };
}

const norm = (s) => s.replace(/\r\n/g, '\n').split('\n').map((l) => l.trimEnd()).join('\n').trim();

let checked = 0;
const failures = [];
function check(label, problem) {
  checked++;
  if (problem) failures.push(`✗ ${label}: ${problem}`);
}

for (const task of WORKSHEETS.flatMap((w) => w.tasks)) {
  const starter = JSON.parse(Runner.Compile(task.starter));
  check(`${task.id} starter`, starter.ok ? null : starter.errors[0]);

  const { compiled, exception, output } = await run(task.solution, task.input ?? '');
  let problem = null;
  if (!compiled.ok) problem = compiled.errors[0];
  else if (exception) problem = `${exception.type}: ${exception.message}`;
  else if (norm(output) !== norm(task.expected)) problem = `expected\n${norm(task.expected)}\nbut got\n${norm(output)}`;
  check(`${task.id} solution`, problem);
}

// Behaviour the page depends on, and that a framework or SDK upgrade could quietly change.
const thrown = await run('int[] a = new int[1];\nConsole.WriteLine("before");\na[5] = 1;');
check(
  'runtime errors keep their real message',
  thrown.exception?.message === 'Index was outside the bounds of the array.' ? null : JSON.stringify(thrown.exception),
);
check('output before a crash is kept', norm(thrown.output) === 'before' ? null : thrown.output);

const broken = await run('int x = "hello";');
check('compile errors name their line', broken.compiled.errors[0]?.startsWith('Line 1: ') ? null : broken.compiled.errors[0]);

const asyncProgram = await run('await Task.Delay(1);\nConsole.WriteLine("ok");');
check('await at the top level', norm(asyncProgram.output ?? '') === 'ok' ? null : JSON.stringify(asyncProgram));

const asyncMain = await run('class P { static async Task Main() { await Task.Delay(1); Console.WriteLine("ok"); } }');
check('async Main', norm(asyncMain.output ?? '') === 'ok' ? null : JSON.stringify(asyncMain));

const currency = await run('Console.WriteLine($"{12.5:C}");');
check('currency is in pounds', norm(currency.output ?? '') === '£12.50' ? null : currency.output);

const chatty = await run('while (true) Console.WriteLine("again");');
check('endless printing is cut off', chatty.truncated ? null : 'it was not stopped');

console.log(failures.join('\n'));
console.log(`${checked - failures.length}/${checked} runner checks passed`);
process.exit(failures.length ? 1 : 0);
