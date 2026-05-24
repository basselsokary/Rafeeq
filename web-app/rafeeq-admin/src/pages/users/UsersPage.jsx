// src/pages/users/UsersPage.jsx
import { useState, useEffect, useCallback, memo } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  getUsers,
  getUserStatistics,
} from '../../api/usersApi';
import { useToast } from '../../components/common/Toast';
import ConfirmDialog from '../../components/common/ConfirmDialog';
import Spinner from '../../components/common/Spinner';

// ── inline button helper ────────────────────────────────────────────────────
const Btn = ({ variant = 'primary', children, style, ...props }) => {
  const base = {
    display: 'inline-flex', alignItems: 'center', gap: 6,
    padding: '8px 16px', borderRadius: 8, fontFamily: 'var(--font-body)',
    fontSize: 13, fontWeight: 600, cursor: 'pointer', border: 'none',
    transition: 'opacity .15s, transform .1s', whiteSpace: 'nowrap',
  };
  const variants = {
    primary: {
      background: 'linear-gradient(135deg, var(--primary), var(--primary-container))',
      color: '#fff', boxShadow: '0 2px 8px rgba(124,87,45,.2)',
    },
    secondary: {
      background: 'var(--surface-container)', color: 'var(--on-surface)',
      border: '1px solid var(--outline-variant)',
    },
    ghost: {
      background: 'transparent', color: 'var(--on-surface-variant)',
      border: '1px solid var(--outline-variant)',
    },
    danger: {
      background: 'linear-gradient(135deg,#ba1a1a,#e05252)',
      color: '#fff', boxShadow: '0 2px 8px rgba(186,26,26,.2)',
    },
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

// ── status badge ────────────────────────────────────────────────────────────
const StatusBadge = ({ status }) => {
  const map = {
    Active:    { bg: '#d1fae5', color: '#065f46', dot: '#10b981' },
    Locked:    { bg: '#fef3c7', color: '#92400e', dot: '#f59e0b' },
    Suspended: { bg: '#fee2e2', color: '#991b1b', dot: '#ef4444' },
    Deleted:   { bg: 'var(--surface-container)', color: 'var(--on-surface-variant)', dot: '#9ca3af' },
  };
  const s = map[status] || map.Active;
  return (
    <span style={{
      display: 'inline-flex', alignItems: 'center', gap: 5,
      padding: '3px 10px', borderRadius: 20, fontSize: 12, fontWeight: 600,
      background: s.bg, color: s.color,
    }}>
      <span style={{ width: 6, height: 6, borderRadius: '50%', background: s.dot }} />
      {status}
    </span>
  );
};

// ── role badge ───────────────────────────────────────────────────────────────
const RoleBadge = ({ role }) => {
  const map = {
    Admin:     { bg: '#ede9fe', color: '#5b21b6' },
    Moderator: { bg: '#dbeafe', color: '#1e40af' },
    Tourist:   { bg: 'var(--primary-fixed)', color: 'var(--primary)' },
  };
  const s = map[role] || map.Tourist;
  return (
    <span style={{
      display: 'inline-block', padding: '2px 8px', borderRadius: 6,
      fontSize: 11, fontWeight: 700, background: s.bg, color: s.color,
      marginRight: 4,
    }}>
      {role}
    </span>
  );
};

// ── stat card ─────────────────────────────────────────────────────────────
const StatCard = ({ label, value, icon, accent }) => (
  <div style={{
    background: 'var(--surface-container-lowest)',
    border: '1px solid var(--outline-variant)',
    borderRadius: 'var(--radius-xl)', padding: '20px 24px',
    display: 'flex', alignItems: 'center', gap: 16,
    position: 'relative', overflow: 'hidden', flex: 1, minWidth: 160,
  }}>
    {/* pyramid corner accent */}
    <div style={{
      position: 'absolute', bottom: 0, right: 0,
      width: 0, height: 0,
      borderLeft: '48px solid transparent',
      borderBottom: `48px solid ${accent || 'var(--primary-fixed)'}`,
      opacity: .35,
    }} />
    <div style={{
      width: 44, height: 44, borderRadius: 12, flexShrink: 0,
      background: accent ? `${accent}22` : 'var(--primary-fixed)',
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      fontSize: 20,
    }}>
      {icon}
    </div>
    <div>
      <div style={{ fontSize: 26, fontWeight: 800, color: 'var(--on-surface)', lineHeight: 1, fontFamily: 'var(--font-display)' }}>
        {value ?? '—'}
      </div>
      <div style={{ fontSize: 12, color: 'var(--on-surface-variant)', marginTop: 2 }}>
        {label}
      </div>
    </div>
  </div>
);

// ── avatar initials ──────────────────────────────────────────────────────────
const Avatar = ({ name, size = 34 }) => {
  const initials = (name || '?').split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase();
  const colors = ['#d4a574','#7c572d','#5b21b6','#1e40af','#065f46'];
  const idx = initials.charCodeAt(0) % colors.length;
  return (
    <div style={{
      width: size, height: size, borderRadius: '50%', flexShrink: 0,
      background: colors[idx], color: '#fff',
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      fontSize: size * 0.36, fontWeight: 700,
    }}>
      {initials}
    </div>
  );
};

// ── filter pill ──────────────────────────────────────────────────────────────
const FilterPill = ({ label, active, onClick }) => (
  <button onClick={onClick} style={{
    padding: '6px 14px', borderRadius: 20, fontSize: 13, fontWeight: 600,
    cursor: 'pointer', border: 'none', transition: 'all .15s',
    background: active ? 'linear-gradient(135deg, var(--primary), var(--primary-container))' : 'var(--surface-container)',
    color: active ? '#fff' : 'var(--on-surface-variant)',
    boxShadow: active ? '0 2px 8px rgba(124,87,45,.2)' : 'none',
  }}>
    {label}
  </button>
);

// Stable field component used by modal to avoid remounting/focus loss
const FieldInput = memo(function FieldInput({ label, value, onChange, type = 'text', placeholder, error }) {
  return (
    <div>
      <label style={{ fontSize: 12, fontWeight: 600, color: 'var(--on-surface-variant)', display: 'block', marginBottom: 5 }}>
        {label}
      </label>
      <input
        type={type}
        placeholder={placeholder}
        value={value}
        onChange={e => onChange(e.target.value)}
        style={{ width: '100%', padding: '10px 12px', borderRadius: 8, boxSizing: 'border-box', border: `1px solid ${error ? 'var(--error)' : 'var(--outline-variant)'}`, background: 'var(--surface-container-low)', fontSize: 14, fontFamily: 'var(--font-body)', color: 'var(--on-surface)', outline: 'none' }}
      />
      {error && <div style={{ fontSize: 11, color: 'var(--error)', marginTop: 3 }}>{error}</div>}
    </div>
  );
});

// ─────────────────────────────────────────────────────────────────────────────
// MAIN PAGE
// ─────────────────────────────────────────────────────────────────────────────
export default function UsersPage() {
  const navigate = useNavigate();
  const toast = useToast();

  // data
  const [users, setUsers]       = useState([]);
  const [stats, setStats]       = useState(null);
  const [loading, setLoading]   = useState(true);
  const [statsLoading, setStatsLoading] = useState(true);

  // pagination
  const [page, setPage]         = useState(1);
  const [pageSize]              = useState(20);
  const [totalItems, setTotal]  = useState(0);

  // filters
  const [activeFilter, setActiveFilter] = useState('All');
  const [roleFilter, setRoleFilter]     = useState('');
  const [search, setSearch]             = useState('');
  const [searchInput, setSearchInput]   = useState('');
  const [sortBy, setSortBy]             = useState('CreatedAt');
  const [sortOrder, setSortOrder]       = useState('desc');

  // ui
  const [showCreate, setShowCreate]     = useState(false);

  const STATUS_FILTERS = ['All', 'Active', 'Locked', 'Suspended', 'Deleted'];
  const ROLE_OPTIONS   = ['', 'Admin', 'Moderator', 'Tourist'];
  const SORT_OPTIONS   = [
    { value: 'CreatedAt', label: 'Date Joined' },
    { value: 'LastLoginAt', label: 'Last Login' },
    { value: 'Email', label: 'Email' },
    { value: 'LastName', label: 'Name' },
  ];

  // ── fetch users ────────────────────────────────────────────────────────────
  const fetchUsers = useCallback(async () => {
    setLoading(true);
    try {
      const params = {
        page,
        pageSize,
        sortBy,
        sortOrder,
        ...(activeFilter !== 'All' && { status: activeFilter }),
        ...(roleFilter && { role: roleFilter }),
        ...(search && { searchTerm: search }),
      };
      const data = await getUsers(params);
      const items = Array.isArray(data) ? data : data?.items ?? data?.value ?? data.data ?? [];
      setUsers(items);
      setTotal(data?.totalItems ?? items.length);
    } catch (e) {
      console.error(e.response?.data || e);
      toast('Failed to load users', 'error');
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, activeFilter, roleFilter, search, sortBy, sortOrder]);

  // ── fetch stats ────────────────────────────────────────────────────────────
  const fetchStats = useCallback(async () => {
    setStatsLoading(true);
    try {
      const data = await getUserStatistics();
      setStats(data);
    } catch (e) {
      console.error(e.response?.data || e);
    } finally {
      setStatsLoading(false);
    }
  }, []);

  useEffect(() => { fetchUsers(); }, [fetchUsers]);
  useEffect(() => { fetchStats(); }, [fetchStats]);

  // ── search on Enter ────────────────────────────────────────────────────────
  const handleSearch = (e) => {
    if (e.key === 'Enter') { setSearch(searchInput); setPage(1); }
  };

  const totalPages = Math.ceil(totalItems / pageSize);

  // ── render ─────────────────────────────────────────────────────────────────
  return (
    <div style={{ minHeight: '100vh', background: 'var(--background)' }}>

      {/* ── TOP BAR ── */}
      <div style={{
        position: 'sticky', top: 0, zIndex: 100,
        height: 64, background: 'var(--surface-container-lowest)',
        borderBottom: '1px solid var(--outline-variant)',
        display: 'flex', alignItems: 'center',
        padding: '0 28px', gap: 16,
      }}>
        <div style={{ flex: 1 }}>
          <h1 style={{
            margin: 0, fontSize: 20, fontWeight: 800,
            fontFamily: 'var(--font-display)', color: 'var(--on-surface)',
          }}>
            User Management
          </h1>
          <div style={{ fontSize: 12, color: 'var(--on-surface-variant)', marginTop: 1 }}>
            {totalItems} total users
          </div>
        </div>
        <Btn onClick={() => setShowCreate(true)}>
          ＋ New Moderator
        </Btn>
      </div>

      <div style={{ padding: '24px 28px', maxWidth: 1400, margin: '0 auto' }}>

        {/* ── STAT CARDS ── */}
        <div style={{ display: 'flex', gap: 16, marginBottom: 28, flexWrap: 'wrap' }}>
          <StatCard label="Total Users"       value={statsLoading ? '…' : stats?.totalUsers}        icon="👥" />
          <StatCard label="Active Users"      value={statsLoading ? '…' : stats?.activeUsers}       icon="✅" accent="#bbf7d0" />
          <StatCard label="Locked"            value={statsLoading ? '…' : stats?.lockedUsers}       icon="🔒" accent="#fef3c7" />
          <StatCard label="Admins"            value={statsLoading ? '…' : stats?.usersByRole?.admins}    icon="👑" accent="#ede9fe" />
          <StatCard label="Moderators"        value={statsLoading ? '…' : stats?.usersByRole?.moderators} icon="🛡" accent="#dbeafe" />
          <StatCard label="New This Month"    value={statsLoading ? '…' : stats?.usersGrowth?.newUsersThisMonth} icon="📈" accent="#d1fae5" />
        </div>

        {/* ── FILTERS ROW ── */}
        <div style={{
          background: 'var(--surface-container-lowest)',
          border: '1px solid var(--outline-variant)',
          borderRadius: 'var(--radius-xl)', padding: '16px 20px',
          marginBottom: 20, display: 'flex', gap: 16, flexWrap: 'wrap', alignItems: 'center',
        }}>
          {/* search */}
          <div style={{ position: 'relative', flex: '1 1 220px' }}>
            <span style={{
              position: 'absolute', left: 10, top: '50%', transform: 'translateY(-50%)',
              fontSize: 14, color: 'var(--on-surface-variant)',
            }}>🔍</span>
            <input
              placeholder="Search by name or email…"
              value={searchInput}
              onChange={e => setSearchInput(e.target.value)}
              onKeyDown={handleSearch}
              style={{
                width: '100%', padding: '8px 12px 8px 32px',
                borderRadius: 8, border: '1px solid var(--outline-variant)',
                background: 'var(--surface-container-low)',
                fontSize: 13, fontFamily: 'var(--font-body)',
                color: 'var(--on-surface)', outline: 'none', boxSizing: 'border-box',
              }}
            />
          </div>

          {/* role filter */}
          <select
            value={roleFilter}
            onChange={e => { setRoleFilter(e.target.value); setPage(1); }}
            style={{
              padding: '8px 12px', borderRadius: 8, fontSize: 13,
              border: '1px solid var(--outline-variant)',
              background: 'var(--surface-container-low)',
              color: 'var(--on-surface)', fontFamily: 'var(--font-body)', cursor: 'pointer',
            }}
          >
            {ROLE_OPTIONS.map(r => (
              <option key={r} value={r}>{r || 'All Roles'}</option>
            ))}
          </select>

          {/* sort */}
          <select
            value={sortBy}
            onChange={e => { setSortBy(e.target.value); setPage(1); }}
            style={{
              padding: '8px 12px', borderRadius: 8, fontSize: 13,
              border: '1px solid var(--outline-variant)',
              background: 'var(--surface-container-low)',
              color: 'var(--on-surface)', fontFamily: 'var(--font-body)', cursor: 'pointer',
            }}
          >
            {SORT_OPTIONS.map(o => (
              <option key={o.value} value={o.value}>{o.label}</option>
            ))}
          </select>

          {/* sort order */}
          <button
            onClick={() => setSortOrder(p => p === 'asc' ? 'desc' : 'asc')}
            style={{
              padding: '8px 12px', borderRadius: 8, fontSize: 13,
              border: '1px solid var(--outline-variant)',
              background: 'var(--surface-container-low)',
              color: 'var(--on-surface)', cursor: 'pointer',
            }}
          >
            {sortOrder === 'asc' ? '↑ Asc' : '↓ Desc'}
          </button>
        </div>

        {/* ── STATUS FILTER PILLS ── */}
        <div style={{ display: 'flex', gap: 8, marginBottom: 20, flexWrap: 'wrap' }}>
          {STATUS_FILTERS.map(f => (
            <FilterPill
              key={f} label={f}
              active={activeFilter === f}
              onClick={() => { setActiveFilter(f); setPage(1); }}
            />
          ))}
        </div>

        {/* ── TABLE ── */}
        <div style={{
          background: 'var(--surface-container-lowest)',
          border: '1px solid var(--outline-variant)',
          borderRadius: 'var(--radius-xl)', overflow: 'hidden',
        }}>
          {/* table header */}
          <div style={{
            display: 'grid',
            gridTemplateColumns: '2fr 1fr 1fr 1fr 1fr 120px',
            padding: '12px 20px',
            background: 'var(--surface-container-low)',
            borderBottom: '1px solid var(--outline-variant)',
            fontSize: 11, fontWeight: 700, textTransform: 'uppercase',
            letterSpacing: '.06em', color: 'var(--on-surface-variant)',
            gap: 12,
          }}>
            <span>User</span>
            <span>Role</span>
            <span>Status</span>
            <span>Email</span>
            <span>Joined</span>
            <span style={{ textAlign: 'right' }}>Actions</span>
          </div>

          {/* rows */}
          {loading ? (
            <div style={{ padding: 60, display: 'flex', justifyContent: 'center' }}>
              <Spinner />
            </div>
          ) : users.length === 0 ? (
            <div style={{
              padding: 60, textAlign: 'center',
              color: 'var(--on-surface-variant)', fontSize: 14,
            }}>
              <div style={{ fontSize: 36, marginBottom: 12 }}>👤</div>
              <div style={{ fontWeight: 600 }}>No users found</div>
              <div style={{ marginTop: 4, fontSize: 12 }}>Try adjusting your filters</div>
            </div>
          ) : (
            users.map((user, idx) => (
              <UserRow
                key={user.id}
                user={user}
                isLast={idx === users.length - 1}
                onView={() => navigate(`/users/${user.id}`)}
                onRefresh={fetchUsers}
              />
            ))
          )}
        </div>

        {/* ── PAGINATION ── */}
        {totalPages > 1 && (
          <div style={{
            display: 'flex', justifyContent: 'space-between', alignItems: 'center',
            marginTop: 20,
          }}>
            <div style={{ fontSize: 13, color: 'var(--on-surface-variant)' }}>
              Showing {((page - 1) * pageSize) + 1}–{Math.min(page * pageSize, totalItems)} of {totalItems}
            </div>
            <div style={{ display: 'flex', gap: 6 }}>
              <Btn variant="ghost" disabled={page === 1} onClick={() => setPage(p => p - 1)}>
                ← Prev
              </Btn>
              {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                let p;
                if (totalPages <= 5) p = i + 1;
                else if (page <= 3) p = i + 1;
                else if (page >= totalPages - 2) p = totalPages - 4 + i;
                else p = page - 2 + i;
                return (
                  <button key={p} onClick={() => setPage(p)} style={{
                    width: 36, height: 36, borderRadius: 8, border: 'none',
                    background: page === p
                      ? 'linear-gradient(135deg, var(--primary), var(--primary-container))'
                      : 'var(--surface-container)',
                    color: page === p ? '#fff' : 'var(--on-surface)',
                    cursor: 'pointer', fontWeight: 600, fontSize: 13,
                  }}>
                    {p}
                  </button>
                );
              })}
              <Btn variant="ghost" disabled={page === totalPages} onClick={() => setPage(p => p + 1)}>
                Next →
              </Btn>
            </div>
          </div>
        )}
      </div>

      {/* ── CREATE MODERATOR MODAL ── */}
      {showCreate && (
        <CreateModeratorModal
          onClose={() => setShowCreate(false)}
          onCreated={() => { setShowCreate(false); fetchUsers(); fetchStats(); }}
        />
      )}
    </div>
  );
}

// ─────────────────────────────────────────────────────────────────────────────
// USER ROW
// ─────────────────────────────────────────────────────────────────────────────
function UserRow({ user, isLast, onView, onRefresh }) {
  const toast = useToast();
  const [confirm, setConfirm] = useState(null);

  const fullName = user.fullName || `${user.firstName || ''} ${user.lastName || ''}`.trim() || user.email;
  const roles    = Array.isArray(user.roles) ? user.roles : [];
  const joined   = user.createdAt ? new Date(user.createdAt).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' }) : '—';
  const lastLogin = user.lastLoginAt ? new Date(user.lastLoginAt).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' }) : 'Never';

  const handleAction = async (action) => {
    try {
      const { lockUserAccount, unlockUserAccount, deleteUser } = await import('../../api/usersApi');
      if (action === 'lock')   await lockUserAccount(user.id, 'Admin action');
      if (action === 'unlock') await unlockUserAccount(user.id);
      if (action === 'delete') await deleteUser(user.id, 'Admin action');
      toast(
        action === 'lock' ? 'Account locked' :
        action === 'unlock' ? 'Account unlocked' : 'User deleted',
        'success'
      );
      onRefresh();
    } catch (e) {
      console.error(e.response?.data || e);
      toast('Action failed', 'error');
    }
  };

  return (
    <>
      <div
        onClick={onView}
        style={{
          display: 'grid',
          gridTemplateColumns: '2fr 1fr 1fr 1fr 1fr 120px',
          padding: '14px 20px', gap: 12,
          borderBottom: isLast ? 'none' : '1px solid var(--outline-variant)',
          alignItems: 'center', cursor: 'pointer',
          transition: 'background .12s',
        }}
        onMouseEnter={e => e.currentTarget.style.background = 'var(--surface-container-low)'}
        onMouseLeave={e => e.currentTarget.style.background = 'transparent'}
      >
        {/* user info */}
        <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
          <Avatar name={fullName} />
          <div>
            <div style={{ fontSize: 14, fontWeight: 600, color: 'var(--on-surface)' }}>
              {fullName}
            </div>
            <div style={{ fontSize: 12, color: 'var(--on-surface-variant)' }}>
              {user.email}
            </div>
          </div>
        </div>

        {/* roles */}
        <div>
          {roles.map(r => <RoleBadge key={r} role={r} />)}
        </div>

        {/* status */}
        <div><StatusBadge status={user.status} /></div>

        {/* email confirmed */}
        <div style={{ fontSize: 12, color: 'var(--on-surface-variant)' }}>
          {user.emailConfirmed
            ? <span style={{ color: '#065f46' }}>✓ Verified</span>
            : <span style={{ color: '#92400e' }}>✗ Unverified</span>}
        </div>

        {/* joined / last login */}
        <div>
          <div style={{ fontSize: 12, color: 'var(--on-surface)', fontWeight: 600 }}>{joined}</div>
          <div style={{ fontSize: 11, color: 'var(--on-surface-variant)', marginTop: 2 }}>Last: {lastLogin}</div>
        </div>

        {/* actions */}
        <div
          style={{ display: 'flex', gap: 6, justifyContent: 'flex-end' }}
          onClick={e => e.stopPropagation()}
        >
          <button
            title="View Details"
            onClick={onView}
            style={{
              width: 30, height: 30, borderRadius: 6, border: 'none',
              background: 'var(--surface-container)', cursor: 'pointer',
              display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 14,
            }}
          >
            👁
          </button>
          {user.status === 'Locked' ? (
            <button
              title="Unlock"
              onClick={() => setConfirm({ action: 'unlock', label: 'Unlock this account?' })}
              style={{
                width: 30, height: 30, borderRadius: 6, border: 'none',
                background: '#fef3c7', cursor: 'pointer',
                display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 14,
              }}
            >
              🔓
            </button>
          ) : user.status === 'Active' && !roles.includes('Admin') && (
            <button
              title="Lock"
              onClick={() => setConfirm({ action: 'lock', label: 'Lock this account?' })}
              style={{
                width: 30, height: 30, borderRadius: 6, border: 'none',
                background: '#fef3c7', cursor: 'pointer',
                display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 14,
              }}
            >
              🔒
            </button>
          )}
        </div>
      </div>

      {confirm && (
        <ConfirmDialog
          open
          message={confirm.label}
          onConfirm={() => { handleAction(confirm.action); setConfirm(null); }}
          onClose={() => setConfirm(null)}
        />
      )}
    </>
  );
}

// ─────────────────────────────────────────────────────────────────────────────
// CREATE MODERATOR MODAL
// ─────────────────────────────────────────────────────────────────────────────
function CreateModeratorModal({ onClose, onCreated }) {
  const toast = useToast();
  const [form, setForm] = useState({
    email: '', firstName: '', lastName: '', fullName: '',
    sendWelcomeEmail: true, requirePasswordChange: true,
  });
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState({});

  const set = (k, v) => {
    setForm(p => {
      const next = { ...p, [k]: v };
      if (k === 'fullName') {
        const parts = (v || '').trim().split(/\s+/);
        next.firstName = parts[0] || '';
        next.lastName = parts[parts.length - 1] || '';
      } else if (k === 'firstName' || k === 'lastName') {
        next.fullName = `${next.firstName || ''} ${next.lastName || ''}`.trim();
      }
      return next;
    });
    setErrors(p => ({ ...p, [k]: '' }));
  };

  const validate = () => {
    const e = {};
    if (!form.email) e.email = 'Email is required';
    else if (!/\S+@\S+\.\S+/.test(form.email)) e.email = 'Invalid email';
    if (!form.fullName) e.fullName = 'Full name is required';
    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const handleSubmit = async () => {
    if (!validate()) return;
    setLoading(true);
    try {
      const { createModerator } = await import('../../api/usersApi');
      const payload = {
        firstName: form.firstName || '',
        lastName: form.lastName || '',
        fullName: form.fullName || `${form.firstName} ${form.lastName}`.trim(),
        email: form.email,
        sendWelcomeEmail: form.sendWelcomeEmail,
        requirePasswordChange: form.requirePasswordChange,
      };
      await createModerator(payload);
      toast('Moderator created successfully', 'success');
      onCreated();
    } catch (e) {
      console.error(e.response?.data || e);
      toast(e.response?.data?.error || 'Failed to create moderator', 'error');
    } finally {
      setLoading(false);
    }
  };

  const inputStyle = (err) => ({
    width: '100%', padding: '10px 12px', borderRadius: 8, boxSizing: 'border-box',
    border: `1px solid ${err ? 'var(--error)' : 'var(--outline-variant)'}`,
    background: 'var(--surface-container-low)', fontSize: 14,
    fontFamily: 'var(--font-body)', color: 'var(--on-surface)', outline: 'none',
  });

  // use the stable FieldInput declared earlier in the file


  return (
    <div style={{
      position: 'fixed', inset: 0, zIndex: 200,
      background: 'rgba(0,0,0,.45)',
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      padding: 20,
    }}>
      <div style={{
        background: 'var(--surface-container-lowest)',
        borderRadius: 'var(--radius-xl)', width: '100%', maxWidth: 520,
        boxShadow: '0 20px 60px rgba(0,0,0,.25)',
        overflow: 'hidden',
      }}>
        {/* header */}
        <div style={{
          padding: '18px 24px', borderBottom: '1px solid var(--outline-variant)',
          display: 'flex', alignItems: 'center', justifyContent: 'space-between',
        }}>
          <div>
            <div style={{ fontSize: 16, fontWeight: 800, fontFamily: 'var(--font-display)', color: 'var(--on-surface)' }}>
              New Moderator
            </div>
            <div style={{ fontSize: 12, color: 'var(--on-surface-variant)', marginTop: 1 }}>
              Create a new moderator account
            </div>
          </div>
          <button onClick={onClose} style={{
            width: 32, height: 32, borderRadius: 8, border: 'none',
            background: 'var(--surface-container)', cursor: 'pointer', fontSize: 16,
          }}>✕</button>
        </div>

        {/* body */}
        <div style={{ padding: 24, display: 'flex', flexDirection: 'column', gap: 16 }}>
          <FieldInput label="Full Name *" value={form.fullName} onChange={v => set('fullName', v)} placeholder="Jane Smith" error={errors.fullName} />
          <FieldInput label="Email Address *" value={form.email} onChange={v => set('email', v)} type="email" placeholder="moderator@rafeeq.com" error={errors.email} />

          {/* toggles */}
          <div style={{
            background: 'var(--surface-container-low)', borderRadius: 10,
            padding: '12px 16px', display: 'flex', flexDirection: 'column', gap: 10,
          }}>
            {[
              { k: 'sendWelcomeEmail', label: 'Send welcome email' },
              { k: 'requirePasswordChange', label: 'Require password change on first login' },
            ].map(({ k, label }) => (
              <label key={k} style={{ display: 'flex', alignItems: 'center', gap: 10, cursor: 'pointer' }}>
                <div
                  onClick={() => set(k, !form[k])}
                  style={{
                    width: 38, height: 22, borderRadius: 11,
                    background: form[k] ? 'var(--primary)' : 'var(--outline-variant)',
                    position: 'relative', cursor: 'pointer', transition: 'background .2s', flexShrink: 0,
                  }}
                >
                  <div style={{
                    position: 'absolute', top: 3,
                    left: form[k] ? 19 : 3,
                    width: 16, height: 16, borderRadius: '50%',
                    background: '#fff', transition: 'left .2s',
                    boxShadow: '0 1px 3px rgba(0,0,0,.2)',
                  }} />
                </div>
                <span style={{ fontSize: 13, color: 'var(--on-surface)' }}>{label}</span>
              </label>
            ))}
          </div>
        </div>

        {/* footer */}
        <div style={{
          padding: '16px 24px', borderTop: '1px solid var(--outline-variant)',
          display: 'flex', justifyContent: 'flex-end', gap: 10,
        }}>
          <Btn variant="ghost" onClick={onClose}>Cancel</Btn>
          <Btn onClick={handleSubmit} disabled={loading}>
            {loading ? '⏳ Creating…' : '✓ Create Moderator'}
          </Btn>
        </div>
      </div>
    </div>
  );
}
