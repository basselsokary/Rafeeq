import React from 'react';

export default function Spinner({ size = 32, center = false }) {
  const el = (
    <svg
      width={size} height={size} viewBox="0 0 24 24"
      fill="none" stroke="var(--gold)"
      strokeWidth="2.5" strokeLinecap="round"
      style={{ animation: 'spin 0.7s linear infinite' }}
    >
      <style>{`@keyframes spin { to { transform: rotate(360deg); } }`}</style>
      <circle cx="12" cy="12" r="9" stroke="var(--border)" />
      <path d="M12 3 A9 9 0 0 1 21 12" />
    </svg>
  );

  if (center) {
    return (
      <div style={{
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        padding: '60px 20px',
      }}>
        {el}
      </div>
    );
  }
  return el;
}
