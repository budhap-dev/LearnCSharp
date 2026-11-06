/*
 * Service worker - makes the site installable as an app and usable offline.
 *
 * Deliberately small and hand-written rather than generated:
 *   - Pages: network first, so a new deploy is picked up as soon as there is a signal;
 *     the last good copy of the app shell is served when offline.
 *   - /assets/*: cache first. Vite content-hashes these names, so a cached copy is never stale.
 *   - /runner/<hash>/*: the C# compiler. Cache first for the same reason, in its own cache so an
 *     older version's large files can be dropped as soon as a newer one is in use.
 *   - Everything else (/data/*, icons, manifest): served from cache if present while a fresh
 *     copy is fetched in the background for next time.
 * Lessons are cached as they are visited, so anything already opened works offline.
 *
 * Bump VERSION to throw away every cached file on the next visit.
 */
const VERSION = 'v1';
const SHELL = `shell-${VERSION}`;
const RUNTIME = `runtime-${VERSION}`;
const RUNNER_PREFIX = 'runner-';

self.addEventListener('install', (event) => {
  event.waitUntil(
    caches
      .open(SHELL)
      .then((cache) => cache.addAll(['/', '/manifest.webmanifest', '/favicon.svg']))
      .then(() => self.skipWaiting()),
  );
});

self.addEventListener('activate', (event) => {
  event.waitUntil(
    caches
      .keys()
      .then((keys) => Promise.all(keys.filter((k) => k !== SHELL && k !== RUNTIME && !k.startsWith(RUNNER_PREFIX)).map((k) => caches.delete(k))))
      .then(() => self.clients.claim()),
  );
});

self.addEventListener('fetch', (event) => {
  const { request } = event;
  const url = new URL(request.url);
  if (request.method !== 'GET' || url.origin !== self.location.origin) return;

  if (request.mode === 'navigate') {
    // Every route is the same index.html (see navigationFallback), so one cached copy serves all.
    event.respondWith(
      fetch(request)
        .then((response) => {
          if (response.ok) {
            const copy = response.clone();
            caches.open(SHELL).then((cache) => cache.put('/', copy));
          }
          return response;
        })
        .catch(() => caches.match('/')),
    );
    return;
  }

  if (url.pathname.startsWith('/runner/')) {
    event.respondWith(fromRunnerCache(request, url));
    return;
  }

  if (url.pathname.startsWith('/assets/')) {
    event.respondWith(
      caches.match(request).then(
        (cached) =>
          cached ||
          fetch(request).then((response) => {
            if (response.ok) {
              const copy = response.clone();
              caches.open(RUNTIME).then((cache) => cache.put(request, copy));
            }
            return response;
          }),
      ),
    );
    return;
  }

  event.respondWith(
    caches.match(request).then((cached) => {
      const fresh = fetch(request)
        .then((response) => {
          if (response.ok) {
            const copy = response.clone();
            caches.open(RUNTIME).then((cache) => cache.put(request, copy));
          }
          return response;
        })
        .catch(() => cached);
      return cached || fresh;
    }),
  );
});

async function fromRunnerCache(request, url) {
  const name = RUNNER_PREFIX + url.pathname.split('/')[2];
  const cache = await caches.open(name);
  const cached = await cache.match(request);
  if (cached) return cached;
  const response = await fetch(request);
  if (response.ok) {
    await cache.put(request, response.clone());
    const stale = (await caches.keys()).filter((k) => k.startsWith(RUNNER_PREFIX) && k !== name);
    await Promise.all(stale.map((k) => caches.delete(k)));
  }
  return response;
}
