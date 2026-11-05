// Runs the .NET runtime and C# compiler off the main thread, so a student's infinite loop can be
// stopped by terminating this worker without ever freezing the page. Driven by web/src/lib/runner.ts.
import { dotnet } from './_framework/dotnet.js';

try {
  const { getAssemblyExports, getConfig } = await dotnet
    .withModuleConfig({
      onDownloadResourceProgress: (done, total) => self.postMessage({ type: 'progress', done, total }),
    })
    .create();
  const { LearnCSharp } = await getAssemblyExports(getConfig().mainAssemblyName);
  const { Runner } = LearnCSharp.Runner;

  const references = await fetch(new URL('refs.zip', import.meta.url));
  if (!references.ok) throw new Error(`refs.zip: HTTP ${references.status}`);
  Runner.Initialise(new Uint8Array(await references.arrayBuffer()));

  self.onmessage = async ({ data: { id, code, input } }) => {
    try {
      const compiled = JSON.parse(Runner.Compile(code));
      self.postMessage({ type: 'compiled', id, compiled });
      if (!compiled.ok) return;
      const run = JSON.parse(await Runner.Execute(input));
      self.postMessage({ type: 'finished', id, run });
    } catch (error) {
      // A stack overflow kills the runtime itself, which surfaces here.
      self.postMessage({ type: 'crashed', id, message: String(error?.message ?? error) });
    }
  };
  self.postMessage({ type: 'ready' });
} catch (error) {
  self.postMessage({ type: 'load-failed', message: String(error?.message ?? error) });
}
