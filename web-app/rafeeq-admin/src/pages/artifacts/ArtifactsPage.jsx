import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { getArtifacts, getArtifactsBySiteId, createArtifact, getDashboardStats } from '../../api/artifactsApi';
import { searchSites } from '../../api/sitesApi';
import Modal from '../../components/common/Modal';
import Spinner from '../../components/common/Spinner';
import ArtifactForm from './components/ArtifactForm';
import { useToast } from '../../components/common/Toast';
import { formatEnum, ARTIFACT_TYPES } from '../../utils/constants';

const TYPE_CFG = {
  pyramid:                { bg: 'rgba(212,165,116,.18)', text: '#7c572d', border: 'rgba(212,165,116,.4)' },
  temple:                 { bg: 'rgba(126,34,206,.08)',  text: '#7e22ce', border: 'rgba(126,34,206,.25)' },
  tomb:                   { bg: 'rgba(107,114,128,.1)',  text: '#6b7280', border: 'rgba(107,114,128,.25)' },
  statue:                 { bg: 'rgba(37,99,235,.08)',   text: '#1d4ed8', border: 'rgba(37,99,235,.25)' },
  monument:               { bg: 'rgba(194,65,10,.08)',   text: '#c2410c', border: 'rgba(194,65,10,.25)' },
  mosque:                 { bg: 'rgba(22,163,74,.08)',   text: '#15803d', border: 'rgba(22,163,74,.25)' },
  church:                 { bg: 'rgba(161,98,7,.08)',    text: '#a16207', border: 'rgba(161,98,7,.25)' },
  palace:                 { bg: 'rgba(190,24,93,.07)',   text: '#be185d', border: 'rgba(190,24,93,.2)' },
  fortress:               { bg: 'rgba(107,114,128,.1)',  text: '#4b5563', border: 'rgba(107,114,128,.3)' },
  ruins:                  { bg: 'rgba(161,98,7,.08)',    text: '#92400e', border: 'rgba(161,98,7,.25)' },
  garden:                 { bg: 'rgba(22,163,74,.06)',   text: '#166534', border: 'rgba(22,163,74,.2)' },
  exhibition:             { bg: 'rgba(37,99,235,.08)',   text: '#2563eb', border: 'rgba(37,99,235,.25)' },
  museumHall:             { bg: 'rgba(37,99,235,.08)',   text: '#1d4ed8', border: 'rgba(37,99,235,.25)' },
  archaeologicalStructure:{ bg: 'rgba(126,34,206,.08)',  text: '#7e22ce', border: 'rgba(126,34,206,.25)' },
  historicBuilding:       { bg: 'rgba(212,165,116,.18)', text: '#7c572d', border: 'rgba(212,165,116,.4)' },
  viewingPoint:           { bg: 'rgba(22,163,74,.08)',   text: '#15803d', border: 'rgba(22,163,74,.25)' },
};

const PAGE_SIZE = 20;

/* ── Small shared atoms ─────────────────────────────────── */
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
        <div style={{ width: 32, height: 32, borderRadius: 8, background: 'rgba(124,87,45,0.08)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>{icon}</div>
      </div>
      <div style={{ fontFamily: 'var(--font-display)', fontSize: 36, fontWeight: 700, color: 'var(--text)', lineHeight: 1 }}>
        {loading ? '…' : value}
      </div>
      {sub && <div style={{ fontSize: 12, color: 'var(--outline)', marginTop: 6 }}>{sub}</div>}
    </div>
  );
}

function ArtifactCard({ artifact, onClick }) {
  const mainImage = artifact.mainImageUrl
    || artifact.images?.find(i => i.isMain)?.url
    || artifact.images?.[0]?.url;

  return (
    <article
      onClick={onClick}
      style={{
        background: 'var(--surface-container-lowest)', borderRadius: 16,
        overflow: 'hidden', cursor: 'pointer', position: 'relative',
        boxShadow: '0 1px 4px rgba(29,27,23,.06)',
        border: '1px solid rgba(212,196,183,.15)',
        transition: 'box-shadow .2s, transform .2s',
      }}
      onMouseOver={e => { e.currentTarget.style.boxShadow = '0 8px 24px rgba(124,87,45,.1)'; e.currentTarget.style.transform = 'translateY(-2px)'; }}
      onMouseOut={e => { e.currentTarget.style.boxShadow = '0 1px 4px rgba(29,27,23,.06)'; e.currentTarget.style.transform = 'translateY(0)'; }}
    >
      <div style={{ position: 'relative', height: 200, overflow: 'hidden', background: 'var(--surface-container)' }}>
        {mainImage ? (
          <img src={mainImage} alt={artifact.name || ''} style={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block' }} onError={e => e.target.style.display = 'none'} />
        ) : (
          <div style={{ width: '100%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--outline)', fontSize: 40 }}>🏛️</div>
        )}
      </div>
      <div style={{ padding: '16px 18px 18px' }}>
        <div style={{ display: 'flex', gap: 6, marginBottom: 10, flexWrap: 'wrap' }}>
          <TypeBadge type={artifact.type} />
        </div>
        <h3 style={{ fontFamily: 'var(--font-display)', fontSize: 16, fontWeight: 700, color: 'var(--text)', marginBottom: 6, lineHeight: 1.3 }}>
          {artifact.name || '(unnamed)'}
        </h3>
        {artifact.siteName && (
          <div style={{ fontSize: 11, color: 'var(--outline)', display: 'flex', alignItems: 'center', gap: 4 }}>
            <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2V9z"/><polyline points="9 22 9 12 15 12 15 22"/></svg>
            {artifact.siteName}
          </div>
        )}
      </div>
      <div className="pyramid-accent" />
    </article>
  );
}

/* ── Pagination bar ─────────────────────────────────────── */
function Pagination({ page, totalPages, totalCount, onPageChange }) {
  if (totalPages <= 1) return null;

  const from = (page - 1) * PAGE_SIZE + 1;
  const to   = Math.min(page * PAGE_SIZE, totalCount);

  const pages = () => {
    // Always show first, last, current ±1, and ellipses
    const set = new Set([1, totalPages, page, page - 1, page + 1].filter(p => p >= 1 && p <= totalPages));
    return [...set].sort((a, b) => a - b);
  };

  const pageList = pages();

  return (
    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginTop: 24, flexWrap: 'wrap', gap: 12 }}>
      <div style={{ fontSize: 12, color: 'var(--outline)' }}>
        Showing <strong style={{ color: 'var(--text)' }}>{from}–{to}</strong> of <strong style={{ color: 'var(--text)' }}>{totalCount.toLocaleString()}</strong> artifacts
      </div>

      <div style={{ display: 'flex', gap: 4, alignItems: 'center' }}>
        {/* Prev */}
        <button
          onClick={() => onPageChange(page - 1)}
          disabled={page === 1}
          style={{
            width: 32, height: 32, borderRadius: 8, border: 'none', cursor: page === 1 ? 'default' : 'pointer',
            background: 'var(--surface-container-low)', color: 'var(--text-2)',
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            opacity: page === 1 ? 0.4 : 1,
          }}
        >
          <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M15 18l-6-6 6-6"/></svg>
        </button>

        {/* Page numbers */}
        {pageList.map((p, i) => {
          const prev = pageList[i - 1];
          const showEllipsis = prev && p - prev > 1;
          return (
            <React.Fragment key={p}>
              {showEllipsis && (
                <span style={{ width: 32, textAlign: 'center', fontSize: 13, color: 'var(--outline)' }}>…</span>
              )}
              <button
                onClick={() => onPageChange(p)}
                style={{
                  width: 32, height: 32, borderRadius: 8, border: 'none', cursor: 'pointer',
                  fontSize: 13, fontWeight: p === page ? 700 : 500,
                  background: p === page
                    ? 'linear-gradient(135deg, var(--primary), var(--primary-container))'
                    : 'var(--surface-container-low)',
                  color: p === page ? '#fff' : 'var(--text-2)',
                  boxShadow: p === page ? '0 2px 8px rgba(124,87,45,0.3)' : 'none',
                  transition: 'all .15s',
                }}
              >
                {p}
              </button>
            </React.Fragment>
          );
        })}

        {/* Next */}
        <button
          onClick={() => onPageChange(page + 1)}
          disabled={page === totalPages}
          style={{
            width: 32, height: 32, borderRadius: 8, border: 'none', cursor: page === totalPages ? 'default' : 'pointer',
            background: 'var(--surface-container-low)', color: 'var(--text-2)',
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            opacity: page === totalPages ? 0.4 : 1,
          }}
        >
          <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
        </button>
      </div>
    </div>
  );
}

/* ══════════════════════════════════════════════════════════
   Main page
══════════════════════════════════════════════════════════ */
export default function ArtifactsPage() {
  const navigate = useNavigate();
  const toast    = useToast();

  const [artifacts,      setArtifacts]      = useState([]);
  const [totalCount,     setTotalCount]     = useState(0);
  const [totalPages,     setTotalPages]     = useState(1);
  const [page,           setPage]           = useState(1);
  const [loading,        setLoading]        = useState(false);
  const [createOpen,     setCreateOpen]     = useState(false);
  const [creating,       setCreating]       = useState(false);
  const [topSearch,      setTopSearch]      = useState('');
  const [siteQuery,      setSiteQuery]      = useState('');
  const [siteResults,    setSiteResults]    = useState([]);
  const [siteSearching,  setSiteSearching]  = useState(false);
  const [siteTouched,    setSiteTouched]    = useState(false);
  const [selectedSiteId, setSelectedSiteId] = useState('');
  const [selectedSiteName,setSelectedSiteName]=useState('');
  const [siteFocused,    setSiteFocused]    = useState(false);
  const [typeFilter,     setTypeFilter]     = useState('');
  const [statsLoading,   setStatsLoading]   = useState(true);
  const [stats,          setStats]          = useState({ totalArtifacts: 0, assignedToSites: 0 });

  /* ── Core load function ── */
  const loadArtifacts = useCallback(async ({ siteId, search, type, pg } = {}) => {
    setLoading(true);
    try {
      if (siteId) {
        /* ── Site selected → fetch by site (no server pagination) ── */
        const params = {};
        if (search?.trim()) params.searchTerm = search.trim();
        if (type)           params.type       = type;

        const res   = await getArtifactsBySiteId(siteId, params);
        const d     = res.data;
        const items = d?.data ?? d?.value?.data ?? d?.items ?? (Array.isArray(d) ? d : []);

        setArtifacts(items);
        setTotalCount(items.length);
        setTotalPages(1);           // no server pagination for this endpoint
        setPage(1);

      } else {
        /* ── No site → fetch all with server-side pagination ── */
        const params = { page: pg ?? 1, pageSize: PAGE_SIZE };
        if (search?.trim()) params.searchTerm = search.trim();
        if (type)           params.type       = type;

        const res = await getArtifacts(params);
        const d   = res.data?.value ?? res.data;

        // PagedResult<ArtifactListDto>
        const items = Array.isArray(d?.data)  ? d.data
                    : Array.isArray(d?.items) ? d.items
                    : Array.isArray(d)        ? d
                    : [];

        const count = d?.totalCount ?? items.length;
        const pages = d?.totalPages ?? Math.ceil(count / PAGE_SIZE) | 1;

        setArtifacts(items);
        setTotalCount(count);
        setTotalPages(pages);
        setPage(d?.pageNumber ?? pg ?? 1);
      }
    } catch {
      setArtifacts([]);
      setTotalCount(0);
      setTotalPages(1);
    } finally {
      setLoading(false);
    }
  }, []);

  /* ── Stats ── */
  const loadStats = useCallback(async () => {
    try {
      setStatsLoading(true);
      const res = await getDashboardStats();
      const d   = res.data?.value ?? res.data?.data ?? res.data;
      setStats({
        totalArtifacts:  Number(d?.totalArtifacts  ?? 0),
        assignedToSites: Number(d?.assignedToSites ?? 0),
      });
    } catch { /* keep defaults */ }
    finally { setStatsLoading(false); }
  }, []);

  /* ── Initial load (all artifacts, page 1) ── */
  // useEffect(() => {
  //   console.log("initial load");
  //   loadStats();
  //   loadArtifacts({ pg: 1 });
  // }, []);
  
  /* ── Re-fetch when selected site changes ── */
  useEffect(() => {
    loadStats();
    loadArtifacts({ siteId: selectedSiteId || undefined, search: topSearch, type: typeFilter, pg: 1 });
  }, [selectedSiteId]);

  /* ── Site search autocomplete ── */
  useEffect(() => {
    if (selectedSiteId && !siteFocused) return;
    const q = siteQuery.trim();
    if (!q) { setSiteResults([]); setSiteSearching(false); return; }

    let cancelled = false;
    const handle  = setTimeout(async () => {
      setSiteSearching(true);
      try {
        const res   = await searchSites({ q });
        const d     = res?.data;
        const items = Array.isArray(d) ? d : (d?.data ?? d?.value ?? d?.items ?? d?.results ?? []);
        if (!cancelled) setSiteResults((Array.isArray(items) ? items : []).slice(0, 10));
      } catch {
        if (!cancelled) setSiteResults([]);
      } finally {
        if (!cancelled) setSiteSearching(false);
      }
    }, 700);

    return () => { cancelled = true; clearTimeout(handle); };
  }, [siteQuery]);

  /* ── Handlers ── */
  const applyFilters = (pg = 1) => {
    loadArtifacts({ siteId: selectedSiteId || undefined, search: topSearch, type: typeFilter, pg });
  };

  const handlePageChange = (newPage) => {
    // Site-mode has no pagination
    if (selectedSiteId) return;
    setPage(newPage);
    loadArtifacts({ search: topSearch, type: typeFilter, pg: newPage });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const clearFilters = () => {
    setTopSearch(''); setTypeFilter('');
    setSiteQuery(''); setSiteResults([]); setSiteTouched(false);
    setSelectedSiteId(''); setSelectedSiteName('');
    setSiteFocused(false);
    loadArtifacts({ pg: 1 });
  };

  const handleCreate = async (payload) => {
    setCreating(true);
    try {
      const res = await createArtifact(payload);
      toast('Artifact created', 'success');
      setCreateOpen(false);
      loadStats();
      const newId = res.data?.value?.id ?? res.data?.value ?? res.data?.id ?? res.data?.data?.id ?? res.data?.data ?? res.data;
      newId ? navigate(`/artifacts/${newId}`) : applyFilters(page);
    } catch (e) {
      console.error('Create artifact failed:', e.response?.data || e);
      toast('Failed to create artifact', 'error');
    } finally { setCreating(false); }
  };

  /* ── Styles ── */
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
    <div style={{ padding: '28px 32px 80px', fontFamily: 'var(--font-body)' }}>

      {/* Breadcrumb */}
      <div style={{ fontSize: 12, color: 'var(--outline)', marginBottom: 10, display: 'flex', alignItems: 'center', gap: 6 }}>
        <span style={{ cursor: 'pointer', color: 'var(--text-2)' }} onClick={() => navigate('/')}>Dashboard</span>
        <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
        <span>Artifacts Management</span>
      </div>

      {/* Header */}
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 24, flexWrap: 'wrap', gap: 12 }}>
        <h1 style={{ fontFamily: 'var(--font-display)', fontSize: 28, fontWeight: 700, color: 'var(--text)', display: 'flex', alignItems: 'baseline', gap: 10, margin: 0 }}>
          Artifacts Management
          <span style={{ fontSize: 18, fontWeight: 400, color: 'var(--outline)' }}>• {totalCount.toLocaleString()} Total</span>
        </h1>
        <button onClick={() => setCreateOpen(true)} style={{
          display: 'flex', alignItems: 'center', gap: 7, padding: '9px 18px', borderRadius: 10,
          background: 'linear-gradient(135deg, var(--primary), var(--primary-container))',
          color: '#fff', border: 'none', fontSize: 13, fontWeight: 700, cursor: 'pointer',
          boxShadow: '0 2px 10px rgba(124,87,45,0.25)',
        }}>
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M12 5v14M5 12h14"/></svg>
          Add Artifact
        </button>
      </div>

      {/* Stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2,1fr)', gap: 16, marginBottom: 28 }}>
        <StatCard
          label="Total Artifacts" value={stats.totalArtifacts} loading={statsLoading}
          sub={<span>{stats.assignedToSites} assigned to sites</span>}
          icon={<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="var(--primary)" strokeWidth="2"><polyline points="22 7 13.5 15.5 8.5 10.5 2 17"/><polyline points="16 7 22 7 22 13"/></svg>}
        />
        <StatCard
          label="Artifacts Assigned to Sites" value={stats.assignedToSites} loading={statsLoading}
          sub="Highlighted artifacts"
          icon={<svg width="16" height="16" viewBox="0 0 24 24" fill="var(--primary)" stroke="none"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>}
        />
      </div>

      {/* Clear filters */}
      {(topSearch || typeFilter || selectedSiteId) && (
        <div style={{ marginBottom: 14 }}>
          <button onClick={clearFilters} style={{ background: 'none', border: 'none', fontSize: 13, color: 'var(--primary)', cursor: 'pointer', fontWeight: 600 }}>
            Clear Filters
          </button>
        </div>
      )}

      {/* Search + filters row */}
      <div style={{ display: 'flex', gap: 10, marginBottom: 20, flexWrap: 'wrap' }}>
        {/* Artifact search */}
        <div style={{ flex: 2, minWidth: 220, position: 'relative' }}>
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="var(--outline)" strokeWidth="2"
            style={{ position: 'absolute', left: 11, top: '50%', transform: 'translateY(-50%)', pointerEvents: 'none' }}>
            <circle cx="11" cy="11" r="8"/><path d="M21 21l-4.35-4.35"/>
          </svg>
          <input
            value={topSearch}
            onChange={e => setTopSearch(e.target.value)}
            onKeyDown={e => e.key === 'Enter' && applyFilters(1)}
            placeholder="Search artifacts…"
            style={{ width: '100%', padding: '9px 14px 9px 34px', background: 'var(--surface-container-lowest)', border: '1px solid rgba(212,196,183,.3)', borderRadius: 10, fontSize: 13, color: 'var(--text)', outline: 'none' }}
          />
        </div>

        {/* Site search */}
        <div style={{ flex: 2, minWidth: 220, position: 'relative' }}>
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="var(--outline)" strokeWidth="2"
            style={{ position: 'absolute', left: 11, top: '50%', transform: 'translateY(-50%)', pointerEvents: 'none' }}>
            <path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2V9z"/><polyline points="9 22 9 12 15 12 15 22"/>
          </svg>
          <input
            value={siteQuery}
            onChange={e => { setSiteQuery(e.target.value); setSiteTouched(true); setSiteFocused(true); if (selectedSiteId) setSelectedSiteId(''); }}
            onKeyDown={e => e.key === 'Enter' && applyFilters(1)}
            onFocus={() => setSiteFocused(true)}
            onBlur={() => setTimeout(() => setSiteFocused(false), 150)}
            placeholder="Filter by site…"
            style={{ width: '100%', padding: '9px 14px 9px 34px', background: 'var(--surface-container-lowest)', border: '1px solid rgba(212,196,183,.3)', borderRadius: 10, fontSize: 13, color: 'var(--text)', outline: 'none' }}
          />

          {/* Site dropdown */}
          {siteFocused && (siteQuery.trim() || siteTouched) && (
            <div style={{ position: 'absolute', top: 'calc(100% + 6px)', left: 0, right: 0, zIndex: 10, background: 'var(--surface-container-lowest)', border: '1px solid rgba(212,196,183,.35)', borderRadius: 12, overflow: 'hidden', boxShadow: '0 10px 25px rgba(29,27,23,.08)' }}>
              {siteSearching ? (
                <div style={{ padding: '10px 12px', fontSize: 12, color: 'var(--text-2)' }}>Searching…</div>
              ) : siteResults.length ? siteResults.map(s => (
                <button key={s.id} type="button" onMouseDown={e => e.preventDefault()}
                  onClick={() => { setSelectedSiteId(s.id ?? ''); setSelectedSiteName(s.name || s.title || s.id || ''); setSiteQuery(s.name || s.title || s.id || ''); setSiteResults([]); setSiteFocused(false); }}
                  style={{ width: '100%', textAlign: 'left', padding: '10px 12px', border: 'none', background: 'transparent', cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 12 }}
                >
                  <span style={{ fontSize: 13, color: 'var(--text)' }}>{s.name || s.title || s.id}</span>
                  <span style={{ fontSize: 11, color: 'var(--outline)' }}>{s.id}</span>
                </button>
              )) : (
                <div style={{ padding: '10px 12px', fontSize: 12, color: 'var(--text-2)' }}>No sites found.</div>
              )}
            </div>
          )}
        </div>

        {/* Type filter */}
        <select value={typeFilter} onChange={e => setTypeFilter(e.target.value)} style={selectStyle}>
          <option value="">All Types</option>
          {ARTIFACT_TYPES.map(t => <option key={t} value={t}>{formatEnum(t)}</option>)}
        </select>

        <button onClick={() => applyFilters(1)} style={{ padding: '9px 20px', borderRadius: 10, background: 'linear-gradient(135deg, var(--primary), var(--primary-container))', color: '#fff', border: 'none', fontSize: 13, fontWeight: 700, cursor: 'pointer', boxShadow: '0 2px 8px rgba(124,87,45,.25)', whiteSpace: 'nowrap' }}>
          Apply Filters
        </button>
      </div>

      {/* Selected site banner */}
      {selectedSiteId && (
        <div style={{ marginBottom: 18, background: 'var(--surface-container-lowest)', borderRadius: 12, padding: '10px 14px', display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 12, boxShadow: '0 1px 4px rgba(29,27,23,.05)', border: '1px solid rgba(124,87,45,.15)' }}>
          <div style={{ minWidth: 0 }}>
            <div style={{ fontSize: 10, fontWeight: 800, letterSpacing: '0.06em', textTransform: 'uppercase', color: 'var(--outline)', marginBottom: 2 }}>Filtered by site</div>
            <div style={{ fontSize: 13, color: 'var(--text)', fontWeight: 600, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
              {selectedSiteName || siteQuery || selectedSiteId}
            </div>
          </div>
          <button type="button" onClick={() => { setSelectedSiteId(''); setSiteQuery(''); setSiteResults([]); setSelectedSiteName(''); setSiteFocused(true); }}
            style={{ border: 'none', background: 'transparent', cursor: 'pointer', color: 'var(--text-2)', fontSize: 12, fontWeight: 600, padding: 0 }}>
            Clear
          </button>
        </div>
      )}

      {/* Card grid */}
      {loading ? (
        <div style={{ padding: 80 }}><Spinner center /></div>
      ) : artifacts.length === 0 ? (
        <div style={{ padding: '80px 20px', textAlign: 'center', color: 'var(--outline)', fontSize: 14 }}>
          No artifacts found.{' '}
          <button onClick={() => setCreateOpen(true)} style={{ color: 'var(--primary)', background: 'none', border: 'none', cursor: 'pointer', fontWeight: 600, fontSize: 14 }}>
            Create the first one.
          </button>
        </div>
      ) : (
        <>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: 20 }}>
            {artifacts.map(a => (
              <ArtifactCard key={a.id} artifact={a} onClick={() => navigate(`/artifacts/${a.id}`)} />
            ))}

            {/* Add new placeholder */}
            <button onClick={() => setCreateOpen(true)} style={{ border: '2px dashed rgba(212,196,183,.4)', borderRadius: 16, display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', minHeight: 320, background: 'transparent', cursor: 'pointer', transition: 'all .2s' }}
              onMouseOver={e => { e.currentTarget.style.borderColor = 'rgba(124,87,45,.4)'; e.currentTarget.style.background = 'rgba(124,87,45,.03)'; }}
              onMouseOut={e => { e.currentTarget.style.borderColor = 'rgba(212,196,183,.4)'; e.currentTarget.style.background = 'transparent'; }}
            >
              <div style={{ width: 56, height: 56, borderRadius: '50%', background: 'var(--surface-container-high)', display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: 12 }}>
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--outline)" strokeWidth="2"><path d="M12 5v14M5 12h14"/></svg>
              </div>
              <span style={{ fontWeight: 700, color: 'var(--text-2)', fontSize: 14 }}>New Artifact</span>
              <span style={{ fontSize: 11, color: 'var(--outline)', marginTop: 4 }}>Add to collection</span>
            </button>
          </div>

          {/* Pagination — only shown when no site is selected (server pagination) */}
          {!selectedSiteId && (
            <Pagination
              page={page}
              totalPages={totalPages}
              totalCount={totalCount}
              onPageChange={handlePageChange}
            />
          )}
        </>
      )}

      <Modal open={createOpen} onClose={() => setCreateOpen(false)} title="Create Artifact" width={680}>
        <ArtifactForm onSubmit={handleCreate} loading={creating} onCancel={() => setCreateOpen(false)} />
      </Modal>
    </div>
  );
}