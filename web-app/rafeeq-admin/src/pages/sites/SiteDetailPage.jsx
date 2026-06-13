import React, { useState, useEffect, useCallback, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getSiteById, updateSite, deleteSite, updateSiteStatus } from '../../api/sitesApi';
import FacilitiesTab from './components/FacilitiesTab';
import LocalizedContentsTab from './components/LocalizedContentsTab';
import OpeningHoursTab from './components/OpeningHoursTab';
import ImagesTab from './components/ImagesTab';
import Modal from '../../components/common/Modal';
import ConfirmDialog from '../../components/common/ConfirmDialog';
import Spinner from '../../components/common/Spinner';
import SiteForm from './components/SiteForm';
import { useToast } from '../../components/common/Toast';
import { STATUS_LABELS, formatEnum } from '../../utils/constants';

const STATUS_COLOR = {
  active:            '#386a20',
  underMaintenance:  '#d97706',
  temporarilyClosed: '#ba1a1a',
  permanentlyClosed: '#827569',
};

const TABS = [
  { id: 'basic',      label: 'Basic Info'     },
  { id: 'localize',   label: 'Localization'   },
  { id: 'images',     label: 'Images & Media' },
  { id: 'hours',      label: 'Opening Hours'  },
  { id: 'transport',  label: 'Transportation' },
  { id: 'facilities', label: 'Facilities'     },
];

/* ── shared button atom ── */
function Btn({ children, onClick, variant = 'primary', size = 'md', loading = false, disabled = false, type = 'button', style: s = {} }) {
  const pad  = size === 'sm' ? '6px 14px' : '9px 20px';
  const fz   = size === 'sm' ? 12 : 13;
  const base = {
    display: 'inline-flex', alignItems: 'center', gap: 7,
    cursor: disabled || loading ? 'not-allowed' : 'pointer',
    border: 'none', borderRadius: size === 'sm' ? 8 : 10,
    fontFamily: 'var(--font-body)', fontWeight: 700, fontSize: fz, padding: pad,
    opacity: disabled ? 0.5 : 1, transition: 'all .15s', ...s,
  };
  const v = {
    primary:   { background: 'linear-gradient(135deg, var(--primary), var(--primary-container))', color: '#fff', boxShadow: '0 2px 8px rgba(124,87,45,.25)' },
    secondary: { background: 'var(--surface-container-lowest)', color: 'var(--text-2)', boxShadow: '0 0 0 1px rgba(212,196,183,.5)' },
    ghost:     { background: 'transparent', color: 'var(--text-2)' },
    danger:    { background: 'rgba(186,26,26,.07)', color: 'var(--error)', boxShadow: '0 0 0 1px rgba(186,26,26,.2)' },
  };
  return (
    <button type={type} onClick={onClick} disabled={disabled || loading} style={{ ...base, ...v[variant] }}>
      {loading ? <Spinner size={13} /> : null}{children}
    </button>
  );
}

/* ── Status toggle ── */
function StatusToggle({ on, onClick, disabled }) {
  return (
    <button type="button" onClick={onClick} disabled={disabled} style={{
      width: 40, height: 22, borderRadius: 11, border: 'none',
      cursor: disabled ? 'not-allowed' : 'pointer', opacity: disabled ? 0.6 : 1,
      background: on
        ? 'linear-gradient(135deg, var(--primary), var(--primary-container))'
        : 'var(--surface-container-high)',
      position: 'relative', boxShadow: on ? '0 2px 6px rgba(124,87,45,.3)' : 'none',
      transition: 'all .2s',
    }}>
      <span style={{
        width: 16, height: 16, borderRadius: '50%', background: '#fff',
        position: 'absolute', top: 3, left: on ? 21 : 3,
        boxShadow: '0 1px 3px rgba(0,0,0,.2)', transition: 'left .18s',
      }} />
    </button>
  );
}

/* ── Info row ── */
function InfoRow({ label, value }) {
  if (value == null || value === '') return null;
  return (
    <div style={{
      display: 'flex', justifyContent: 'space-between', alignItems: 'baseline',
      padding: '11px 18px',
      borderBottom: '1px solid rgba(212,196,183,.2)',
    }}>
      <span style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', color: 'var(--outline)', flexShrink: 0, marginRight: 20 }}>
        {label}
      </span>
      <span style={{ fontSize: 13, color: 'var(--text)', textAlign: 'right' }}>{String(value)}</span>
    </div>
  );
}

/* ── Basic Info Tab ── */
function BasicInfoTab({
  site,
  onEdit,
  statusForm,
  setStatusForm,
  onStatusSave,
  statusSaving,
  statusDirty,
}) {
  const [showFullDesc, setShowFullDesc] = useState(false);
  const [canExpandDesc, setCanExpandDesc] = useState(false);
  const descRef = useRef(null);

  useEffect(() => {
    setShowFullDesc(false);
  }, [site.description]);

  useEffect(() => {
    if (!descRef.current || showFullDesc) return;
    const el = descRef.current;
    setCanExpandDesc(el.scrollHeight > el.clientHeight + 1);
  }, [site.description, showFullDesc]);
  return (
    <div style={{ maxWidth: 1000 }}>
      <div style={{ display: 'flex', alignItems: 'flex-start', gap: 24, flexWrap: 'wrap' }}>
        <div style={{ flex: '1 1 520px', minWidth: 320 }}>
          {/* Cover image */}
          <div style={{ borderRadius: 16, overflow: 'hidden', marginBottom: 20, background: 'var(--surface-container)', aspectRatio: '21/9', position: 'relative' }}>
            {site.mainImageUrl
              ? <img src={site.mainImageUrl} alt={site.name} style={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block' }} />
              : <div style={{ width: '100%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--outline)', fontSize: 13 }}>No cover image</div>
            }
            {/* Pyramid accent */}
            <div style={{
              position: 'absolute', top: 0, right: 0,
              width: 0, height: 0, borderStyle: 'solid',
              borderWidth: '0 40px 40px 0',
              borderColor: `transparent rgba(124,87,45,0.2) transparent transparent`,
            }} />
          </div>

          {/* Description */}
          {site.description && (
            <div style={{
              color: 'var(--text-2)', fontSize: 14, lineHeight: 1.75, marginBottom: 20,
              background: 'var(--surface-container-low)',
              borderRadius: 12, padding: '14px 18px',
            }}>
              <p ref={descRef} style={{
                margin: 0,
                display: showFullDesc ? 'block' : '-webkit-box',
                WebkitLineClamp: showFullDesc ? 'unset' : 3,
                WebkitBoxOrient: 'vertical',
                overflow: 'hidden',
              }}>
                {site.description}
              </p>
              {canExpandDesc && (
                <button type="button" onClick={() => setShowFullDesc(s => !s)} style={{
                  marginTop: 8,
                  background: 'none', border: 'none', padding: 0,
                  color: 'var(--primary)', fontSize: 12, fontWeight: 600, cursor: 'pointer',
                }}>
                  {showFullDesc ? 'Show Less' : 'Show More'}
                </button>
              )}
            </div>
          )}

          {/* Info card */}
          <div style={{
            background: 'var(--surface-container-lowest)',
            borderRadius: 14, overflow: 'hidden', marginBottom: 20,
            boxShadow: '0 1px 4px rgba(29,27,23,.05)',
          }}>
            <InfoRow label="City"             value={site.cityName} />
            <InfoRow label="Address"          value={site.address} />
            <InfoRow label="Status"           value={STATUS_LABELS[site.status] || site.status} />
            <InfoRow label="Featured"         value={site.isFeatured ? 'Yes' : 'No'} />
            <InfoRow label="Hidden Gem"       value={site.isHiddenGem ? 'Yes' : 'No'} />
            <InfoRow label="Popular"          value={site.isPopular ? 'Yes' : 'No'} />
            <InfoRow label="Type"             value={formatEnum(site.type)} />
            <InfoRow label="Free Entry"       value={site.isFree ? 'Yes' : 'No'} />
            <InfoRow
              label="Entry Fee"
              value={(() => {
                const egyptian = site.entryTicket?.egyptianTicketPrice;
                const foreigner = site.entryTicket?.foreingerTicketPrice;
                if (!egyptian && !foreigner) return site.isFree ? null : 'Not provided';
                const parts = [];
                if (egyptian) {
                  parts.push(egyptian.formattedAmount ?? `${egyptian.currency ?? 'EGP'} ${egyptian.amount}`);
                }
                if (foreigner) {
                  parts.push(foreigner.formattedAmount ?? `${foreigner.currency ?? 'USD'} ${foreigner.amount}`);
                }
                return parts.join(' / ');
              })()}
            />
            <InfoRow label="Est. Duration"    value={site.estimatedDurationMinutes ? `${site.estimatedDurationMinutes} minutes` : null} />
            <InfoRow label="Phone"            value={site.contactPhone} />
            <InfoRow label="Website"          value={site.website} />
            <InfoRow label="Coordinates"      value={site.location ? `${site.location.latitude}, ${site.location.longitude}` : null} />
            <InfoRow label="Rating"           value={site.averageRating ? `${site.averageRating.toFixed(1)} / 5  (${site.totalRatings} reviews)` : null} />

            {site.auditInfo && (
              <>
                <InfoRow label="Created By"    value={`${site.auditInfo.createdByName || ''} — ${new Date(site.auditInfo.createdAt).toLocaleString()}`} />
                {site.auditInfo.lastModifiedAt && (
                  <InfoRow label="Last Modified" value={`${site.auditInfo.lastModifiedByName || ''} — ${new Date(site.auditInfo.lastModifiedAt).toLocaleString()}`} />
                )}
              </>
            )}

            {/* last row has no border */}
            <div style={{ height: 1 }} />
          </div>

          <Btn onClick={onEdit}>
            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/>
              <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4z"/>
            </svg>
            Edit Basic Info
          </Btn>
        </div>

        {/* Status controls */}
        <div style={{
          flex: '0 0 280px',
          minWidth: 240,
          background: 'var(--surface-container-lowest)',
          borderRadius: 14,
          padding: 16,
          boxShadow: '0 1px 4px rgba(29,27,23,.05)',
        }}>
          <div style={{ fontSize: 11, fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', color: 'var(--outline)', marginBottom: 10 }}>
            Status & Flags
          </div>

          <div style={{ marginBottom: 12 }}>
            <div style={{ fontSize: 12, color: 'var(--text-2)', marginBottom: 6 }}>Status</div>
            <select
              value={statusForm.status}
              onChange={e => setStatusForm(s => ({ ...s, status: e.target.value }))}
              style={{
                width: '100%',
                padding: '8px 10px',
                borderRadius: 10,
                border: '1px solid rgba(212,196,183,.5)',
                background: 'var(--surface-container-high)',
                color: 'var(--text)',
                fontSize: 13,
                outline: 'none',
              }}
            >
              {Object.entries(STATUS_LABELS).map(([value, label]) => (
                <option key={value} value={value}>{label}</option>
              ))}
            </select>
          </div>

          <div style={{ display: 'grid', gap: 10, marginBottom: 14 }}>
            <label style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', fontSize: 13, color: 'var(--text)' }}>
              <span>Featured</span>
              <StatusToggle
                on={statusForm.isFeatured}
                onClick={() => setStatusForm(s => ({ ...s, isFeatured: !s.isFeatured }))}
                disabled={statusSaving}
              />
            </label>
            <label style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', fontSize: 13, color: 'var(--text)' }}>
              <span>Hidden Gem</span>
              <StatusToggle
                on={statusForm.isHiddenGem}
                onClick={() => setStatusForm(s => ({ ...s, isHiddenGem: !s.isHiddenGem }))}
                disabled={statusSaving}
              />
            </label>
          </div>

          <Btn
            size="sm"
            onClick={onStatusSave}
            loading={statusSaving}
            disabled={!statusDirty}
            style={{ width: '100%', justifyContent: 'center' }}
          >
            Save Status
          </Btn>
        </div>
      </div>
    </div>
  );
}

/* ── Main page ── */
export default function SiteDetailPage() {
  const { id }   = useParams();
  const navigate = useNavigate();
  const toast    = useToast();

  const [site,         setSite]         = useState(null);
  const [loading,      setLoading]      = useState(true);
  const [tab,          setTab]          = useState('basic');
  const [editOpen,     setEditOpen]     = useState(false);
  const [delOpen,      setDelOpen]      = useState(false);
  const [saving,       setSaving]       = useState(false);
  const [statusSaving, setStatusSaving] = useState(false);
  const [deleting,     setDeleting]     = useState(false);
  const [statusForm,   setStatusForm]   = useState({
    status: 'active',
    isFeatured: false,
    isHiddenGem: false,
  });

  const load = useCallback(async () => {
    try {
      setLoading(true);
      const res = await getSiteById(id);
      setSite(res.data?.value ?? res.data);
    } catch (e) {
      toast(e.response?.data.detail || 'Failed to load site', 'error');
    }
    finally { setLoading(false); }
  }, [id]);

  useEffect(() => { load(); }, [load]);

  useEffect(() => {
    if (!site) return;
    setStatusForm({
      status: site.status || 'active',
      isFeatured: !!site.isFeatured,
      isHiddenGem: !!site.isHiddenGem,
    });
  }, [site]);

  const handleUpdate = async (payload) => {
    setSaving(true);
    try {
      await updateSite(id, payload);
      toast('Saved', 'success');
      setEditOpen(false);
      load();
    } catch (e) {
      console.error('Update failed:', e.response?.data || e);
      toast('Update failed', 'error');
    } finally { setSaving(false); }
  };

  const handleDelete = async () => {
    setDeleting(true);
    try { await deleteSite(id); toast('Site deleted', 'success'); navigate('/sites'); }
    catch { toast('Delete failed', 'error'); }
    finally { setDeleting(false); }
  };

  const handleStatusSave = async () => {
    if (!site) return;
    setStatusSaving(true);
    try {
      await updateSiteStatus(id, {
        status: statusForm.status,
        isFeatured: statusForm.isFeatured,
        isHiddenGem: statusForm.isHiddenGem,
      });
      setSite(s => ({
        ...s,
        status: statusForm.status,
        isFeatured: statusForm.isFeatured,
        isHiddenGem: statusForm.isHiddenGem,
      }));
      toast('Status updated', 'success');
    } catch (e) {
      console.error('Status update failed:', e.response?.data || e);
      toast('Status update failed', 'error');
    } finally { setStatusSaving(false); }
  };

  if (loading) return (
    <div style={{ minHeight: '100vh', background: 'var(--background)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <Spinner size={36} />
    </div>
  );

  if (!site) return (
    <div style={{ minHeight: '100vh', background: 'var(--background)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--outline)' }}>
      Site not found.
    </div>
  );

  const statusColor = STATUS_COLOR[site.status] || 'var(--outline)';
  const statusDirty = (
    statusForm.status !== site.status
    || statusForm.isFeatured !== !!site.isFeatured
    || statusForm.isHiddenGem !== !!site.isHiddenGem
  );

  return (
    <div style={{ minHeight: '100vh', background: 'var(--background)', fontFamily: 'var(--font-body)', display: 'flex', flexDirection: 'column' }}>

      <div style={{ padding: '24px 32px 0', display: 'flex', alignItems: 'center', gap: 20, flexWrap: 'wrap' }}>
        <button onClick={() => navigate('/sites')} style={{
          display: 'flex', alignItems: 'center', gap: 6,
          background: 'none', border: 'none', cursor: 'pointer',
          color: 'var(--text-2)', fontSize: 13, fontWeight: 500, padding: 0,
        }}>
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
            <path d="M19 12H5M12 5l-7 7 7 7"/>
          </svg>
          Back to Sites
        </button>

        <div style={{ width: 1, height: 24, background: 'rgba(212,196,183,.5)' }} />

        <div style={{ display: 'flex', alignItems: 'center', gap: 10, flex: 1, minWidth: 0 }}>
          <span style={{ fontFamily: 'var(--font-display)', fontWeight: 700, fontSize: 16, color: 'var(--text)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
            Edit Site: {site.name || 'Unnamed'}
          </span>
          <span style={{
            fontSize: 10, fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase',
            padding: '3px 9px', borderRadius: 4, flexShrink: 0,
            background: `${statusColor}18`, color: statusColor,
          }}>
            {STATUS_LABELS[site.status] || site.status}
          </span>
        </div>

        <div style={{ display: 'flex', gap: 8, flexShrink: 0 }}>
          <Btn variant="danger" size="sm" onClick={() => setDelOpen(true)}>Delete</Btn>
        </div>
      </div>

      {/* ── Tab bar ── */}
      <div style={{
        background: 'var(--topbar-bg)', backdropFilter: 'blur(12px)',
        borderBottom: '1px solid var(--topbar-border)',
        padding: '0 32px', display: 'flex', gap: 2,
        position: 'sticky', top: 64, zIndex: 49,
      }}>
        {TABS.map(t => (
          <button key={t.id} onClick={() => setTab(t.id)} style={{
            padding: '13px 16px', border: 'none', background: 'none', cursor: 'pointer',
            fontSize: 13, fontWeight: tab === t.id ? 700 : 500,
            color: tab === t.id ? 'var(--primary)' : 'var(--text-2)',
            borderBottom: `2px solid ${tab === t.id ? 'var(--primary)' : 'transparent'}`,
            marginBottom: -1, transition: 'all .15s',
          }}>
            {t.label}
          </button>
        ))}
      </div>

      {/* ── Content ── */}
      <div style={{ flex: 1, padding: '32px', maxWidth: 1000, width: '100%', margin: '0 auto', boxSizing: 'border-box' }}>
        {tab === 'basic'      && (
          <BasicInfoTab
            site={site}
            onEdit={() => setEditOpen(true)}
            statusForm={statusForm}
            setStatusForm={setStatusForm}
            onStatusSave={handleStatusSave}
            statusSaving={statusSaving}
            statusDirty={statusDirty}
          />
        )}
        {tab === 'localize'   && <LocalizedContentsTab siteId={id} />}
        {tab === 'images'     && <ImagesTab siteId={id} />}
        {tab === 'hours'      && <OpeningHoursTab siteId={id} />}
        {tab === 'transport'  && (
          <div style={{ padding: '80px 0', textAlign: 'center', color: 'var(--outline)' }}>
            <div style={{ fontSize: 40, marginBottom: 16 }}>🚌</div>
            <div style={{ fontSize: 15, fontFamily: 'var(--font-display)', color: 'var(--text-2)' }}>Transportation management coming soon.</div>
          </div>
        )}
        {tab === 'facilities' && <FacilitiesTab siteId={id} />}
      </div>

      {/* ── Modals ── */}
      <Modal open={editOpen} onClose={() => setEditOpen(false)} title="Edit Site" width={680}>
        <SiteForm initial={site} onSubmit={handleUpdate} loading={saving} onCancel={() => setEditOpen(false)} />
      </Modal>

      <ConfirmDialog
        open={delOpen} onClose={() => setDelOpen(false)}
        onConfirm={handleDelete} loading={deleting}
        title="Delete Site"
        message={`Permanently delete "${site.name || 'this site'}"? This cannot be undone.`}
        confirmLabel="Delete Site"
      />
    </div>
  );
}
