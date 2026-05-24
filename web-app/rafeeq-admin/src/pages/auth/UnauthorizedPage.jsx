import React from 'react';
import { useNavigate } from 'react-router-dom';

export default function UnauthorizedPage() {
  const navigate = useNavigate();

  return (
    <div style={{
      minHeight: '100vh', background: 'var(--background)',
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      fontFamily: 'var(--font-body)',
    }}>
      <div style={{ textAlign: 'center', maxWidth: 400, padding: '0 24px' }}>

        {/* Icon */}
        <div style={{
          width: 72, height: 72, borderRadius: '50%', margin: '0 auto 24px',
          background: 'var(--red-bg)',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
          <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="var(--error)" strokeWidth="2">
            <circle cx="12" cy="12" r="10"/>
            <line x1="12" y1="8" x2="12" y2="12"/>
            <line x1="12" y1="16" x2="12.01" y2="16"/>
          </svg>
        </div>

        <h1 style={{ fontFamily: 'var(--font-display)', fontSize: 28, fontWeight: 700, color: 'var(--text)', marginBottom: 10 }}>
          Access Denied
        </h1>
        <p style={{ fontSize: 14, color: 'var(--text-muted)', lineHeight: 1.7, marginBottom: 28 }}>
          You don't have permission to view this page. Contact your administrator if you think this is a mistake.
        </p>

        <div style={{ display: 'flex', gap: 10, justifyContent: 'center' }}>
          <button onClick={() => navigate(-1)} style={{
            padding: '9px 20px', borderRadius: 10, border: '1px solid var(--outline-variant)',
            background: 'var(--surface-container-lowest)', color: 'var(--text-2)',
            fontSize: 13, fontWeight: 600, cursor: 'pointer', fontFamily: 'var(--font-body)',
          }}>
            Go Back
          </button>
          <button onClick={() => navigate('/dashboard')} style={{
            padding: '9px 20px', borderRadius: 10, border: 'none',
            background: 'linear-gradient(135deg, var(--primary), var(--primary-container))',
            color: '#fff', fontSize: 13, fontWeight: 700, cursor: 'pointer',
            fontFamily: 'var(--font-body)',
            boxShadow: '0 2px 8px rgba(124,87,45,.25)',
          }}>
            Go to Dashboard
          </button>
        </div>
      </div>
    </div>
  );
}