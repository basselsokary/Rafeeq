import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { getDashboardStats, getSites } from '../../api/sitesApi';
import Sidebar from '../../components/layout/Sidebar';
import Spinner from '../../components/common/Spinner';
import { useToast } from '../../components/common/Toast';
import EgyptTourismMap from '../../components/map/EgyptTourismMap';

import { MapContainer, TileLayer, Marker, Tooltip, GeoJSON, Polygon, useMap } from 'react-leaflet';
import L from 'leaflet';
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png';
import markerIcon from 'leaflet/dist/images/marker-icon.png';
import markerShadow from 'leaflet/dist/images/marker-shadow.png';

import egyptGeoJson from '../../assets/egypt.json';
import { getCities } from '../../api/citiesApi';

// Fix default marker icons for Vite builds
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: markerIcon2x,
  iconUrl: markerIcon,
  shadowUrl: markerShadow,
});

const EGYPT_POSITION = [26.8206, 30.8025];

const normName = (v) => (v || '').toString().trim().toLowerCase();

function FitToBounds({ bounds }) {
  const map = useMap();

  useEffect(() => {
    if (!bounds) return;
    map.fitBounds(bounds, { padding: [20, 20] });
  }, [map, bounds]);

  return null;
}

/* ══ Small reusable atoms ══ */
function KpiCard({ label, value, sub, tag, icon, accent, loading, barPercent }) {
  return (
    <div style={{
      background: 'var(--surface-container-lowest)',
      borderRadius: 16, padding: '22px 22px 18px',
      boxShadow: '0 2px 12px rgba(29,27,23,.06)',
      borderTop: `4px solid ${accent}`,
      position: 'relative', overflow: 'hidden',
      transition: 'box-shadow .2s',
      flex: 1, minWidth: 0,
    }}>
      <div className="pyramid-accent" />
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 14 }}>
        <div style={{
          width: 36, height: 36, borderRadius: 10,
          background: `${accent}14`,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          color: accent, flexShrink: 0,
        }}>{icon}</div>
        {tag && (
          <span style={{
            fontSize: 9, fontWeight: 800, letterSpacing: '0.08em', textTransform: 'uppercase',
            padding: '3px 9px', borderRadius: 20,
            background: `${accent}14`, color: accent,
          }}>{tag}</span>
        )}
      </div>
      <div style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.09em', textTransform: 'uppercase', color: 'var(--outline)', marginBottom: 4 }}>
        {label}
      </div>
      <div style={{ fontFamily: 'var(--font-display)', fontSize: 34, fontWeight: 700, color: 'var(--text)', lineHeight: 1.1 }}>
        {loading ? '…' : typeof value === 'number' ? value.toLocaleString() : value}
      </div>
      {sub && <div style={{ fontSize: 11, color: 'var(--outline)', marginTop: 5 }}>{sub}</div>}
      {barPercent != null && (
        <div style={{ marginTop: 14, height: 3, background: 'var(--surface-container-high)', borderRadius: 2, overflow: 'hidden' }}>
          <div style={{ height: '100%', width: `${barPercent}%`, background: accent, borderRadius: 2, transition: 'width 1s ease' }} />
        </div>
      )}
    </div>
  );
}

/* ══ Top rated list ══ */
function TopRatedList({ sites, loading }) {
  const top = [...sites]
    .filter(s => s.averageRating > 0)
    .sort((a, b) => b.averageRating - a.averageRating)
    .slice(0, 5);

  const max = top[0]?.averageRating || 5;

  return (
    <div style={{
      background: 'var(--surface-container-lowest)',
      borderRadius: 16, padding: '22px 22px',
      boxShadow: '0 2px 12px rgba(29,27,23,.06)',
    }}>
      <h3 style={{ fontSize: 14, fontWeight: 700, color: 'var(--text)', marginBottom: 20 }}>
        Top 5 Rated Sites
      </h3>
      {loading ? <Spinner center /> : top.length === 0 ? (
        <div style={{ color: 'var(--outline)', fontSize: 13, textAlign: 'center', padding: '20px 0' }}>No rated sites yet.</div>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
          {top.map((s, i) => (
            <div key={s.id}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'baseline', marginBottom: 5 }}>
                <span style={{ fontSize: 12, fontWeight: 600, color: 'var(--text)', maxWidth: '75%', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                  {i + 1}. {s.name}
                </span>
                <span style={{ fontSize: 12, fontWeight: 800, color: 'var(--primary)', flexShrink: 0 }}>
                  {s.averageRating.toFixed(1)}/5
                </span>
              </div>
              <div style={{ height: 5, background: 'var(--surface-container-high)', borderRadius: 3, overflow: 'hidden' }}>
                <div style={{
                  height: '100%',
                  width: `${(s.averageRating / max) * 100}%`,
                  background: i === 0
                    ? 'linear-gradient(90deg, var(--primary), var(--primary-container))'
                    : 'var(--primary-fixed-dim)',
                  borderRadius: 3,
                  transition: 'width 1s ease',
                }} />
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

/* ══════════════════════════════════════════════════════
  EGYPT MAP COMPONENT (Leaflet)
  Shows Egypt-only view with city markers.
  Tooltip on hover shows total sites.
═══════════════════════════════════════════════════════ */
function EgyptMap({ sites, cities, loading }) {
  const siteCountsByCityName = useMemo(() => {
    const map = {};
    for (const s of sites) {
      const k = normName(s.cityName);
      if (!k) continue;
      map[k] = (map[k] || 0) + 1;
    }
    return map;
  }, [sites]);

  const cityMarkers = useMemo(() => {
    const list = Array.isArray(cities) ? cities : [];
    return list
      .map(c => {
        const lat = Number(c?.centerLocation?.latitude);
        const lng = Number(c?.centerLocation?.longitude);
        if (!Number.isFinite(lat) || !Number.isFinite(lng)) return null;

        const name = c?.name || 'Unknown';
        const count = Number(c?.totalSites ?? siteCountsByCityName[normName(name)] ?? 0);
        return {
          id: c?.id ?? name,
          name,
          lat,
          lng,
          count,
        };
      })
      .filter(Boolean);
  }, [cities, siteCountsByCityName]);

  const egyptBounds = useMemo(() => {
    try {
      return L.geoJSON(egyptGeoJson).getBounds();
    } catch {
      return null;
    }
  }, []);

  const maxBounds = useMemo(() => {
    if (!egyptBounds) return null;
    return egyptBounds.pad(0.08);
  }, [egyptBounds]);

  const egyptHoles = useMemo(() => {
    const holes = [];
    const features = egyptGeoJson?.features || [];

    const pushOuterRing = (ring) => {
      if (!Array.isArray(ring) || ring.length < 3) return;
      holes.push(ring.map(([lng, lat]) => [lat, lng]));
    };

    for (const f of features) {
      const g = f?.geometry;
      if (!g) continue;
      if (g.type === 'Polygon') {
        pushOuterRing(g.coordinates?.[0]);
      } else if (g.type === 'MultiPolygon') {
        for (const poly of g.coordinates || []) {
          pushOuterRing(poly?.[0]);
        }
      }
    }

    return holes;
  }, []);

  return (
    <div style={{
      background: 'var(--surface-container-low)',
      borderRadius: 20, overflow: 'hidden',
      position: 'relative', minHeight: 460,
      boxShadow: 'inset 0 2px 8px rgba(29,27,23,.08)',
      border: '1px solid rgba(212,196,183,.2)',
    }}>
      <div style={{
        position: 'absolute', top: 0, left: 0, right: 0, zIndex: 10,
        padding: '16px 20px',
        background: 'rgba(255,248,240,0.9)', backdropFilter: 'blur(8px)',
        borderBottom: '1px solid rgba(212,196,183,.2)',
        display: 'flex', alignItems: 'center', justifyContent: 'space-between',
      }}>
        <div>
          <div style={{ fontSize: 14, fontWeight: 700, color: 'var(--text)' }}>Sites Across Egypt</div>
          <div style={{ fontSize: 11, color: 'var(--outline)', marginTop: 1 }}>
            {loading ? 'Loading…' : `${sites.length} sites across ${cityMarkers.length} cities`}
          </div>
        </div>
      </div>

      <div style={{ paddingTop: 60, height: '100%', position: 'relative' }}>
        <MapContainer
          center={EGYPT_POSITION}
          zoom={6}
          zoomControl={false}
          maxBounds={maxBounds || undefined}
          maxBoundsViscosity={1.0}
          minZoom={6}
          maxZoom={9}
          worldCopyJump={false}
          style={{ height: 400, width: '100%', backgroundColor: 'transparent' }}
        >
          {egyptBounds ? <FitToBounds bounds={egyptBounds} /> : null}
          <TileLayer
            url="https://mt1.google.com/vt/lyrs=m&x={x}&y={y}&z={z}"
          />

          {/* Mask everything outside Egypt */}
          <Polygon
            positions={[
              [
                [90, -180],
                [90, 180],
                [-90, 180],
                [-90, -180],
              ],
              ...egyptHoles,
            ]}
            pathOptions={{
              color: 'transparent',
              weight: 0,
              fillColor: 'var(--background)',
              fillOpacity: 1,
            }}
            interactive={false}
          />

          {/* Egypt borders */}
          <GeoJSON
            data={egyptGeoJson}
            style={() => ({
              color: 'var(--primary)',
              weight: 1.25,
              fillColor: 'transparent',
              fillOpacity: 0,
            })}
            interactive={false}
          />

          {/* City markers */}
          {cityMarkers.map((city) => (
            <Marker
              key={city.id}
              position={[city.lat, city.lng]}
            >
              <Tooltip direction="top" offset={[0, -10]} opacity={1} sticky>
                <div style={{ fontSize: 12, fontWeight: 800, color: 'var(--text)' }}>{city.name}</div>
                <div style={{ fontSize: 11, color: 'var(--text-2)' }}>
                  {city.count} site{city.count !== 1 ? 's' : ''}
                </div>
              </Tooltip>
            </Marker>
          ))}
        </MapContainer>

        {loading && (
          <div style={{
            position: 'absolute', inset: 0, display: 'flex', alignItems: 'center', justifyContent: 'center',
            background: 'rgba(255,248,240,.7)', backdropFilter: 'blur(4px)', zIndex: 20,
          }}>
            <Spinner size={32} />
          </div>
        )}
      </div>
    </div>
  );
}

/* ══════════════════════════════════════════════════════
   MAIN DASHBOARD PAGE
═══════════════════════════════════════════════════════ */
export default function DashboardPage() {
  const navigate = useNavigate();
  const toast    = useToast();

  const [stats,        setStats]        = useState(null);
  const [sites,        setSites]        = useState([]);
  const [cities,       setCities]       = useState([]);
  const [statsLoading, setStatsLoading] = useState(true);
  const [sitesLoading, setSitesLoading] = useState(true);
  const [citiesLoading, setCitiesLoading] = useState(true);

  /* ── Fetch dashboard KPIs ── */
  const loadStats = useCallback(async () => {
    try {
      setStatsLoading(true);
      const res = await getDashboardStats();
      const d   = res.data?.value ?? res.data?.data ?? res.data;
      setStats({
        totalSites:     Number(d?.totalSites    ?? 0),
        activeSites:    Number(d?.activeSites   ?? 0),
        featuredSites:  Number(d?.featuredSites  ?? 0),
        hiddenGemSites: Number(d?.hiddenGemSites ?? 0),
        averageRating:  Number(d?.averageRating  ?? 0),
        totalRating:    Number(d?.totalRating    ?? 0),
      });
    } catch { toast('Failed to load dashboard stats', 'error'); }
    finally   { setStatsLoading(false); }
  }, []);

  /* ── Fetch all sites for the map (no page limit) ── */
  const loadSites = useCallback(async () => {
    try {
      setSitesLoading(true);
      const res  = await getSites({ pageSize: 99 });
      const d    = res.data;
      const arr  = Array.isArray(d) ? d
                 : Array.isArray(d?.data)  ? d.data
                 : Array.isArray(d?.value) ? d.value
                 : Array.isArray(d?.items) ? d.items
                 : [];
      setSites(arr);
    } catch { toast('Failed to load sites', 'error'); }
    finally   { setSitesLoading(false); }
  }, []);

  /* ── Fetch cities for marker coordinates ── */
  const loadCities = useCallback(async () => {
    try {
      setCitiesLoading(true);
      const res = await getCities();
      const d = res.data;
      const items = Array.isArray(d) ? d : d?.value ?? d?.data ?? d?.items ?? [];
      setCities(items);
    } catch {
      setCities([]);
    } finally {
      setCitiesLoading(false);
    }
  }, []);

  useEffect(() => { loadStats(); loadSites(); loadCities(); }, [loadStats, loadSites, loadCities]);

  /* ── Derived stats ── */
  const activePercent = stats
    ? Math.round((stats.activeSites / Math.max(stats.totalSites, 1)) * 100)
    : 0;

  const now = new Date();
  const timeStr = now.toLocaleTimeString('en-EG', { hour: '2-digit', minute: '2-digit' });
  const dateStr = now.toLocaleDateString('en-EG', { month: 'short', day: 'numeric' });

  return (
    <div style={{ display: 'flex', minHeight: '100vh', background: 'var(--background)', fontFamily: 'var(--font-body)' }}>
      <Sidebar />

      <div style={{ marginLeft: 'var(--sidebar-width)', flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>

        {/* ── Top bar ── */}
        <header style={{
          background: 'rgba(255,248,240,0.92)', backdropFilter: 'blur(12px)',
          borderBottom: '1px solid rgba(212,196,183,.2)',
          padding: '0 32px', height: 64,
          display: 'flex', alignItems: 'center', gap: 20,
          position: 'sticky', top: 0, zIndex: 50,
        }}>
          <div style={{ fontFamily: 'var(--font-display)', fontSize: 18, fontWeight: 800, color: 'var(--text)', letterSpacing: '-0.01em' }}>
            Rafeeq Admin
          </div>

          {/* Search pill */}
          <div style={{ flex: 1, maxWidth: 440, position: 'relative' }}>
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="var(--outline)" strokeWidth="2"
              style={{ position: 'absolute', left: 14, top: '50%', transform: 'translateY(-50%)', pointerEvents: 'none' }}>
              <circle cx="11" cy="11" r="8"/><path d="M21 21l-4.35-4.35"/>
            </svg>
            <input placeholder="Search Sites, Users, Reviews, Trips..." style={{
              width: '100%', padding: '8px 14px 8px 38px',
              background: 'var(--surface-container-low)',
              border: 'none', borderRadius: 999, fontSize: 13,
              color: 'var(--text)', outline: 'none',
            }} />
          </div>

          <div style={{ flex: 1 }} />

          {/* Nav links */}
          <nav style={{ display: 'flex', gap: 24 }}>
            <button style={{ background: 'none', border: 'none', cursor: 'pointer', fontSize: 14, fontWeight: 700, color: 'var(--primary)', borderBottom: '2px solid var(--primary)', paddingBottom: 2 }}>
              Platform Overview
            </button>
          </nav>

          {/* Right actions */}
          <div style={{ display: 'flex', alignItems: 'center', gap: 12, borderLeft: '1px solid rgba(212,196,183,.4)', paddingLeft: 16 }}>
            {/* Notification bell */}
            <button style={{ position: 'relative', background: 'none', border: 'none', cursor: 'pointer', padding: 6, borderRadius: 20 }}>
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="var(--on-surface-variant)" strokeWidth="2">
                <path d="M18 8A6 6 0 006 8c0 7-3 9-3 9h18s-3-2-3-9M13.73 21a2 2 0 01-3.46 0"/>
              </svg>
              <span style={{
                position: 'absolute', top: 4, right: 4,
                width: 14, height: 14, borderRadius: '50%',
                background: 'var(--error)', color: '#fff',
                fontSize: 8, fontWeight: 800,
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                border: '2px solid white',
              }}>12</span>
            </button>

            {/* Clock */}
            <div style={{ textAlign: 'right' }}>
              <div style={{ fontSize: 11, fontWeight: 800, color: 'var(--text)' }}>Cairo, EG</div>
              <div style={{ fontSize: 10, color: 'var(--outline)' }}>{timeStr} • {dateStr}</div>
            </div>

            {/* Avatar */}
            <div style={{
              width: 36, height: 36, borderRadius: '50%',
              background: 'linear-gradient(135deg, var(--primary), var(--primary-container))',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              color: '#fff', fontSize: 14, fontWeight: 800,
              border: '2px solid var(--primary-fixed)',
              flexShrink: 0,
            }}>R</div>
          </div>
        </header>

        {/* ── Body ── */}
        <div style={{ padding: '28px 32px 60px', flex: 1 }}>

          {/* Breadcrumb */}
          <div style={{ fontSize: 12, color: 'var(--outline)', marginBottom: 20, display: 'flex', alignItems: 'center', gap: 6 }}>
            <span>Dashboard</span>
            <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
            <span style={{ fontWeight: 700, color: 'var(--primary)' }}>Overview</span>
          </div>

          {/* KPI cards */}
          <div style={{ display: 'flex', gap: 16, marginBottom: 28, flexWrap: 'wrap' }}>
            <KpiCard
              label="Total Sites"
              value={stats?.totalSites}
              loading={statsLoading}
              sub={`${activePercent}% currently active`}
              tag="+12%"
              accent="var(--primary)"
              barPercent={activePercent}
              icon={
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2V9z"/>
                  <polyline points="9 22 9 12 15 12 15 22"/>
                </svg>
              }
            />
            <KpiCard
              label="Active Sites"
              value={stats?.activeSites}
              loading={statsLoading}
              sub="Currently accepting visitors"
              accent="#386a20"
              icon={
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <circle cx="12" cy="12" r="10"/>
                  <polyline points="12 6 12 12 16 14"/>
                </svg>
              }
            />
            <KpiCard
              label="Featured Sites"
              value={stats?.featuredSites}
              loading={statsLoading}
              sub={`+ ${stats?.hiddenGemSites ?? '…'} hidden gems`}
              tag="Promoted"
              accent="#d97706"
              icon={
                <svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor" stroke="none">
                  <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/>
                </svg>
              }
            />
            <KpiCard
              label="Avg Rating"
              value={stats ? (stats.averageRating > 0 ? stats.averageRating.toFixed(1) : '—') : null}
              loading={statsLoading}
              sub={`Across ${stats?.totalRating?.toLocaleString() ?? '…'} total ratings`}
              accent="var(--primary-container)"
              icon={
                <svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor" stroke="none">
                  <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/>
                </svg>
              }
            />
          </div>

          {/* <EgyptTourismMap /> */}
          {/* <EgyptTourismMap height={360} showLegend={false} /> */}

          {/* Map + Side panel */}
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 340px', gap: 20, alignItems: 'start' }}>
            {/* Map */}
            <EgyptMap
              sites={sites}
              cities={cities}
              loading={sitesLoading || citiesLoading}
            />

            {/* Right column */}
            <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
              {/* City breakdown */}
              <div style={{
                background: 'var(--surface-container-lowest)',
                borderRadius: 16, padding: '20px',
                boxShadow: '0 2px 12px rgba(29,27,23,.06)',
              }}>
                <h3 style={{ fontSize: 13, fontWeight: 700, color: 'var(--text)', marginBottom: 14 }}>
                  Sites by City
                </h3>
                {sitesLoading ? <Spinner center /> : (() => {
                  const byCity = {};
                  sites.forEach(s => {
                    const c = s.cityName || 'Unknown';
                    byCity[c] = (byCity[c] || 0) + 1;
                  });
                  const sorted = Object.entries(byCity).sort((a, b) => b[1] - a[1]).slice(0, 6);
                  const max = sorted[0]?.[1] || 1;
                  return (
                    <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
                      {sorted.map(([city, count]) => (
                        <div key={city} style={{ cursor: 'pointer' }} onClick={() => navigate(`/sites?city=${encodeURIComponent(city)}`)}>
                          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'baseline', marginBottom: 4 }}>
                            <span style={{ fontSize: 12, fontWeight: 600, color: 'var(--text)' }}>{city}</span>
                            <span style={{ fontSize: 11, fontWeight: 800, color: 'var(--primary)' }}>{count}</span>
                          </div>
                          <div style={{ height: 5, background: 'var(--surface-container-high)', borderRadius: 3, overflow: 'hidden' }}>
                            <div style={{
                              height: '100%', width: `${(count / max) * 100}%`,
                              background: 'linear-gradient(90deg, var(--primary), var(--primary-container))',
                              borderRadius: 3, transition: 'width .8s ease',
                            }} />
                          </div>
                        </div>
                      ))}
                      {sorted.length === 0 && <div style={{ color: 'var(--outline)', fontSize: 12, textAlign: 'center', padding: '12px 0' }}>No data yet.</div>}
                    </div>
                  );
                })()}
              </div>

              {/* Top rated */}
              <TopRatedList sites={sites} loading={sitesLoading} />

              
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
