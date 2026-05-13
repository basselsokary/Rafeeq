import React, { useState, useEffect } from 'react';
import { getOpeningHours, addOpeningHours, deleteOpeningHours } from '../../../api/sitesApi';
import Spinner from '../../../components/common/Spinner';
import ConfirmDialog from '../../../components/common/ConfirmDialog';
import { WEEKDAYS, formatEnum } from '../../../utils/constants';
import { useToast } from '../../../components/common/Toast';

const DAY_ORDER = { saturday: 0, sunday: 1, monday: 2, tuesday: 3, wednesday: 4, thursday: 5, friday: 6 };
const EMPTY = { day: 'saturday', startTime: '09:00:00', endTime: '17:00:00', isClosed: false };

const toEnum = (val) =>
  typeof val === 'string' && val.length ? val.charAt(0).toUpperCase() + val.slice(1) : val;

function Btn({ children, onClick, variant = 'primary', size = 'md', loading = false, disabled = false }) {
  const sm = size === 'sm';
  const v = {
    primary:   { background: 'linear-gradient(135deg, var(--primary), var(--primary-container))', color: '#fff', boxShadow: '0 2px 8px rgba(124,87,45,.2)' },
    secondary: { background: 'var(--surface-container-lowest)', color: 'var(--text-2)', boxShadow: '0 0 0 1px rgba(212,196,183,.5)' },
    ghost:     { background: 'transparent', color: 'var(--text-2)' },
    danger:    { background: 'rgba(186,26,26,.07)', color: 'var(--error)', boxShadow: '0 0 0 1px rgba(186,26,26,.2)' },
  };
  return (
    <button onClick={onClick} disabled={disabled || loading} style={{
      display: 'inline-flex', alignItems: 'center', gap: 6,
      padding: sm ? '6px 14px' : '9px 20px',
      fontSize: sm ? 12 : 13, fontWeight: 700,
      borderRadius: sm ? 8 : 10, border: 'none',
      cursor: disabled || loading ? 'not-allowed' : 'pointer',
      opacity: disabled ? 0.5 : 1, transition: 'all .15s',
      fontFamily: 'var(--font-body)', whiteSpace: 'nowrap',
      ...v[variant],
    }}>
      {loading ? <Spinner size={12} /> : null}{children}
    </button>
  );
}

export default function OpeningHoursTab({ siteId }) {
  const toast = useToast();
  const [hours,          setHours]          = useState([]);
  const [loading,        setLoading]        = useState(true);
  const [saving,         setSaving]         = useState(false);
  const [deleteDay,      setDeleteDay]      = useState(null);
  const [deleting,       setDeleting]       = useState(false);
  const [bulkDelete,     setBulkDelete]     = useState([]);
  const [bulkDeleteOpen, setBulkDeleteOpen] = useState(false);
  const [showAdd,        setShowAdd]        = useState(false);
  const [form,           setForm]           = useState(EMPTY);

  const load = async () => {
    try {
      setLoading(true);
      const res = await getOpeningHours(siteId);
      setHours(Array.isArray(res.data) ? res.data : res.data?.value ?? []);
    } catch { toast('Failed to load opening hours', 'error'); }
    finally { setLoading(false); }
  };

  useEffect(() => { load(); }, [siteId]);

  const configured = new Set(hours.map(h => h.day));
  const remaining  = WEEKDAYS.filter(d => !configured.has(d));
  const sorted     = [...hours].sort((a, b) => (DAY_ORDER[a.day] ?? 9) - (DAY_ORDER[b.day] ?? 9));

  const handleAdd = async () => {
    setSaving(true);
    try {
      await addOpeningHours(siteId, [form]);
      toast('Hours added', 'success');
      setHours(prev => [...prev, form]);
      setShowAdd(false);
      setForm(EMPTY);
    } catch { toast('Failed to save', 'error'); }
    finally { setSaving(false); }
  };

  const handleDelete = async () => {
    setDeleting(true);
    try {
      await deleteOpeningHours(siteId, [toEnum(deleteDay)]);
      toast('Day removed', 'success');
      setHours(prev => prev.filter(h => h.day !== deleteDay));
      setBulkDelete(prev => prev.filter(d => d !== deleteDay));
      setDeleteDay(null);
    } catch { toast('Failed to remove', 'error'); }
    finally { setDeleting(false); }
  };

  const handleBulkDelete = async () => {
    setDeleting(true);
    try {
      await deleteOpeningHours(siteId, bulkDelete.map(toEnum));
      toast(`Removed ${bulkDelete.length} day${bulkDelete.length !== 1 ? 's' : ''}`, 'success');
      setHours(prev => prev.filter(h => !bulkDelete.includes(h.day)));
      setBulkDelete([]); setBulkDeleteOpen(false);
    } catch { toast('Failed', 'error'); }
    finally { setDeleting(false); }
  };

  const toggleBulk = (day) =>
    setBulkDelete(prev => prev.includes(day) ? prev.filter(d => d !== day) : [...prev, day]);

  const inputStyle = {
    width: '100%', padding: '9px 13px', fontSize: 13, color: 'var(--text)',
    background: 'var(--surface-container-low)',
    border: '1px solid rgba(212,196,183,.3)', borderRadius: 10,
    outline: 'none', fontFamily: 'var(--font-body)',
  };

  const selectStyle = {
    ...inputStyle,
    appearance: 'none',
    backgroundImage: `url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='%23827569' stroke-width='2'%3E%3Cpath d='M6 9l6 6 6-6'/%3E%3C/svg%3E")`,
    backgroundRepeat: 'no-repeat', backgroundPosition: 'right 10px center', paddingRight: 32,
  };

  const STATUS_COLORS = {
    open: { bg: 'rgba(56,106,32,.07)', text: '#386a20', dot: '#386a20' },
    closed: { bg: 'rgba(186,26,26,.07)', text: 'var(--error)', dot: 'var(--error)' },
  };

  return (
    <div style={{ maxWidth: 680 }}>
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24 }}>
        <div>
          <h2 style={{ fontFamily: 'var(--font-display)', fontSize: 22, color: 'var(--primary)', margin: '0 0 2px' }}>
            Opening Hours
          </h2>
          <p style={{ fontSize: 12, color: 'var(--outline)', margin: 0 }}>
            {hours.length} day{hours.length !== 1 ? 's' : ''} configured
          </p>
        </div>
        <div style={{ display: 'flex', gap: 8 }}>
          {bulkDelete.length > 0 && (
            <Btn variant="danger" size="sm" onClick={() => setBulkDeleteOpen(true)}>
              Delete ({bulkDelete.length})
            </Btn>
          )}
          {remaining.length > 0 && (
            <Btn size="sm" onClick={() => { setShowAdd(true); setForm({ ...EMPTY, day: remaining[0] }); }}>
              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M12 5v14M5 12h14"/></svg>
              Add Day
            </Btn>
          )}
        </div>
      </div>

      {/* Days list */}
      {loading ? <Spinner center /> : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 8, marginBottom: 20 }}>
          {sorted.map(h => {
            const isClosed = h.isClosed;
            const sc = STATUS_COLORS[isClosed ? 'closed' : 'open'];
            return (
              <div key={h.day} style={{
                display: 'flex', alignItems: 'center', gap: 14, padding: '14px 18px',
                background: 'var(--surface-container-lowest)',
                borderRadius: 12, boxShadow: '0 1px 3px rgba(29,27,23,.04)',
                transition: 'box-shadow .15s',
              }}>
                <input
                  type="checkbox" checked={bulkDelete.includes(h.day)} onChange={() => toggleBulk(h.day)}
                  style={{ width: 15, height: 15, accentColor: 'var(--primary)', cursor: 'pointer', flexShrink: 0 }}
                />
                <span style={{ width: 90, flexShrink: 0, fontWeight: 700, color: 'var(--text)', fontSize: 13 }}>
                  {formatEnum(h.day)}
                </span>
                <span style={{
                  fontSize: 11, fontWeight: 700, padding: '3px 10px', borderRadius: 20,
                  background: sc.bg, color: sc.text,
                  display: 'flex', alignItems: 'center', gap: 5,
                }}>
                  <span style={{ width: 6, height: 6, borderRadius: '50%', background: sc.dot }} />
                  {h.isClosed ? 'Closed' : `${h.openTime?.slice(0, 5) || h.startTime?.slice(0, 5) || '—'} – ${h.closeTime?.slice(0, 5) || h.endTime?.slice(0, 5) || '—'}`}
                </span>
                <button
                  onClick={() => setDeleteDay(h.day)}
                  style={{
                    marginLeft: 'auto', background: 'none', border: 'none', cursor: 'pointer',
                    color: 'var(--outline)', display: 'flex', padding: 4, borderRadius: 6, transition: '.15s',
                  }}
                  onMouseOver={e => { e.currentTarget.style.background = 'rgba(186,26,26,.08)'; e.currentTarget.style.color = 'var(--error)'; }}
                  onMouseOut={e => { e.currentTarget.style.background = 'none'; e.currentTarget.style.color = 'var(--outline)'; }}
                >
                  <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                    <polyline points="3 6 5 6 21 6"/><path d="M19 6l-1 14H6L5 6"/><path d="M9 6V4h6v2"/>
                  </svg>
                </button>
              </div>
            );
          })}
          {hours.length === 0 && (
            <div style={{ padding: '48px 0', textAlign: 'center', color: 'var(--outline)', fontSize: 14 }}>
              No opening hours configured yet.
            </div>
          )}
        </div>
      )}

      {/* Add form */}
      {showAdd && (
        <div style={{
          background: 'var(--surface-container-lowest)',
          borderRadius: 14, padding: 22,
          boxShadow: '0 2px 12px rgba(29,27,23,.08)',
          borderLeft: '3px solid var(--primary)',
        }}>
          <h4 style={{ fontFamily: 'var(--font-display)', fontSize: 15, color: 'var(--primary)', margin: '0 0 18px' }}>
            Add Opening Hours
          </h4>

          <div style={{ marginBottom: 14 }}>
            <label style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', color: 'var(--outline)', display: 'block', marginBottom: 6 }}>
              Day of Week
            </label>
            <select value={form.day} onChange={e => setForm(f => ({ ...f, day: e.target.value }))} style={selectStyle}>
              {remaining.map(d => <option key={d} value={d}>{formatEnum(d)}</option>)}
            </select>
          </div>

          <label style={{
            display: 'flex', alignItems: 'center', gap: 9, marginBottom: 14,
            fontSize: 13, color: 'var(--text-2)', cursor: 'pointer', fontWeight: 500,
          }}>
            <input
              type="checkbox" checked={form.isClosed} onChange={e => setForm(f => ({ ...f, isClosed: e.target.checked }))}
              style={{ width: 15, height: 15, accentColor: 'var(--primary)', cursor: 'pointer' }}
            />
            Mark as closed on this day
          </label>

          {!form.isClosed && (
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12, marginBottom: 18 }}>
              <div>
                <label style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', color: 'var(--outline)', display: 'block', marginBottom: 6 }}>Opens</label>
                <input type="time" value={form.startTime?.slice(0, 5)} onChange={e => setForm(f => ({ ...f, startTime: e.target.value + ':00' }))} style={inputStyle} />
              </div>
              <div>
                <label style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', color: 'var(--outline)', display: 'block', marginBottom: 6 }}>Closes</label>
                <input type="time" value={form.endTime?.slice(0, 5)} onChange={e => setForm(f => ({ ...f, endTime: e.target.value + ':00' }))} style={inputStyle} />
              </div>
            </div>
          )}

          <div style={{ display: 'flex', gap: 10 }}>
            <Btn variant="ghost" size="sm" onClick={() => setShowAdd(false)} disabled={saving}>Cancel</Btn>
            <Btn size="sm" onClick={handleAdd} loading={saving}>Save</Btn>
          </div>
        </div>
      )}

      <ConfirmDialog
        open={!!deleteDay} onClose={() => setDeleteDay(null)}
        onConfirm={handleDelete} loading={deleting}
        title="Remove Day" message={`Remove ${formatEnum(deleteDay)} from the opening hours?`}
        confirmLabel="Remove"
      />
      <ConfirmDialog
        open={bulkDeleteOpen} onClose={() => setBulkDeleteOpen(false)}
        onConfirm={handleBulkDelete} loading={deleting}
        title="Remove Days" message={`Remove ${bulkDelete.length} day${bulkDelete.length !== 1 ? 's' : ''} from the opening hours?`}
        confirmLabel="Remove"
      />
    </div>
  );
}
