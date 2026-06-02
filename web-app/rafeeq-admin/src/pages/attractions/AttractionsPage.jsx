import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { getAttractionsBySiteId, createAttraction, getDashboardStats } from '../../api/attractionsApi';
import { searchSites } from '../../api/sitesApi';
import Modal from '../../components/common/Modal';
import Spinner from '../../components/common/Spinner';
import AttractionForm from './components/AttractionForm';
import { useToast } from '../../components/common/Toast';
import { formatEnum, ATTRACTION_TYPES, HISTORICAL_PERIODS } from '../../utils/constants';

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

function AttractionCard({ attraction, onClick }) {
  const mainImage = attraction.primaryImageUrl
    || attraction.images?.find(i => i.isMain)?.url
    || attraction.images?.[0]?.url;

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
      onMouseOver={e => {
        e.currentTarget.style.boxShadow = '0 8px 24px rgba(124,87,45,.1)';
        e.currentTarget.style.transform = 'translateY(-2px)';
      }}
      onMouseOut={e => {
        e.currentTarget.style.boxShadow = '0 1px 4px rgba(29,27,23,.06)';
        e.currentTarget.style.transform = 'translateY(0)';
      }}
    >
      {/* Image */}
      <div style={{ position: 'relative', height: 200, overflow: 'hidden', background: 'var(--surface-container)' }}>
        {mainImage ? (
          <img
            src={mainImage} alt={attraction.name || ''}
            style={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block', transition: 'transform .4s' }}
            onError={e => e.target.style.display = 'none'}
          />
        ) : (
          <div style={{ width: '100%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--outline)', fontSize: 40 }}>
            🏛️
          </div>
        )}
        {attraction.isFeatured && (
          <span style={{
            position: 'absolute', top: 12, left: 12,
            background: 'rgba(212,165,116,.9)', backdropFilter: 'blur(4px)',
            color: '#5b3a13', fontSize: 9, fontWeight: 800, padding: '3px 8px',
            borderRadius: 6, display: 'flex', alignItems: 'center', gap: 3,
            letterSpacing: '0.06em', textTransform: 'uppercase',
          }}>
            <svg width="10" height="10" viewBox="0 0 24 24" fill="currentColor"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>
            Featured
          </span>
        )}
      </div>

      {/* Content */}
      <div style={{ padding: '16px 18px 18px' }}>
        <div style={{ display: 'flex', gap: 6, marginBottom: 10, flexWrap: 'wrap' }}>
          <TypeBadge type={attraction.type} />
        </div>

        <h3 style={{
          fontFamily: 'var(--font-display)', fontSize: 16, fontWeight: 700,
          color: 'var(--text)', marginBottom: 6, lineHeight: 1.3,
        }}>
          {attraction.name || '(unnamed)'}
        </h3>

        {attraction.description && (
          <p style={{
            fontSize: 12, color: 'var(--text-2)', lineHeight: 1.6, marginBottom: 14,
            display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden',
          }}>
            {attraction.description}
          </p>
        )}

        <div style={{
          display: 'flex', alignItems: 'center', justifyContent: 'space-between',
          paddingTop: 12, borderTop: '1px solid rgba(212,196,183,.15)',
        }}>
          {attraction.location && (
            <div style={{ display: 'flex', alignItems: 'center', gap: 4, fontSize: 11, color: 'var(--outline)' }}>
              <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M21 10c0 7-9 13-9 13S3 17 3 10a9 9 0 0118 0z"/><circle cx="12" cy="10" r="3"/></svg>
              {attraction.location.latitude?.toFixed(2)}°, {attraction.location.longitude?.toFixed(2)}°
            </div>
          )}
          <button
            onClick={e => { e.stopPropagation(); onClick(); }}
            style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'var(--outline)', display: 'flex', padding: 4 }}
          >
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
          </button>
        </div>
      </div>

      <div className="pyramid-accent" />
    </article>
  );
}

export default function AttractionsPage() {
  const navigate = useNavigate();
  const toast    = useToast();

  const [attractions, setAttractions] = useState([]);
  const [totalCount,  setTotalCount]  = useState(0);
  const [loading,     setLoading]     = useState(false);
  const [createOpen,  setCreateOpen]  = useState(false);
  const [creating,    setCreating]    = useState(false);
  const [topSearch,   setTopSearch]   = useState('');
  const [siteQuery,   setSiteQuery]   = useState('');
  const [siteResults, setSiteResults] = useState([]);
  const [siteSearching, setSiteSearching] = useState(false);
  const [siteTouched, setSiteTouched] = useState(false);
  const [selectedSiteId, setSelectedSiteId] = useState('');
  const [typeFilter,  setTypeFilter]  = useState('');
  const [statsLoading, setStatsLoading] = useState(true);
  const [stats, setStats] = useState({ totalAttractions: 0, featuredAttractions: 0 });
  const [siteFocused, setSiteFocused] = useState(false);
  const [selectedSiteName, setSelectedSiteName] = useState('');

  const buildParams = () => {
    const p = {};
    if (topSearch?.trim()) p.searchTerm = topSearch.trim();
    if (typeFilter) p.type = typeFilter;
    return p;
  };

  const loadAttractions = useCallback(async (siteId, params) => {
    if (!siteId) {
      setAttractions([]);
      setTotalCount(0);
      return;
    }
    try {
      setLoading(true);
      const p = params || {};
      const res = await getAttractionsBySiteId(siteId, p);
      const d = res.data;
      const items = d?.data ?? d?.value?.data ?? d?.items ?? (Array.isArray(d) ? d : []);
      setAttractions(items);
      setTotalCount(items.length);
    } catch { setAttractions([]); setTotalCount(0); }
    finally { setLoading(false); }
  }, []);

  const loadStats = useCallback(async () => {
    try {
      setStatsLoading(true);
      const res = await getDashboardStats();
      const d = res.data?.value ?? res.data?.data ?? res.data;
      setStats({
        totalAttractions:    Number(d?.totalAttractions    ?? 0),
        featuredAttractions: Number(d?.featuredAttractions  ?? 0),
      });
    } catch { /* keep defaults */ }
    finally { setStatsLoading(false); }
  }, []);

  useEffect(() => {
    if (!selectedSiteId) {
      setAttractions([]);
      setTotalCount(0);
      return;
    }
    loadAttractions(selectedSiteId, buildParams());
  }, [selectedSiteId]);
  useEffect(() => { loadStats(); }, []);

  useEffect(() => {
    if (selectedSiteId && !siteFocused) return;
    const q = siteQuery.trim();
    if (!q) {
      setSiteResults([]);
      setSiteSearching(false);
      return;
    }

    let cancelled = false;
    const handle = setTimeout(async () => {
      setSiteSearching(true);
      try {
        const res = await searchSites({ q });
        const d = res?.data;
        const items = Array.isArray(d)
          ? d
          : (d?.data ?? d?.value ?? d?.items ?? d?.results ?? []);
        const list = Array.isArray(items) ? items : [];
        if (!cancelled) setSiteResults(list.slice(0, 10));
      } catch {
        if (!cancelled) setSiteResults([]);
      } finally {
        if (!cancelled) setSiteSearching(false);
      }
    }, 700);

    return () => {
      cancelled = true;
      clearTimeout(handle);
    };
  }, [siteQuery]);

  const applyFilters = () => {
    if (!selectedSiteId) {
      toast('Select a site first', 'error');
      return;
    }
    loadAttractions(selectedSiteId, buildParams());
  };

  const clearFilters = () => {
    setTopSearch(''); setTypeFilter('');
    setSiteQuery(''); setSiteResults([]); setSiteTouched(false); setSelectedSiteId('');
    setSelectedSiteName('');
    setSiteFocused(false);
    setAttractions([]);
    setTotalCount(0);
  };

  const handleCreate = async (payload) => {
    setCreating(true);
    try {
      const res = await createAttraction(payload);
      toast('Attraction created', 'success');
      setCreateOpen(false);
      loadStats();
      const newId = res.data?.value?.id ?? res.data?.value ?? res.data?.id ?? res.data?.data?.id ?? res.data?.data ?? res.data;
      newId ? navigate(`/attractions/${newId}`) : loadAttractions(selectedSiteId, buildParams());
    } catch (e) {
      console.error('Create attraction failed:', e.response?.data || e);
      toast('Failed to create attraction', 'error');
    } finally { setCreating(false); }
  };

  const featuredCount = stats.featuredAttractions;

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
        <span>Attractions Management</span>
      </div>

          {/* Header */}
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 24, flexWrap: 'wrap', gap: 12 }}>
            <h1 style={{ fontFamily: 'var(--font-display)', fontSize: 28, fontWeight: 700, color: 'var(--text)', display: 'flex', alignItems: 'baseline', gap: 10, margin: 0 }}>
              Attractions Management
              <span style={{ fontSize: 18, fontWeight: 400, color: 'var(--outline)' }}>• {totalCount} Total</span>
            </h1>
            <div style={{ display: 'flex', gap: 10 }}>
              <button onClick={() => setCreateOpen(true)} style={{
                display: 'flex', alignItems: 'center', gap: 7, padding: '9px 18px', borderRadius: 10,
                background: 'linear-gradient(135deg, var(--primary), var(--primary-container))',
                color: '#fff', border: 'none', fontSize: 13, fontWeight: 700, cursor: 'pointer',
                boxShadow: '0 2px 10px rgba(124,87,45,0.25)',
              }}>
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M12 5v14M5 12h14"/></svg>
                Add Attraction
              </button>
            </div>
          </div>

          {/* Stats */}
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2,1fr)', gap: 16, marginBottom: 28 }}>
            <StatCard
              label="Total Attractions" value={stats.totalAttractions} loading={statsLoading}
              sub={<span>{featuredCount} featured</span>}
              icon={<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="var(--primary)" strokeWidth="2"><polyline points="22 7 13.5 15.5 8.5 10.5 2 17"/><polyline points="16 7 22 7 22 13"/></svg>}
            />
            <StatCard
              label="Featured Attractions" value={featuredCount} loading={statsLoading}
              sub="Highlighted attractions"
              icon={<svg width="16" height="16" viewBox="0 0 24 24" fill="var(--primary)" stroke="none"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>}
            />
          </div>

          {/* Clear filters */}
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 14 }}>
            {(topSearch || typeFilter || selectedSiteId) && (
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
                placeholder="Search attractions..."
                style={{
                  width: '100%', padding: '9px 14px 9px 34px',
                  background: 'var(--surface-container-lowest)',
                  border: '1px solid rgba(212,196,183,.3)',
                  borderRadius: 10, fontSize: 13, color: 'var(--text)', outline: 'none',
                }}
              />
            </div>

            <div style={{ flex: 2, minWidth: 220, position: 'relative' }}>
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="var(--outline)" strokeWidth="2"
                style={{ position: 'absolute', left: 11, top: '50%', transform: 'translateY(-50%)', pointerEvents: 'none' }}>
                <circle cx="11" cy="11" r="8"/><path d="M21 21l-4.35-4.35"/>
              </svg>
              <input
                value={siteQuery}
                onChange={e => {
                  setSiteQuery(e.target.value);
                  setSiteTouched(true);
                  setSiteFocused(true);
                  if (selectedSiteId) setSelectedSiteId('');
                }}
                onKeyDown={e => e.key === 'Enter' && applyFilters()}
                onFocus={() => setSiteFocused(true)}
                onBlur={() => setTimeout(() => setSiteFocused(false), 0)}
                placeholder="Search site name..."
                style={{
                  width: '100%', padding: '9px 14px 9px 34px',
                  background: 'var(--surface-container-lowest)',
                  border: '1px solid rgba(212,196,183,.3)',
                  borderRadius: 10, fontSize: 13, color: 'var(--text)', outline: 'none',
                }}
              />

              {siteFocused && (siteQuery.trim() || siteTouched) && (
                <div style={{
                  position: 'absolute',
                  top: 'calc(100% + 6px)',
                  left: 0,
                  right: 0,
                  zIndex: 10,
                  background: 'var(--surface-container-lowest)',
                  border: '1px solid rgba(212,196,183,.35)',
                  borderRadius: 12,
                  overflow: 'hidden',
                  boxShadow: '0 10px 25px rgba(29,27,23,.08)',
                }}>
                  {siteSearching ? (
                    <div style={{ padding: '10px 12px', fontSize: 12, color: 'var(--text-2)' }}>
                      Searching…
                    </div>
                  ) : siteResults.length ? (
                    siteResults.map((s) => (
                      <button
                        key={s.id}
                        type="button"
                        onMouseDown={(e) => e.preventDefault()}
                        onClick={() => {
                          setSelectedSiteId(s.id ?? '');
                          setSelectedSiteName(s.name || s.title || s.id || '');
                          setSiteQuery(s.name || s.title || s.id || '');
                          setSiteResults([]);
                          setSiteFocused(false);
                        }}
                        style={{
                          width: '100%',
                          textAlign: 'left',
                          padding: '10px 12px',
                          border: 'none',
                          background: 'transparent',
                          cursor: 'pointer',
                          display: 'flex',
                          alignItems: 'center',
                          justifyContent: 'space-between',
                          gap: 12,
                        }}
                      >
                        <span style={{ fontSize: 13, color: 'var(--text)' }}>
                          {s.name || s.title || s.id}
                        </span>
                        <span style={{ fontSize: 11, color: 'var(--outline)' }}>
                          {s.id}
                        </span>
                      </button>
                    ))
                  ) : (
                    <div style={{ padding: '10px 12px', fontSize: 12, color: 'var(--text-2)' }}>
                      No sites found.
                    </div>
                  )}
                </div>
              )}
            </div>

            <select value={typeFilter} onChange={e => setTypeFilter(e.target.value)} style={selectStyle}>
              <option value="">All Types</option>
              {ATTRACTION_TYPES.map(t => <option key={t} value={t}>{formatEnum(t)}</option>)}
            </select>

            <button onClick={applyFilters} style={{
              padding: '9px 20px', borderRadius: 10,
              background: 'linear-gradient(135deg, var(--primary), var(--primary-container))',
              color: '#fff', border: 'none', fontSize: 13, fontWeight: 700, cursor: 'pointer',
              boxShadow: '0 2px 8px rgba(124,87,45,.25)', whiteSpace: 'nowrap',
            }}>Apply Filters</button>
          </div>

          {selectedSiteId && (
            <div style={{
              marginBottom: 18,
              background: 'var(--surface-container-lowest)',
              borderRadius: 12,
              padding: '10px 14px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'space-between',
              gap: 12,
              boxShadow: '0 1px 4px rgba(29,27,23,.05)',
            }}>
              <div style={{ minWidth: 0 }}>
                <div style={{ fontSize: 11, fontWeight: 800, letterSpacing: '0.06em', textTransform: 'uppercase', color: 'var(--outline)' }}>
                  Selected site
                </div>
                <div style={{ fontSize: 13, color: 'var(--text)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                  {selectedSiteName || siteQuery || selectedSiteId}
                </div>
              </div>
              <button
                type="button"
                onClick={() => {
                  setSelectedSiteId('');
                  setSiteQuery('');
                  setSiteResults([]);
                  setSelectedSiteName('');
                  setSiteFocused(true);
                }}
                style={{
                  border: 'none',
                  background: 'transparent',
                  cursor: 'pointer',
                  color: 'var(--text-2)',
                  fontSize: 12,
                  fontWeight: 600,
                  padding: 0,
                }}
              >
                Clear
              </button>
            </div>
          )}

          {/* Card Grid */}
          {!selectedSiteId ? (
            <div style={{ padding: '80px 20px', textAlign: 'center', color: 'var(--outline)', fontSize: 14 }}>
              Select a site to view its attractions.
            </div>
          ) : loading ? (
            <div style={{ padding: 80 }}><Spinner center /></div>
          ) : attractions.length === 0 ? (
            <div style={{ padding: '80px 20px', textAlign: 'center', color: 'var(--outline)', fontSize: 14 }}>
              No attractions found.{' '}
              <button onClick={() => setCreateOpen(true)} style={{ color: 'var(--primary)', background: 'none', border: 'none', cursor: 'pointer', fontWeight: 600, fontSize: 14 }}>
                Create the first one.
              </button>
            </div>
          ) : (
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: 20 }}>
              {attractions.map(a => (
                <AttractionCard
                  key={a.id}
                  attraction={a}
                  onClick={() => navigate(`/attractions/${a.id}`)}
                />
              ))}
              {/* Add new card placeholder */}
              <button
                onClick={() => setCreateOpen(true)}
                style={{
                  border: '2px dashed rgba(212,196,183,.4)', borderRadius: 16,
                  display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center',
                  minHeight: 320, background: 'transparent', cursor: 'pointer',
                  transition: 'all .2s',
                }}
                onMouseOver={e => { e.currentTarget.style.borderColor = 'rgba(124,87,45,.4)'; e.currentTarget.style.background = 'rgba(124,87,45,.03)'; }}
                onMouseOut={e => { e.currentTarget.style.borderColor = 'rgba(212,196,183,.4)'; e.currentTarget.style.background = 'transparent'; }}
              >
                <div style={{
                  width: 56, height: 56, borderRadius: '50%',
                  background: 'var(--surface-container-high)',
                  display: 'flex', alignItems: 'center', justifyContent: 'center',
                  marginBottom: 12,
                }}>
                  <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--outline)" strokeWidth="2"><path d="M12 5v14M5 12h14"/></svg>
                </div>
                <span style={{ fontWeight: 700, color: 'var(--text-2)', fontSize: 14 }}>New Attraction</span>
                <span style={{ fontSize: 11, color: 'var(--outline)', marginTop: 4 }}>Add to collection</span>
              </button>
            </div>
          )}

      <Modal open={createOpen} onClose={() => setCreateOpen(false)} title="Create Attraction" width={680}>
        <AttractionForm onSubmit={handleCreate} loading={creating} onCancel={() => setCreateOpen(false)} />
      </Modal>
    </div>
  );
}
