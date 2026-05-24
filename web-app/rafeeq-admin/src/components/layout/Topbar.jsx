import React from 'react';
import { useAuth } from '../../context/AuthContext.jsx';
import { useTheme } from '../../context/ThemeContext.jsx';

export default function Topbar() {
  const { user } = useAuth();
  const { isDark, toggleTheme } = useTheme();
  const initial  = user?.userName?.[0]?.toUpperCase() ?? '…';

  const now     = new Date();
  const timeStr = now.toLocaleTimeString('en-EG', { hour: '2-digit', minute: '2-digit' });
  const dateStr = now.toLocaleDateString('en-EG', { month: 'short', day: 'numeric' });

  return (
    <header style={{
      background: 'var(--topbar-bg)', backdropFilter: 'blur(12px)',
      borderBottom: '1px solid var(--topbar-border)',
      padding: '0 32px', height: 64,
      display: 'flex', alignItems: 'center', gap: 20,
      position: 'sticky', top: 0, zIndex: 50,
      flexShrink: 0,
    }}>

      {/* Search */}
      {/* <div style={{ flex: 1, maxWidth: 440, position: 'relative' }}>
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="var(--outline)" strokeWidth="2"
          style={{ position: 'absolute', left: 14, top: '50%', transform: 'translateY(-50%)', pointerEvents: 'none' }}>
          <circle cx="11" cy="11" r="8"/><path d="M21 21l-4.35-4.35"/>
        </svg>
        <input
          placeholder="Search Sites, Users, Reviews…"
          style={{
            width: '100%', padding: '8px 14px 8px 38px',
            background: 'var(--surface-container-low)',
            border: 'none', borderRadius: 999,
            fontSize: 13, color: 'var(--text)', outline: 'none',
          }}
        />
      </div> */}

      <div style={{ flex: 1 }} />

      {/* Right side */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>

        {/* Notification bell */}
        {/* <button style={{ position: 'relative', background: 'none', border: 'none', cursor: 'pointer', padding: 6, borderRadius: 20 }}>
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="var(--outline)" strokeWidth="2">
            <path d="M18 8A6 6 0 006 8c0 7-3 9-3 9h18s-3-2-3-9"/>
            <path d="M13.73 21a2 2 0 01-3.46 0"/>
          </svg>
        </button> */}

        <button onClick={toggleTheme} title="Toggle theme" style={{ background:'none', border:'none', cursor:'pointer', padding:6, borderRadius:20 }}>
          {isDark
            ? <span aria-hidden style={{fontSize:16}}>🌙</span>
            : <span aria-hidden style={{fontSize:16}}>☀️</span>
          }
        </button>

        {/* Clock */}
        <div style={{ textAlign: 'right' }}>
          <div style={{ fontSize: 11, fontWeight: 800, color: 'var(--text)' }}>Cairo, EG</div>
          <div style={{ fontSize: 10, color: 'var(--outline)' }}>{timeStr} • {dateStr}</div>
        </div>

        {/* User avatar */}
        <div title={user?.userName ?? ''} style={{
          width: 36, height: 36, borderRadius: '50%', flexShrink: 0,
          background: 'linear-gradient(135deg, var(--primary), var(--primary-container))',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          color: '#fff', fontSize: 14, fontWeight: 800,
          border: '2px solid var(--primary-fixed)',
          cursor: 'pointer',
        }}>
          {initial}
        </div>

      </div>
    </header>
  );
}