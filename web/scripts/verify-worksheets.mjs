/**
 * Proves the worksheets are honest: every starter compiles, and every solution - fed the
 * task's input - prints exactly the expected output. Runs each program through a real
 * `dotnet run` in a scratch console project, so what students see is what the code does.
 *
 *   node scripts/verify-worksheets.mjs            # all tasks
 *   node scripts/verify-worksheets.mjs 1.3 2.1    # just these
 */
import { spawnSync } from 'node:child_process';
import { mkdtempSync, writeFileSync, rmSync, mkdirSync } from 'node:fs';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { WORKSHEETS } from '../src/data/worksheets/index.js';

const only = new Set(process.argv.slice(2));
const dir = mkdtempSync(join(tmpdir(), 'lcs-worksheets-'));
mkdirSync(join(dir, 'app'));
writeFileSync(
  join(dir, 'app', 'app.csproj'),
  `<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <SatelliteResourceLanguages>en</SatelliteResourceLanguages>
  </PropertyGroup>
</Project>
`,
);

const program = join(dir, 'app', 'Program.cs');
const dll = join(dir, 'app', 'bin', 'Release', 'net10.0', 'app.dll');

function build(source) {
  writeFileSync(program, source);
  const r = spawnSync('dotnet', ['build', join(dir, 'app'), '-c', 'Release', '-nologo', '-v', 'q', '--property:WarningLevel=0'], {
    encoding: 'utf8',
  });
  return r.status === 0 ? null : (r.stdout + r.stderr).split('\n').filter((l) => l.includes('error')).join('\n');
}

function run(input) {
  const r = spawnSync('dotnet', [dll], { encoding: 'utf8', input: input ?? '', timeout: 20_000, cwd: join(dir, 'app') });
  return { out: r.stdout, err: r.stderr, status: r.status };
}

const norm = (s) => s.replace(/\r\n/g, '\n').split('\n').map((l) => l.trimEnd()).join('\n').trim();

let failed = 0;
let checked = 0;
for (const ws of WORKSHEETS) {
  for (const t of ws.tasks) {
    if (only.size && !only.has(t.id)) continue;
    checked++;
    const label = `${t.id} ${t.title}`;

    const starterErr = build(t.starter);
    if (starterErr) {
      failed++;
      console.log(`✗ ${label}: starter does not compile\n${starterErr}`);
      continue;
    }

    const solErr = build(t.solution);
    if (solErr) {
      failed++;
      console.log(`✗ ${label}: solution does not compile\n${solErr}`);
      continue;
    }

    const { out, err, status } = run(t.input);
    if (status !== 0) {
      failed++;
      console.log(`✗ ${label}: solution exited ${status}\n${err}`);
      continue;
    }
    if (norm(out) !== norm(t.expected)) {
      failed++;
      console.log(`✗ ${label}: output differs\n--- expected\n${norm(t.expected)}\n--- actual\n${norm(out)}\n---`);
      continue;
    }
    console.log(`✓ ${label}`);
  }
}

rmSync(dir, { recursive: true, force: true });
console.log(`\n${checked - failed}/${checked} worksheet tasks verified`);
process.exit(failed ? 1 : 0);
