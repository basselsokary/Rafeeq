import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ToastProvider } from './components/common/Toast';
import DashboardPage from './pages/dashboard/DashboardPage';
import SitesPage from './pages/sites/SitesPage';
import SiteDetailPage from './pages/sites/SiteDetailPage';
import AttractionsPage from './pages/attractions/AttractionsPage';
import AttractionDetailPage from './pages/attractions/AttractionDetailPage';
import ArtifactsPage from './pages/artifacts/ArtifactsPage';
import ArtifactDetailPage from './pages/artifacts/ArtifactDetailPage';
import CitiesPage from './pages/cities/CitiesPage';
import CityDetailPage from './pages/cities/CityDetailPage';
import SponsorsPage from './pages/sponsors/SponsorsPage';
import SponsorDetailPage from './pages/sponsors/SponsorDetailPage';
import UsersPage from './pages/users/UsersPage';
import UserDetailPage from './pages/users/UserDetailPage';
import './styles/global.css';
import 'leaflet/dist/leaflet.css';
import './styles/map.css';
import LoginPage from './pages/auth/LoginPage';
import RequireAuth from './components/auth/RequireAuth';
import Layout from './components/layout/Layout';
import ProtectedRoute from './components/auth/ProtectedRoute';
import UnauthorizedPage from './pages/auth/UnauthorizedPage';
import { ROLES } from './utils/constants';

export default function App() {
  return (
    <BrowserRouter>
      <ToastProvider>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/unauthorized" element={<UnauthorizedPage />} />
          <Route element={<RequireAuth />}>
            <Route path="/" element={<Navigate to="/dashboard" replace />} />

            <Route path="/dashboard" element={
              <ProtectedRoute>
                <Layout><DashboardPage /></Layout>
              </ProtectedRoute>
            } />

            <Route path="/sites" element={
              <ProtectedRoute>
                <Layout><SitesPage /></Layout>
              </ProtectedRoute>
            } />

            <Route path="/sites/:id" element={
              <ProtectedRoute>
                <Layout><SiteDetailPage /></Layout>
              </ProtectedRoute>
            } />

            <Route path="/artifacts" element={
              <ProtectedRoute>
                <Layout><ArtifactsPage /></Layout>
              </ProtectedRoute>
            } />

            <Route path="/artifacts/:id" element={
              <ProtectedRoute>
                <Layout><ArtifactDetailPage /></Layout>
              </ProtectedRoute>
            } />

            <Route path="/attractions" element={
              <ProtectedRoute>
                <Layout><AttractionsPage /></Layout>
              </ProtectedRoute>
            } />

            <Route path="/attractions/:id" element={
              <ProtectedRoute>
                <Layout><AttractionDetailPage /></Layout>
              </ProtectedRoute>
            } />

            <Route path="/cities" element={
              <ProtectedRoute>
                <Layout><CitiesPage /></Layout>
              </ProtectedRoute>
            } />

            <Route path="/cities/:id" element={
              <ProtectedRoute>
                <Layout><CityDetailPage /></Layout>
              </ProtectedRoute>
            } />

            <Route path="/sponsors" element={
              <ProtectedRoute roles={[ROLES.superAdmin, ROLES.admin]}>
                <Layout><SponsorsPage /></Layout>
              </ProtectedRoute>
            } />

            <Route path="/sponsors/:id" element={
              <ProtectedRoute roles={[ROLES.superAdmin, ROLES.admin]}>
                <Layout><SponsorDetailPage /></Layout>
              </ProtectedRoute>
            } />

            <Route path="/users" element={
              <ProtectedRoute roles={[ROLES.superAdmin, ROLES.admin]}>
                <Layout><UsersPage /></Layout>
              </ProtectedRoute>
            } />

            <Route path="/users/:id" element={
              <ProtectedRoute roles={[ROLES.superAdmin, ROLES.admin]}>
                <Layout><UserDetailPage /></Layout>
              </ProtectedRoute>
            } />
          </Route>
        </Routes>
      </ToastProvider>
    </BrowserRouter>
  );
}
