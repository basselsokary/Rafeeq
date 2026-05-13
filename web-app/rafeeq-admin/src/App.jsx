import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ToastProvider } from './components/common/Toast';
import SitesPage from './pages/sites/SitesPage';
import SiteDetailPage from './pages/sites/SiteDetailPage';
import AttractionsPage from './pages/attractions/AttractionsPage';
import AttractionDetailPage from './pages/attractions/AttractionDetailPage';
import CitiesPage from './pages/cities/CitiesPage';
import CityDetailPage from './pages/cities/CityDetailPage';
import SponsorsPage from './pages/sponsors/SponsorsPage';
import SponsorDetailPage from './pages/sponsors/SponsorDetailPage';
import DashboardPage from './pages/dashboard/DashboardPage';
import './styles/global.css';
import 'leaflet/dist/leaflet.css';
import './styles/map.css';
import LoginPage from './pages/auth/LoginPage';
import RequireAuth from './components/auth/RequireAuth';

export default function App() {
  return (
    <BrowserRouter>
      <ToastProvider>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route element={<RequireAuth />}>
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/sites" element={<SitesPage />} />
            <Route path="/sites/:id" element={<SiteDetailPage />} />
            <Route path="/attractions" element={<AttractionsPage />} />
            <Route path="/attractions/:id" element={<AttractionDetailPage />} />
            <Route path="/cities" element={<CitiesPage />} />
            <Route path="/cities/:id" element={<CityDetailPage />} />
            <Route path="/sponsors"     element={<SponsorsPage />} />
            <Route path="/sponsors/:id" element={<SponsorDetailPage />} />
          </Route>
        </Routes>
      </ToastProvider>
    </BrowserRouter>
  );
}
