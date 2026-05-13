import React, { useEffect } from 'react';

export default function Modal({ open, onClose, title, children, width = 560 }) {
  useEffect(() => {
    const onKey = (e) => e.key === 'Escape' && onClose();
    if (open) document.addEventListener('keydown', onKey);
    return () => document.removeEventListener('keydown', onKey);
  }, [open, onClose]);

  if (!open) return null;

  return (
    <div
      onClick={onClose}
      style={{
        position: 'fixed', inset: 0, zIndex: 1000,
        background: 'rgba(0,0,0,0.7)', backdropFilter: 'blur(4px)',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        padding: 16,
        animation: 'fadeIn 0.15s ease',
      }}
    >
      <style>{`@keyframes fadeIn { from { opacity:0 } to { opacity:1 } }
        @keyframes slideUp { from { transform:translateY(16px); opacity:0 } to { transform:translateY(0); opacity:1 } }`}</style>
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: 'var(--bg-surface)',
          border: '1px solid var(--border)',
          borderRadius: 'var(--radius-lg)',
          width: '100%', maxWidth: width,
          maxHeight: '90vh', display: 'flex', flexDirection: 'column',
          boxShadow: 'var(--shadow-modal)',
          animation: 'slideUp 0.2s ease',
        }}
      >
        {/* Header */}
        <div style={{
          display: 'flex', alignItems: 'center', justifyContent: 'space-between',
          padding: '18px 24px',
          borderBottom: '1px solid var(--border)',
          flexShrink: 0,
        }}>
          <h2 style={{
            fontFamily: 'var(--font-display)',
            fontSize: 18, fontWeight: 600,
            color: 'var(--text)',
          }}>
            {title}
          </h2>
          <button
            onClick={onClose}
            style={{
              background: 'none', border: 'none', cursor: 'pointer',
              color: 'var(--text-muted)', padding: 4, borderRadius: 4,
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              transition: 'color var(--transition)',
            }}
            onMouseOver={(e) => e.currentTarget.style.color = 'var(--text)'}
            onMouseOut={(e) => e.currentTarget.style.color = 'var(--text-muted)'}
          >
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <path d="M18 6L6 18M6 6l12 12"/>
            </svg>
          </button>
        </div>

        {/* Body */}
        <div style={{ padding: '24px', overflowY: 'auto', flex: 1 }}>
          {children}
        </div>
      </div>
    </div>
  );
}
