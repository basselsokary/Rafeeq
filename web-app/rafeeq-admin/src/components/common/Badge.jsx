import React from 'react';

const COLOR_MAP = {
  green:  { bg: 'rgba(34,197,94,0.12)',  border: 'rgba(34,197,94,0.3)',  text: '#22c55e' },
  yellow: { bg: 'rgba(245,158,11,0.12)', border: 'rgba(245,158,11,0.3)', text: '#f59e0b' },
  red:    { bg: 'rgba(239,68,68,0.12)',  border: 'rgba(239,68,68,0.3)',  text: '#ef4444' },
  gray:   { bg: 'rgba(107,114,128,0.15)',border: 'rgba(107,114,128,0.3)',text: '#9ca3af' },
  blue:   { bg: 'rgba(59,130,246,0.12)', border: 'rgba(59,130,246,0.3)', text: '#3b82f6' },
  teal:   { bg: 'rgba(20,184,166,0.12)', border: 'rgba(20,184,166,0.3)', text: '#14b8a6' },
  gold:   { bg: 'rgba(245,166,35,0.12)', border: 'rgba(245,166,35,0.3)', text: '#f5a623' },
};

export default function Badge({ color = 'gray', children }) {
  const c = COLOR_MAP[color] || COLOR_MAP.gray;
  return (
    <span style={{
      display: 'inline-flex',
      alignItems: 'center',
      gap: 5,
      padding: '3px 10px',
      borderRadius: 20,
      fontSize: 11,
      fontWeight: 600,
      letterSpacing: '0.04em',
      textTransform: 'uppercase',
      background: c.bg,
      border: `1px solid ${c.border}`,
      color: c.text,
      whiteSpace: 'nowrap',
    }}>
      <span style={{
        width: 5, height: 5, borderRadius: '50%',
        background: c.text, flexShrink: 0,
      }} />
      {children}
    </span>
  );
}
