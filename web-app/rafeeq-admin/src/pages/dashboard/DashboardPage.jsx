import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import Spinner from '../../components/common/Spinner';
import { useToast } from '../../components/common/Toast';
import { MapContainer, TileLayer, Marker, Tooltip, GeoJSON, Polygon, useMap } from 'react-leaflet';
import L from 'leaflet';
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png';
import markerIcon from 'leaflet/dist/images/marker-icon.png';
import markerShadow from 'leaflet/dist/images/marker-shadow.png';
import egyptGeoJson from '../../assets/egypt.json';

import { getDashboardStats } from '../../api/dashboardApi';
import { getCities } from '../../api/citiesApi';

// Fix default marker icons for Vite builds
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: markerIcon2x,
  iconUrl: markerIcon,
  shadowUrl: markerShadow,
});

const EGYPT_POSITION = [26.8206, 30.8025];

function FitToBounds({ bounds }) {
  const map = useMap();

  useEffect(() => {
    if (!bounds) return;
    const minZoom = Math.max(4, map.getBoundsZoom(bounds, true) - 1);
    map.setMinZoom(minZoom);
    map.setMaxBounds(bounds);
    map.fitBounds(bounds, { padding: [20, 20], maxZoom: minZoom });
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

/* ══════════════════════════════════════════════════════
  EGYPT MAP COMPONENT (Leaflet)
  Shows Egypt-only view with city markers.
  Tooltip on hover shows total sites.
═══════════════════════════════════════════════════════ */
function EgyptMap({ cities, loading }) {
  const primaryMarkerIcon = useMemo(() => (
    L.divIcon({
      className: 'primary-marker-icon',
      html: `
        <div style="width:40px;height:40px;display:flex;align-items:center;justify-content:center;">
          <svg width="40" height="40" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" style="filter:drop-shadow(0 6px 12px rgba(29,27,23,.35));">
            <path d="M12 21s6-6 6-10a6 6 0 10-12 0c0 4 6 10 6 10z" fill="var(--primary)" stroke="var(--surface-container-lowest)" stroke-width="1.6"/>
            <circle cx="12" cy="11" r="2.8" fill="var(--surface-container-lowest)"/>
          </svg>
        </div>
      `,
      iconSize: [40, 40],
      iconAnchor: [20, 38],
      popupAnchor: [0, -34],
    })
  ), []);

  const totalSites = useMemo(() => (
    (Array.isArray(cities) ? cities : []).reduce((sum, c) => sum + Number(c?.totalSites ?? 0), 0)
  ), [cities]);

  const cityMarkers = useMemo(() => {
    const list = Array.isArray(cities) ? cities : [];
    return list
      .map(c => {
        const lat = Number(c?.centerLocation?.latitude);
        const lng = Number(c?.centerLocation?.longitude);
        if (!Number.isFinite(lat) || !Number.isFinite(lng)) return null;

        const name = c?.name || 'Unknown';
        const count = Number(c?.totalSites ?? 0);
        return {
          id: c?.id ?? name,
          name,
          lat,
          lng,
          count,
        };
      })
      .filter(Boolean);
  }, [cities]);

  const egyptBounds = useMemo(() => {
    try {
      return L.geoJSON(egyptGeoJson).getBounds();
    } catch {
      return null;
    }
  }, []);

  const maxBounds = useMemo(() => {
    if (!egyptBounds) return null;
    return egyptBounds.pad(0.25);
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
        background: 'var(--topbar-bg)', backdropFilter: 'blur(8px)',
        borderBottom: '1px solid var(--topbar-border)',
        display: 'flex', alignItems: 'center', justifyContent: 'space-between',
      }}>
        <div>
          <div style={{ fontSize: 14, fontWeight: 700, color: 'var(--text)' }}>Sites Across Egypt</div>
          <div style={{ fontSize: 11, color: 'var(--outline)', marginTop: 1 }}>
            {loading ? 'Loading…' : `${totalSites} sites across ${cityMarkers.length} cities`}
          </div>
        </div>
      </div>

      <div style={{ paddingTop: 60, height: '100%', position: 'relative' }}>
        <div style={{ position: 'relative', zIndex: 1 }}>
        <MapContainer
          center={EGYPT_POSITION}
          zoom={6}
          zoomControl={false}
          dragging={true}
          scrollWheelZoom={true}
          doubleClickZoom={true}
          touchZoom={true}
          boxZoom={false}
          keyboard={true}
          inertia={false}
          maxBounds={maxBounds || undefined}
          maxBoundsViscosity={1.0}
          minZoom={5}
          maxZoom={10}
          worldCopyJump={false}
          style={{ height: 400, width: '100%', backgroundColor: 'transparent', zIndex: 1 }}
        >
          {maxBounds ? <FitToBounds bounds={maxBounds} /> : null}
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
              icon={primaryMarkerIcon}
            >
              <Tooltip direction="top" offset={[0, -10]} opacity={1} sticky className="map-tooltip">
                <div className="map-tooltip-title">{city.name}</div>
                <div className="map-tooltip-sub">{city.count} site{city.count !== 1 ? 's' : ''}</div>
              </Tooltip>
            </Marker>
          ))}
        </MapContainer>
        </div>

        {loading && (
          <div style={{
            position: 'absolute', inset: 0, display: 'flex', alignItems: 'center', justifyContent: 'center',
            background: 'var(--topbar-bg)', backdropFilter: 'blur(4px)', zIndex: 20,
          }}>
            <Spinner size={32} />
          </div>
        )}

        <div style={{
          position: 'absolute', bottom: 16, left: 14, zIndex: 999,
          background: 'var(--surface-container-lowest)',
          border: '1px solid var(--border-solid)',
          borderRadius: 10, padding: '8px 10px',
          boxShadow: 'var(--shadow-sm)',
          pointerEvents: 'auto',
        }}>
          <div style={{ fontSize: 9, letterSpacing: '0.08em', textTransform: 'uppercase', color: 'var(--text-muted)', fontWeight: 700, marginBottom: 6 }}>
            Legend
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="var(--primary)" strokeWidth="2" style={{ flexShrink: 0 }}>
              <path d="M12 21s6-6 6-10a6 6 0 10-12 0c0 4 6 10 6 10z" />
              <circle cx="12" cy="11" r="2.5" fill="var(--primary)" stroke="none" />
            </svg>
            <span style={{ fontSize: 11, color: 'var(--text)' }}>Location marker</span>
          </div>
          <div style={{ marginTop: 6, fontSize: 10, color: 'var(--text-2)' }}>Scroll to zoom</div>
        </div>
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
  const [cities,       setCities]       = useState([]);
  const [statsLoading, setStatsLoading] = useState(true);
  const [citiesLoading, setCitiesLoading] = useState(true);

  /* ── Fetch dashboard KPIs ── */
  const loadStats = useCallback(async () => {
    try {
      setStatsLoading(true);
      const res = await getDashboardStats();
      const d   = res.data?.value ?? res.data?.data ?? res.data;
      setStats({
        totalCities:   Number(d?.totalCities   ?? 0),
        totalSites:    Number(d?.totalSites    ?? 0),
        totalSponsors: Number(d?.totalSponsors ?? 0),
        totalUsers:    Number(d?.totalUsers    ?? 0),
      });
    } catch { toast('Failed to load dashboard stats', 'error'); }
    finally   { setStatsLoading(false); }
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

  useEffect(() => { loadStats(); loadCities(); }, [loadStats, loadCities]);

  return (
    <div style={{ padding: '28px 32px 60px', fontFamily: 'var(--font-body)' }}>

          {/* Breadcrumb */}
          <div style={{ fontSize: 12, color: 'var(--outline)', marginBottom: 20, display: 'flex', alignItems: 'center', gap: 6 }}>
            <span>Dashboard</span>
            <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M9 18l6-6-6-6"/></svg>
            <span style={{ fontWeight: 700, color: 'var(--primary)' }}>Overview</span>
          </div>

          {/* KPI cards */}
          <div style={{ display: 'flex', gap: 16, marginBottom: 28, flexWrap: 'wrap' }}>
            <KpiCard
              label="Total Cities"
              value={stats?.totalCities}
              loading={statsLoading}
              sub="Active destinations managed"
              accent="var(--primary)"
              icon={
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2V9z"/>
                  <polyline points="9 22 9 12 15 12 15 22"/>
                </svg>
              }
            />
            <KpiCard
              label="Total Sites"
              value={stats?.totalSites}
              loading={statsLoading}
              sub="Sites listed across cities"
              accent="#386a20"
              icon={
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <circle cx="12" cy="12" r="10"/>
                  <polyline points="12 6 12 12 16 14"/>
                </svg>
              }
            />
            <KpiCard
              label="Total Sponsors"
              value={stats?.totalSponsors}
              loading={statsLoading}
              sub="Active brand partners"
              accent="#d97706"
              icon={
                <svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor" stroke="none">
                  <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/>
                </svg>
              }
            />
            <KpiCard
              label="Total Users"
              value={stats?.totalUsers}
              loading={statsLoading}
              sub="Registered platform users"
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
              cities={cities}
              loading={citiesLoading}
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
                {citiesLoading ? <Spinner center /> : (() => {
                  const list = Array.isArray(cities) ? cities : [];
                  const sorted = [...list]
                    .map((c) => ({
                      name: c?.name || 'Unknown',
                      count: Number(c?.totalSites ?? 0),
                    }))
                    .sort((a, b) => b.count - a.count)
                    .slice(0, 6);
                  const max = sorted[0]?.count || 1;
                  return (
                    <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
                      {sorted.map(({ name, count }) => (
                        <div key={name} style={{ cursor: 'pointer' }} onClick={() => navigate(`/sites?city=${encodeURIComponent(name)}`)}>
                          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'baseline', marginBottom: 4 }}>
                            <span style={{ fontSize: 12, fontWeight: 600, color: 'var(--text)' }}>{name}</span>
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
            </div>
          </div>
    </div>
  );
}
