import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { logout } from '../../api/authApi';

const NAV = [
  { label: 'Dashboard',           to: '/dashboard',     icon: 'M3 3h7v7H3V3zm11 0h7v7h-7V3zM3 14h7v7H3v-7zm11 7v-2h2v-2h-2v-2h2v2h2v-2h2v2h-2v2h2v2h-4v-2h-2z' },
  { label: 'Cities Management',   to: '/cities',        icon: 'M3 21h18M5 21V7l8-4v18M19 21V11l-6-4M9 9h1M9 13h1M9 17h1' },
  { label: 'Sites Management',    to: '/sites',         icon: 'M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2V9z M9 22V12h6v10' },
  { label: 'Attractions Management',         to: '/attractions',   icon: 'M12 22s8-4.5 8-11.8A8 8 0 0012 2a8 8 0 00-8 8.2C4 17.5 12 22 12 22z M12 13a3 3 0 100-6 3 3 0 000 6z' },
  { label: 'Sponsors & Offers',   to: '/sponsors',      icon: 'M20.84 4.61a5.5 5.5 0 00-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 00-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 000-7.78z' },
  { label: 'Users & Tourists',    to: '/users',         icon: 'M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2 M9 7a4 4 0 100 8 4 4 0 000-8z M23 21v-2a4 4 0 00-3-3.87 M16 3.13a4 4 0 010 7.75' },
  // { label: 'Reviews & Moderation',to: '/reviews',    icon: 'M21 15a2 2 0 01-2 2H7l-4 4V5a2 2 0 012-2h14a2 2 0 012 2z' },
  // { label: 'Trips & Itineraries', to: '/trips',      icon: 'M12 22s8-4.5 8-11.8A8 8 0 0012 2a8 8 0 00-8 8.2C4 17.5 12 22 12 22z M12 13a3 3 0 100-6 3 3 0 000 6z' },
];

function NavIcon({ d }) {
  return (
    <svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentCo r" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round" style={{ flexShrink: 0 }}>
      {d.split(' M').map((segment, i) => (
        <path key={i} d={i === 0 ? segment : 'M' + segment} />
      ))}
    </svg>
  );
}

export default function Sidebar() {
  const navigate = useNavigate();

  const handleSignOut = async () => {
    try {
      await logout();
    } finally {
      navigate('/login', { replace: true });
    }
  };

  return (
    <aside style={{
      width: 'var(--sidebar-width)',
      background: 'var(--surface-container-lowest)',
      borderRight: '1px solid rgba(212,196,183,.25)',
      height: '100vh', position: 'fixed', left: 0, top: 0,
      display: 'flex', flexDirection: 'column', zIndex: 100,
    }}>
      {/* Logo */}
      <div style={{ padding: '20px 20px 16px', borderBottom: '1px solid rgba(212,196,183,.2)' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
          <div style={{
            width: 36, height: 36, borderRadius: 10, flexShrink: 0,
            background: 'rgba(124,87,45,0.1)',
            display: 'flex', alignItems: 'center', justifyContent: 'center',
          }}>
            <svg width="18" height="18" viewBox="0 0 24 24" fill="var(--primary)">
              <polygon points="12 2 2 22 22 22"/>
            </svg>
          </div>
          <div>
            <div style={{ fontFamily: 'var(--font-display)', fontSize: 16, fontWeight: 700, color: 'var(--text)', lineHeight: 1.1 }}>
              Rafeeq
            </div>
            <div style={{ fontSize: 9, color: 'var(--text-muted)', letterSpacing: '0.1em', textTransform: 'uppercase', fontWeight: 700 }}>
              Egypt's Premier Platform
            </div>
          </div>
        </div>
      </div>

      {/* Nav */}
      <nav style={{ padding: '12px 12px', flex: 1, overflowY: 'auto' }}>
        {NAV.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            style={({ isActive }) => ({
              display: 'flex', alignItems: 'center', gap: 10,
              padding: '9px 12px',
              borderRadius: isActive ? '8px 0 0 8px' : 8,
              textDecoration: 'none',
              fontSize: 13, fontWeight: isActive ? 700 : 500,
              color: isActive ? 'var(--primary)' : 'var(--text-2)',
              background: isActive ? 'rgba(124,87,45,0.07)' : 'transparent',
              borderRight: isActive ? '3px solid var(--primary)' : '3px solid transparent',
              marginBottom: 1,
              transition: 'all 0.15s',
            })}
          >
            {({ isActive }) => (
              <>
                <span style={{ color: isActive ? 'var(--primary)' : 'var(--outline)', display: 'flex' }}>
                  <NavIcon d={item.icon} />
                </span>
                {item.label}
              </>
            )}
          </NavLink>
        ))}
      </nav>

      {/* Footer */}
      <div style={{ padding: '12px 16px 20px', borderTop: '1px solid rgba(212,196,183,.2)' }}>
        <div style={{ display: 'flex', gap: 2, marginBottom: 14 }}>
          {[['Help Center', 'M12 22c5.523 0 10-4.477 10-10S17.523 2 12 2 2 6.477 2 12s4.477 10 10 10z M12 16v-4 M12 8h.01'],
            ['Sign Out', 'M9 21H5a2 2 0 01-2-2V5a2 2 0 012-2h4 M16 17l5-5-5-5 M21 12H9']
          ].map(([label, d]) => (
            <button key={label} style={{
              flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 5,
              background: 'none', border: 'none', cursor: 'pointer',
              color: 'var(--text-muted)', fontSize: 11, fontWeight: 500, padding: '6px 4px',
              borderRadius: 8, transition: 'all .15s',
            }}
              onMouseOver={e => { e.currentTarget.style.background = 'var(--surface-container)'; e.currentTarget.style.color = 'var(--text-2)'; }}
              onMouseOut={e => { e.currentTarget.style.background = 'none'; e.currentTarget.style.color = 'var(--text-muted)'; }}
              onClick={label === 'Sign Out' ? handleSignOut : undefined}
            >
              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                {d.split(' M').map((seg, i) => <path key={i} d={i === 0 ? seg : 'M' + seg} />)}
              </svg>
              {label}
            </button>
          ))}
        </div>

        <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
          <div style={{
            width: 34, height: 34, borderRadius: '50%', flexShrink: 0,
            background: 'rgba(124,87,45,0.12)',
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            fontSize: 13, fontWeight: 700, color: 'var(--primary)',
            border: '2px solid rgba(124,87,45,0.2)',
          }}>R</div>
          <div>
            <div style={{ fontSize: 12, fontWeight: 700, color: 'var(--text)' }}>Rafeeq Admin</div>
            <div style={{ fontSize: 10, color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.06em' }}>Administrator</div>
          </div>
        </div>
      </div>
    </aside>
  );
}
