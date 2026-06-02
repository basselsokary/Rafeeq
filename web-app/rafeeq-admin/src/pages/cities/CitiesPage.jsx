import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { getCities, getDashboardStats, createCity } from '../../api/citiesApi';
import Modal from '../../components/common/Modal';
import Spinner from '../../components/common/Spinner';
import CityForm from './components/CityForm';
import { useToast } from '../../components/common/Toast';

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

function CityCard({ city, index, onClick }) {
  return (
    <div
      onClick={onClick}
      style={{
        background: 'var(--surface-container-lowest)', borderRadius: 16,
        overflow: 'hidden', cursor: 'pointer',
        boxShadow: '0 4px 20px rgba(29,27,23,.06)',
        border: '1px solid rgba(255,255,255,.4)',
        display: 'flex', flexDirection: 'column',
        transition: 'box-shadow .25s, transform .25s',
      }}
      onMouseOver={e => {
        e.currentTarget.style.boxShadow = '0 12px 32px rgba(124,87,45,.12)';
        e.currentTarget.style.transform = 'translateY(-3px)';
      }}
      onMouseOut={e => {
        e.currentTarget.style.boxShadow = '0 4px 20px rgba(29,27,23,.06)';
        e.currentTarget.style.transform = 'translateY(0)';
      }}
    >
      {/* Image */}
      <div style={{ position: 'relative', height: 210, overflow: 'hidden', background: 'var(--surface-container)' }}>
        {city.imageUrl ? (
          <img
            src={city.imageUrl} alt={city.name || ''}
            style={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block', transition: 'transform .5s' }}
            onError={e => e.target.style.display = 'none'}
          />
        ) : (
          <div style={{ width: '100%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--outline)', fontSize: 48 }}>
            🏙️
          </div>
        )}
        <div style={{ position: 'absolute', inset: 0, background: 'linear-gradient(to top, rgba(0,0,0,.55), transparent)' }} />

        {/* Display order badge */}
        <div style={{
          position: 'absolute', top: 12, left: 12,
          background: 'rgba(255,255,255,.9)', backdropFilter: 'blur(4px)',
          padding: '3px 10px', borderRadius: 20,
          fontSize: 11, fontWeight: 900, color: 'var(--primary)',
          boxShadow: '0 2px 8px rgba(0,0,0,.1)',
          display: 'flex', alignItems: 'center', gap: 3,
        }}>
          <span style={{ fontSize: 9 }}>#</span>
          {city.displayOrder ?? index + 1}
        </div>

        {/* City name + coordinates overlay */}
        <div style={{ position: 'absolute', bottom: 14, left: 18 }}>
          {city.centerLocation && (
            <div style={{
              display: 'flex', alignItems: 'center', gap: 4,
              color: 'rgba(255,255,255,.75)', fontSize: 9, fontWeight: 700,
              letterSpacing: '0.1em', textTransform: 'uppercase', marginBottom: 4,
            }}>
              <svg width="9" height="9" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M21 10c0 7-9 13-9 13S3 17 3 10a9 9 0 0118 0z"/><circle cx="12" cy="10" r="3"/></svg>
              {city.centerLocation.latitude?.toFixed(4)}° N, {city.centerLocation.longitude?.toFixed(4)}° E
            </div>
          )}
          <h2 style={{
            fontFamily: 'var(--font-display)', fontSize: 22, fontWeight: 800,
            color: '#fff', letterSpacing: '-0.01em',
          }}>
            {city.name || '(unnamed)'}
          </h2>
        </div>

        {/* Pyramid accent */}
        <div style={{
          position: 'absolute', top: 0, right: 0,
          width: 0, height: 0, borderStyle: 'solid',
          borderWidth: '0 32px 32px 0',
          borderColor: 'transparent rgba(212,165,116,.35) transparent transparent',
        }} />
      </div>

      {/* Content */}
      <div style={{ padding: '18px 20px', flex: 1, display: 'flex', flexDirection: 'column', justifyContent: 'space-between' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 14 }}>
          <div>
            <div style={{ fontSize: 9, fontWeight: 700, letterSpacing: '0.1em', textTransform: 'uppercase', color: 'var(--outline)', marginBottom: 4 }}>
              Infrastructure
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: 5, fontWeight: 700, color: 'var(--text)', fontSize: 14 }}>
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="var(--primary)" strokeWidth="2"><path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2V9z"/></svg>
              {city.totalSites ?? 0} sites
            </div>
          </div>
          {city.localizedContents?.length > 0 && (
            <div style={{ textAlign: 'right' }}>
              <div style={{ fontSize: 9, fontWeight: 700, letterSpacing: '0.1em', textTransform: 'uppercase', color: 'var(--outline)', marginBottom: 4 }}>
                Available In
              </div>
              <div style={{ display: 'flex', gap: 3, justifyContent: 'flex-end' }}>
                {city.localizedContents.map(lc => (
                  <span key={lc.contentId || lc.language} style={{
                    width: 24, height: 16, borderRadius: 2,
                    background: 'var(--surface-container-high)',
                    display: 'flex', alignItems: 'center', justifyContent: 'center',
                    fontSize: 9, fontWeight: 700, color: 'var(--text-2)', textTransform: 'uppercase',
                  }}>
                    {(lc.language || '').slice(0, 2)}
                  </span>
                ))}
              </div>
            </div>
          )}
        </div>

        {/* Action row */}
        <div style={{
          display: 'flex', gap: 10, paddingTop: 14,
          borderTop: '1px solid rgba(212,196,183,.2)', marginTop: 'auto',
        }}>
          <button
            onClick={e => { e.stopPropagation(); onClick(); }}
            style={{
              flex: 1, padding: '10px 0', borderRadius: 10,
              background: 'var(--surface-container-high)', border: 'none',
              fontWeight: 700, fontSize: 13, color: 'var(--text)',
              cursor: 'pointer', transition: 'background .15s',
            }}
            onMouseOver={e => e.currentTarget.style.background = 'var(--surface-container-highest)'}
            onMouseOut={e => e.currentTarget.style.background = 'var(--surface-container-high)'}
          >
            View Details
          </button>
          <button
            onClick={e => { e.stopPropagation(); onClick(); }}
            style={{
              padding: '10px 14px', borderRadius: 10,
              background: 'var(--surface-container-high)', border: 'none',
              cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center',
              transition: 'background .15s',
            }}
            onMouseOver={e => e.currentTarget.style.background = 'var(--surface-container-highest)'}
            onMouseOut={e => e.currentTarget.style.background = 'var(--surface-container-high)'}
          >
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="var(--text)" strokeWidth="2">
              <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/>
              <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4z"/>
            </svg>
          </button>
        </div>
      </div>
    </div>
  );
}

export default function CitiesPage() {
  const navigate = useNavigate();
  const toast    = useToast();

  const [cities,       setCities]       = useState([]);
  const [loading,      setLoading]      = useState(false);
  const [createOpen,   setCreateOpen]   = useState(false);
  const [creating,     setCreating]     = useState(false);
  const [topSearch,    setTopSearch]    = useState('');
  const [statsLoading, setStatsLoading] = useState(true);
  const [stats, setStats] = useState({ totalCities: 0, totalSites: 0 });

  const loadCities = useCallback(async () => {
    try {
      setLoading(true);
      const res = await getCities();
      const d = res.data;
      const items = Array.isArray(d) ? d : d?.value ?? d?.data ?? d?.items ?? [];
      setCities(items);
    } catch { setCities([]); }
    finally { setLoading(false); }
  }, []);

  const loadStats = useCallback(async () => {
    try {
      setStatsLoading(true);
      const res = await getDashboardStats();
      const d = res.data?.value ?? res.data?.data ?? res.data;
      setStats({
        totalCities: Number(d?.totalCities ?? 0),
        totalSites:  Number(d?.totalSites  ?? 0),
      });
    } catch { /* keep defaults */ }
    finally { setStatsLoading(false); }
  }, []);

  useEffect(() => { loadCities(); loadStats(); }, [loadCities, loadStats]);

  const handleCreate = async (formData) => {
    setCreating(true);
    try {
      const res = await createCity(formData);
      toast('City created', 'success');
      setCreateOpen(false);
      loadStats();
      const newId = res.data?.value?.id ?? res.data?.value ?? res.data?.id ?? res.data?.data?.id ?? res.data?.data ?? res.data;
      newId ? navigate(`/cities/${newId}`) : loadCities();
    } catch (e) {
      console.error('Create city failed:', e.response?.data || e);
      toast('Failed to create city', 'error');
    } finally { setCreating(false); }
  };

  const filtered = cities.filter(c => {
    if (!topSearch.trim()) return true;
    const term = topSearch.toLowerCase();
    return (c.name || '').toLowerCase().includes(term)
      || (c.description || '').toLowerCase().includes(term);
  });

  const sorted = [...filtered].sort((a, b) => (a.displayOrder ?? 999) - (b.displayOrder ?? 999));

  return (
    <div style={{ padding: '28px 32px 80px', fontFamily: 'var(--font-body)' }}>
      {/* Breadcrumb */}
      <div style={{ fontSize: 12, color: 'var(--outline)', marginBottom: 10, display: 'flex', alignItems: 'center', gap: 6 }}>
        <span style={{ cursor: 'pointer', color: 'var(--text-2)' }} onClick={() => navigate('/')}>Dashboard</span>
        <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
        <span>Cities</span>
      </div>

      {/* Header */}
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 24, flexWrap: 'wrap', gap: 12 }}>
        <h1 style={{ fontFamily: 'var(--font-display)', fontSize: 28, fontWeight: 700, color: 'var(--text)', display: 'flex', alignItems: 'baseline', gap: 10, margin: 0 }}>
          Cities Management
          <span style={{
            padding: '3px 12px', borderRadius: 20,
            background: 'rgba(124,87,45,.08)', color: 'var(--primary)',
            fontSize: 12, fontWeight: 700,
          }}>
            {cities.length} Cities
          </span>
        </h1>
        <div style={{ display: 'flex', gap: 10 }}>
          <button onClick={() => setCreateOpen(true)} style={{
            display: 'flex', alignItems: 'center', gap: 7, padding: '9px 18px', borderRadius: 10,
            background: 'linear-gradient(135deg, var(--primary), var(--primary-container))',
            color: '#fff', border: 'none', fontSize: 13, fontWeight: 700, cursor: 'pointer',
            boxShadow: '0 2px 10px rgba(124,87,45,0.25)',
          }}>
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M12 5v14M5 12h14"/></svg>
            Add New City
          </button>
        </div>
      </div>

      {/* Stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2,1fr)', gap: 16, marginBottom: 28 }}>
        <StatCard
          label="Total Cities" value={stats.totalCities} loading={statsLoading}
          sub="Across Egypt"
          icon={<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="var(--primary)" strokeWidth="2"><path d="M21 10c0 7-9 13-9 13S3 17 3 10a9 9 0 0118 0z"/><circle cx="12" cy="10" r="3"/></svg>}
        />
        <StatCard
          label="Total Sites" value={stats.totalSites} loading={statsLoading}
          sub="Tourist destinations"
          icon={<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="var(--primary)" strokeWidth="2"><path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2V9z"/></svg>}
        />
      </div>

      {/* Search */}
      <div style={{ display: 'flex', gap: 10, marginBottom: 24 }}>
        <div style={{ flex: 1, maxWidth: 400, position: 'relative' }}>
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="var(--outline)" strokeWidth="2"
            style={{ position: 'absolute', left: 11, top: '50%', transform: 'translateY(-50%)', pointerEvents: 'none' }}>
            <circle cx="11" cy="11" r="8"/><path d="M21 21l-4.35-4.35"/>
          </svg>
          <input
            value={topSearch}
            onChange={e => setTopSearch(e.target.value)}
            placeholder="Search by name or GPS..."
            style={{
              width: '100%', padding: '9px 14px 9px 34px',
              background: 'var(--surface-container-lowest)',
              border: '1px solid rgba(212,196,183,.3)',
              borderRadius: 10, fontSize: 13, color: 'var(--text)', outline: 'none',
            }}
          />
        </div>
      </div>

      {/* Card Grid */}
      {loading ? (
        <div style={{ padding: 80 }}><Spinner center /></div>
      ) : sorted.length === 0 ? (
        <div style={{ padding: '80px 20px', textAlign: 'center', color: 'var(--outline)', fontSize: 14 }}>
          No cities found.{' '}
          <button onClick={() => setCreateOpen(true)} style={{ color: 'var(--primary)', background: 'none', border: 'none', cursor: 'pointer', fontWeight: 600, fontSize: 14 }}>
            Create the first one.
          </button>
        </div>
      ) : (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))', gap: 24 }}>
          {sorted.map((city, i) => (
            <CityCard
              key={city.id}
              city={city}
              index={i}
              onClick={() => navigate(`/cities/${city.id}`)}
            />
          ))}
          {/* Add new card placeholder */}
          <button
            onClick={() => setCreateOpen(true)}
            style={{
              border: '2px dashed rgba(212,196,183,.4)', borderRadius: 16,
              display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center',
              minHeight: 380, background: 'transparent', cursor: 'pointer',
              transition: 'all .2s',
            }}
            onMouseOver={e => { e.currentTarget.style.borderColor = 'rgba(124,87,45,.4)'; e.currentTarget.style.background = 'rgba(124,87,45,.03)'; }}
            onMouseOut={e => { e.currentTarget.style.borderColor = 'rgba(212,196,183,.4)'; e.currentTarget.style.background = 'transparent'; }}
          >
            <div style={{
              width: 56, height: 56, borderRadius: '50%',
              background: 'var(--surface-container-high)',
              display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: 12,
            }}>
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--outline)" strokeWidth="2"><path d="M12 5v14M5 12h14"/></svg>
            </div>
            <span style={{ fontWeight: 700, color: 'var(--text-2)', fontSize: 14 }}>New City</span>
            <span style={{ fontSize: 11, color: 'var(--outline)', marginTop: 4 }}>Add to Egypt's network</span>
          </button>
        </div>
      )}

      <Modal open={createOpen} onClose={() => setCreateOpen(false)} title="Create New City" width={640}>
        <CityForm onSubmit={handleCreate} loading={creating} onCancel={() => setCreateOpen(false)} />
      </Modal>
    </div>
  );
}
