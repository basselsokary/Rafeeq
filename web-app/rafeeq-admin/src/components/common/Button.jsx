import React from 'react';
import Spinner from './Spinner';

const VARIANTS = {
  primary: {
    background: 'var(--gold)',
    color: '#0b0f1a',
    border: '1px solid var(--gold)',
    hoverBg: 'var(--gold-dim)',
  },
  secondary: {
    background: 'var(--bg-elevated)',
    color: 'var(--text-primary)',
    border: '1px solid var(--border)',
    hoverBg: 'var(--bg-card)',
  },
  danger: {
    background: 'rgba(239,68,68,0.1)',
    color: '#ef4444',
    border: '1px solid rgba(239,68,68,0.3)',
    hoverBg: 'rgba(239,68,68,0.18)',
  },
  ghost: {
    background: 'transparent',
    color: 'var(--text-secondary)',
    border: '1px solid transparent',
    hoverBg: 'var(--bg-elevated)',
  },
};

export default function Button({
  children, onClick, variant = 'primary',
  size = 'md', loading = false, disabled = false,
  type = 'button', fullWidth = false, icon = null,
  style: extraStyle = {},
}) {
  const v = VARIANTS[variant] || VARIANTS.primary;
  const pad = size === 'sm' ? '6px 12px' : size === 'lg' ? '11px 22px' : '8px 18px';
  const fz  = size === 'sm' ? 12 : size === 'lg' ? 15 : 13;

  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled || loading}
      style={{
        display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
        gap: 7, padding: pad, fontSize: fz, fontWeight: 600,
        fontFamily: 'var(--font-body)', letterSpacing: '0.02em',
        borderRadius: 'var(--radius-sm)', cursor: disabled || loading ? 'not-allowed' : 'pointer',
        opacity: disabled ? 0.5 : 1,
        transition: 'background var(--transition), opacity var(--transition)',
        width: fullWidth ? '100%' : undefined,
        whiteSpace: 'nowrap',
        ...v,
        ...extraStyle,
      }}
      onMouseOver={(e) => {
        if (!disabled && !loading) e.currentTarget.style.background = v.hoverBg;
      }}
      onMouseOut={(e) => {
        e.currentTarget.style.background = v.background;
      }}
    >
      {loading ? <Spinner size={14} /> : icon}
      {children}
    </button>
  );
}
