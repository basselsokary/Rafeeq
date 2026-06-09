import React, { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { changePassword } from '../../api/authApi';
import { useToast } from '../../components/common/Toast';
import Spinner from '../../components/common/Spinner';

/* ─────────────────────────────────────────────────────────
   Password strength scorer
───────────────────────────────────────────────────────── */
function scorePassword(pw) {
  if (!pw) return 0;
  let score = 0;
  if (pw.length >= 8)  score++;
  if (pw.length >= 12) score++;
  if (/[A-Z]/.test(pw)) score++;
  if (/[0-9]/.test(pw)) score++;
  if (/[^A-Za-z0-9]/.test(pw)) score++;
  return score; // 0–5
}

const STRENGTH = [
  { label: '',          color: 'transparent',       bg: 'var(--surface-container)' },
  { label: 'Very Weak', color: '#ba1a1a',            bg: '#ba1a1a' },
  { label: 'Weak',      color: '#d97706',            bg: '#d97706' },
  { label: 'Fair',      color: '#ca8a04',            bg: '#ca8a04' },
  { label: 'Strong',    color: '#16a34a',            bg: '#16a34a' },
  { label: 'Very Strong',color: '#15803d',           bg: '#15803d' },
];

function StrengthBar({ password }) {
  const score = scorePassword(password);
  if (!password) return null;
  const s = STRENGTH[score];
  return (
    <div style={{ marginTop: 8 }}>
      <div style={{ display: 'flex', gap: 4, marginBottom: 5 }}>
        {[1, 2, 3, 4, 5].map(i => (
          <div key={i} style={{
            flex: 1, height: 4, borderRadius: 2,
            background: i <= score ? s.bg : 'var(--surface-container-high)',
            transition: 'background .25s',
          }} />
        ))}
      </div>
      <div style={{ fontSize: 11, fontWeight: 600, color: s.color }}>{s.label}</div>
    </div>
  );
}

/* ─────────────────────────────────────────────────────────
   Password input with show/hide toggle
───────────────────────────────────────────────────────── */
function PasswordInput({ value, onChange, placeholder, id }) {
  const [visible, setVisible] = useState(false);
  return (
    <div style={{ position: 'relative' }}>
      <input
        id={id}
        type={visible ? 'text' : 'password'}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        style={{
          width: '100%', padding: '10px 44px 10px 14px',
          background: 'var(--surface-container-low)',
          border: '1px solid var(--border)',
          borderRadius: 'var(--radius-lg)',
          fontSize: 14, color: 'var(--text)',
          outline: 'none', fontFamily: 'var(--font-body)',
          transition: 'border-color .15s, box-shadow .15s',
        }}
        onFocus={e => { e.target.style.borderColor = 'var(--primary)'; e.target.style.boxShadow = '0 0 0 2px rgba(124,87,45,.12)'; }}
        onBlur={e => { e.target.style.borderColor = 'var(--border)'; e.target.style.boxShadow = 'none'; }}
      />
      <button
        type="button"
        onClick={() => setVisible(v => !v)}
        style={{
          position: 'absolute', right: 12, top: '50%', transform: 'translateY(-50%)',
          background: 'none', border: 'none', cursor: 'pointer',
          color: 'var(--outline)', display: 'flex', padding: 2,
          transition: 'color .15s',
        }}
        onMouseOver={e => e.currentTarget.style.color = 'var(--primary)'}
        onMouseOut={e => e.currentTarget.style.color = 'var(--outline)'}
        tabIndex={-1}
      >
        {visible ? (
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M17.94 17.94A10.07 10.07 0 0112 20c-7 0-11-8-11-8a18.45 18.45 0 015.06-5.94"/>
            <path d="M9.9 4.24A9.12 9.12 0 0112 4c7 0 11 8 11 8a18.5 18.5 0 01-2.16 3.19"/>
            <line x1="1" y1="1" x2="23" y2="23"/>
          </svg>
        ) : (
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
            <circle cx="12" cy="12" r="3"/>
          </svg>
        )}
      </button>
    </div>
  );
}

/* ─────────────────────────────────────────────────────────
   Section card wrapper
───────────────────────────────────────────────────────── */
function Section({ title, subtitle, icon, children }) {
  return (
    <div style={{
      background: 'var(--surface-container-lowest)',
      borderRadius: 'var(--radius-xl)',
      overflow: 'hidden',
      boxShadow: '0 1px 4px rgba(29,27,23,.05)',
    }}>
      <div style={{
        padding: '18px 22px 16px',
        borderBottom: '1px solid var(--border)',
        display: 'flex', alignItems: 'center', gap: 12,
      }}>
        <div style={{
          width: 36, height: 36, borderRadius: 10, flexShrink: 0,
          background: 'rgba(124,87,45,0.08)',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          color: 'var(--primary)',
        }}>{icon}</div>
        <div>
          <div style={{ fontSize: 14, fontWeight: 700, color: 'var(--text)' }}>{title}</div>
          {subtitle && <div style={{ fontSize: 12, color: 'var(--outline)', marginTop: 1 }}>{subtitle}</div>}
        </div>
      </div>
      <div style={{ padding: '20px 22px' }}>
        {children}
      </div>
    </div>
  );
}

/* ─────────────────────────────────────────────────────────
   Info row
───────────────────────────────────────────────────────── */
function InfoRow({ label, value, copyable }) {
  const toast = useToast();
  const copy = () => {
    navigator.clipboard.writeText(String(value));
    toast('Copied to clipboard', 'success');
  };
  return (
    <div style={{
      display: 'flex', justifyContent: 'space-between', alignItems: 'center',
      padding: '10px 0',
      borderBottom: '1px solid rgba(212,196,183,.2)',
    }}>
      <span style={{ fontSize: 11, fontWeight: 700, letterSpacing: '0.07em', textTransform: 'uppercase', color: 'var(--outline)' }}>
        {label}
      </span>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
        <span style={{ fontSize: 13, color: 'var(--text)', fontWeight: 500, maxWidth: 240, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
          {value ?? '—'}
        </span>
        {copyable && value && (
          <button onClick={copy} style={{
            background: 'none', border: 'none', cursor: 'pointer',
            color: 'var(--outline)', display: 'flex', padding: 2,
            borderRadius: 4, transition: 'color .15s',
          }}
            onMouseOver={e => e.currentTarget.style.color = 'var(--primary)'}
            onMouseOut={e => e.currentTarget.style.color = 'var(--outline)'}
            title="Copy to clipboard"
          >
            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <rect x="9" y="9" width="13" height="13" rx="2" ry="2"/>
              <path d="M5 15H4a2 2 0 01-2-2V4a2 2 0 012-2h9a2 2 0 012 2v1"/>
            </svg>
          </button>
        )}
      </div>
    </div>
  );
}

/* ─────────────────────────────────────────────────────────
   Change password form
───────────────────────────────────────────────────────── */
function ChangePasswordForm() {
  const toast = useToast();
  const [form, setForm] = useState({ currentPassword: '', newPassword: '', confirmPassword: '' });
  const [saving,  setSaving]  = useState(false);
  const [errors,  setErrors]  = useState({});
  const [success, setSuccess] = useState(false);

  const validate = () => {
    const e = {};
    if (!form.currentPassword)              e.currentPassword = 'Current password is required.';
    if (!form.newPassword)                  e.newPassword     = 'New password is required.';
    else if (form.newPassword.length < 8)   e.newPassword     = 'Must be at least 8 characters.';
    else if (scorePassword(form.newPassword) < 2)
                                            e.newPassword     = 'Password is too weak.';
    if (form.newPassword === form.currentPassword && form.newPassword)
                                            e.newPassword     = 'New password must differ from current.';
    if (!form.confirmPassword)              e.confirmPassword = 'Please confirm your new password.';
    else if (form.newPassword !== form.confirmPassword)
                                            e.confirmPassword = 'Passwords do not match.';
    return e;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const e2 = validate();
    if (Object.keys(e2).length) { setErrors(e2); return; }

    setSaving(true);
    setErrors({});
    try {
      await changePassword({ currentPassword: form.currentPassword, newPassword: form.newPassword });
      setSuccess(true);
      setForm({ currentPassword: '', newPassword: '', confirmPassword: '' });
      toast('Password changed successfully', 'success');
      setTimeout(() => setSuccess(false), 4000);
    } catch (err) {
      const msg = err.response?.data?.message
               || err.response?.data?.errors?.currentPassword?.[0]
               || err.response?.data?.title
               || 'Failed to change password.';
      setErrors({ server: msg });
      toast(msg, 'error');
    } finally { setSaving(false); }
  };

  const fieldStyle = { marginBottom: 16 };
  const labelStyle = { display: 'block', fontSize: 11, fontWeight: 700, letterSpacing: '0.07em', textTransform: 'uppercase', color: 'var(--text-2)', marginBottom: 6 };
  const errorStyle = { fontSize: 11, color: 'var(--error)', marginTop: 5, display: 'flex', alignItems: 'center', gap: 4 };

  return (
    <form onSubmit={handleSubmit}>
      {/* Server error */}
      {errors.server && (
        <div style={{ padding: '10px 14px', borderRadius: 10, background: 'var(--red-bg)', border: '1px solid rgba(186,26,26,.2)', marginBottom: 16, fontSize: 13, color: 'var(--error)', display: 'flex', alignItems: 'center', gap: 8 }}>
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><circle cx="12" cy="12" r="10"/><path d="M12 8v4M12 16h.01"/></svg>
          {errors.server}
        </div>
      )}

      {/* Success banner */}
      {success && (
        <div style={{ padding: '10px 14px', borderRadius: 10, background: 'var(--green-bg)', border: '1px solid rgba(56,106,32,.2)', marginBottom: 16, fontSize: 13, color: 'var(--green)', display: 'flex', alignItems: 'center', gap: 8 }}>
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><polyline points="20 6 9 17 4 12"/></svg>
          Password changed successfully.
        </div>
      )}

      {/* Current password */}
      <div style={fieldStyle}>
        <label htmlFor="currentPw" style={labelStyle}>Current Password</label>
        <PasswordInput id="currentPw" value={form.currentPassword} onChange={e => setForm(f => ({ ...f, currentPassword: e.target.value }))} placeholder="Enter your current password" />
        {errors.currentPassword && <div style={errorStyle}><svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><circle cx="12" cy="12" r="10"/><path d="M12 8v4M12 16h.01"/></svg>{errors.currentPassword}</div>}
      </div>

      {/* New password */}
      <div style={fieldStyle}>
        <label htmlFor="newPw" style={labelStyle}>New Password</label>
        <PasswordInput id="newPw" value={form.newPassword} onChange={e => setForm(f => ({ ...f, newPassword: e.target.value }))} placeholder="Choose a strong password" />
        <StrengthBar password={form.newPassword} />
        {errors.newPassword && <div style={errorStyle}><svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><circle cx="12" cy="12" r="10"/><path d="M12 8v4M12 16h.01"/></svg>{errors.newPassword}</div>}
      </div>

      {/* Confirm */}
      <div style={fieldStyle}>
        <label htmlFor="confirmPw" style={labelStyle}>Confirm New Password</label>
        <PasswordInput id="confirmPw" value={form.confirmPassword} onChange={e => setForm(f => ({ ...f, confirmPassword: e.target.value }))} placeholder="Re-enter your new password" />
        {errors.confirmPassword && <div style={errorStyle}><svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><circle cx="12" cy="12" r="10"/><path d="M12 8v4M12 16h.01"/></svg>{errors.confirmPassword}</div>}
        {form.confirmPassword && !errors.confirmPassword && form.newPassword === form.confirmPassword && (
          <div style={{ ...errorStyle, color: 'var(--green)', marginTop: 5 }}>
            <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><polyline points="20 6 9 17 4 12"/></svg>
            Passwords match
          </div>
        )}
      </div>

      <div style={{ display: 'flex', justifyContent: 'flex-end', paddingTop: 4 }}>
        <button type="submit" disabled={saving} style={{
          display: 'inline-flex', alignItems: 'center', gap: 8,
          padding: '9px 22px', borderRadius: 'var(--radius-lg)', border: 'none',
          background: 'linear-gradient(135deg, var(--primary), var(--primary-container))',
          color: '#fff', fontSize: 13, fontWeight: 700,
          cursor: saving ? 'not-allowed' : 'pointer',
          opacity: saving ? 0.7 : 1,
          boxShadow: '0 2px 8px rgba(124,87,45,.25)',
          fontFamily: 'var(--font-body)',
          transition: 'opacity .15s',
        }}>
          {saving ? <Spinner size={13} /> : (
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/>
            </svg>
          )}
          {saving ? 'Saving…' : 'Change Password'}
        </button>
      </div>
    </form>
  );
}

/* ─────────────────────────────────────────────────────────
   Main profile page
───────────────────────────────────────────────────────── */
export default function ProfilePage() {
  const { user } = useAuth();

  const initial = user?.fullName?.charAt(0)?.toUpperCase()
               || user?.userName?.charAt(0)?.toUpperCase()
               || '?';

  const displayName = user?.fullName || user?.userName || 'Unknown';

  const roleColors = ['var(--primary)', '#2563eb', '#7e22ce', '#16a34a', '#d97706'];

  return (
    <div style={{ padding: '28px 32px 60px', fontFamily: 'var(--font-body)', maxWidth: 1100 }}>

      {/* Breadcrumb */}
      <div style={{ fontSize: 12, color: 'var(--outline)', marginBottom: 20, display: 'flex', alignItems: 'center', gap: 6 }}>
        <span>Dashboard</span>
        <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
        <span style={{ color: 'var(--text-2)', fontWeight: 600 }}>My Profile</span>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '370px 2fr', gap: 20, alignItems: 'start' }}>

        {/* ── Left column ── */}
        <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>

          {/* Avatar card */}
        <div
            style={{
                background: 'var(--surface-container-lowest)',
                borderRadius: 'var(--radius-xl)',
                overflow: 'hidden',
                boxShadow: '0 1px 4px rgba(29,27,23,.05)',
            }}
        >
            {/* Header gradient strip */}
            <div
                style={{
                    height: 88,
                    background:
                        'linear-gradient(135deg, var(--primary) 0%, var(--primary-container) 100%)',
                    position: 'relative',
                }}
            >
                <div
                    style={{
                        position: 'absolute',
                        top: 0,
                        right: 0,
                        width: 0,
                        height: 0,
                        borderStyle: 'solid',
                        borderWidth: '0 48px 48px 0',
                        borderColor:
                            'transparent rgba(255,255,255,0.12) transparent transparent',
                    }}
                />

                {/* Avatar */}
                <div
                    style={{
                        position: 'absolute',
                        left: 22,
                        bottom: -36,
                        width: 72,
                        height: 72,
                        borderRadius: '50%',
                        background:
                            'linear-gradient(135deg, var(--primary), var(--primary-container))',
                        border: '4px solid var(--surface-container-lowest)',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        fontSize: 28,
                        fontWeight: 800,
                        color: '#fff',
                        fontFamily: 'var(--font-display)',
                        boxShadow: '0 4px 16px rgba(124,87,45,.3)',
                    }}
                >
                    {initial}
                </div>
            </div>

            {/* Content */}
            <div
                style={{
                    padding: '52px 22px 22px',
                }}
            >
                <div
                    style={{
                        fontFamily: 'var(--font-display)',
                        fontSize: 18,
                        fontWeight: 700,
                        color: 'var(--text)',
                        marginBottom: 3,
                    }}
                >
                    {displayName}
                </div>

                <div
                    style={{
                        fontSize: 13,
                        color: 'var(--outline)',
                        marginBottom: 14,
                    }}
                >
                    {user?.email}
                </div>

                {/* Roles */}
                {user?.roles?.length > 0 && (
                    <div
                        style={{
                            display: 'flex',
                            flexWrap: 'wrap',
                            gap: 6,
                        }}
                    >
                        {user.roles.map((role, i) => (
                            <span
                                key={role}
                                style={{
                                    padding: '3px 10px',
                                    borderRadius: 'var(--radius-full)',
                                    fontSize: 10,
                                    fontWeight: 700,
                                    letterSpacing: '0.06em',
                                    textTransform: 'uppercase',
                                    background: `${roleColors[i % roleColors.length]}14`,
                                    color: roleColors[i % roleColors.length],
                                    border: `1px solid ${roleColors[i % roleColors.length]
                                        }30`,
                                }}
                            >
                                {role}
                            </span>
                        ))}
                    </div>
                )}
            </div>
        </div>

          {/* Account details */}
          <Section
            title="Account Details"
            subtitle="Your account information"
            icon={<svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>}
          >
            <div style={{ marginBottom: -10 }}>
              <InfoRow label="User ID"   value={user?.id}       copyable />
              <InfoRow label="Username"  value={user?.userName} copyable />
              <InfoRow label="Email"     value={user?.email}    copyable />
              <InfoRow
                label="Roles"
                value={user?.roles?.join(', ') || '—'}
              />
            </div>
          </Section>
        </div>

        {/* ── Right column ── */}
        <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>

          {/* Change password */}
          <Section
            title="Change Password"
            subtitle="Use a strong password you don't use elsewhere"
            icon={<svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><rect x="3" y="11" width="18" height="11" rx="2" ry="2"/><path d="M7 11V7a5 5 0 0110 0v4"/></svg>}
          >
            <ChangePasswordForm />
          </Section>

          {/* Password tips */}
          <div style={{
            background: 'rgba(124,87,45,.04)',
            border: '1px solid rgba(124,87,45,.12)',
            borderRadius: 'var(--radius-xl)',
            padding: '16px 20px',
          }}>
            <div style={{ fontSize: 12, fontWeight: 700, color: 'var(--primary)', marginBottom: 10, display: 'flex', alignItems: 'center', gap: 6 }}>
              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><circle cx="12" cy="12" r="10"/><path d="M12 16v-4M12 8h.01"/></svg>
              Tips for a strong password
            </div>
            <ul style={{ paddingLeft: 16, display: 'flex', flexDirection: 'column', gap: 5 }}>
              {[
                'At least 8 characters long',
                'Mix uppercase and lowercase letters',
                'Include numbers and special characters (!@#$…)',
                'Avoid common words or personal info',
              ].map(tip => (
                <li key={tip} style={{ fontSize: 12, color: 'var(--text-2)', lineHeight: 1.5 }}>{tip}</li>
              ))}
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
}