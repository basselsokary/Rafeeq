import React, { useState, useEffect, useCallback } from 'react';
import { MapContainer, TileLayer, ZoomControl } from 'react-leaflet';
import MarkerClusterGroup from 'react-leaflet-markercluster';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import 'leaflet.markercluster/dist/MarkerCluster.css';
import 'leaflet.markercluster/dist/MarkerCluster.Default.css';

import { getCities as getMapData } from '../../api/citiesApi';
import MapMarker from './MapMarker';

/* ─────────────────────────────────────────────────────────
   Fix Leaflet's broken default icon paths in Webpack/Vite.
   Must run once, before any map renders.
───────────────────────────────────────────────────────── */
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: new URL('leaflet/dist/images/marker-icon-2x.png', import.meta.url).href,
  iconUrl:       new URL('leaflet/dist/images/marker-icon.png',    import.meta.url).href,
  shadowUrl:     new URL('leaflet/dist/images/marker-shadow.png',  import.meta.url).href,
});

/* ─────────────────────────────────────────────────────────
   Egypt map config
───────────────────────────────────────────────────────── */
const EGYPT_CENTER = [26.8206, 30.8025]; // geographic center of Egypt
const DEFAULT_ZOOM = 6;
const MIN_ZOOM     = 5;
const MAX_ZOOM     = 14;

/* ─────────────────────────────────────────────────────────
   Cluster icon factory — styled to match Rafeeq palette
───────────────────────────────────────────────────────── */
function createClusterIcon(cluster) {
  const count  = cluster.getChildCount();
  const size   = count >= 100 ? 52 : count >= 20 ? 44 : 36;
  const bg     = count >= 100 ? '#5a3d1a' : count >= 20 ? '#7c572d' : '#a06830';

  return L.divIcon({
    html: `
      <div style="
        width:${size}px; height:${size}px;
        background:${bg};
        border:3px solid rgba(255,255,255,0.5);
        border-radius:50%;
        display:flex; align-items:center; justify-content:center;
        color:#fff;
        font-size:${size >= 52 ? 14 : 12}px;
        font-weight:800;
        font-family:'DM Sans',system-ui,sans-serif;
        box-shadow:0 4px 14px rgba(0,0,0,0.3);
      ">${count}</div>`,
    className:  '',
    iconSize:   [size, size],
    iconAnchor: [size / 2, size / 2],
  });
}

/* ─────────────────────────────────────────────────────────
   Loading skeleton
───────────────────────────────────────────────────────── */
function LoadingSkeleton() {
  return (
    <div style={{
      position: 'absolute', inset: 0, zIndex: 1000,
      background: 'rgba(249,243,235,0.85)',
      backdropFilter: 'blur(4px)',
      display: 'flex', flexDirection: 'column',
      alignItems: 'center', justifyContent: 'center',
      gap: 14, borderRadius: 'inherit',
    }}>
      <div style={{
        width: 44, height: 44, borderRadius: '50%',
        border: '3px solid rgba(124,87,45,0.15)',
        borderTop: '3px solid #7c572d',
        animation: 'spin 0.7s linear infinite',
      }} />
      <span style={{
        fontSize: 13, fontWeight: 600,
        color: '#9c8b7a',
        fontFamily: "'DM Sans', system-ui, sans-serif",
      }}>Loading map data…</span>
      <style>{`@keyframes spin { to { transform: rotate(360deg); } }`}</style>
    </div>
  );
}

/* ─────────────────────────────────────────────────────────
   Error state
───────────────────────────────────────────────────────── */
function ErrorState({ message, onRetry }) {
  return (
    <div style={{
      position: 'absolute', inset: 0, zIndex: 1000,
      background: 'rgba(249,243,235,0.92)',
      backdropFilter: 'blur(4px)',
      display: 'flex', flexDirection: 'column',
      alignItems: 'center', justifyContent: 'center',
      gap: 12, borderRadius: 'inherit',
      padding: 24,
    }}>
      <div style={{ fontSize: 36 }}>⚠️</div>
      <p style={{
        fontSize: 13, color: '#6b5d4f', textAlign: 'center',
        fontFamily: "'DM Sans', system-ui, sans-serif",
        maxWidth: 260, lineHeight: 1.6,
      }}>
        {message || 'Failed to load map data.'}
      </p>
      <button
        onClick={onRetry}
        style={{
          padding: '8px 20px', borderRadius: 8, border: 'none',
          background: 'linear-gradient(135deg, #7c572d, #d4a574)',
          color: '#fff', fontSize: 13, fontWeight: 700,
          cursor: 'pointer', fontFamily: "'DM Sans', system-ui, sans-serif",
          boxShadow: '0 2px 8px rgba(124,87,45,0.3)',
        }}
      >
        Retry
      </button>
    </div>
  );
}

/* ─────────────────────────────────────────────────────────
   Map legend
───────────────────────────────────────────────────────── */
function MapLegend({ totalSites, cityCount }) {
  const tiers = [
    { label: '300+',   color: '#7c572d' },
    { label: '100+',   color: '#a06830' },
    { label: '40+',    color: '#c28040' },
    { label: '10+',    color: '#d4a574' },
    { label: '< 10',   color: '#e8c89a' },
  ];

  return (
    <div style={{
      position: 'absolute', bottom: 28, left: 12, zIndex: 999,
      background: 'rgba(255,250,245,0.95)',
      backdropFilter: 'blur(8px)',
      borderRadius: 12, padding: '12px 14px',
      boxShadow: '0 4px 16px rgba(29,21,10,0.12)',
      border: '1px solid rgba(212,196,183,0.5)',
      fontFamily: "'DM Sans', system-ui, sans-serif",
      minWidth: 160,
    }}>
      {/* Summary */}
      <div style={{ marginBottom: 10, paddingBottom: 10, borderBottom: '1px solid rgba(212,196,183,0.4)' }}>
        <div style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', color: '#9c8b7a', marginBottom: 4 }}>
          Overview
        </div>
        <div style={{ display: 'flex', gap: 14 }}>
          <div>
            <div style={{ fontSize: 18, fontWeight: 800, color: '#7c572d', lineHeight: 1 }}>{totalSites.toLocaleString()}</div>
            <div style={{ fontSize: 10, color: '#9c8b7a', fontWeight: 500 }}>Total Sites</div>
          </div>
          <div>
            <div style={{ fontSize: 18, fontWeight: 800, color: '#7c572d', lineHeight: 1 }}>{cityCount}</div>
            <div style={{ fontSize: 10, color: '#9c8b7a', fontWeight: 500 }}>Cities</div>
          </div>
        </div>
      </div>

      {/* Count range legend */}
      <div style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', color: '#9c8b7a', marginBottom: 6 }}>
        Sites Count
      </div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 5 }}>
        {tiers.map(t => (
          <div key={t.label} style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
            <div style={{ width: 20, height: 14, borderRadius: 7, background: t.color, flexShrink: 0 }} />
            <span style={{ fontSize: 11, color: '#6b5d4f', fontWeight: 500 }}>{t.label}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

/* ─────────────────────────────────────────────────────────
   Main EgyptTourismMap component
───────────────────────────────────────────────────────── */
export default function EgyptTourismMap({
  height = 480,       // container height in px
  showLegend = true,  // toggle legend overlay
  // Future prop: mode = 'markers' | 'heatmap'
}) {
  const [cities,  setCities]  = useState([]);
  const [loading, setLoading] = useState(true);
  const [error,   setError]   = useState(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res  = await getMapData();
      const data = res.data;

      // Normalise — handle wrapped or direct array responses
      const arr = Array.isArray(data) ? data
                : Array.isArray(data?.value) ? data.value
                : Array.isArray(data?.data)  ? data.data
                : [];

      // Guard: drop entries missing coordinates
      setCities(arr.filter(c => c.centerLocation.latitude != null && c.centerLocation.longitude != null));
    } catch (err) {
      console.error('[EgyptTourismMap] fetch error:', err);
      setError(err?.response?.data?.message || err?.message || 'Unknown error');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchData(); }, [fetchData]);

  const totalSites = cities.reduce((sum, c) => sum + (c.totalSites || 0), 0);

  return (
    <div style={{
      position: 'relative',
      height,
      width: '100%',
      borderRadius: 16,
      overflow: 'hidden',
      background: '#e8f4f8',
      boxShadow: '0 2px 12px rgba(29,21,10,0.08)',
      border: '1px solid rgba(212,196,183,0.3)',
    }}>

      {/* Leaflet map */}
      <MapContainer
        center={EGYPT_CENTER}
        zoom={DEFAULT_ZOOM}
        minZoom={MIN_ZOOM}
        maxZoom={MAX_ZOOM}
        zoomControl={false}           // we add it in a custom position below
        style={{ height: '100%', width: '100%' }}
        // Egypt approximate bounds — prevents panning too far off
        maxBounds={[[19.0, 22.0], [32.5, 38.0]]}
        maxBoundsViscosity={0.8}
      >
        {/* Base tile layer — OpenStreetMap */}
        <TileLayer
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          // Warm/sand retina tiles when available
          tileSize={256}
          zoomOffset={0}
        />

        {/* Zoom controls — bottom right */}
        <ZoomControl position="bottomright" />

        {/* Marker clustering */}
        {!loading && !error && (
          <MarkerClusterGroup
            iconCreateFunction={createClusterIcon}
            showCoverageOnHover={false}
            spiderfyOnMaxZoom={true}
            disableClusteringAtZoom={10}
            chunkedLoading={true}
            maxClusterRadius={60}
          >
            {cities.map(city => (
              <MapMarker key={city.id} city={city} />
            ))}
          </MarkerClusterGroup>
        )}
      </MapContainer>

      {/* Overlays — rendered outside MapContainer so z-index works cleanly */}
      {loading && <LoadingSkeleton />}
      {!loading && error && <ErrorState message={error} onRetry={fetchData} />}
      {!loading && !error && showLegend && (
        <MapLegend totalSites={totalSites} cityCount={cities.length} />
      )}

      {/* Top-right label */}
      {!loading && !error && (
        <div style={{
          position: 'absolute', top: 12, right: 12, zIndex: 999,
          background: 'rgba(255,250,245,0.95)',
          backdropFilter: 'blur(8px)',
          borderRadius: 20, padding: '5px 13px',
          boxShadow: '0 2px 10px rgba(29,21,10,0.1)',
          border: '1px solid rgba(212,196,183,0.5)',
          fontSize: 11, fontWeight: 700,
          color: '#7c572d',
          fontFamily: "'DM Sans', system-ui, sans-serif",
          letterSpacing: '0.04em',
          display: 'flex', alignItems: 'center', gap: 5,
        }}>
          <span style={{ width: 7, height: 7, borderRadius: '50%', background: '#386a20', display: 'inline-block', boxShadow: '0 0 0 2px rgba(56,106,32,0.25)' }} />
          LIVE
        </div>
      )}

      {/* Popup global styles injected once */}
      <style>{`
        .rafeeq-popup .leaflet-popup-content-wrapper {
          border-radius: 12px;
          box-shadow: 0 8px 28px rgba(29,21,10,0.18);
          border: 1px solid rgba(212,196,183,0.5);
          padding: 0;
        }
        .rafeeq-popup .leaflet-popup-content {
          margin: 14px 16px;
          min-width: 160px;
        }
        .rafeeq-popup .leaflet-popup-tip-container {
          margin-top: -1px;
        }
        .rafeeq-popup .leaflet-popup-tip {
          background: #fff;
          box-shadow: none;
        }
        .rafeeq-popup .leaflet-popup-close-button {
          top: 8px !important;
          right: 10px !important;
          font-size: 16px !important;
          color: #9c8b7a !important;
        }
        /* Cluster styles override */
        .marker-cluster-small,
        .marker-cluster-medium,
        .marker-cluster-large {
          background: transparent !important;
        }
        .marker-cluster-small div,
        .marker-cluster-medium div,
        .marker-cluster-large div {
          background: transparent !important;
        }
        /* Leaflet tooltip */
        .leaflet-tooltip {
          border-radius: 8px !important;
          border: 1px solid rgba(212,196,183,0.5) !important;
          box-shadow: 0 2px 8px rgba(29,21,10,0.12) !important;
          padding: 4px 10px !important;
          background: rgba(255,250,245,0.97) !important;
          color: #1c1814 !important;
          font-size: 12px;
        }
        .leaflet-tooltip-top::before {
          border-top-color: rgba(212,196,183,0.5) !important;
        }
      `}</style>
    </div>
  );
}
