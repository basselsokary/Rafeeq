import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { getMe } from '../api/authApi';
import { isAccessTokenExpired } from '../utils/tokenExpiry';
import { subscribeAuth, getAuthState } from '../auth/authState';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user,    setUser]    = useState(null);
  const [loading, setLoading] = useState(true);
  
  const fetchUser = useCallback(async () => {
    try {
      if (isAccessTokenExpired()) {
        setUser(null);
        return;
      }
      
      const res = await getMe();
      setUser(res.data?.value ?? res.data);
    } catch {
      setUser(null);
    } finally {
      setLoading(false);
    }
  }, []);

  // Run once on app mount
  // useEffect(() => { fetchUser(); }, [fetchUser]);
  // Subscribe to auth state changes and fetch the user only when auth flips on.
  useEffect(() => {
    // subscribe to auth state changes (login/logout)
    const unsub = subscribeAuth((s) => {
      // when becoming authenticated, re-fetch the user and show loading
      if (s.isAuthenticated) {
        setLoading(true);
        fetchUser();
        return;
      }

      // when logged out elsewhere, clear user immediately
      setUser(null);
      setLoading(false);
    });

    // if auth was already set before this provider subscribed, ensure we fetch
    try {
      const initial = getAuthState();
      if (initial.isAuthenticated) {
        setLoading(true);
        fetchUser();
      }
    } catch {}

    return () => unsub();
  }, [fetchUser]);

  const hasRole = useCallback((role) => {
    return user?.roles?.includes(role) ?? false;
  }, [user]);

  const hasAnyRole = useCallback((roles = []) => {
    return roles.some(r => user?.roles?.includes(r));
  }, [user]);

  const logout = useCallback(() => {
    setUser(null);
    // Optionally call your logout endpoint here:
    // api.post('/api/auth/logout').finally(() => {});
  }, []);

  return (
    <AuthContext.Provider value={{ user, loading, logout, refetch: fetchUser, hasRole, hasAnyRole }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);