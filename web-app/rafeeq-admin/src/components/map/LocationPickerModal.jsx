import React, {
  useState, useEffect, useCallback, useRef,
} from 'react';
import ReactDOM from 'react-dom';
import { MapContainer, TileLayer, Marker, useMapEvents, useMap } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

/* ─────────────────────────────────────────────────────────
   Fix Leaflet's broken default icon in Vite/Webpack
───────────────────────────────────────────────────────── */
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: new URL('leaflet/dist/images/marker-icon-2x.png', import.meta.url).href,
  iconUrl:       new URL('leaflet/dist/images/marker-icon.png',    import.meta.url).href,
  shadowUrl:     new URL('leaflet/dist/images/marker-shadow.png',  import.meta.url).href,
});

/* ─────────────────────────────────────────────────────────
   Custom pin marker — matches Rafeeq brand palette
───────────────────────────────────────────────────────── */
const PIN_ICON = L.divIcon({
  html: `
    <div style="position:relative;width:28px;height:36px;">
      <div style="
        width:28px;height:28px;
        background:linear-gradient(135deg,#7c572d,#d4a574);
        border:3px solid #fff;
        border-radius:50% 50% 50% 0;
        transform:rotate(-45deg);
        box-shadow:0 4px 14px rgba(0,0,0,0.35);
      "></div>
      <div style="
        position:absolute;top:7px;left:7px;
        width:10px;height:10px;
        background:#fff;border-radius:50%;
        transform:rotate(45deg);
        opacity:0.9;
      "></div>
    </div>`,
  className:   '',
  iconSize:    [28, 36],
  iconAnchor:  [14, 36],
  popupAnchor: [0, -38],
});

/* ─────────────────────────────────────────────────────────
   Map sub-components
───────────────────────────────────────────────────────── */

/** Captures map click events */
function ClickHandler({ onMapClick }) {
  useMapEvents({
    click(e) { onMapClick(e.latlng.lat, e.latlng.lng); },
  });
  return null;
}

/** Smoothly flies to a new position whenever it changes */
function FlyTo({ target }) {
  const map = useMap();
  const prev = useRef(null);

  useEffect(() => {
    if (!target) return;
    const key = `${target[0]},${target[1]}`;
    if (key === prev.current) return;
    prev.current = key;
    map.flyTo(target, Math.max(map.getZoom(), 14), { duration: 1.0 });
  }, [target, map]);

  return null;
}

/* ─────────────────────────────────────────────────────────
   Nominatim search (OpenStreetMap geocoding — no API key)
───────────────────────────────────────────────────────── */
async function searchNominatim(query) {
  const url = new URL('https://nominatim.openstreetmap.org/search');
  url.searchParams.set('q',              query);
  url.searchParams.set('format',         'json');
  url.searchParams.set('limit',          '6');
  url.searchParams.set('countrycodes',   'eg');   // bias to Egypt; remove for global
  url.searchParams.set('addressdetails', '0');

  const res = await fetch(url, {
    headers: { 'Accept-Language': 'en' },
  });
  if (!res.ok) throw new Error('Search failed');
  return res.json();
}

/* ─────────────────────────────────────────────────────────
   Search bar with dropdown
───────────────────────────────────────────────────────── */
function SearchBar({ onSelect }) {
  const [query,   setQuery]   = useState('');
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [open,    setOpen]    = useState(false);
  const debounce = useRef(null);
  const wrapRef  = useRef(null);

  // Close on outside click
  useEffect(() => {
    const handler = (e) => {
      if (wrapRef.current && !wrapRef.current.contains(e.target)) setOpen(false);
    };
    document.addEventListener('mousedown', handler);
    return () => document.removeEventListener('mousedown', handler);
  }, []);

  const handleChange = (e) => {
    const val = e.target.value;
    setQuery(val);
    clearTimeout(debounce.current);
    if (val.trim().length < 2) { setResults([]); setOpen(false); return; }

    debounce.current = setTimeout(async () => {
      setLoading(true);
      try {
        const data = await searchNominatim(val.trim());
        setResults(data);
        setOpen(data.length > 0);
      } catch { /* ignore */ }
      finally { setLoading(false); }
    }, 400);
  };

  const pick = (item) => {
    setQuery(item.display_name.split(',').slice(0, 2).join(','));
    setOpen(false);
    onSelect(parseFloat(item.lat), parseFloat(item.lon), item.display_name);
  };

  return (
    <div ref={wrapRef} style={{ position: 'relative' }}>
      <div style={{ position: 'relative' }}>
        <svg width="15" height="15" viewBox="0 0 24 24" fill="none"
          stroke="var(--outline)" strokeWidth="2"
          style={{ position: 'absolute', left: 12, top: '50%', transform: 'translateY(-50%)', pointerEvents: 'none' }}>
          <circle cx="11" cy="11" r="8"/><path d="M21 21l-4.35-4.35"/>
        </svg>
        <input
          value={query}
          onChange={handleChange}
          onFocus={() => results.length > 0 && setOpen(true)}
          placeholder="Search for a place or address…"
          style={{
            width: '100%', padding: '10px 40px 10px 38px',
            background: 'var(--surface-container-low)',
            border: '1px solid var(--border)',
            borderRadius: 10, fontSize: 13,
            color: 'var(--text)', outline: 'none',
            fontFamily: 'var(--font-body)',
            transition: 'border-color .15s, box-shadow .15s',
          }}
          onMouseOver={e => e.target.style.borderColor = 'var(--primary)'}
          onMouseOut={e => { if (document.activeElement !== e.target) e.target.style.borderColor = 'var(--border)'; }}
        />
        {loading && (
          <div style={{ position: 'absolute', right: 12, top: '50%', transform: 'translateY(-50%)' }}>
            <div style={{
              width: 14, height: 14, borderRadius: '50%',
              border: '2px solid var(--outline-variant)',
              borderTop: '2px solid var(--primary)',
              animation: 'spin .6s linear infinite',
            }} />
          </div>
        )}
      </div>

      {/* Dropdown results */}
      {open && results.length > 0 && (
        <div style={{
          position: 'absolute', top: 'calc(100% + 4px)', left: 0, right: 0,
          background: 'var(--surface-container-lowest)',
          border: '1px solid var(--border)',
          borderRadius: 10, zIndex: 9999,
          boxShadow: 'var(--shadow-card)',
          maxHeight: 220, overflowY: 'auto',
        }}>
          {results.map((item) => (
            <button
              key={item.place_id}
              onMouseDown={() => pick(item)}
              style={{
                width: '100%', textAlign: 'left',
                padding: '10px 14px',
                background: 'none', border: 'none', cursor: 'pointer',
                borderBottom: '1px solid var(--border)',
                fontSize: 12, color: 'var(--text)',
                fontFamily: 'var(--font-body)',
                display: 'flex', flexDirection: 'column', gap: 2,
                transition: 'background .1s',
              }}
              onMouseOver={e => e.currentTarget.style.background = 'var(--surface-container-low)'}
              onMouseOut={e => e.currentTarget.style.background = 'none'}
            >
              <span style={{ fontWeight: 600, fontSize: 13, color: 'var(--text)' }}>
                {item.display_name.split(',')[0]}
              </span>
              <span style={{ color: 'var(--outline)', fontSize: 11 }}>
                {item.display_name.split(',').slice(1, 3).join(',').trim()}
              </span>
            </button>
          ))}
        </div>
      )}
    </div>
  );
}

/* ─────────────────────────────────────────────────────────
   Coordinate display strip
───────────────────────────────────────────────────────── */
function CoordDisplay({ lat, lng }) {
  const has = lat !== null && lng !== null;
  return (
    <div style={{
      display: 'flex', gap: 12, padding: '12px 16px',
      background: 'var(--surface-container-low)',
      border: '1px solid var(--border)',
      borderRadius: 10,
    }}>
      {[['Latitude', lat], ['Longitude', lng]].map(([label, val]) => (
        <div key={label} style={{ flex: 1 }}>
          <div style={{ fontSize: 10, fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', color: 'var(--outline)', marginBottom: 3 }}>
            {label}
          </div>
          <div style={{
            fontSize: 15, fontWeight: 700, fontFamily: 'monospace',
            color: has ? 'var(--primary)' : 'var(--outline)',
            letterSpacing: '0.02em',
          }}>
            {has ? val.toFixed(6) : '—'}
          </div>
        </div>
      ))}
      {has && (
        <div style={{ display: 'flex', alignItems: 'center' }}>
          <div style={{
            display: 'flex', alignItems: 'center', gap: 5,
            fontSize: 11, color: 'var(--green)', fontWeight: 600,
          }}>
            <div style={{ width: 7, height: 7, borderRadius: '50%', background: 'var(--green)', boxShadow: '0 0 0 2px var(--green-bg)' }} />
            Location selected
          </div>
        </div>
      )}
    </div>
  );
}

/* ─────────────────────────────────────────────────────────
   Main modal component — rendered via portal at document.body
   so it sits above any existing modals (z-index: 2000)
───────────────────────────────────────────────────────── */
export default function LocationPickerModal({ open, onClose, onConfirm, initial = null }) {
  const [position,  setPosition]  = useState(null);  // [lat, lng]
  const [flyTarget, setFlyTarget] = useState(null);

  // Seed with existing coordinates when editing
  useEffect(() => {
    if (!open) return;
    if (initial?.latitude && initial?.longitude) {
      const p = [parseFloat(initial.latitude), parseFloat(initial.longitude)];
      setPosition(p);
      setFlyTarget(p);
    } else {
      setPosition(null);
      setFlyTarget(null);
    }
  }, [open, initial?.latitude, initial?.longitude]);

  const handleMapClick = useCallback((lat, lng) => {
    setPosition([lat, lng]);
  }, []);

  const handleSearchSelect = useCallback((lat, lng) => {
    const p = [lat, lng];
    setPosition(p);
    setFlyTarget(p);
  }, []);

  const handleDragEnd = useCallback((e) => {
    const { lat, lng } = e.target.getLatLng();
    setPosition([lat, lng]);
  }, []);

  const handleConfirm = () => {
    if (!position) return;
    onConfirm({ latitude: position[0], longitude: position[1] });
    onClose();
  };

  const handleKeyDown = useCallback((e) => {
    if (e.key === 'Escape') onClose();
  }, [onClose]);

  useEffect(() => {
    if (open) document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [open, handleKeyDown]);

  if (!open) return null;

  const lat = position?.[0] ?? null;
  const lng = position?.[1] ?? null;

  return ReactDOM.createPortal(
    <div
      onClick={(e) => { if (e.target === e.currentTarget) onClose(); }}
      style={{
        position: 'fixed', inset: 0, zIndex: 2000,
        background: 'rgba(0,0,0,0.6)',
        backdropFilter: 'blur(4px)',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        padding: 20,
        animation: 'lpFadeIn .15s ease',
      }}
    >
      <style>{`
        @keyframes lpFadeIn  { from { opacity:0 } to { opacity:1 } }
        @keyframes lpSlideUp { from { transform:translateY(16px);opacity:0 } to { transform:translateY(0);opacity:1 } }
        @keyframes spin      { to { transform:rotate(360deg) } }
        .lp-map-wrap .leaflet-container { border-radius: 12px; }
        .lp-map-wrap .leaflet-control-zoom { border-radius: 8px !important; overflow:hidden; }
        .lp-map-wrap .leaflet-control-zoom a {
          background: var(--surface-container-lowest) !important;
          color: var(--text) !important;
          border-bottom: 1px solid var(--border) !important;
          font-size: 16px !important;
        }
        .lp-map-wrap .leaflet-control-zoom a:hover {
          background: var(--surface-container-low) !important;
        }
      `}</style>

      <div
        style={{
          background: 'var(--surface-container-lowest)',
          borderRadius: 20, overflow: 'hidden',
          width: '100%', maxWidth: 640,
          display: 'flex', flexDirection: 'column',
          boxShadow: 'var(--shadow-modal)',
          animation: 'lpSlideUp .2s ease',
          maxHeight: '92vh',
        }}
      >
        {/* ── Header ── */}
        <div style={{
          display: 'flex', alignItems: 'center', justifyContent: 'space-between',
          padding: '18px 22px 16px',
          borderBottom: '1px solid var(--border)',
          flexShrink: 0,
        }}>
          <div>
            <h2 style={{ fontFamily: 'var(--font-display)', fontSize: 18, fontWeight: 700, color: 'var(--text)', margin: 0 }}>
              Select Location
            </h2>
            <p style={{ fontSize: 12, color: 'var(--outline)', margin: '2px 0 0' }}>
              Search, click the map, or drag the pin
            </p>
          </div>
          <button onClick={onClose} style={{
            background: 'none', border: 'none', cursor: 'pointer',
            color: 'var(--outline)', padding: 6, borderRadius: 8,
            display: 'flex', transition: 'color .15s',
          }}
            onMouseOver={e => e.currentTarget.style.color = 'var(--text)'}
            onMouseOut={e => e.currentTarget.style.color = 'var(--outline)'}
          >
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <path d="M18 6L6 18M6 6l12 12"/>
            </svg>
          </button>
        </div>

        {/* ── Body ── */}
        <div style={{ padding: '16px 22px', display: 'flex', flexDirection: 'column', gap: 14, flex: 1, overflowY: 'auto' }}>

          {/* Search */}
          <SearchBar onSelect={handleSearchSelect} />

          {/* Map */}
          <div
            className="lp-map-wrap"
            style={{
              height: 360, borderRadius: 12, overflow: 'hidden',
              border: '1px solid var(--border)',
              cursor: 'crosshair',
              flexShrink: 0,
            }}
          >
            <MapContainer
              center={position ?? [26.8206, 30.8025]}
              zoom={position ? 14 : 6}
              style={{ height: '100%', width: '100%' }}
              zoomControl={true}
            >
              <TileLayer
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
              />
              <ClickHandler onMapClick={handleMapClick} />
              {flyTarget && <FlyTo target={flyTarget} />}
              {position && (
                <Marker
                  position={position}
                  icon={PIN_ICON}
                  draggable={true}
                  eventHandlers={{ dragend: handleDragEnd }}
                />
              )}
            </MapContainer>
          </div>

          {/* Coordinates */}
          <CoordDisplay lat={lat} lng={lng} />

          {/* Hint */}
          <div style={{
            display: 'flex', alignItems: 'center', gap: 8,
            padding: '10px 14px',
            background: 'rgba(124,87,45,0.06)',
            borderRadius: 9, border: '1px solid rgba(124,87,45,0.15)',
          }}>
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="var(--primary)" strokeWidth="2" style={{ flexShrink: 0 }}>
              <circle cx="12" cy="12" r="10"/>
              <path d="M12 16v-4M12 8h.01"/>
            </svg>
            <span style={{ fontSize: 12, color: 'var(--text-2)', lineHeight: 1.5 }}>
              Click anywhere on the map to place a pin, or drag the pin to adjust. You can also search for an address above.
            </span>
          </div>
        </div>

        {/* ── Footer ── */}
        <div style={{
          display: 'flex', justifyContent: 'flex-end', gap: 10,
          padding: '14px 22px',
          borderTop: '1px solid var(--border)',
          flexShrink: 0,
          background: 'var(--surface-container-low)',
        }}>
          <button onClick={onClose} style={{
            padding: '9px 20px', borderRadius: 10, border: 'none',
            background: 'var(--surface-container-lowest)',
            boxShadow: '0 0 0 1px var(--outline-variant)',
            color: 'var(--text-2)', fontSize: 13, fontWeight: 700,
            cursor: 'pointer', fontFamily: 'var(--font-body)',
            transition: 'background .15s',
          }}
            onMouseOver={e => e.currentTarget.style.background = 'var(--surface-container)'}
            onMouseOut={e => e.currentTarget.style.background = 'var(--surface-container-lowest)'}
          >
            Cancel
          </button>
          <button
            onClick={handleConfirm}
            disabled={!position}
            style={{
              padding: '9px 22px', borderRadius: 10, border: 'none',
              background: position
                ? 'linear-gradient(135deg, var(--primary), var(--primary-container))'
                : 'var(--surface-container-high)',
              color: position ? '#fff' : 'var(--outline)',
              fontSize: 13, fontWeight: 700,
              cursor: position ? 'pointer' : 'not-allowed',
              fontFamily: 'var(--font-body)',
              boxShadow: position ? '0 2px 10px rgba(124,87,45,0.3)' : 'none',
              transition: 'all .15s',
              display: 'flex', alignItems: 'center', gap: 7,
            }}
          >
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <path d="M21 10c0 7-9 13-9 13S3 17 3 10a9 9 0 0118 0z"/>
              <circle cx="12" cy="10" r="3"/>
            </svg>
            Use Selected Location
          </button>
        </div>
      </div>
    </div>,
    document.body
  );
}