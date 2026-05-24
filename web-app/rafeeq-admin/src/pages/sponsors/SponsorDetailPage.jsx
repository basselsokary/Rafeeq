import React, { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getSponsorById, updateSponsor, deleteSponsor, activateSponsor, getSponsorOffers } from '../../api/sponsorsApi';
import SponsorLocalizedContentsTab from './components/SponsorLocalizedContentsTab';
import SponsorImagesTab from './components/SponsorImagesTab';
import Modal from '../../components/common/Modal';
import ConfirmDialog from '../../components/common/ConfirmDialog';
import Spinner from '../../components/common/Spinner';
import SponsorForm from './components/SponsorForm';
import { useToast } from '../../components/common/Toast';

/* ── Enums / helpers ─────────────────────────────────── */
const STATUS_COLOR = { active: '#386a20', inactive: '#d97706', expired: '#ba1a1a' };
const STATUS_LABEL = { active: 'Active', inactive: 'Inactive', expired: 'Expired' };

const TIER_CFG = {
  platinum: { bg: 'rgba(148,163,184,.15)', text: '#64748b', border: 'rgba(148,163,184,.5)', emoji: '💠' },
  gold:     { bg: 'rgba(212,165,116,.18)', text: '#7c572d', border: 'rgba(212,165,116,.5)', emoji: '🥇' },
  silver:   { bg: 'rgba(148,163,184,.12)', text: '#4b5563', border: 'rgba(148,163,184,.4)', emoji: '🥈' },
  bronze:   { bg: 'rgba(180,120,60,.12)',  text: '#92400e', border: 'rgba(180,120,60,.4)',  emoji: '🥉' },
};

const TYPE_ICONS = { restaurant:'🍽️', hotel:'🏨', shop:'🛍️', service:'⚙️', tour:'🗺️', transportation:'🚌' };
const fmt = v => v ? v.replace(/([A-Z])/g,' $1').replace(/^./,s=>s.toUpperCase()) : '—';

const TABS = [
  { id: 'overview',  label: 'Overview'          },
  { id: 'localize',  label: 'Localization'       },
  { id: 'images',    label: 'Images & Media'     },
  { id: 'offers',    label: 'Offers'             },
];

/* ── Shared button ── */
function Btn({ children, onClick, variant='primary', size='md', loading=false, disabled=false, type='button', style:s={} }) {
  const sm = size==='sm';
  const v = {
    primary:   { background:'linear-gradient(135deg,var(--primary),var(--primary-container))', color:'#fff', boxShadow:'0 2px 8px rgba(124,87,45,.2)' },
    secondary: { background:'var(--surface-container-lowest)', color:'var(--text-2)', boxShadow:'0 0 0 1px rgba(212,196,183,.5)' },
    ghost:     { background:'transparent', color:'var(--text-2)' },
    danger:    { background:'rgba(186,26,26,.07)', color:'var(--error)', boxShadow:'0 0 0 1px rgba(186,26,26,.2)' },
  };
  return (
    <button type={type} onClick={onClick} disabled={disabled||loading} style={{ display:'inline-flex', alignItems:'center', gap:7, padding:sm?'6px 14px':'9px 20px', fontSize:sm?12:13, fontWeight:700, borderRadius:sm?8:10, border:'none', cursor:disabled||loading?'not-allowed':'pointer', opacity:disabled?0.5:1, transition:'all .15s', fontFamily:'var(--font-body)', ...v[variant], ...s }}>
      {loading && <Spinner size={13} />}{children}
    </button>
  );
}

/* ── Activate toggle ── */
function ActivateToggle({ isActive, onClick, disabled }) {
  return (
    <button type="button" onClick={onClick} disabled={disabled} style={{ width:44, height:24, borderRadius:12, border:'none', cursor:disabled?'not-allowed':'pointer', opacity:disabled?0.6:1, background:isActive?'linear-gradient(135deg,var(--primary),var(--primary-container))':'var(--surface-container-high)', position:'relative', boxShadow:isActive?'0 2px 6px rgba(124,87,45,.3)':'none', transition:'all .2s' }}>
      <span style={{ width:18, height:18, borderRadius:'50%', background:'#fff', position:'absolute', top:3, left:isActive?23:3, boxShadow:'0 1px 3px rgba(0,0,0,.2)', transition:'left .18s' }} />
    </button>
  );
}

/* ── Info row ── */
function InfoRow({ label, value }) {
  if (value == null || value === '' || value === '—') return null;
  return (
    <div style={{ display:'flex', justifyContent:'space-between', alignItems:'baseline', padding:'11px 18px', borderBottom:'1px solid rgba(212,196,183,.2)' }}>
      <span style={{ fontSize:10, fontWeight:700, letterSpacing:'0.08em', textTransform:'uppercase', color:'var(--outline)', flexShrink:0, marginRight:20 }}>{label}</span>
      <span style={{ fontSize:13, color:'var(--text)', textAlign:'right' }}>{String(value)}</span>
    </div>
  );
}

/* ── Offers tab ── */
function OffersTab({ sponsorId }) {
  const toast = useToast();
  const [offers,   setOffers]   = useState([]);
  const [loading,  setLoading]  = useState(true);

  useEffect(() => {
    (async () => {
      try {
        setLoading(true);
        const res = await getSponsorOffers(sponsorId);
        const d   = res.data;
        setOffers(Array.isArray(d) ? d : Array.isArray(d?.value) ? d.value : Array.isArray(d?.data) ? d.data : []);
      } catch { toast('Failed to load offers', 'error'); }
      finally { setLoading(false); }
    })();
  }, [sponsorId]);

  const validColor = v => v ? '#386a20' : '#ba1a1a';

  return (
    <div style={{ maxWidth:720 }}>
      <div style={{ display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom:24 }}>
        <div>
          <h2 style={{ fontFamily:'var(--font-display)', fontSize:22, color:'var(--primary)', margin:'0 0 2px' }}>Offers</h2>
          <p style={{ fontSize:12, color:'var(--outline)', margin:0 }}>{offers.length} offer{offers.length!==1?'s':''} for this sponsor</p>
        </div>
      </div>

      {loading ? <Spinner center /> : offers.length === 0 ? (
        <div style={{ padding:'64px 0', textAlign:'center', color:'var(--outline)', fontSize:14 }}>
          <div style={{ fontSize:40, marginBottom:16 }}>🎟️</div>
          No offers yet.
        </div>
      ) : (
        <div style={{ display:'flex', flexDirection:'column', gap:12 }}>
          {offers.map(offer => (
            <div key={offer.id} style={{ background:'var(--surface-container-lowest)', borderRadius:14, padding:'18px 20px', boxShadow:'0 1px 4px rgba(29,27,23,.06)', borderLeft:`4px solid ${offer.isActive?'var(--primary)':'var(--outline-variant)'}` }}>
              <div style={{ display:'flex', alignItems:'flex-start', justifyContent:'space-between', gap:12, marginBottom:10 }}>
                <div style={{ fontWeight:700, fontSize:14, color:'var(--text)' }}>{offer.title || 'Unnamed Offer'}</div>
                <div style={{ display:'flex', gap:6, flexShrink:0 }}>
                  <span style={{ fontSize:10, fontWeight:700, padding:'3px 9px', borderRadius:10, background:offer.isActive?'rgba(56,106,32,.1)':'rgba(130,117,105,.1)', color:offer.isActive?'#386a20':'var(--outline)' }}>
                    {offer.isActive ? 'Active' : 'Inactive'}
                  </span>
                  <span style={{ fontSize:10, fontWeight:700, padding:'3px 9px', borderRadius:10, background:offer.isValid?'rgba(56,106,32,.08)':'rgba(186,26,26,.08)', color:validColor(offer.isValid) }}>
                    {offer.isValid ? 'Valid' : 'Expired'}
                  </span>
                </div>
              </div>

              {offer.description && <p style={{ fontSize:12, color:'var(--text-2)', margin:'0 0 12px', lineHeight:1.6 }}>{offer.description}</p>}

              <div style={{ display:'grid', gridTemplateColumns:'repeat(auto-fill,minmax(150px,1fr))', gap:10 }}>
                {offer.discountPercentage != null && (
                  <div style={{ background:'var(--surface-container-low)', borderRadius:10, padding:'9px 13px' }}>
                    <div style={{ fontSize:9, fontWeight:700, color:'var(--outline)', textTransform:'uppercase', letterSpacing:'0.08em', marginBottom:3 }}>Discount</div>
                    <div style={{ fontSize:18, fontWeight:800, color:'var(--primary)', fontFamily:'var(--font-display)' }}>{offer.discountPercentage}%</div>
                  </div>
                )}
                {offer.discountAmount?.amount > 0 && (
                  <div style={{ background:'var(--surface-container-low)', borderRadius:10, padding:'9px 13px' }}>
                    <div style={{ fontSize:9, fontWeight:700, color:'var(--outline)', textTransform:'uppercase', letterSpacing:'0.08em', marginBottom:3 }}>Amount Off</div>
                    <div style={{ fontSize:16, fontWeight:800, color:'var(--primary)', fontFamily:'var(--font-display)' }}>{offer.discountAmount.currency} {offer.discountAmount.amount}</div>
                  </div>
                )}
                {offer.promoCode && (
                  <div style={{ background:'var(--surface-container-low)', borderRadius:10, padding:'9px 13px' }}>
                    <div style={{ fontSize:9, fontWeight:700, color:'var(--outline)', textTransform:'uppercase', letterSpacing:'0.08em', marginBottom:3 }}>Promo Code</div>
                    <div style={{ fontSize:13, fontWeight:800, color:'var(--text)', letterSpacing:'0.1em', fontFamily:'monospace' }}>{offer.promoCode}</div>
                  </div>
                )}
                {offer.validityPeriod && (
                  <div style={{ background:'var(--surface-container-low)', borderRadius:10, padding:'9px 13px' }}>
                    <div style={{ fontSize:9, fontWeight:700, color:'var(--outline)', textTransform:'uppercase', letterSpacing:'0.08em', marginBottom:3 }}>Validity</div>
                    <div style={{ fontSize:11, fontWeight:600, color:'var(--text-2)' }}>
                      {new Date(offer.validityPeriod.start).toLocaleDateString()} –<br/>
                      {new Date(offer.validityPeriod.end).toLocaleDateString()}
                    </div>
                  </div>
                )}
                <div style={{ background:'var(--surface-container-low)', borderRadius:10, padding:'9px 13px' }}>
                  <div style={{ fontSize:9, fontWeight:700, color:'var(--outline)', textTransform:'uppercase', letterSpacing:'0.08em', marginBottom:3 }}>Redemptions</div>
                  <div style={{ fontSize:16, fontWeight:800, color:'var(--text)', fontFamily:'var(--font-display)' }}>
                    {offer.redemptionCount ?? 0}{offer.maxRedemptions ? ` / ${offer.maxRedemptions}` : ''}
                  </div>
                </div>
                {offer.daysUntilExpiry != null && offer.daysUntilExpiry >= 0 && (
                  <div style={{ background:'var(--surface-container-low)', borderRadius:10, padding:'9px 13px' }}>
                    <div style={{ fontSize:9, fontWeight:700, color:'var(--outline)', textTransform:'uppercase', letterSpacing:'0.08em', marginBottom:3 }}>Days Left</div>
                    <div style={{ fontSize:16, fontWeight:800, color: offer.daysUntilExpiry<=7?'#ba1a1a':'var(--text)', fontFamily:'var(--font-display)' }}>{offer.daysUntilExpiry}</div>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

/* ── Overview tab ── */
function OverviewTab({ sponsor, onEdit, onToggleActive, statusSaving }) {
  const tc = TIER_CFG[sponsor.tier] || TIER_CFG.bronze;
  return (
    <div style={{ maxWidth:720 }}>
      {/* Hero image */}
      <div style={{ borderRadius:16, overflow:'hidden', marginBottom:20, background:'var(--surface-container)', aspectRatio:'21/9', position:'relative' }}>
        {sponsor.mainImageUrl
          ? <img src={sponsor.mainImageUrl} alt={sponsor.title} style={{ width:'100%', height:'100%', objectFit:'cover', display:'block' }} />
          : <div style={{ width:'100%', height:'100%', display:'flex', alignItems:'center', justifyContent:'center', fontSize:56 }}>{TYPE_ICONS[sponsor.type]||'🤝'}</div>
        }
        <div style={{ position:'absolute', top:0, right:0, width:0, height:0, borderStyle:'solid', borderWidth:'0 36px 36px 0', borderColor:`transparent rgba(124,87,45,.2) transparent transparent` }} />
      </div>

      {/* Description */}
      {sponsor.description && (
        <p style={{ color:'var(--text-2)', fontSize:14, lineHeight:1.75, marginBottom:20, background:'var(--surface-container-low)', borderRadius:12, padding:'14px 18px' }}>{sponsor.description}</p>
      )}

      {/* Info card */}
      <div style={{ background:'var(--surface-container-lowest)', borderRadius:14, overflow:'hidden', marginBottom:20, boxShadow:'0 1px 4px rgba(29,27,23,.05)' }}>
        {/* Status row */}
        <div style={{ display:'flex', alignItems:'center', justifyContent:'space-between', padding:'14px 18px', borderBottom:'1px solid rgba(212,196,183,.2)' }}>
          <div>
            <div style={{ fontSize:10, fontWeight:700, letterSpacing:'0.08em', textTransform:'uppercase', color:'var(--outline)', marginBottom:3 }}>Status</div>
            <div style={{ fontSize:13, color: STATUS_COLOR[sponsor.status]||'var(--outline)', fontWeight:600 }}>
              {STATUS_LABEL[sponsor.status]||sponsor.status}
            </div>
          </div>
          <ActivateToggle isActive={sponsor.status==='active'} onClick={onToggleActive} disabled={statusSaving||sponsor.status==='expired'} />
        </div>

        {/* Tier row */}
        <div style={{ display:'flex', justifyContent:'space-between', alignItems:'center', padding:'11px 18px', borderBottom:'1px solid rgba(212,196,183,.2)' }}>
          <span style={{ fontSize:10, fontWeight:700, letterSpacing:'0.08em', textTransform:'uppercase', color:'var(--outline)' }}>Tier</span>
          <span style={{ display:'inline-block', padding:'3px 9px', borderRadius:4, fontSize:10, fontWeight:700, letterSpacing:'0.07em', background:tc.bg, color:tc.text, border:`1px solid ${tc.border}` }}>
            {tc.emoji} {fmt(sponsor.tier)}
          </span>
        </div>

        <InfoRow label="Type"           value={`${TYPE_ICONS[sponsor.type]||''} ${fmt(sponsor.type)}`} />
        <InfoRow label="Address"        value={sponsor.address} />
        <InfoRow label="Phone"          value={sponsor.contactPhone} />
        <InfoRow label="Email"          value={sponsor.contactEmail} />
        <InfoRow label="Website"        value={sponsor.websiteUrl} />
        <InfoRow label="Contract Start" value={sponsor.dateRange?.start ? new Date(sponsor.dateRange.start).toLocaleDateString() : null} />
        <InfoRow label="Contract End"   value={sponsor.dateRange?.end   ? new Date(sponsor.dateRange.end).toLocaleDateString()   : null} />
        <InfoRow label="Contract Valid" value={sponsor.isContractValid != null ? (sponsor.isContractValid ? 'Yes ✅' : 'Expired ❌') : null} />
        <InfoRow label="Coordinates"    value={sponsor.location ? `${sponsor.location.latitude}, ${sponsor.location.longitude}` : null} />
        <InfoRow label="Total Redemptions" value={sponsor.totalRedemptions} />
        {sponsor.createdByName && <InfoRow label="Created By" value={`${sponsor.createdByName} — ${new Date(sponsor.createdAt).toLocaleString()}`} />}
        {sponsor.lastModifiedByName && <InfoRow label="Last Modified" value={`${sponsor.lastModifiedByName} — ${new Date(sponsor.lastModifiedAt).toLocaleString()}`} />}
        <div style={{ height:1 }} />
      </div>

      <Btn onClick={onEdit}>
        <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4z"/></svg>
        Edit Sponsor
      </Btn>
    </div>
  );
}

/* ── Main page ─────────────────────────────────────────── */
export default function SponsorDetailPage() {
  const { id }   = useParams();
  const navigate = useNavigate();
  const toast    = useToast();

  const [sponsor,      setSponsor]      = useState(null);
  const [loading,      setLoading]      = useState(true);
  const [tab,          setTab]          = useState('overview');
  const [editOpen,     setEditOpen]     = useState(false);
  const [delOpen,      setDelOpen]      = useState(false);
  const [saving,       setSaving]       = useState(false);
  const [statusSaving, setStatusSaving] = useState(false);
  const [deleting,     setDeleting]     = useState(false);

  const load = useCallback(async () => {
    try {
      setLoading(true);
      const res = await getSponsorById(id);
      setSponsor(res.data?.value ?? res.data);
    } catch { toast('Failed to load sponsor', 'error'); }
    finally { setLoading(false); }
  }, [id]);

  useEffect(() => { load(); }, [load]);

  const handleUpdate = async (payload) => {
    setSaving(true);
    try {
      await updateSponsor(id, payload);
      toast('Saved', 'success');
      setEditOpen(false); load();
    } catch (e) {
      console.error(e.response?.data || e);
      toast('Update failed', 'error');
    } finally { setSaving(false); }
  };

  const handleDelete = async () => {
    setDeleting(true);
    try { await deleteSponsor(id); toast('Sponsor deleted', 'success'); navigate('/sponsors'); }
    catch { toast('Delete failed', 'error'); }
    finally { setDeleting(false); }
  };

  const handleToggleActive = async () => {
    if (!sponsor) return;
    const next = sponsor.status === 'active' ? false : true;
    const prev = sponsor.status;
    setStatusSaving(true);
    setSponsor(s => ({ ...s, status: next ? 'active' : 'inactive' }));
    try {
      await activateSponsor(id, next);
      toast('Status updated', 'success');
    } catch (e) {
      setSponsor(s => ({ ...s, status: prev }));
      console.error(e.response?.data || e);
      toast('Status update failed', 'error');
    } finally { setStatusSaving(false); }
  };

  if (loading) return (
    <div style={{ minHeight:'100vh', background:'var(--background)', display:'flex', alignItems:'center', justifyContent:'center' }}>
      <Spinner size={36} />
    </div>
  );

  if (!sponsor) return (
    <div style={{ minHeight:'100vh', background:'var(--background)', display:'flex', alignItems:'center', justifyContent:'center', color:'var(--outline)' }}>
      Sponsor not found.
    </div>
  );

  const sc = STATUS_COLOR[sponsor.status] || 'var(--outline)';

  return (
    <div style={{ minHeight:'100vh', background:'var(--background)', fontFamily:'var(--font-body)', display:'flex', flexDirection:'column' }}>

      <div style={{ padding:'24px 32px 0', display:'flex', alignItems:'center', gap:20, flexWrap:'wrap' }}>
        <button onClick={() => navigate('/sponsors')} style={{ display:'flex', alignItems:'center', gap:6, background:'none', border:'none', cursor:'pointer', color:'var(--text-2)', fontSize:13, fontWeight:500, padding:0 }}>
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M19 12H5M12 5l-7 7 7 7"/></svg>
          Back to Sponsors
        </button>
        <div style={{ width:1, height:24, background:'rgba(212,196,183,.5)' }} />
        <div style={{ display:'flex', alignItems:'center', gap:10, flex:1, minWidth:0 }}>
          <span style={{ fontFamily:'var(--font-display)', fontWeight:700, fontSize:16, color:'var(--text)', whiteSpace:'nowrap', overflow:'hidden', textOverflow:'ellipsis' }}>
            {TYPE_ICONS[sponsor.type]||'🤝'} {sponsor.title || 'Unnamed Sponsor'}
          </span>
          <span style={{ fontSize:10, fontWeight:700, letterSpacing:'0.08em', textTransform:'uppercase', padding:'3px 9px', borderRadius:4, flexShrink:0, background:`${sc}18`, color:sc }}>
            {STATUS_LABEL[sponsor.status]||sponsor.status}
          </span>
        </div>
        <div style={{ display:'flex', gap:8, flexShrink:0 }}>
          <Btn variant="danger" size="sm" onClick={() => setDelOpen(true)}>Delete</Btn>
          <Btn variant="secondary" size="sm" onClick={() => setEditOpen(true)}>
            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4z"/></svg>
            Edit
          </Btn>
        </div>
      </div>

      {/* Tab bar */}
      <div style={{ background:'var(--topbar-bg)', backdropFilter:'blur(12px)', borderBottom:'1px solid var(--topbar-border)', padding:'0 32px', display:'flex', gap:2, position:'sticky', top:64, zIndex:49 }}>
        {TABS.map(t => (
          <button key={t.id} onClick={() => setTab(t.id)} style={{ padding:'13px 16px', border:'none', background:'none', cursor:'pointer', fontSize:13, fontWeight:tab===t.id?700:500, color:tab===t.id?'var(--primary)':'var(--text-2)', borderBottom:`2px solid ${tab===t.id?'var(--primary)':'transparent'}`, marginBottom:-1, transition:'all .15s' }}>
            {t.label}
          </button>
        ))}
      </div>

      {/* Content */}
      <div style={{ flex:1, padding:'32px', maxWidth:1000, width:'100%', margin:'0 auto', boxSizing:'border-box' }}>
        {tab === 'overview' && <OverviewTab sponsor={sponsor} onEdit={() => setEditOpen(true)} onToggleActive={handleToggleActive} statusSaving={statusSaving} />}
        {tab === 'localize' && <SponsorLocalizedContentsTab sponsorId={id} />}
        {tab === 'images'   && <SponsorImagesTab sponsorId={id} />}
        {tab === 'offers'   && <OffersTab sponsorId={id} />}
      </div>

      {/* Modals */}
      <Modal open={editOpen} onClose={() => setEditOpen(false)} title="Edit Sponsor" width={640}>
        <SponsorForm initial={sponsor} onSubmit={handleUpdate} loading={saving} onCancel={() => setEditOpen(false)} />
      </Modal>
      <ConfirmDialog
        open={delOpen} onClose={() => setDelOpen(false)}
        onConfirm={handleDelete} loading={deleting}
        title="Delete Sponsor"
        message={`Permanently delete "${sponsor.title || 'this sponsor'}"? This cannot be undone.`}
        confirmLabel="Delete Sponsor"
      />
    </div>
  );
}
