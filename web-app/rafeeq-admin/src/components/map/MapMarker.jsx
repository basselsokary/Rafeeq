import React, { useRef } from 'react';
import { Marker, Popup, Tooltip } from 'react-leaflet';
import L from 'leaflet';

/* ─────────────────────────────────────────────────────────
   Badge colour by site count range
───────────────────────────────────────────────────────── */
function getBadgeColor(count) {
  if (count >= 300) return { bg: '#7c572d', border: '#5a3d1a', text: '#fff' };
  if (count >= 100) return { bg: '#a06830', border: '#7c4f1e', text: '#fff' };
  if (count >= 40)  return { bg: '#c28040', border: '#9a6020', text: '#fff' };
  if (count >= 10)  return { bg: '#d4a574', border: '#b07840', text: '#fff' };
  return               { bg: '#e8c89a', border: '#c09060', text: '#4a3010' };
}

/* ─────────────────────────────────────────────────────────
   Build a DivIcon for each marker.
   The icon is a pill badge showing the count.
───────────────────────────────────────────────────────── */
function createBadgeIcon(totalSites) {
  const { bg, border, text } = getBadgeColor(totalSites);

  const label =
    totalSites >= 1000
      ? `${(totalSites / 1000).toFixed(1)}k`
      : String(totalSites);

  // Width adapts to label length
  const w = label.length <= 2 ? 36 : label.length <= 3 ? 44 : 52;
  const h = 28;

  const html = `
    <div style="
      width:${w}px; height:${h}px;
      background:${bg};
      border:2px solid ${border};
      border-radius:14px;
      display:flex; align-items:center; justify-content:center;
      color:${text};
      font-size:12px;
      font-weight:800;
      font-family:'DM Sans',system-ui,sans-serif;
      letter-spacing:0.02em;
      box-shadow:0 3px 10px rgba(0,0,0,0.25), 0 1px 3px rgba(0,0,0,0.15);
      cursor:pointer;
      position:relative;
      white-space:nowrap;
      transition:transform 0.15s ease;
    ">${label}
      <div style="
        position:absolute; bottom:-7px; left:50%; transform:translateX(-50%);
        width:0; height:0;
        border-left:6px solid transparent;
        border-right:6px solid transparent;
        border-top:7px solid ${border};
      "></div>
      <div style="
        position:absolute; bottom:-5px; left:50%; transform:translateX(-50%);
        width:0; height:0;
        border-left:5px solid transparent;
        border-right:5px solid transparent;
        border-top:6px solid ${bg};
      "></div>
    </div>`;

  return L.divIcon({
    html,
    className: '',           // remove Leaflet's default white box
    iconSize:  [w, h + 7],
    iconAnchor:[w / 2, h + 7],
    popupAnchor:[0, -(h + 10)],
  });
}

/* ─────────────────────────────────────────────────────────
   MapMarker component
───────────────────────────────────────────────────────── */
export default function MapMarker({ city }) {
  const name = city.name;
  const latitude = city.centerLocation.latitude;
  const longitude = city.centerLocation.longitude;
  const totalSites = city.totalSites || 0;

  const markerRef = useRef(null);

  const icon = createBadgeIcon(totalSites);

  return (
    <Marker
      ref={markerRef}
      position={[latitude, longitude]}
      icon={icon}
    >
      {/* Tooltip — visible on hover without clicking */}
      <Tooltip
        direction="top"
        offset={[0, -10]}
        opacity={1}
        permanent={false}
      >
        <span style={{ fontFamily: "'DM Sans',system-ui,sans-serif", fontSize: 12, fontWeight: 600 }}>
          {name}
        </span>
      </Tooltip>

      {/* Popup — opens on click */}
      <Popup
        minWidth={180}
        maxWidth={220}
        className="rafeeq-popup"
      >
        <div style={{
          fontFamily: "'DM Sans', system-ui, sans-serif",
          padding: '4px 2px',
        }}>
          {/* City name */}
          <div style={{
            fontSize: 15,
            fontWeight: 800,
            color: '#1c1814',
            marginBottom: 8,
            borderBottom: '1px solid #e5ddd0',
            paddingBottom: 8,
            display: 'flex',
            alignItems: 'center',
            gap: 6,
          }}>
            <span style={{ fontSize: 16 }}>📍</span>
            {name}
          </div>

          {/* Sites count */}
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
            <span style={{ fontSize: 12, color: '#9c8b7a', fontWeight: 500 }}>
              Tourist Sites
            </span>
            <span style={{
              fontSize: 18,
              fontWeight: 800,
              color: '#7c572d',
              fontVariantNumeric: 'tabular-nums',
            }}>
              {totalSites}
            </span>
          </div>

          {/* Visual bar proportional to count (max assumed ~500) */}
          <div style={{
            marginTop: 10,
            height: 5,
            background: '#e5ddd0',
            borderRadius: 3,
            overflow: 'hidden',
          }}>
            <div style={{
              height: '100%',
              width: `${Math.min((totalSites / 500) * 100, 100)}%`,
              background: 'linear-gradient(90deg, #7c572d, #d4a574)',
              borderRadius: 3,
              transition: 'width 0.6s ease',
            }} />
          </div>

          {/* Future: link to filtered sites list */}
          {/* <button onClick={() => navigate(`/sites?city=${name}`)}>View Sites</button> */}
        </div>
      </Popup>
    </Marker>
  );
}
