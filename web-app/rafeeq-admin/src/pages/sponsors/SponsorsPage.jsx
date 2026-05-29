import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { getSponsors, getSponsorDashboard, createSponsor } from '../../api/sponsorsApi';
import Modal from '../../components/common/Modal';
import Spinner from '../../components/common/Spinner';
import SponsorForm from './components/SponsorForm';
import { useToast } from '../../components/common/Toast';
import { formatEnum } from '../../utils/constants';

/* ── Enums ─────────────────────────────────────────────── */
const SPONSOR_TYPES   = ['restaurant','hotel','shop','service','tour','transportation'];
const SPONSOR_TIERS   = ['bronze','silver','gold','platinum'];
const SPONSOR_FILTERS = ['All','Active','Inactive','Expired','Gold','Platinum'];

const TIER_CFG = {
  platinum: { bg: 'rgba(148,163,184,.15)', text: '#64748b', border: 'rgba(148,163,184,.5)', label: '💠 Platinum' },
  gold:     { bg: 'rgba(212,165,116,.18)', text: '#7c572d', border: 'rgba(212,165,116,.5)', label: '🥇 Gold'     },
  silver:   { bg: 'rgba(148,163,184,.12)', text: '#4b5563', border: 'rgba(148,163,184,.4)', label: '🥈 Silver'   },
  bronze:   { bg: 'rgba(180,120,60,.12)',  text: '#92400e', border: 'rgba(180,120,60,.4)',  label: '🥉 Bronze'   },
};

const STATUS_CFG = {
  active:   { dot: '#386a20', label: 'Active'   },
  inactive: { dot: '#d97706', label: 'Inactive' },
  expired:  { dot: '#ba1a1a', label: 'Expired'  },
};

const TYPE_ICONS = {
  restaurant: '🍽️', hotel: '🏨', shop: '🛍️',
  service: '⚙️', tour: '🗺️', transportation: '🚌',
};

/* ── Atoms ──────────────────────────────────────────────── */
function TierBadge({ tier }) {
  const c = TIER_CFG[tier] || TIER_CFG.bronze;
  return (
    <span style={{
      display: 'inline-block', padding: '3px 9px', borderRadius: 4,
      fontSize: 10, fontWeight: 700, letterSpacing: '0.07em',
      background: c.bg, color: c.text, border: `1px solid ${c.border}`,
    }}>{c.label}</span>
  );
}

function StatusDot({ status }) {
  const s = STATUS_CFG[status] || STATUS_CFG.inactive;
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 7 }}>
      <span style={{ width: 7, height: 7, borderRadius: '50%', background: s.dot, flexShrink: 0, boxShadow: `0 0 0 2px ${s.dot}30` }} />
      <span style={{ fontSize: 12, color: 'var(--text-2)', fontWeight: 600 }}>{s.label}</span>
    </div>
  );
}

function StatCard({ label, value, sub, icon, accent, loading }) {
  return (
    <div style={{
      background: 'var(--surface-container-lowest)', borderRadius: 16,
      padding: '20px 22px', flex: 1, minWidth: 0,
      boxShadow: '0 1px 3px rgba(29,27,23,.05)',
      borderTop: `4px solid ${accent}`,
      position: 'relative', overflow: 'hidden',
    }}>
      <div style={{ position: 'absolute', top: 0, right: 0, width: 0, height: 0, borderStyle: 'solid', borderWidth: '0 28px 28px 0', borderColor: `transparent ${accent}20 transparent transparent`, borderRadius: '0 16px 0 0' }} />
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 12 }}>
        <div style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.09em', textTransform: 'uppercase', color: 'var(--outline)' }}>{label}</div>
        <div style={{ width: 32, height: 32, borderRadius: 8, background: `${accent}14`, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 16 }}>{icon}</div>
      </div>
      <div style={{ fontFamily: 'var(--font-display)', fontSize: 34, fontWeight: 700, color: 'var(--text)', lineHeight: 1.1 }}>
        {loading ? '…' : (value ?? '—')}
      </div>
      {sub && <div style={{ fontSize: 11, color: 'var(--outline)', marginTop: 5 }}>{sub}</div>}
    </div>
  );
}

function PaginationBtn({ children, active, onClick, disabled }) {
  return (
    <button onClick={onClick} disabled={disabled} style={{
      width: 32, height: 32, borderRadius: 8, fontSize: 13, fontWeight: 600, border: 'none',
      background: active ? 'linear-gradient(135deg,var(--primary),var(--primary-container))' : 'var(--surface-container-low)',
      color: active ? '#fff' : 'var(--text-2)',
      cursor: disabled ? 'default' : 'pointer',
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      opacity: disabled ? 0.4 : 1,
      boxShadow: active ? '0 2px 8px rgba(124,87,45,0.3)' : 'none',
    }}>{children}</button>
  );
}

const PER_PAGE = 20;

/* ── Main page ──────────────────────────────────────────── */
export default function SponsorsPage() {
  const navigate = useNavigate();
  const toast    = useToast();

  const [sponsors,     setSponsors]     = useState([]);
  const [loading,      setLoading]      = useState(false);
  const [createOpen,   setCreateOpen]   = useState(false);
  const [creating,     setCreating]     = useState(false);
  const [activeFilter, setActiveFilter] = useState('All');
  const [search,       setSearch]       = useState('');
  const [typeFilter,   setTypeFilter]   = useState('');
  const [tierFilter,   setTierFilter]   = useState('');
  const [appliedFilters, setAppliedFilters] = useState({
    search: '',
    typeFilter: '',
    tierFilter: '',
    activeFilter: 'All',
  });
  const [page,         setPage]         = useState(1);
  const [totalCount,   setTotalCount]   = useState(0);
  const [dashboard,    setDashboard]    = useState(null);
  const [dashLoading,  setDashLoading]  = useState(true);

  /* ── Fetch ── */
  const buildParams = (pageNumber, filters) => {
    const p = { page: pageNumber, pageSize: PER_PAGE };
    if (filters.search.trim()) p.searchTerm = filters.search.trim();
    if (filters.typeFilter)    p.type = filters.typeFilter;
    if (filters.tierFilter)    p.tier = filters.tierFilter;
    if (filters.activeFilter === 'Active')   p.activeOnly = true;
    return p;
  };

  const loadSponsors = useCallback(async (params) => {
    try {
      setLoading(true);
      const res = await getSponsors(params);
      const d   = res.data;
      setSponsors(Array.isArray(d) ? d : Array.isArray(d?.data) ? d.data : Array.isArray(d?.value) ? d.value : []);
      setTotalCount(d?.totalCount ?? d?.length ?? 0);
    } catch { setSponsors([]); }
    finally { setLoading(false); }
  }, []);

  const loadDashboard = useCallback(async () => {
    try {
      setDashLoading(true);
      const res = await getSponsorDashboard();
      const d   = res.data?.value ?? res.data?.data ?? res.data;
      setDashboard({
        totalSponsors:  Number(d?.totalSponsors  ?? 0),
        activeSponsors: Number(d?.activeSponsors ?? 0),
        expiredSponsors:Number(d?.expiredSponsors?? 0),
        totalOffers:    Number(d?.totalOffers    ?? 0),
        activeOffers:   Number(d?.activeOffers   ?? 0),
      });
    } catch { /* keep null */ }
    finally { setDashLoading(false); }
  }, []);

  useEffect(() => { loadDashboard(); }, [loadDashboard]);
  useEffect(() => { loadSponsors(buildParams(page, appliedFilters)); }, [page, appliedFilters, loadSponsors]);

  const applyFilters = () => {
    const nextFilters = { search, typeFilter, tierFilter, activeFilter };
    setAppliedFilters(nextFilters);
    setPage(1);
  };

  const clearFilters = () => {
    setSearch(''); setTypeFilter(''); setTierFilter('');
    setActiveFilter('All'); setPage(1);
    const clearedFilters = { search: '', typeFilter: '', tierFilter: '', activeFilter: 'All' };
    setAppliedFilters(clearedFilters);
  };

  /* ── Filter pills (client-side fallback for quick tabs) ── */
  const displayed = sponsors.filter(s => {
    if (activeFilter === 'Active'   && s.status !== 'active')   return false;
    if (activeFilter === 'Inactive' && s.status !== 'inactive') return false;
    if (activeFilter === 'Expired'  && s.status !== 'expired')  return false;
    if (activeFilter === 'Gold'     && s.tier !== 'gold')       return false;
    if (activeFilter === 'Platinum' && s.tier !== 'platinum')   return false;
    return true;
  });

  const totalPages = Math.max(1, Math.ceil((totalCount || displayed.length) / PER_PAGE));

  /* ── Create ── */
  const handleCreate = async (payload) => {
    setCreating(true);
    try {
      const res = await createSponsor(payload);
      toast('Sponsor created', 'success');
      setCreateOpen(false);
      loadDashboard();
      const newId = res.data?.value?.id ?? res.data?.id;
      newId ? navigate(`/sponsors/${newId}`) : loadSponsors();
    } catch (e) {
      console.error(e.response?.data || e);
      toast('Failed to create sponsor', 'error');
    } finally { setCreating(false); }
  };

  const GRID = '2.2fr 1fr 1fr 1fr 1fr 1.2fr';

  const selectStyle = {
    flex: 1, minWidth: 120, padding: '9px 32px 9px 12px',
    background: 'var(--surface-container-lowest)',
    border: '1px solid rgba(212,196,183,.3)',
    borderRadius: 10, fontSize: 13, color: 'var(--text-2)',
    outline: 'none', cursor: 'pointer', appearance: 'none',
    backgroundImage: `url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='%23827569' stroke-width='2'%3E%3Cpath d='M6 9l6 6 6-6'/%3E%3C/svg%3E")`,
    backgroundRepeat: 'no-repeat', backgroundPosition: 'right 10px center',
  };

  return (
    <div style={{ padding: '28px 32px 80px', fontFamily: 'var(--font-body)' }}>
      {/* Breadcrumb */}
      <div style={{ fontSize: 12, color: 'var(--outline)', marginBottom: 10, display: 'flex', alignItems: 'center', gap: 6 }}>
        <span style={{ cursor: 'pointer', color: 'var(--text-2)' }} onClick={() => navigate('/dashboard')}>Dashboard</span>
        <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
        <span>Sponsors & Offers</span>
      </div>

      {/* Page header */}
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 24, flexWrap: 'wrap', gap: 12 }}>
        <h1 style={{ fontFamily: 'var(--font-display)', fontSize: 28, fontWeight: 700, color: 'var(--text)', display: 'flex', alignItems: 'baseline', gap: 10, margin: 0 }}>
          Sponsors & Offers
          <span style={{ fontSize: 18, fontWeight: 400, color: 'var(--outline)' }}>• {dashboard?.totalSponsors ?? sponsors.length} Total</span>
        </h1>
        <button onClick={() => setCreateOpen(true)} style={{ display: 'flex', alignItems: 'center', gap: 7, padding: '9px 18px', borderRadius: 10, background: 'linear-gradient(135deg,var(--primary),var(--primary-container))', color: '#fff', border: 'none', fontSize: 13, fontWeight: 700, cursor: 'pointer', boxShadow: '0 2px 10px rgba(124,87,45,.25)' }}>
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M12 5v14M5 12h14"/></svg>
          Add Sponsor
        </button>
      </div>

      {/* Stat cards */}
      <div style={{ display: 'flex', gap: 16, marginBottom: 28, flexWrap: 'wrap' }}>
        <StatCard label="Total Sponsors"  value={dashboard?.totalSponsors}  loading={dashLoading} sub={`${dashboard?.activeSponsors ?? 0} active`}   accent="var(--primary)"  icon="🤝" />
        <StatCard label="Active Sponsors" value={dashboard?.activeSponsors} loading={dashLoading} sub="Currently live"                                 accent="#386a20"         icon="✅" />
        <StatCard label="Total Offers"    value={dashboard?.totalOffers}    loading={dashLoading} sub={`${dashboard?.activeOffers ?? 0} active`}       accent="#d97706"         icon="🎟️" />
        <StatCard label="Expired"         value={dashboard?.expiredSponsors}loading={dashLoading} sub="Contracts ended"                                accent="#ba1a1a"         icon="⏱️" />
      </div>

      {/* Filter pills */}
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 14 }}>
        <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
          {SPONSOR_FILTERS.map(f => (
            <button key={f} onClick={() => { setActiveFilter(f); setPage(1); }} style={{
              padding: '6px 15px', borderRadius: 999, fontSize: 13, fontWeight: 600,
              border: 'none', cursor: 'pointer', transition: '.15s',
              background: activeFilter === f ? 'linear-gradient(135deg,var(--primary),var(--primary-container))' : 'var(--surface-container-lowest)',
              color: activeFilter === f ? '#fff' : 'var(--text-2)',
              boxShadow: activeFilter === f ? '0 2px 8px rgba(124,87,45,.3)' : '0 0 0 1px rgba(212,196,183,.5)',
            }}>{f}</button>
          ))}
        </div>
        {(activeFilter !== 'All' || search || typeFilter || tierFilter) && (
          <button onClick={clearFilters} style={{ background: 'none', border: 'none', fontSize: 13, color: 'var(--primary)', cursor: 'pointer', fontWeight: 600 }}>Clear Filters</button>
        )}
      </div>

      {/* Search + dropdowns row */}
      <div style={{ display: 'flex', gap: 10, marginBottom: 20, flexWrap: 'wrap' }}>
        <div style={{ flex: 2, minWidth: 220, position: 'relative' }}>
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="var(--outline)" strokeWidth="2" style={{ position: 'absolute', left: 11, top: '50%', transform: 'translateY(-50%)', pointerEvents: 'none' }}>
            <circle cx="11" cy="11" r="8"/><path d="M21 21l-4.35-4.35"/>
          </svg>
          <input value={search} onChange={e => setSearch(e.target.value)} onKeyDown={e => e.key === 'Enter' && applyFilters()}
            placeholder="Search by name…"
            style={{ width: '100%', padding: '9px 14px 9px 34px', background: 'var(--surface-container-lowest)', border: '1px solid rgba(212,196,183,.3)', borderRadius: 10, fontSize: 13, color: 'var(--text)', outline: 'none' }}
          />
        </div>
        <select value={typeFilter} onChange={e => setTypeFilter(e.target.value)} style={selectStyle}>
          <option value="">Type</option>
          {SPONSOR_TYPES.map(t => <option key={t} value={t}>{formatEnum(t)}</option>)}
        </select>
        <select value={tierFilter} onChange={e => setTierFilter(e.target.value)} style={selectStyle}>
          <option value="">Tier</option>
          {SPONSOR_TIERS.map(t => <option key={t} value={t}>{formatEnum(t)}</option>)}
        </select>
        <button onClick={applyFilters} style={{ padding: '9px 20px', borderRadius: 10, background: 'linear-gradient(135deg,var(--primary),var(--primary-container))', color: '#fff', border: 'none', fontSize: 13, fontWeight: 700, cursor: 'pointer', boxShadow: '0 2px 8px rgba(124,87,45,.25)', whiteSpace: 'nowrap' }}>
          Apply
        </button>
      </div>

      {/* Table */}
      <div style={{ background: 'var(--surface-container-lowest)', borderRadius: 16, overflow: 'hidden', boxShadow: '0 1px 4px rgba(29,27,23,.06)' }}>
        {/* Header */}
        <div style={{ display: 'grid', gridTemplateColumns: GRID, padding: '10px 20px', background: 'var(--surface-container-low)', borderBottom: '1px solid rgba(212,196,183,.25)' }}>
          {['SPONSOR','TYPE','TIER','STATUS','OFFERS','CONTRACT'].map(h => (
            <div key={h} style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.08em', color: 'var(--outline)', textTransform: 'uppercase' }}>{h}</div>
          ))}
        </div>

        {loading ? (
          <div style={{ padding: 56 }}><Spinner center /></div>
        ) : displayed.length === 0 ? (
          <div style={{ padding: '56px 20px', textAlign: 'center', color: 'var(--outline)', fontSize: 14 }}>
            No sponsors found.{' '}
            <button onClick={() => setCreateOpen(true)} style={{ color: 'var(--primary)', background: 'none', border: 'none', cursor: 'pointer', fontWeight: 600, fontSize: 14 }}>Add the first one.</button>
          </div>
        ) : displayed.map((s, i) => (
          <div
            key={s.id}
            onClick={() => navigate(`/sponsors/${s.id}`)}
            style={{ display: 'grid', gridTemplateColumns: GRID, padding: '13px 20px', alignItems: 'center', borderBottom: i < displayed.length - 1 ? '1px solid rgba(212,196,183,.2)' : 'none', cursor: 'pointer', transition: 'background .1s' }}
            onMouseOver={e => e.currentTarget.style.background = 'var(--surface-container-low)'}
            onMouseOut={e => e.currentTarget.style.background = 'transparent'}
          >
            {/* Sponsor name */}
            <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
              <div style={{ width: 44, height: 44, borderRadius: 10, flexShrink: 0, background: 'rgba(212,165,116,.2)', overflow: 'hidden', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 20 }}>
                {s.primaryImageUrl
                  ? <img src={s.primaryImageUrl} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} onError={e => e.target.style.display = 'none'} />
                  : TYPE_ICONS[s.type] || '🤝'
                }
              </div>
              <div>
                <div style={{ fontSize: 13, fontWeight: 700, color: 'var(--text)', marginBottom: 2 }}>
                  {s.title || s.name || 'Unnamed Sponsor'}
                </div>
                {s.address && <div style={{ fontSize: 11, color: 'var(--outline)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap', maxWidth: 180 }}>{s.address}</div>}
              </div>
            </div>

            {/* Type */}
            <div>
              <span style={{ fontSize: 12, color: 'var(--text-2)', fontWeight: 500 }}>
                {TYPE_ICONS[s.type] || '•'} {formatEnum(s.type)}
              </span>
            </div>

            {/* Tier */}
            <div><TierBadge tier={s.tier} /></div>

            {/* Status */}
            <div onClick={e => e.stopPropagation()}>
              <StatusDot status={s.status} />
            </div>

            {/* Offers count */}
            <div>
              {s.activeOffersCount > 0
                ? <span style={{ fontSize: 12, fontWeight: 700, color: 'var(--primary)' }}>{s.activeOffersCount} active</span>
                : <span style={{ fontSize: 12, color: 'var(--outline)' }}>—</span>
              }
            </div>

            {/* Contract validity + chevron */}
            <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
              {s.isContractValid != null && (
                <span style={{
                  fontSize: 10, fontWeight: 700, padding: '2px 8px', borderRadius: 10,
                  background: s.isContractValid ? 'rgba(56,106,32,.1)' : 'rgba(186,26,26,.08)',
                  color: s.isContractValid ? '#386a20' : '#ba1a1a',
                }}>{s.isContractValid ? 'Valid' : 'Expired'}</span>
              )}
              <button onClick={e => { e.stopPropagation(); navigate(`/sponsors/${s.id}`); }}
                style={{ marginLeft: 'auto', background: 'none', border: 'none', cursor: 'pointer', color: 'var(--outline)', display: 'flex', padding: 4 }}>
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
              </button>
            </div>
          </div>
        ))}
      </div>

      {/* Pagination */}
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginTop: 18 }}>
        <div style={{ fontSize: 12, color: 'var(--outline)' }}>
          Showing {displayed.length === 0 ? 0 : (page - 1) * PER_PAGE + 1}–{Math.min(page * PER_PAGE, totalCount || displayed.length)} of {totalCount || displayed.length}
        </div>
        <div style={{ display: 'flex', gap: 4, alignItems: 'center' }}>
          <PaginationBtn onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>
            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M15 18l-6-6 6-6"/></svg>
          </PaginationBtn>
          {Array.from({ length: Math.min(totalPages, 5) }, (_, i) => i + 1).map(p => (
            <PaginationBtn key={p} active={p === page} onClick={() => setPage(p)}>{p}</PaginationBtn>
          ))}
          {totalPages > 5 && <span style={{ color: 'var(--outline)', padding: '0 4px' }}>…</span>}
          {totalPages > 5 && <PaginationBtn onClick={() => setPage(totalPages)}>{totalPages}</PaginationBtn>}
          <PaginationBtn onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}>
            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
          </PaginationBtn>
        </div>
      </div>

      <Modal open={createOpen} onClose={() => setCreateOpen(false)} title="Add New Sponsor" width={640}>
        <SponsorForm onSubmit={handleCreate} loading={creating} onCancel={() => setCreateOpen(false)} />
      </Modal>
    </div>
  );
}
