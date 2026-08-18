/**
 * Small celebration helpers: a dependency-free confetti burst and encouraging copy. All motion
 * respects prefers-reduced-motion. Colours match the module palette in index.css.
 */
const CONFETTI_COLORS = ['#7c5cff', '#2b8fd9', '#17b0a6', '#f0883e', '#e05264', '#2e9e5b', '#ffb703'];

export function confetti(count = 90): void {
  if (window.matchMedia?.('(prefers-reduced-motion: reduce)').matches) return;

  for (let i = 0; i < count; i++) {
    const el = document.createElement('div');
    el.className = 'confetti-piece';
    el.style.left = `${Math.random() * 100}vw`;
    el.style.background = CONFETTI_COLORS[i % CONFETTI_COLORS.length];
    const duration = 2 + Math.random() * 1.6;
    const delay = Math.random() * 0.5;
    el.style.animation = `confetti-fall ${duration}s ${delay}s ease-in forwards`;
    el.style.rotate = `${Math.random() * 360}deg`;
    document.body.appendChild(el);
    window.setTimeout(() => el.remove(), (duration + delay) * 1000 + 250);
  }
}

export function quizMessage(percent: number): { emoji: string; text: string } {
  if (percent === 100) return { emoji: '🏆', text: 'Perfect score! You nailed every one.' };
  if (percent >= 80) return { emoji: '🎉', text: 'Brilliant — lesson complete!' };
  if (percent >= 60) return { emoji: '👍', text: 'Nice work. A quick review and you have this.' };
  if (percent >= 40) return { emoji: '💪', text: 'Getting there — another read, then try again.' };
  return { emoji: '🌱', text: 'Every expert started here. Re-read and give it another go.' };
}

export function examMessage(grade: string): string {
  const map: Record<string, string> = {
    Distinction: 'Outstanding! 🏆',
    Merit: 'Great result! 🥈',
    Pass: 'Passed — solid work. ✅',
    'Not yet': 'Not yet — but closer than you think. 💪',
  };
  return map[grade] ?? '';
}

export function homeMessage(done: number, total: number): { emoji: string; text: string } {
  if (done === 0) return { emoji: '🌱', text: 'Your C# journey starts here.' };
  if (done >= total) return { emoji: '🏆', text: 'Course complete — you legend!' };
  const pct = Math.round((done / total) * 100);
  if (pct >= 75) return { emoji: '🔥', text: `Almost there — ${done} of ${total} done!` };
  if (pct >= 40) return { emoji: '🚀', text: `Great momentum — ${done} of ${total} lessons done.` };
  return { emoji: '✨', text: `You are on your way — ${done} of ${total} done.` };
}
