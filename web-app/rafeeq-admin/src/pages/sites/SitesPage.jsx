import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { getSites, createSite, getDashboardStats } from '../../api/sitesApi';
import { getCities } from '../../api/citiesApi';
import Sidebar from '../../components/layout/Sidebar';
import Modal from '../../components/common/Modal';
import Spinner from '../../components/common/Spinner';
import SiteForm from './components/SiteForm';
import { useToast } from '../../components/common/Toast';
import { formatEnum, SITE_TYPES, SITE_STATUSES } from '../../utils/constants';

/* ── Type badge colors ─────────────────────────────────── */
const TYPE_CFG = {
  historical:     { bg: 'rgba(212,165,116,.18)', text: '#7c572d', border: 'rgba(212,165,116,.4)' },
  museum:         { bg: 'rgba(37,99,235,.08)',   text: '#1d4ed8', border: 'rgba(37,99,235,.25)' },
  archaeological: { bg: 'rgba(126,34,206,.08)',  text: '#7e22ce', border: 'rgba(126,34,206,.25)' },
  natural:        { bg: 'rgba(22,163,74,.08)',   text: '#15803d', border: 'rgba(22,163,74,.25)' },
  religious:      { bg: 'rgba(194,65,10,.08)',   text: '#c2410c', border: 'rgba(194,65,10,.25)' },
  cultural:       { bg: 'rgba(161,98,7,.08)',    text: '#a16207', border: 'rgba(161,98,7,.25)' },
  park:           { bg: 'rgba(22,163,74,.06)',   text: '#166534', border: 'rgba(22,163,74,.2)' },
  entertainment:  { bg: 'rgba(190,24,93,.07)',   text: '#be185d', border: 'rgba(190,24,93,.2)' },
};

const STATUS_CFG = {
  active:            { dot: '#386a20', label: 'Active' },
  underMaintenance:  { dot: '#d97706', label: 'Maintenance' },
  temporarilyClosed: { dot: '#ba1a1a', label: 'Closed' },
  permanentlyClosed: { dot: '#827569', label: 'Perm. Closed' },
};

const FILTERS = ['All', 'Active', 'Inactive', 'Featured', 'Hidden Gems', 'Free', 'Paid'];
const PER_PAGE = 20;

/* ── Atoms ─────────────────────────────────────────────── */
function TypeBadge({ type }) {
  const c = TYPE_CFG[type] || { bg: 'var(--surface-container)', text: 'var(--outline)', border: 'var(--outline-variant)' };
  return (
    <span style={{
      display: 'inline-block', padding: '3px 9px', borderRadius: 4,
      fontSize: 10, fontWeight: 700, letterSpacing: '0.07em', textTransform: 'uppercase',
      background: c.bg, color: c.text, border: `1px solid ${c.border}`,
    }}>
      {formatEnum(type)}
    </span>
  );
}

function StatusDot({ status }) {
  const s = STATUS_CFG[status] || STATUS_CFG.active;
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 7 }}>
      <span style={{ width: 7, height: 7, borderRadius: '50%', background: s.dot, flexShrink: 0, boxShadow: `0 0 0 2px ${s.dot}30` }} />
      <span style={{ fontSize: 12, color: 'var(--text-2)', fontWeight: 600 }}>{s.label}</span>
    </div>
  );
}

function StatCard({ label, value, sub, icon, loading }) {
  return (
    <div style={{
      background: 'var(--surface-container-lowest)', borderRadius: 16,
      padding: '20px 22px', position: 'relative', overflow: 'hidden',
      boxShadow: '0 1px 3px rgba(29,27,23,0.05)',
    }}>
      <div className="pyramid-accent" />
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 10 }}>
        <div style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.09em', textTransform: 'uppercase', color: 'var(--outline)' }}>{label}</div>
        <div style={{
          width: 32, height: 32, borderRadius: 8, flexShrink: 0,
          background: 'rgba(124,87,45,0.08)',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>{icon}</div>
      </div>
      <div style={{ fontFamily: 'var(--font-display)', fontSize: 36, fontWeight: 700, color: 'var(--text)', lineHeight: 1 }}>
        {loading ? '…' : value}
      </div>
      {sub && <div style={{ fontSize: 12, color: 'var(--outline)', marginTop: 6 }}>{sub}</div>}
    </div>
  );
}

function PaginationBtn({ children, active, onClick, disabled }) {
  return (
    <button onClick={onClick} disabled={disabled} style={{
      width: 32, height: 32, borderRadius: 8, fontSize: 13, fontWeight: 600,
      border: 'none',
      background: active ? 'linear-gradient(135deg, var(--primary), var(--primary-container))' : 'var(--surface-container-low)',
      color: active ? '#fff' : 'var(--text-2)',
      cursor: disabled ? 'default' : 'pointer',
      display: 'flex', alignItems: 'center', justifyContent: 'center',
      opacity: disabled ? 0.4 : 1,
      boxShadow: active ? '0 2px 8px rgba(124,87,45,0.3)' : 'none',
    }}>{children}</button>
  );
}

/* ── Main ──────────────────────────────────────────────── */
export default function SitesPage() {
  const navigate = useNavigate();
  const toast    = useToast();

  const [sites,        setSites]        = useState([]);
  const [totalCount,   setTotalCount]   = useState(0);
  const [loading,      setLoading]      = useState(false);
  const [createOpen,   setCreateOpen]   = useState(false);
  const [creating,     setCreating]     = useState(false);
  const [activeFilter, setActiveFilter] = useState('All');
  const [topSearch,    setTopSearch]    = useState('');
  const [page,         setPage]         = useState(1);
  const [cities,       setCities]       = useState([]);
  const [cityFilter,   setCityFilter]   = useState('');
  const [typeFilter,   setTypeFilter]   = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [statsLoading, setStatsLoading] = useState(true);
  const [stats, setStats] = useState({ totalSites: 0, activeSites: 0, featuredSites: 0, hiddenGemSites: 0, averageRating: 0 });

  /* ── fetch ── */
  const buildParams = (page = 1) => {
    const p = { page, pageSize: PER_PAGE };
    if (topSearch?.trim()) p.searchTerm = topSearch.trim();
    if (typeFilter)   p.type = typeFilter;
    if (cityFilter)   p.city = cityFilter;
    if (statusFilter) p.status = statusFilter;
    if (activeFilter === 'Free') p.isFree = true;
    if (activeFilter === 'Paid') p.isFree = false;
    return p;
  };

  const loadSites = useCallback(async (page = 1) => {
    try {
      setLoading(true);
      const res = await getSites(buildParams(page));
      const d = res.data;
      const items = Array.isArray(d)
        ? d
        : Array.isArray(d?.data)
          ? d.data
          : Array.isArray(d?.value)
            ? d.value
            : Array.isArray(d?.items)
              ? d.items
              : [];
      setSites(items);
      setTotalCount(Number(d?.totalCount ?? d?.value?.totalCount ?? items.length ?? 0));
    } catch {
      setSites([]);
      setTotalCount(0);
    }
    finally { setLoading(false); }
  }, [topSearch, typeFilter, cityFilter, statusFilter, activeFilter]);

  const loadStats = useCallback(async () => {
    try {
      setStatsLoading(true);
      const res = await getDashboardStats();
      const d = res.data?.value ?? res.data?.data ?? res.data;
      setStats({
        totalSites:    Number(d?.totalSites    ?? 0),
        activeSites:   Number(d?.activeSites   ?? 0),
        featuredSites: Number(d?.featuredSites  ?? 0),
        hiddenGemSites:Number(d?.hiddenGemSites ?? 0),
        averageRating: Number(d?.averageRating  ?? 0),
      });
    } catch { /* keep defaults */ }
    finally { setStatsLoading(false); }
  }, []);

  useEffect(() => { loadSites(1); loadStats(); }, [loadSites, loadStats]);

  useEffect(() => {
    let active = true;
    getCities().then(res => {
      const d = res.data;
      const items = Array.isArray(d) ? d : Array.isArray(d?.value) ? d.value : Array.isArray(d?.items) ? d.items : Array.isArray(d?.data) ? d.data : [];
      if (active) setCities(items);
    }).catch(() => {});
    return () => { active = false; };
  }, []);

  const applyFilters = () => {
    setPage(1);
    loadSites(1);
  };

  const clearFilters = () => {
    setTopSearch(''); setTypeFilter(''); setCityFilter(''); setStatusFilter('');
    setActiveFilter('All');
    setPage(1);
    loadSites(1);
  };

  const handlePageChange = (nextPage) => {
    setPage(nextPage);
    loadSites(nextPage);
  };

  const handleCreate = async (payload) => {
    setCreating(true);
    try {
      const res = await createSite(payload);
      toast('Site created', 'success');
      setCreateOpen(false);
      loadStats();
      const newId = res.data?.value?.id ?? res.data?.id;
      newId ? navigate(`/sites/${newId}`) : loadSites(page);
    } catch (e) {
      console.error('Create site failed:', e.response?.data || e);
      toast('Failed to create site', 'error');
    } finally { setCreating(false); }
  };

  /* ── client-side filter for quick pills ── */
  const filtered = sites.filter(s => {
    if (activeFilter === 'Active'      && s.status !== 'active') return false;
    if (activeFilter === 'Inactive'    && s.status === 'active') return false;
    if (activeFilter === 'Featured'    && !s.isFeatured)         return false;
    if (activeFilter === 'Hidden Gems' && !s.isHiddenGem)        return false;
    return true;
  });

  const totalPages = Math.max(1, Math.ceil(totalCount / PER_PAGE));

  const GRID = '2.6fr 1fr 1.3fr 0.8fr 1fr 1fr';

  const selectStyle = {
    flex: 1, minWidth: 130, padding: '9px 32px 9px 12px',
    background: 'var(--surface-container-lowest)',
    border: '1px solid rgba(212,196,183,.3)',
    borderRadius: 10, fontSize: 13, color: 'var(--text-2)', outline: 'none',
    cursor: 'pointer', appearance: 'none',
    backgroundImage: `url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='%23827569' stroke-width='2'%3E%3Cpath d='M6 9l6 6 6-6'/%3E%3C/svg%3E")`,
    backgroundRepeat: 'no-repeat', backgroundPosition: 'right 10px center',
  };

  return (
    <div style={{ display: 'flex', minHeight: '100vh', background: 'var(--background)', fontFamily: 'var(--font-body)' }}>
      <Sidebar />

      <div style={{ marginLeft: 'var(--sidebar-width)', flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>

        {/* Top bar */}
        <header style={{
          background: 'rgba(255,248,240,0.92)', backdropFilter: 'blur(12px)',
          borderBottom: '1px solid rgba(212,196,183,.2)',
          padding: '0 32px', height: 64,
          display: 'flex', alignItems: 'center', gap: 14,
          position: 'sticky', top: 0, zIndex: 50,
        }}>
        </header>

        {/* Body */}
        <div style={{ padding: '28px 32px 80px', flex: 1 }}>

          {/* Breadcrumb */}
          <div style={{ fontSize: 12, color: 'var(--outline)', marginBottom: 10, display: 'flex', alignItems: 'center', gap: 6 }}>
            <span style={{ cursor: 'pointer', color: 'var(--text-2)' }}>Dashboard</span>
            <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
            <span>Sites Management</span>
          </div>

          {/* Header */}
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 24, flexWrap: 'wrap', gap: 12 }}>
            <h1 style={{ fontFamily: 'var(--font-display)', fontSize: 28, fontWeight: 700, color: 'var(--text)', display: 'flex', alignItems: 'baseline', gap: 10, margin: 0 }}>
              Sites Management
              <span style={{ fontSize: 18, fontWeight: 400, color: 'var(--outline)' }}>• {sites.length} Total</span>
            </h1>
            <div style={{ display: 'flex', gap: 10 }}>
              <button onClick={() => setCreateOpen(true)} style={{
                display: 'flex', alignItems: 'center', gap: 7, padding: '9px 18px', borderRadius: 10,
                background: 'linear-gradient(135deg, var(--primary), var(--primary-container))',
                color: '#fff', border: 'none', fontSize: 13, fontWeight: 700, cursor: 'pointer',
                boxShadow: '0 2px 10px rgba(124,87,45,0.25)',
              }}>
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M12 5v14M5 12h14"/></svg>
                Add New Site
              </button>
            </div>
          </div>

          {/* Stats */}
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3,1fr)', gap: 16, marginBottom: 28 }}>
            <StatCard
              label="Total Active Sites" value={stats.activeSites} loading={statsLoading}
              sub={<span>of {stats.totalSites} total • <span style={{ color: 'var(--green)', fontWeight: 600 }}>Across Egypt</span></span>}
              icon={<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="var(--primary)" strokeWidth="2"><polyline points="22 7 13.5 15.5 8.5 10.5 2 17"/><polyline points="16 7 22 7 22 13"/></svg>}
            />
            <StatCard
              label="Featured Sites" value={stats.featuredSites} loading={statsLoading}
              sub={`Hidden Gems: ${stats.hiddenGemSites}`}
              icon={<svg width="16" height="16" viewBox="0 0 24 24" fill="var(--primary)" stroke="none"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>}
            />
            <StatCard
              label="Avg Rating" loading={statsLoading}
              value={<span>{stats.averageRating > 0 ? stats.averageRating.toFixed(1) : '—'} <span style={{ fontSize: 18, color: 'var(--primary-container)' }}>★★★★½</span></span>}
              sub="Overall satisfaction"
              icon={<svg width="16" height="16" viewBox="0 0 24 24" fill="var(--primary)" stroke="none"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>}
            />
          </div>

          {/* Filter pills */}
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 14 }}>
            {(activeFilter !== 'All' || topSearch || typeFilter || cityFilter || statusFilter) && (
              <button onClick={clearFilters} style={{
                background: 'none', border: 'none', fontSize: 13, color: 'var(--primary)', cursor: 'pointer', fontWeight: 600,
              }}>Clear Filters</button>
            )}
          </div>

          {/* Search + filters row */}
          <div style={{ display: 'flex', gap: 10, marginBottom: 20, flexWrap: 'wrap' }}>
            <div style={{ flex: 2, minWidth: 220, position: 'relative' }}>
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="var(--outline)" strokeWidth="2"
                style={{ position: 'absolute', left: 11, top: '50%', transform: 'translateY(-50%)', pointerEvents: 'none' }}>
                <circle cx="11" cy="11" r="8"/><path d="M21 21l-4.35-4.35"/>
              </svg>
              <input
                value={topSearch}
                onChange={e => setTopSearch(e.target.value)}
                onKeyDown={e => e.key === 'Enter' && applyFilters()}
                placeholder="Global search input..."
                style={{
                  width: '100%', padding: '9px 14px 9px 34px',
                  background: 'var(--surface-container-lowest)',
                  border: '1px solid rgba(212,196,183,.3)',
                  borderRadius: 10, fontSize: 13, color: 'var(--text)', outline: 'none',
                }}
              />
            </div>

            <select value={cityFilter} onChange={e => setCityFilter(e.target.value)} style={selectStyle}>
              <option value="">City</option>
              {cities.map(c => <option key={c.id} value={c.id}>{c.name || c.id}</option>)}
            </select>

            <select value={typeFilter} onChange={e => setTypeFilter(e.target.value)} style={selectStyle}>
              <option value="">Site Type</option>
              {SITE_TYPES.map(t => <option key={t} value={t}>{formatEnum(t)}</option>)}
            </select>

            <select value={statusFilter} onChange={e => setStatusFilter(e.target.value)} style={selectStyle}>
              <option value="">Status</option>
              {SITE_STATUSES.map(s => <option key={s} value={s}>{formatEnum(s)}</option>)}
            </select>

            <button onClick={applyFilters} style={{
              padding: '9px 20px', borderRadius: 10,
              background: 'linear-gradient(135deg, var(--primary), var(--primary-container))',
              color: '#fff', border: 'none', fontSize: 13, fontWeight: 700, cursor: 'pointer',
              boxShadow: '0 2px 8px rgba(124,87,45,.25)', whiteSpace: 'nowrap',
            }}>Apply Filters</button>
          </div>

          {/* Table */}
          <div style={{
            background: 'var(--surface-container-lowest)',
            borderRadius: 16, overflow: 'hidden',
            boxShadow: '0 1px 4px rgba(29,27,23,.06)',
          }}>
            {/* Header */}
            <div style={{
              display: 'grid', gridTemplateColumns: GRID,
              padding: '10px 20px',
              background: 'var(--surface-container-low)',
              borderBottom: '1px solid rgba(212,196,183,.25)',
            }}>
              {['SITE', 'TYPE', 'STATUS', 'FEE', 'STATS', 'FEATURES'].map(h => (
                <div key={h} style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.08em', color: 'var(--outline)', textTransform: 'uppercase' }}>{h}</div>
              ))}
            </div>

            {loading ? (
              <div style={{ padding: 56 }}><Spinner center /></div>
            ) : filtered.length === 0 ? (
              <div style={{ padding: '56px 20px', textAlign: 'center', color: 'var(--outline)', fontSize: 14 }}>
                No sites found.{' '}
                {sites.length === 0 && <button onClick={() => setCreateOpen(true)} style={{ color: 'var(--primary)', background: 'none', border: 'none', cursor: 'pointer', fontWeight: 600, fontSize: 14 }}>Create the first one.</button>}
              </div>
            ) : filtered.map((site, i) => (
              <div
                key={site.id}
                onClick={() => navigate(`/sites/${site.id}`)}
                style={{
                  display: 'grid', gridTemplateColumns: GRID,
                  padding: '13px 20px', alignItems: 'center',
                  borderBottom: i < filtered.length - 1 ? '1px solid rgba(212,196,183,.2)' : 'none',
                  cursor: 'pointer', transition: 'background .1s',
                }}
                onMouseOver={e => e.currentTarget.style.background = 'var(--surface-container-low)'}
                onMouseOut={e => e.currentTarget.style.background = 'transparent'}
              >
                {/* Site */}
                <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                  <div style={{
                    width: 44, height: 44, borderRadius: 10, flexShrink: 0,
                    background: 'rgba(212,165,116,.2)',
                    overflow: 'hidden', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 20,
                  }}>
                    {site.primaryImageUrl
                      ? <img src={site.primaryImageUrl} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} onError={e => e.target.style.display = 'none'} />
                      : '🏛️'
                    }
                  </div>
                  <div>
                    <div style={{ fontWeight: 700, color: 'var(--text)', fontSize: 13, marginBottom: 2 }}>{site.name || '(unnamed)'}</div>
                    <div style={{ fontSize: 11, color: 'var(--outline)', display: 'flex', alignItems: 'center', gap: 3 }}>
                      <svg width="9" height="9" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M21 10c0 7-9 13-9 13S3 17 3 10a9 9 0 0118 0z"/><circle cx="12" cy="10" r="3"/></svg>
                      {site.cityName || 'Egypt'}
                    </div>
                  </div>
                </div>

                {/* Type */}
                <div><TypeBadge type={site.type} /></div>

                {/* Status */}
                <div onClick={e => e.stopPropagation()}><StatusDot status={site.status} /></div>

                {/* Fee */}
                <div>
                  {site.isFree
                    ? <span style={{ fontSize: 12, color: 'var(--green)', fontWeight: 700 }}>Free</span>
                    : <div>
                        <div style={{ fontSize: 9, color: 'var(--outline)', fontWeight: 700, textTransform: 'uppercase', letterSpacing: '0.06em' }}>EGP</div>
                        <div style={{ fontSize: 13, fontWeight: 700, color: 'var(--text)' }}>
                          {site.entryTicket?.egyptianTicketPrice?.amount ?? '—'}
                        </div>
                      </div>
                  }
                </div>

                {/* Stats */}
                <div>
                  {site.averageRating
                    ? <div>
                        <div style={{ display: 'flex', alignItems: 'center', gap: 4 }}>
                          <span style={{ color: 'var(--primary-container)', fontWeight: 700 }}>★</span>
                          <span style={{ fontSize: 13, fontWeight: 700, color: 'var(--text)' }}>{site.averageRating.toFixed(1)}</span>
                        </div>
                        {site.totalRatings > 0 && (
                          <div style={{ fontSize: 11, color: 'var(--outline)' }}>{site.totalRatings.toLocaleString()} reviews</div>
                        )}
                      </div>
                    : <span style={{ color: 'var(--outline)', fontSize: 12 }}>—</span>
                  }
                </div>

                {/* Features */}
                <div style={{ display: 'flex', alignItems: 'center', gap: 5 }}>
                  {site.isFeatured && (
                    <span title="Featured" style={{
                      width: 24, height: 24, borderRadius: '50%',
                      background: 'rgba(212,165,116,.15)', border: '1px solid rgba(212,165,116,.4)',
                      display: 'inline-flex', alignItems: 'center', justifyContent: 'center', fontSize: 11,
                    }}>⭐</span>
                  )}
                  {site.isHiddenGem && (
                    <span title="Hidden Gem" style={{
                      width: 24, height: 24, borderRadius: '50%',
                      background: 'rgba(37,99,235,.08)', border: '1px solid rgba(37,99,235,.2)',
                      display: 'inline-flex', alignItems: 'center', justifyContent: 'center', fontSize: 11,
                    }}>💎</span>
                  )}
                  <button
                    onClick={e => { e.stopPropagation(); navigate(`/sites/${site.id}`); }}
                    style={{ marginLeft: 'auto', background: 'none', border: 'none', cursor: 'pointer', color: 'var(--outline)', display: 'flex', padding: 4 }}
                  >
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
                  </button>
                </div>
              </div>
            ))}
          </div>

          {/* Pagination */}
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginTop: 18 }}>
            <div style={{ fontSize: 12, color: 'var(--outline)', display: 'flex', alignItems: 'center', gap: 10 }}>
              Showing {filtered.length === 0 ? 0 : (page - 1) * PER_PAGE + 1}–{Math.min(page * PER_PAGE, filtered.length)} of {filtered.length}
            </div>
            <div style={{ display: 'flex', gap: 4, alignItems: 'center' }}>
              <PaginationBtn onClick={() => handlePageChange(Math.max(1, page - 1))} disabled={page === 1}>
                <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M15 18l-6-6 6-6"/></svg>
              </PaginationBtn>
              {Array.from({ length: Math.min(totalPages, 5) }, (_, i) => i + 1).map(p => (
                <PaginationBtn key={p} active={p === page} onClick={() => handlePageChange(p)}>{p}</PaginationBtn>
              ))}
              {totalPages > 5 && <span style={{ color: 'var(--outline)', padding: '0 4px' }}>…</span>}
              {totalPages > 5 && <PaginationBtn onClick={() => handlePageChange(totalPages)}>{totalPages}</PaginationBtn>}
              <PaginationBtn onClick={() => handlePageChange(Math.min(totalPages, page + 1))} disabled={page === totalPages}>
                <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
              </PaginationBtn>
            </div>
          </div>
        </div>
      </div>

      <Modal open={createOpen} onClose={() => setCreateOpen(false)} title="Create New Site" width={680}>
        <SiteForm onSubmit={handleCreate} loading={creating} onCancel={() => setCreateOpen(false)} />
      </Modal>
    </div>
  );
}
