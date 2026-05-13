import React, { useState, useEffect } from 'react';
import { getFacilities, addFacilities, deleteFacilities } from '../../../api/sitesApi';
import Spinner from '../../../components/common/Spinner';
import ConfirmDialog from '../../../components/common/ConfirmDialog';
import { FACILITY_TYPES, formatEnum } from '../../../utils/constants';
import { useToast } from '../../../components/common/Toast';

/* ── Grouped by category (mirrors the UI design) ── */
const CATEGORIES = [
  {
    label: 'Visitor Services',
    items: ['restrooms', 'parking', 'cafeteriaOrRestaurant', 'giftShop', 'informationCenter', 'firstAid', 'atm', 'lockers'],
  },
  {
    label: 'Connectivity & Tech',
    items: ['wiFi', 'audioGuide'],
  },
  {
    label: 'Accessibility',
    items: ['wheelchairAccess', 'elevator', 'signLanguageTours', 'strollerAccess'],
  },
  {
    label: 'Family',
    items: ['babyChanging', 'petFriendly'],
  },
  {
    label: 'Experiences',
    items: ['guidedTours', 'photographyAllowed', 'bicycleRental', 'prayerRoom', 'galleries', 'exhibition', 'theaters', 'gardens', 'library', 'readingRooms', 'boardRides', 'localMarkets', 'natureTrails'],
  },
];

const ICONS = {
  restrooms: '🚻', parking: '🅿️', cafeteriaOrRestaurant: '🍽️', giftShop: '🛍️',
  informationCenter: 'ℹ️', firstAid: '🏥', wiFi: '📶', atm: '💳',
  prayerRoom: '🕌', wheelchairAccess: '♿', audioGuide: '🎧', lockers: '🔒',
  babyChanging: '👶', petFriendly: '🐾', photographyAllowed: '📷', guidedTours: '🗺️',
  bicycleRental: '🚲', strollerAccess: '🍼', signLanguageTours: '🤟', elevator: '🛗',
  galleries: '🖼️', exhibition: '🎨', theaters: '🎭', gardens: '🌿',
  library: '📚', readingRooms: '📖', boardRides: '⛵', localMarkets: '🛒',
  natureTrails: '🌲',
};

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

export default function FacilitiesTab({ siteId }) {
  const toast = useToast();
  const [facilities,     setFacilities]     = useState([]);
  const [loading,        setLoading]        = useState(true);
  const [saving,         setSaving]         = useState(false);
  const [deleting,       setDeleting]       = useState(false);
  const [confirmDelete,  setConfirmDelete]  = useState(null);   // single type
  const [bulkDelete,     setBulkDelete]     = useState([]);
  const [bulkDeleteOpen, setBulkDeleteOpen] = useState(false);
  const [showAdd,        setShowAdd]        = useState(false);
  const [pending,        setPending]        = useState([]);

  const load = async () => {
    try {
      setLoading(true);
      const res = await getFacilities(siteId);
      setFacilities(Array.isArray(res.data) ? res.data : res.data?.value ?? []);
    } catch { toast('Failed to load facilities', 'error'); }
    finally { setLoading(false); }
  };

  useEffect(() => { load(); }, [siteId]);

  const existing  = new Set(facilities.map(f => f.type));
  const available = FACILITY_TYPES.filter(t => !existing.has(t));

  const toggleBulk = (type) =>
    setBulkDelete(prev => prev.includes(type) ? prev.filter(t => t !== type) : [...prev, type]);

  const handleAdd = async () => {
    if (!pending.length) return;
    setSaving(true);
    try {
      await addFacilities(siteId, pending);
      toast('Facilities added', 'success');
      setPending([]); setShowAdd(false); load();
    } catch { toast('Failed to add', 'error'); }
    finally { setSaving(false); }
  };

  const handleDelete = async () => {
    setDeleting(true);
    try {
      await deleteFacilities(siteId, [confirmDelete]);
      toast('Facility removed', 'success');
      setBulkDelete(prev => prev.filter(t => t !== confirmDelete));
      setConfirmDelete(null); load();
    } catch { toast('Failed to delete', 'error'); }
    finally { setDeleting(false); }
  };

  const handleBulkDelete = async () => {
    setDeleting(true);
    try {
      await deleteFacilities(siteId, bulkDelete);
      toast(`Removed ${bulkDelete.length} facilit${bulkDelete.length !== 1 ? 'ies' : 'y'}`, 'success');
      setBulkDelete([]); setBulkDeleteOpen(false); load();
    } catch { toast('Failed', 'error'); }
    finally { setDeleting(false); }
  };

  /* ── Render ── */
  if (loading) return <Spinner center />;

  return (
    <div>
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24 }}>
        <div>
          <h2 style={{ fontFamily: 'var(--font-display)', fontSize: 22, color: 'var(--primary)', margin: '0 0 2px' }}>
            Facilities
          </h2>
          <p style={{ fontSize: 12, color: 'var(--outline)', margin: 0 }}>
            {facilities.length} facilit{facilities.length !== 1 ? 'ies' : 'y'} enabled
          </p>
        </div>
        <div style={{ display: 'flex', gap: 8 }}>
          {bulkDelete.length > 0 && (
            <Btn variant="danger" size="sm" onClick={() => setBulkDeleteOpen(true)}>
              Remove ({bulkDelete.length})
            </Btn>
          )}
          {available.length > 0 && (
            <Btn size="sm" onClick={() => { setShowAdd(!showAdd); setPending([]); }}>
              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M12 5v14M5 12h14"/></svg>
              Add Facilities
            </Btn>
          )}
        </div>
      </div>

      {/* Categorised grid */}
      {CATEGORIES.map(cat => {
        const catItems = cat.items.filter(t => existing.has(t));
        if (catItems.length === 0) return null;
        return (
          <section key={cat.label} style={{ marginBottom: 28 }}>
            {/* Category divider */}
            <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: 14 }}>
              <span style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.09em', textTransform: 'uppercase', color: 'var(--outline)', whiteSpace: 'nowrap' }}>
                {cat.label}
              </span>
              <div style={{ flex: 1, height: 1, background: 'rgba(212,196,183,.35)' }} />
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(160px, 1fr))', gap: 12 }}>
              {catItems.map(type => (
                <div key={type} style={{
                  background: 'var(--surface-container-lowest)',
                  borderRadius: 12, padding: '16px 14px',
                  boxShadow: '0 1px 4px rgba(29,27,23,.05)',
                  position: 'relative', overflow: 'hidden',
                  border: bulkDelete.includes(type) ? '2px solid var(--error)' : '2px solid transparent',
                  transition: 'all .15s',
                }}>
                  {/* Pyramid accent */}
                  <div style={{
                    position: 'absolute', top: 0, right: 0,
                    width: 0, height: 0, borderStyle: 'solid',
                    borderWidth: '0 24px 24px 0',
                    borderColor: 'transparent rgba(212,165,116,.2) transparent transparent',
                  }} />

                  {/* Checkbox + icon row */}
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 10 }}>
                    <span style={{ fontSize: 28, lineHeight: 1 }}>{ICONS[type] || '✓'}</span>
                    <input
                      type="checkbox"
                      checked={bulkDelete.includes(type)}
                      onChange={() => toggleBulk(type)}
                      style={{ width: 15, height: 15, accentColor: 'var(--error)', cursor: 'pointer' }}
                    />
                  </div>

                  <div style={{ fontWeight: 700, fontSize: 13, color: 'var(--text)', marginBottom: 4 }}>
                    {formatEnum(type)}
                  </div>

                  {/* Delete single */}
                  <button
                    onClick={() => setConfirmDelete(type)}
                    style={{
                      background: 'none', border: 'none', cursor: 'pointer', padding: 0,
                      fontSize: 11, color: 'var(--outline)', fontFamily: 'var(--font-body)',
                      display: 'flex', alignItems: 'center', gap: 4, marginTop: 4,
                    }}
                    onMouseOver={e => e.currentTarget.style.color = 'var(--error)'}
                    onMouseOut={e => e.currentTarget.style.color = 'var(--outline)'}
                  >
                    <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                      <polyline points="3 6 5 6 21 6"/><path d="M19 6l-1 14H6L5 6"/><path d="M9 6V4h6v2"/>
                    </svg>
                    Remove
                  </button>
                </div>
              ))}
            </div>
          </section>
        );
      })}

      {/* Remaining items not in any category */}
      {(() => {
        const categorised = new Set(CATEGORIES.flatMap(c => c.items));
        const uncategorised = facilities.filter(f => !categorised.has(f.type));
        if (!uncategorised.length) return null;
        return (
          <section style={{ marginBottom: 28 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: 14 }}>
              <span style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.09em', textTransform: 'uppercase', color: 'var(--outline)', whiteSpace: 'nowrap' }}>Other</span>
              <div style={{ flex: 1, height: 1, background: 'rgba(212,196,183,.35)' }} />
            </div>
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: 8 }}>
              {uncategorised.map(f => (
                <span key={f.type} style={{
                  display: 'inline-flex', alignItems: 'center', gap: 7,
                  padding: '7px 12px', borderRadius: 20,
                  background: 'var(--surface-container-lowest)',
                  boxShadow: '0 1px 3px rgba(29,27,23,.04)',
                  fontSize: 12, fontWeight: 600, color: 'var(--text)',
                }}>
                  {ICONS[f.type] || '✓'} {formatEnum(f.type)}
                  <button onClick={() => setConfirmDelete(f.type)} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'var(--outline)', display: 'flex', padding: 0 }}
                    onMouseOver={e => e.currentTarget.style.color = 'var(--error)'}
                    onMouseOut={e => e.currentTarget.style.color = 'var(--outline)'}>
                    <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3"><path d="M18 6L6 18M6 6l12 12"/></svg>
                  </button>
                </span>
              ))}
            </div>
          </section>
        );
      })()}

      {facilities.length === 0 && (
        <div style={{ padding: '56px 0', textAlign: 'center', color: 'var(--outline)', fontSize: 14 }}>
          No facilities added yet.
        </div>
      )}

      {/* Add panel */}
      {showAdd && available.length > 0 && (
        <div style={{
          marginTop: 8, background: 'var(--surface-container-lowest)',
          borderRadius: 14, padding: 20,
          boxShadow: '0 2px 12px rgba(29,27,23,.08)',
          borderLeft: '3px solid var(--primary)',
        }}>
          <div style={{ fontSize: 12, fontWeight: 700, color: 'var(--outline)', textTransform: 'uppercase', letterSpacing: '0.07em', marginBottom: 14 }}>
            Select facilities to add
          </div>
          <div style={{ display: 'flex', flexWrap: 'wrap', gap: 8, marginBottom: 16 }}>
            {available.map(t => (
              <button key={t} onClick={() => setPending(prev => prev.includes(t) ? prev.filter(x => x !== t) : [...prev, t])} style={{
                padding: '7px 13px', borderRadius: 20, fontSize: 12, cursor: 'pointer',
                border: `1px solid ${pending.includes(t) ? 'var(--primary)' : 'rgba(212,196,183,.5)'}`,
                background: pending.includes(t) ? 'rgba(124,87,45,.07)' : 'transparent',
                color: pending.includes(t) ? 'var(--primary)' : 'var(--text-2)',
                fontWeight: pending.includes(t) ? 700 : 500, transition: '.1s',
                fontFamily: 'var(--font-body)',
              }}>
                {ICONS[t] || '•'} {formatEnum(t)}
              </button>
            ))}
          </div>
          <div style={{ display: 'flex', gap: 10 }}>
            <Btn variant="ghost" size="sm" onClick={() => setShowAdd(false)}>Cancel</Btn>
            <Btn size="sm" onClick={handleAdd} loading={saving} disabled={!pending.length}>
              Add {pending.length > 0 ? `(${pending.length})` : ''}
            </Btn>
          </div>
        </div>
      )}

      <ConfirmDialog
        open={!!confirmDelete} onClose={() => setConfirmDelete(null)}
        onConfirm={handleDelete} loading={deleting}
        title="Remove Facility" message={`Remove "${formatEnum(confirmDelete)}" from this site?`}
        confirmLabel="Remove"
      />
      <ConfirmDialog
        open={bulkDeleteOpen} onClose={() => setBulkDeleteOpen(false)}
        onConfirm={handleBulkDelete} loading={deleting}
        title="Remove Facilities" message={`Remove ${bulkDelete.length} facilit${bulkDelete.length !== 1 ? 'ies' : 'y'} from this site?`}
        confirmLabel="Remove"
      />
    </div>
  );
}
