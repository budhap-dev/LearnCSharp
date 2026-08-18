import { useEffect, useRef, useState } from 'react';

/**
 * A one-second countdown. Runs only while `active`, calls `onExpire` exactly once at zero, and
 * survives re-renders (it captures the latest onExpire via a ref, so submitting with current
 * answers works). Anchored to a wall-clock end time so a throttled tab still expires on schedule.
 */
export function useCountdown(totalSeconds: number, active: boolean, onExpire: () => void): number {
  const [remaining, setRemaining] = useState(totalSeconds);
  const firedRef = useRef(false);
  const onExpireRef = useRef(onExpire);
  onExpireRef.current = onExpire;

  useEffect(() => {
    if (!active) return;

    firedRef.current = false;
    setRemaining(totalSeconds);
    const endAt = Date.now() + totalSeconds * 1000;

    const tick = () => {
      const left = Math.max(0, Math.round((endAt - Date.now()) / 1000));
      setRemaining(left);
      if (left <= 0 && !firedRef.current) {
        firedRef.current = true;
        clearInterval(id);
        onExpireRef.current();
      }
    };

    const id = setInterval(tick, 250);
    tick();
    return () => clearInterval(id);
  }, [active, totalSeconds]);

  return remaining;
}

export function formatTime(seconds: number): string {
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${m}:${String(s).padStart(2, '0')}`;
}
