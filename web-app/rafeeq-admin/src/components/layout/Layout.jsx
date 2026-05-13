import React from 'react';
import Sidebar from './Sidebar';

export default function Layout({ children }) {
  return (
    <div style={{ display: 'flex', minHeight: '100vh' }}>
      <Sidebar />
      <main style={{
        marginLeft: 'var(--sidebar-width)',
        flex: 1,
        padding: '32px',
        background: 'var(--bg-base)',
        minHeight: '100vh',
        overflow: 'auto',
      }}>
        {children}
      </main>
    </div>
  );
}
