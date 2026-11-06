import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import 'highlight.js/styles/github.css';
import './index.css';
import App from './App';
import { syncThemeColor } from './lib/theme';

syncThemeColor();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
);

// Production only: in dev a caching service worker would hide Vite's hot reloads.
if (import.meta.env.PROD && 'serviceWorker' in navigator) {
  window.addEventListener('load', () => {
    navigator.serviceWorker.register('/sw.js').catch(() => {
      /* the site works without it - it just won't install or work offline */
    });
  });
}
