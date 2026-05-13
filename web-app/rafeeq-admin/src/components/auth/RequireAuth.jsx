import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import Spinner from '../common/Spinner';
import useAuth from '../../hooks/useAuth';

export default function RequireAuth() {
  const { isAuthenticated, loading } = useAuth({ restoreOnMount: true });

  if (loading) {
    return (
      <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <Spinner />
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}
