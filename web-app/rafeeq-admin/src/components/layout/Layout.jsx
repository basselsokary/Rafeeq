import React from 'react';
import Sidebar from './Sidebar';
import Topbar from './Topbar';
import { useSidebar } from '../../context/SidebarContext';

export default function Layout({ children }) {
  const { collapsed } = useSidebar();
  return (
    <div style={{ display: 'flex', minHeight: '100vh', background: 'var(--background)' }}>
      <Sidebar />
      <div style={{ 
        marginLeft: collapsed ? 'var(--sidebar-width-collapsed)' : 'var(--sidebar-width)',
        transition: 'margin-left 0.2s ease',
        flex: 1, 
        display: 'flex', 
        flexDirection: 'column', 
        minWidth: 0 }}>
        <Topbar />
        <main style={{ flex: 1, overflowY: 'auto' }}>
          {children}
        </main>
      </div>
    </div>
  );
}
