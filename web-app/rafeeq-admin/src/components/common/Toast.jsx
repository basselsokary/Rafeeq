import React, { createContext, useContext, useState, useCallback } from 'react';

const ToastContext = createContext(null);

let _id = 0;

export function ToastProvider({ children }) {
  const [toasts, setToasts] = useState([]);

  const toast = useCallback((message, type = 'info') => {
    const id = ++_id;
    setToasts((t) => [...t, { id, message, type }]);
    setTimeout(() => setToasts((t) => t.filter((x) => x.id !== id)), 3500);
  }, []);

  const remove = (id) => setToasts((t) => t.filter((x) => x.id !== id));

  const ICONS = {
    success: (
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#22c55e" strokeWidth="2.5">
        <path d="M20 6L9 17l-5-5"/>
      </svg>
    ),
    error: (
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#ef4444" strokeWidth="2.5">
        <circle cx="12" cy="12" r="10"/><path d="M12 8v4m0 4h.01"/>
      </svg>
    ),
    info: (
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#3b82f6" strokeWidth="2.5">
        <circle cx="12" cy="12" r="10"/><path d="M12 16v-4m0-4h.01"/>
      </svg>
    ),
  };

  const COLOR = {
    success: { border: 'rgba(34,197,94,0.3)',  bg: 'rgba(34,197,94,0.08)' },
    error:   { border: 'rgba(239,68,68,0.3)',  bg: 'rgba(239,68,68,0.08)' },
    info:    { border: 'rgba(59,130,246,0.3)', bg: 'rgba(59,130,246,0.08)' },
  };

  return (
    <ToastContext.Provider value={toast}>
      {children}
      <div style={{
        position: 'fixed', bottom: 24, right: 24,
        display: 'flex', flexDirection: 'column', gap: 10, zIndex: 9999,
      }}>
        {toasts.map((t) => (
          <div
            key={t.id}
            onClick={() => remove(t.id)}
            style={{
              display: 'flex', alignItems: 'center', gap: 10,
              padding: '12px 16px',
              background: 'var(--bg-card)',
              border: `1px solid ${(COLOR[t.type] || COLOR.info).border}`,
              borderRadius: 'var(--radius-md)',
              boxShadow: 'var(--shadow-card)',
              cursor: 'pointer', minWidth: 240, maxWidth: 360,
              animation: 'slideUp 0.2s ease',
            }}
          >
            {ICONS[t.type] || ICONS.info}
            <span style={{ fontSize: 13, color: 'var(--text-primary)', flex: 1 }}>
              {t.message}
            </span>
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  );
}

export const useToast = () => useContext(ToastContext);
