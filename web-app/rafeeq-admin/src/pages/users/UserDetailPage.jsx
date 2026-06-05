import { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  getUserById, promoteToAdmin, demoteToModerator,
  lockUserAccount, unlockUserAccount, suspendUserAccount,
  reactivateUserAccount, resetUserPassword, requirePasswordChange,
  confirmUserEmail, resendVerificationEmail,
  deleteUser, permanentlyDeleteUser, getUserActivity, getUserLoginHistory,
} from '../../api/usersApi';
import { useToast } from '../../components/common/Toast';
import ConfirmDialog from '../../components/common/ConfirmDialog';
import Spinner from '../../components/common/Spinner';

// ── inline Btn ───────────────────────────────────────────────────────────────
const Btn = ({ variant = 'primary', children, style, ...props }) => {
  const base = {
    display: 'inline-flex', alignItems: 'center', gap: 6,
    padding: '8px 16px', borderRadius: 8, fontFamily: 'var(--font-body)',
    fontSize: 13, fontWeight: 600, cursor: 'pointer', border: 'none',
    transition: 'opacity .15s', whiteSpace: 'nowrap',
  };
  const variants = {
    primary:   { background: 'linear-gradient(135deg, var(--primary), var(--primary-container))', color: '#fff', boxShadow: '0 2px 8px rgba(124,87,45,.2)' },
    secondary: { background: 'var(--surface-container)', color: 'var(--on-surface)', border: '1px solid var(--outline-variant)' },
    ghost:     { background: 'transparent', color: 'var(--on-surface-variant)', border: '1px solid var(--outline-variant)' },
    danger:    { background: 'linear-gradient(135deg,#ba1a1a,#e05252)', color: '#fff', boxShadow: '0 2px 8px rgba(186,26,26,.2)' },
    warning:   { background: 'linear-gradient(135deg,#d97706,#f59e0b)', color: '#fff' },
    success:   { background: 'linear-gradient(135deg,#065f46,#10b981)', color: '#fff' },
  };
  return (
    <button
      style={{ ...base, ...variants[variant], ...style }}
      onMouseEnter={e => e.currentTarget.style.opacity = '.85'}
      onMouseLeave={e => e.currentTarget.style.opacity = '1'}
      {...props}
    >
      {children}
    </button>
  );
};

// ── status badge ─────────────────────────────────────────────────────────────
const StatusBadge = ({ status }) => {
  const map = {
    Active:    { bg: '#d1fae5', color: '#065f46' },
    Locked:    { bg: '#fef3c7', color: '#92400e' },
    Suspended: { bg: '#fee2e2', color: '#991b1b' },
    Deleted:   { bg: 'var(--surface-container)', color: 'var(--on-surface-variant)' },
  };
  const s = map[status] || map.Active;
  return (
    <span style={{
      padding: '4px 12px', borderRadius: 20, fontSize: 12, fontWeight: 700,
      background: s.bg, color: s.color,
    }}>
      {status}
    </span>
  );
};

// ── role badge ────────────────────────────────────────────────────────────────
const RoleBadge = ({ role }) => {
  const map = {
    Admin:     { bg: '#ede9fe', color: '#5b21b6' },
    Moderator: { bg: '#dbeafe', color: '#1e40af' },
    Tourist:   { bg: 'var(--primary-fixed)', color: 'var(--primary)' },
  };
  const s = map[role] || map.Tourist;
  return (
    <span style={{
      display: 'inline-block', padding: '3px 10px', borderRadius: 6,
      fontSize: 12, fontWeight: 700, background: s.bg, color: s.color,
      marginRight: 4,
    }}>
      {role}
    </span>
  );
};

// ── avatar ───────────────────────────────────────────────────────────────────
const Avatar = ({ name, size = 72 }) => {
  const initials = (name || '?').split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase();
  const colors = ['#d4a574','#7c572d','#5b21b6','#1e40af','#065f46'];
  const idx = initials.charCodeAt(0) % colors.length;
  return (
    <div style={{
      width: size, height: size, borderRadius: '50%',
      background: colors[idx], color: '#fff',
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      fontSize: size * 0.33, fontWeight: 800, flexShrink: 0,
      boxShadow: '0 4px 12px rgba(0,0,0,.15)',
    }}>
      {initials}
    </div>
  );
};

// ── info row ─────────────────────────────────────────────────────────────────
const InfoRow = ({ label, value, children }) => (
  <div style={{
    display: 'flex', alignItems: 'flex-start', gap: 12,
    padding: '12px 0', borderBottom: '1px solid var(--outline-variant)',
  }}>
    <div style={{ width: 160, flexShrink: 0, fontSize: 12, fontWeight: 600, color: 'var(--on-surface-variant)', paddingTop: 1 }}>
      {label}
    </div>
    <div style={{ fontSize: 14, color: 'var(--on-surface)', flex: 1 }}>
      {children || value || <span style={{ color: 'var(--on-surface-variant)', fontStyle: 'italic' }}>—</span>}
    </div>
  </div>
);

// ── section card ──────────────────────────────────────────────────────────────
const SectionCard = ({ title, children, action }) => (
  <div style={{
    background: 'var(--surface-container-lowest)',
    border: '1px solid var(--outline-variant)',
    borderRadius: 'var(--radius-xl)', overflow: 'hidden', marginBottom: 20,
  }}>
    <div style={{
      padding: '14px 20px', borderBottom: '1px solid var(--outline-variant)',
      display: 'flex', justifyContent: 'space-between', alignItems: 'center',
      background: 'var(--surface-container-low)',
    }}>
      <div style={{ fontSize: 14, fontWeight: 700, color: 'var(--on-surface)', fontFamily: 'var(--font-display)' }}>
        {title}
      </div>
      {action}
    </div>
    <div style={{ padding: '4px 20px 16px' }}>{children}</div>
  </div>
);

// ── tabs ──────────────────────────────────────────────────────────────────────
const TABS = ['Overview', 'Security', 'Activity', 'Login History'];

// ─────────────────────────────────────────────────────────────────────────────
// MAIN PAGE
// ─────────────────────────────────────────────────────────────────────────────
export default function UserDetailPage() {
  const { id: userId } = useParams();
  const navigate   = useNavigate();
  const toast  = useToast();

  const [user, setUser]         = useState(null);
  const [loading, setLoading]   = useState(true);
  const [activeTab, setActiveTab] = useState('Overview');
  const [confirm, setConfirm]   = useState(null);
  const [actionLoading, setActionLoading] = useState('');

  // modal state
  const [showSuspend, setShowSuspend] = useState(false);
  const [showLock, setShowLock]       = useState(false);

  const fetchUser = useCallback(async () => {
    setLoading(true);
    try {
      const data = await getUserById(userId);
      setUser(data);
    } catch (e) {
      console.error(e.response?.data || e);
      toast('Failed to load user', 'error');
    } finally {
      setLoading(false);
    }
  }, [userId]);

  useEffect(() => { fetchUser(); }, [fetchUser]);

  const doAction = async (actionKey, fn, successMsg) => {
    setActionLoading(actionKey);
    try {
      await fn();
      toast(successMsg, 'success');
      fetchUser();
    } catch (e) {
      console.error(e.response?.data || e);
      toast(e.response?.data?.error || 'Action failed', 'error');
    } finally {
      setActionLoading('');
    }
  };

  if (loading) return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '60vh' }}>
      <Spinner />
    </div>
  );
  if (!user) return (
    <div style={{ padding: 40, textAlign: 'center', color: 'var(--on-surface-variant)' }}>
      User not found.
    </div>
  );

  const fullName = user.fullName || `${user.firstName || ''} ${user.lastName || ''}`.trim() || user.email;
  const roles    = Array.isArray(user.roles) ? user.roles : [];
  const isAdmin  = roles.includes('Admin');
  const isMod    = roles.includes('Moderator');

  const fmtDate = (d) => d ? new Date(d).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' }) : '—';
  const fmtDateTime = (d) => d ? new Date(d).toLocaleString('en-GB', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' }) : '—';

  return (
    <div style={{ minHeight: '100vh', background: 'var(--background)' }}>

      {/* ── TOP BAR ── */}
      <div style={{
        position: 'sticky', top: 0, zIndex: 100, height: 64,
        background: 'var(--surface-container-lowest)',
        borderBottom: '1px solid var(--outline-variant)',
        display: 'flex', alignItems: 'center', padding: '0 28px', gap: 12,
      }}>
        <button onClick={() => navigate('/users')} style={{
          width: 36, height: 36, borderRadius: 8, border: 'none',
          background: 'var(--surface-container)', cursor: 'pointer', fontSize: 16,
        }}>←</button>

        <Avatar name={fullName} size={36} />

        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ fontSize: 16, fontWeight: 800, fontFamily: 'var(--font-display)', color: 'var(--on-surface)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
            {fullName}
          </div>
          <div style={{ fontSize: 12, color: 'var(--on-surface-variant)' }}>{user.email}</div>
        </div>

        <StatusBadge status={user.status} />
        {roles.map(r => <RoleBadge key={r} role={r} />)}

        {/* top-bar actions */}
        {!isAdmin && (
          <>
            {user.status === 'Active' && (
              <>
                <Btn variant="warning" onClick={() => setShowLock(true)} disabled={!!actionLoading}>
                  🔒 Lock
                </Btn>
                <Btn variant="warning" style={{ background: 'linear-gradient(135deg,#7c3aed,#a78bfa)' }} onClick={() => setShowSuspend(true)} disabled={!!actionLoading}>
                  ⏸ Suspend
                </Btn>
                <Btn variant="danger" onClick={() => setConfirm({
                  msg: `Delete ${fullName}? This will soft-delete the account.`,
                  action: 'delete',
                })}>
                  🗑 Delete
                </Btn>
              </>
            )}
            {(user.status === 'Locked' || user.status === 'Suspended') && (
              <Btn variant="success" onClick={() => doAction('reactivate', () => reactivateUserAccount(userId), 'Account reactivated')} disabled={!!actionLoading}>
                ▶ Reactivate
              </Btn>
            )}
          </>
        )}

        {!isAdmin && user.status === 'Deleted' && (
          <Btn variant="danger" onClick={() => setConfirm({
            msg: `Permanently delete ${fullName}? This cannot be undone.`,
            action: 'permanentlyDelete',
          })}>
            🗑 Permanently Delete
          </Btn>
        )}
      </div>

      {/* ── TAB BAR ── */}
      <div style={{
        position: 'sticky', top: 64, zIndex: 99,
        background: 'var(--surface-container-lowest)',
        borderBottom: '1px solid var(--outline-variant)',
        display: 'flex', gap: 0, padding: '0 28px', overflowX: 'auto',
      }}>
        {TABS.map(tab => (
          <button
            key={tab}
            onClick={() => setActiveTab(tab)}
            style={{
              padding: '14px 18px', border: 'none', background: 'transparent',
              fontSize: 13, fontWeight: 600, cursor: 'pointer', whiteSpace: 'nowrap',
              color: activeTab === tab ? 'var(--primary)' : 'var(--on-surface-variant)',
              borderBottom: activeTab === tab ? '2px solid var(--primary)' : '2px solid transparent',
              transition: 'color .15s',
            }}
          >
            {tab}
          </button>
        ))}
      </div>

      {/* ── CONTENT ── */}
      <div style={{ padding: '28px', maxWidth: 1100, margin: '0 auto' }}>

        {/* ── OVERVIEW TAB ── */}
        {activeTab === 'Overview' && (
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 340px', gap: 24, alignItems: 'start' }}>

            {/* left */}
            <div>
              <SectionCard title="Personal Information">
                <InfoRow label="Full Name" value={fullName} />
                <InfoRow label="Email">{user.email}
                  {user.emailConfirmed
                    ? <span style={{ marginLeft: 8, fontSize: 11, color: '#065f46', fontWeight: 700 }}>✓ Verified</span>
                    : <span style={{ marginLeft: 8, fontSize: 11, color: '#92400e', fontWeight: 700 }}>✗ Unverified</span>}
                </InfoRow>
                <InfoRow label="Phone">{user.phoneNumber || '—'}
                  {user.phoneNumber && (user.phoneNumberConfirmed
                    ? <span style={{ marginLeft: 8, fontSize: 11, color: '#065f46', fontWeight: 700 }}>✓ Verified</span>
                    : <span style={{ marginLeft: 8, fontSize: 11, color: '#92400e', fontWeight: 700 }}>✗ Unverified</span>)}
                </InfoRow>
                <InfoRow label="Date of Birth" value={fmtDate(user.dateOfBirth)} />
                <InfoRow label="Gender" value={user.gender} />
                <InfoRow label="Nationality" value={user.nationality} />
              </SectionCard>

              <SectionCard title="Account Details">
                <InfoRow label="User ID"><code style={{ fontSize: 12, background: 'var(--surface-container)', padding: '2px 6px', borderRadius: 4 }}>{user.id}</code></InfoRow>
                <InfoRow label="Roles">{roles.map(r => <RoleBadge key={r} role={r} />)}</InfoRow>
                <InfoRow label="Status"><StatusBadge status={user.status} /></InfoRow>
                <InfoRow label="Date Joined" value={fmtDateTime(user.createdAt)} />
                <InfoRow label="Last Updated" value={fmtDateTime(user.updatedAt)} />
                <InfoRow label="Last Login" value={fmtDateTime(user.lastLoginAt)} />
                {user.deletedAt && <InfoRow label="Deleted At" value={fmtDateTime(user.deletedAt)} />}
                {user.lockoutEnd && <InfoRow label="Locked Until" value={fmtDateTime(user.lockoutEnd)} />}
              </SectionCard>

              {/* tourist stats */}
              {roles.includes('Tourist') && (
                <SectionCard title="Activity Statistics">
                  <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: 12, paddingTop: 8 }}>
                    {[
                      { label: 'Total Trips', value: user.touristDetails.totalTrips ?? 0, icon: '🗺' },
                      { label: 'Total Reviews', value: user.touristDetails.totalRatings ?? 0, icon: '⭐' },
                    ].map(s => (
                      <div key={s.label} style={{
                        background: 'var(--surface-container-low)', borderRadius: 10,
                        padding: '14px 16px', textAlign: 'center',
                      }}>
                        <div style={{ fontSize: 24 }}>{s.icon}</div>
                        <div style={{ fontSize: 22, fontWeight: 800, color: 'var(--on-surface)', fontFamily: 'var(--font-display)' }}>
                          {s.value}
                        </div>
                        <div style={{ fontSize: 11, color: 'var(--on-surface-variant)', marginTop: 2 }}>{s.label}</div>
                      </div>
                    ))}
                  </div>
                </SectionCard>
              )}
            </div>

            {/* right sidebar */}
            <div>
              {/* profile card */}
              <div style={{
                background: 'var(--surface-container-lowest)',
                border: '1px solid var(--outline-variant)',
                borderRadius: 'var(--radius-xl)', padding: '24px 20px',
                textAlign: 'center', marginBottom: 16, position: 'relative', overflow: 'hidden',
              }}>
                {/* pyramid accent */}
                <div style={{
                  position: 'absolute', bottom: 0, right: 0,
                  width: 0, height: 0, borderLeft: '60px solid transparent',
                  borderBottom: '60px solid var(--primary-fixed)', opacity: .4,
                }} />
                <Avatar name={fullName} size={80} />
                <div style={{ marginTop: 12, fontSize: 16, fontWeight: 800, fontFamily: 'var(--font-display)', color: 'var(--on-surface)' }}>
                  {fullName}
                </div>
                <div style={{ fontSize: 12, color: 'var(--on-surface-variant)', marginTop: 2 }}>{user.email}</div>
                <div style={{ marginTop: 10, display: 'flex', gap: 4, justifyContent: 'center', flexWrap: 'wrap' }}>
                  {roles.map(r => <RoleBadge key={r} role={r} />)}
                </div>
                <div style={{ marginTop: 8 }}><StatusBadge status={user.status} /></div>
              </div>

              {/* role management */}
              <div style={{
                background: 'var(--surface-container-lowest)',
                border: '1px solid var(--outline-variant)',
                borderRadius: 'var(--radius-xl)', overflow: 'hidden', marginBottom: 16,
              }}>
                <div style={{ padding: '14px 16px', borderBottom: '1px solid var(--outline-variant)', background: 'var(--surface-container-low)' }}>
                  <div style={{ fontSize: 13, fontWeight: 700, color: 'var(--on-surface)' }}>Role Management</div>
                </div>
                <div style={{ padding: '12px 16px', display: 'flex', flexDirection: 'column', gap: 8 }}>
                  {isMod && (
                    <Btn style={{ width: '100%', justifyContent: 'center' }}
                      onClick={() => setConfirm({ msg: `Promote ${fullName} to Admin?`, action: 'promote' })}
                      disabled={!!actionLoading}>
                      👑 Promote to Admin
                    </Btn>
                  )}
                  {isAdmin && (
                    <Btn variant="secondary" style={{ width: '100%', justifyContent: 'center' }}
                      onClick={() => setConfirm({ msg: `Demote ${fullName} to Moderator?`, action: 'demote' })}
                      disabled={!!actionLoading}>
                      🛡 Demote to Moderator
                    </Btn>
                  )}
                  {!isAdmin && !isMod && (
                    <div style={{ fontSize: 12, color: 'var(--on-surface-variant)', textAlign: 'center', padding: '8px 0' }}>
                      Tourist accounts cannot be promoted directly. Create a moderator account instead.
                    </div>
                  )}
                </div>
              </div>

              {/* quick actions */}
              <div style={{
                background: 'var(--surface-container-lowest)',
                border: '1px solid var(--outline-variant)',
                borderRadius: 'var(--radius-xl)', overflow: 'hidden',
              }}>
                <div style={{ padding: '14px 16px', borderBottom: '1px solid var(--outline-variant)', background: 'var(--surface-container-low)' }}>
                  <div style={{ fontSize: 13, fontWeight: 700, color: 'var(--on-surface)' }}>Quick Actions</div>
                </div>
                <div style={{ padding: '12px 16px', display: 'flex', flexDirection: 'column', gap: 8 }}>
                  <Btn variant="secondary" style={{ width: '100%', justifyContent: 'center' }}
                    onClick={() => doAction('resetPw', () => resetUserPassword(userId), 'Password reset email sent')}
                    disabled={!!actionLoading}>
                    🔑 Send Password Reset
                  </Btn>
                  <Btn variant="secondary" style={{ width: '100%', justifyContent: 'center' }}
                    onClick={() => doAction('reqPwChange', () => requirePasswordChange(userId), 'Password change required on next login')}
                    disabled={!!actionLoading}>
                    🔄 Require Password Change
                  </Btn>
                  {!user.emailConfirmed && (
                    <Btn variant="secondary" style={{ width: '100%', justifyContent: 'center' }}
                      onClick={() => doAction('resendEmail', () => resendVerificationEmail(userId), 'Verification email sent')}
                      disabled={!!actionLoading}>
                      📧 Resend Verification Email
                    </Btn>
                  )}
                  {!user.emailConfirmed && (
                    <Btn variant="secondary" style={{ width: '100%', justifyContent: 'center' }}
                      onClick={() => doAction('confirmEmail', () => confirmUserEmail(userId), 'Email confirmed')}
                      disabled={!!actionLoading}>
                      ✅ Manually Confirm Email
                    </Btn>
                  )}
                </div>
              </div>
            </div>
          </div>
        )}

        {/* ── SECURITY TAB ── */}
        {activeTab === 'Security' && (
          <SecurityTab user={user} onRefresh={fetchUser} />
        )}

        {/* ── ACTIVITY TAB ── */}
        {activeTab === 'Activity' && (
          <ActivityTab userId={userId} />
        )}

        {/* ── LOGIN HISTORY TAB ── */}
        {activeTab === 'Login History' && (
          <LoginHistoryTab userId={userId} />
        )}
      </div>

      {/* ── CONFIRM DIALOG ── */}
      {confirm && (
        <ConfirmDialog
          open
          message={confirm.msg}
          onConfirm={async () => {
            const c = confirm; setConfirm(null);
            if (c.action === 'promote') await doAction('promote', () => promoteToAdmin(userId), 'User promoted to Admin');
            if (c.action === 'demote')  await doAction('demote',  () => demoteToModerator(userId), 'User demoted to Moderator');
            if (c.action === 'delete')  await doAction('delete',  () => deleteUser(userId, 'Admin action'), 'User deleted');
            if (c.action === 'permanentlyDelete')  await doAction('permanentlyDelete',  () => permanentlyDeleteUser(userId, 'Admin action'), 'User permanently deleted');
          }}
          onClose={() => setConfirm(null)}
        />
      )}

      {/* ── LOCK MODAL ── */}
      {showLock && (
        <LockSuspendModal
          mode="lock"
          userName={fullName}
          onClose={() => setShowLock(false)}
          onConfirm={async ({ reason, until }) => {
            setShowLock(false);
            await doAction('lock', () => lockUserAccount(userId, reason, until), 'Account locked');
          }}
        />
      )}

      {/* ── SUSPEND MODAL ── */}
      {showSuspend && (
        <LockSuspendModal
          mode="suspend"
          userName={fullName}
          onClose={() => setShowSuspend(false)}
          onConfirm={async ({ reason, until }) => {
            setShowSuspend(false);
            await doAction('suspend', () => suspendUserAccount(userId, reason, until), 'Account suspended');
          }}
        />
      )}
    </div>
  );
}

// ─────────────────────────────────────────────────────────────────────────────
// SECURITY TAB
// ─────────────────────────────────────────────────────────────────────────────
function SecurityTab({ user, onRefresh }) {
  const checks = [
    { label: 'Email Confirmed',       ok: user.emailConfirmed },
    { label: 'Phone Confirmed',       ok: user.phoneNumberConfirmed },
    { label: '2FA Enabled',           ok: user.twoFactorEnabled },
    { label: 'Password Up to Date',   ok: !user.mustChangePassword },
    { label: 'Account Not Locked',    ok: !user.lockoutEnd || new Date(user.lockoutEnd) <= new Date() },
  ];

  const score = checks.filter(c => c.ok).length;
  const pct   = Math.round((score / checks.length) * 100);
  const scoreColor = pct >= 80 ? '#065f46' : pct >= 50 ? '#92400e' : '#991b1b';

  return (
    <div style={{ display: 'grid', gridTemplateColumns: '1fr 340px', gap: 24, alignItems: 'start' }}>
      <div>
        <SectionCard title="Security Settings">
          {[
            { label: 'Two-Factor Authentication', value: user.twoFactorEnabled ? '✅ Enabled' : '❌ Disabled' },
            { label: 'Lockout Enabled',           value: user.lockoutEnabled ? 'Yes' : 'No' },
            { label: 'Failed Login Attempts',     value: user.accessFailedCount ?? 0 },
            { label: 'Locked Until',              value: user.lockoutEnd ? new Date(user.lockoutEnd).toLocaleString() : 'Not locked' },
            { label: 'Must Change Password',      value: user.mustChangePassword ? '⚠️ Yes' : 'No' },
            { label: 'Last Password Change',      value: user.lastPasswordChangedAt ? new Date(user.lastPasswordChangedAt).toLocaleDateString() : 'Never' },
          ].map(({ label, value }) => (
            <InfoRow key={label} label={label} value={value} />
          ))}
        </SectionCard>
      </div>

      <div>
        {/* security score */}
        <div style={{
          background: 'var(--surface-container-lowest)',
          border: '1px solid var(--outline-variant)',
          borderRadius: 'var(--radius-xl)', padding: '20px', marginBottom: 16,
          position: 'relative', overflow: 'hidden',
        }}>
          <div style={{ position: 'absolute', bottom: 0, right: 0, width: 0, height: 0, borderLeft: '50px solid transparent', borderBottom: '50px solid var(--primary-fixed)', opacity: .35 }} />
          <div style={{ fontSize: 13, fontWeight: 700, marginBottom: 16, color: 'var(--on-surface)' }}>Security Score</div>
          <div style={{ textAlign: 'center', marginBottom: 16 }}>
            <div style={{ fontSize: 48, fontWeight: 900, fontFamily: 'var(--font-display)', color: scoreColor, lineHeight: 1 }}>
              {pct}%
            </div>
            <div style={{ fontSize: 12, color: 'var(--on-surface-variant)', marginTop: 4 }}>
              {score} of {checks.length} checks passed
            </div>
          </div>
          {/* progress bar */}
          <div style={{ height: 8, borderRadius: 4, background: 'var(--outline-variant)', overflow: 'hidden' }}>
            <div style={{ height: '100%', width: `${pct}%`, borderRadius: 4, background: scoreColor, transition: 'width .4s' }} />
          </div>
          {/* checklist */}
          <div style={{ marginTop: 16, display: 'flex', flexDirection: 'column', gap: 8 }}>
            {checks.map(c => (
              <div key={c.label} style={{ display: 'flex', alignItems: 'center', gap: 8, fontSize: 13 }}>
                <span style={{ fontSize: 14 }}>{c.ok ? '✅' : '❌'}</span>
                <span style={{ color: c.ok ? 'var(--on-surface)' : 'var(--on-surface-variant)' }}>{c.label}</span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}

// ─────────────────────────────────────────────────────────────────────────────
// ACTIVITY TAB
// ─────────────────────────────────────────────────────────────────────────────
function ActivityTab({ userId }) {
  const { toast } = useToast();
  const [items, setItems]   = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage]     = useState(1);
  const [total, setTotal]   = useState(0);
  const pageSize = 20;

  useEffect(() => {
    setLoading(true);
    getUserActivity(userId, page, pageSize)
      .then(data => {
        const arr = Array.isArray(data) ? data : data?.items ?? [];
        setItems(arr);
        setTotal(data?.totalItems ?? arr.length);
      })
      .catch(e => { console.error(e.response?.data || e); toast('Failed to load activity', 'error'); })
      .finally(() => setLoading(false));
  }, [userId, page]);

  const activityIcon = (type) => {
    const map = { Login: '🔐', Logout: '🚪', PasswordChange: '🔑', ProfileUpdate: '✏️', TripCreated: '🗺', ReviewCreated: '⭐' };
    return map[type] || '📌';
  };

  return (
    <div style={{ background: 'var(--surface-container-lowest)', border: '1px solid var(--outline-variant)', borderRadius: 'var(--radius-xl)', overflow: 'hidden' }}>
      <div style={{ padding: '14px 20px', borderBottom: '1px solid var(--outline-variant)', background: 'var(--surface-container-low)' }}>
        <div style={{ fontSize: 14, fontWeight: 700, color: 'var(--on-surface)' }}>Activity Log</div>
        <div style={{ fontSize: 12, color: 'var(--on-surface-variant)', marginTop: 2 }}>{total} total events</div>
      </div>
      {loading ? (
        <div style={{ padding: 40, display: 'flex', justifyContent: 'center' }}><Spinner /></div>
      ) : items.length === 0 ? (
        <div style={{ padding: 40, textAlign: 'center', color: 'var(--on-surface-variant)' }}>
          <div style={{ fontSize: 32, marginBottom: 8 }}>📭</div>No activity recorded
        </div>
      ) : (
        <>
          {items.map((item, i) => (
            <div key={item.id || i} style={{
              display: 'flex', gap: 14, padding: '14px 20px',
              borderBottom: i === items.length - 1 ? 'none' : '1px solid var(--outline-variant)',
              alignItems: 'flex-start',
            }}>
              <div style={{ width: 36, height: 36, borderRadius: 10, background: 'var(--surface-container)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 16, flexShrink: 0 }}>
                {activityIcon(item.activityType)}
              </div>
              <div style={{ flex: 1 }}>
                <div style={{ fontSize: 13, fontWeight: 600, color: 'var(--on-surface)' }}>{item.description || item.activityType}</div>
                <div style={{ fontSize: 11, color: 'var(--on-surface-variant)', marginTop: 3, display: 'flex', gap: 12 }}>
                  {item.ipAddress && <span>🌐 {item.ipAddress}</span>}
                  {item.location && <span>📍 {item.location}</span>}
                </div>
              </div>
              <div style={{ fontSize: 11, color: 'var(--on-surface-variant)', flexShrink: 0 }}>
                {item.timestamp ? new Date(item.timestamp).toLocaleString('en-GB', { day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit' }) : '—'}
              </div>
            </div>
          ))}
          {Math.ceil(total / pageSize) > 1 && (
            <div style={{ padding: '12px 20px', borderTop: '1px solid var(--outline-variant)', display: 'flex', gap: 8, justifyContent: 'center' }}>
              <button disabled={page === 1} onClick={() => setPage(p => p - 1)} style={{ padding: '6px 12px', borderRadius: 6, border: '1px solid var(--outline-variant)', background: 'var(--surface-container)', cursor: 'pointer', fontSize: 12 }}>← Prev</button>
              <span style={{ padding: '6px 12px', fontSize: 12, color: 'var(--on-surface-variant)' }}>{page} / {Math.ceil(total / pageSize)}</span>
              <button disabled={page >= Math.ceil(total / pageSize)} onClick={() => setPage(p => p + 1)} style={{ padding: '6px 12px', borderRadius: 6, border: '1px solid var(--outline-variant)', background: 'var(--surface-container)', cursor: 'pointer', fontSize: 12 }}>Next →</button>
            </div>
          )}
        </>
      )}
    </div>
  );
}

// ─────────────────────────────────────────────────────────────────────────────
// LOGIN HISTORY TAB
// ─────────────────────────────────────────────────────────────────────────────
function LoginHistoryTab({ userId }) {
  const { toast } = useToast();
  const [items, setItems]   = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage]     = useState(1);
  const [total, setTotal]   = useState(0);
  const pageSize = 20;

  useEffect(() => {
    setLoading(true);
    getUserLoginHistory(userId, page, pageSize)
      .then(data => {
        const arr = Array.isArray(data) ? data : data?.items ?? [];
        setItems(arr);
        setTotal(data?.totalItems ?? arr.length);
      })
      .catch(e => { console.error(e.response?.data || e); toast('Failed to load login history', 'error'); })
      .finally(() => setLoading(false));
  }, [userId, page]);

  return (
    <div style={{ background: 'var(--surface-container-lowest)', border: '1px solid var(--outline-variant)', borderRadius: 'var(--radius-xl)', overflow: 'hidden' }}>
      <div style={{ padding: '14px 20px', borderBottom: '1px solid var(--outline-variant)', background: 'var(--surface-container-low)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <div style={{ fontSize: 14, fontWeight: 700, color: 'var(--on-surface)' }}>Login History</div>
          <div style={{ fontSize: 12, color: 'var(--on-surface-variant)', marginTop: 2 }}>{total} total logins</div>
        </div>
      </div>
      {/* table header */}
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr 100px 100px', padding: '10px 20px', background: 'var(--surface-container-low)', borderBottom: '1px solid var(--outline-variant)', fontSize: 11, fontWeight: 700, textTransform: 'uppercase', letterSpacing: '.06em', color: 'var(--on-surface-variant)', gap: 12 }}>
        <span>Date & Time</span><span>Location</span><span>Device / Browser</span><span>IP</span><span>Result</span>
      </div>
      {loading ? (
        <div style={{ padding: 40, display: 'flex', justifyContent: 'center' }}><Spinner /></div>
      ) : items.length === 0 ? (
        <div style={{ padding: 40, textAlign: 'center', color: 'var(--on-surface-variant)' }}>
          <div style={{ fontSize: 32, marginBottom: 8 }}>🔐</div>No login records
        </div>
      ) : (
        <>
          {items.map((item, i) => (
            <div key={item.id || i} style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr 100px 100px', padding: '12px 20px', borderBottom: i === items.length - 1 ? 'none' : '1px solid var(--outline-variant)', alignItems: 'center', gap: 12, fontSize: 13 }}>
              <div style={{ color: 'var(--on-surface)', fontWeight: 500 }}>
                {item.loginAt ? new Date(item.loginAt).toLocaleString('en-GB', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' }) : '—'}
              </div>
              <div style={{ color: 'var(--on-surface-variant)', fontSize: 12 }}>{item.location || '—'}</div>
              <div style={{ color: 'var(--on-surface-variant)', fontSize: 12 }}>
                <div>{item.device || '—'}</div>
                <div style={{ fontSize: 11 }}>{item.browser || ''}</div>
              </div>
              <div style={{ color: 'var(--on-surface-variant)', fontSize: 12 }}>{item.ipAddress || '—'}</div>
              <div>
                <span style={{ padding: '3px 8px', borderRadius: 6, fontSize: 11, fontWeight: 700, background: item.wasSuccessful ? '#d1fae5' : '#fee2e2', color: item.wasSuccessful ? '#065f46' : '#991b1b' }}>
                  {item.wasSuccessful ? '✓ OK' : '✗ Failed'}
                </span>
              </div>
            </div>
          ))}
          {Math.ceil(total / pageSize) > 1 && (
            <div style={{ padding: '12px 20px', borderTop: '1px solid var(--outline-variant)', display: 'flex', gap: 8, justifyContent: 'center' }}>
              <button disabled={page === 1} onClick={() => setPage(p => p - 1)} style={{ padding: '6px 12px', borderRadius: 6, border: '1px solid var(--outline-variant)', background: 'var(--surface-container)', cursor: 'pointer', fontSize: 12 }}>← Prev</button>
              <span style={{ padding: '6px 12px', fontSize: 12, color: 'var(--on-surface-variant)' }}>{page} / {Math.ceil(total / pageSize)}</span>
              <button disabled={page >= Math.ceil(total / pageSize)} onClick={() => setPage(p => p + 1)} style={{ padding: '6px 12px', borderRadius: 6, border: '1px solid var(--outline-variant)', background: 'var(--surface-container)', cursor: 'pointer', fontSize: 12 }}>Next →</button>
            </div>
          )}
        </>
      )}
    </div>
  );
}

// ─────────────────────────────────────────────────────────────────────────────
// LOCK / SUSPEND MODAL
// ─────────────────────────────────────────────────────────────────────────────
function LockSuspendModal({ mode, userName, onClose, onConfirm }) {
  const isLock   = mode === 'lock';
  const [reason, setReason]     = useState('');
  const [until, setUntil]       = useState('');
  const [indefinite, setIndef]  = useState(false);
  const [error, setError]       = useState('');

  const handleSubmit = () => {
    if (!reason.trim()) { setError('Please provide a reason'); return; }
    if (!indefinite && !until) { setError('Please set a date or select indefinite'); return; }
    onConfirm({ reason, until: indefinite ? null : until });
  };

  return (
    <div style={{ position: 'fixed', inset: 0, zIndex: 200, background: 'rgba(0,0,0,.45)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: 20 }}>
      <div style={{ background: 'var(--surface-container-lowest)', borderRadius: 'var(--radius-xl)', width: '100%', maxWidth: 440, boxShadow: '0 20px 60px rgba(0,0,0,.25)', overflow: 'hidden' }}>
        <div style={{ padding: '16px 20px', borderBottom: '1px solid var(--outline-variant)', background: isLock ? '#fef3c7' : '#fee2e2', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div style={{ fontSize: 15, fontWeight: 800, color: isLock ? '#92400e' : '#991b1b', fontFamily: 'var(--font-display)' }}>
            {isLock ? '🔒 Lock Account' : '⏸ Suspend Account'}
          </div>
          <button onClick={onClose} style={{ width: 28, height: 28, borderRadius: 6, border: 'none', background: 'transparent', cursor: 'pointer', fontSize: 14 }}>✕</button>
        </div>
        <div style={{ padding: 20, display: 'flex', flexDirection: 'column', gap: 14 }}>
          <div style={{ fontSize: 13, color: 'var(--on-surface-variant)' }}>
            {isLock ? 'Locking' : 'Suspending'} account for <strong style={{ color: 'var(--on-surface)' }}>{userName}</strong>
          </div>

          <div>
            <label style={{ fontSize: 12, fontWeight: 600, color: 'var(--on-surface-variant)', display: 'block', marginBottom: 5 }}>Reason *</label>
            <textarea
              rows={3} placeholder="Why is this account being restricted?"
              value={reason} onChange={e => { setReason(e.target.value); setError(''); }}
              style={{ width: '100%', padding: '9px 12px', borderRadius: 8, border: `1px solid ${error && !reason ? 'var(--error)' : 'var(--outline-variant)'}`, background: 'var(--surface-container-low)', fontSize: 13, fontFamily: 'var(--font-body)', color: 'var(--on-surface)', resize: 'vertical', boxSizing: 'border-box', outline: 'none' }}
            />
          </div>

          <div>
            <label style={{ display: 'flex', alignItems: 'center', gap: 8, cursor: 'pointer', marginBottom: 8 }}>
              <input type="checkbox" checked={indefinite} onChange={e => setIndef(e.target.checked)} />
              <span style={{ fontSize: 13, color: 'var(--on-surface)' }}>Indefinite (no end date)</span>
            </label>
            {!indefinite && (
              <>
                <label style={{ fontSize: 12, fontWeight: 600, color: 'var(--on-surface-variant)', display: 'block', marginBottom: 5 }}>
                  {isLock ? 'Lock' : 'Suspend'} Until *
                </label>
                <input
                  type="datetime-local" value={until}
                  onChange={e => { setUntil(e.target.value); setError(''); }}
                  min={new Date().toISOString().slice(0, 16)}
                  style={{ width: '100%', padding: '9px 12px', borderRadius: 8, border: `1px solid ${error && !until ? 'var(--error)' : 'var(--outline-variant)'}`, background: 'var(--surface-container-low)', fontSize: 13, fontFamily: 'var(--font-body)', color: 'var(--on-surface)', boxSizing: 'border-box', outline: 'none' }}
                />
              </>
            )}
          </div>

          {error && <div style={{ fontSize: 12, color: 'var(--error)', fontWeight: 600 }}>⚠ {error}</div>}
        </div>
        <div style={{ padding: '14px 20px', borderTop: '1px solid var(--outline-variant)', display: 'flex', justifyContent: 'flex-end', gap: 10 }}>
          <Btn variant="ghost" onClick={onClose}>Cancel</Btn>
          <Btn variant={isLock ? 'warning' : 'danger'} onClick={handleSubmit}>
            {isLock ? '🔒 Lock Account' : '⏸ Suspend Account'}
          </Btn>
        </div>
      </div>
    </div>
  );
}
