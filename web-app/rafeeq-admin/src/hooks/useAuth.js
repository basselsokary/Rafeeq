import { useCallback, useEffect, useState } from 'react';
import { refresh as refreshRequest, logout as logoutRequest } from '../api/authApi';
import { getAuthState, setAuthState, subscribeAuth } from '../auth/authState';
import { isAccessTokenExpired } from '../utils/tokenExpiry';

export default function useAuth({ restoreOnMount = true } = {}) {
  const [authState, setLocalAuthState] = useState(() => getAuthState());
  const [loading, setLoading] = useState(() => restoreOnMount && !getAuthState().hasChecked);

  useEffect(() => {
    return subscribeAuth((s) => setLocalAuthState(s));
  }, []);

  const restore = useCallback(async () => {
    const current = getAuthState();
    if (current.isAuthenticated && current.hasChecked) {
      setLoading(false);
      return;
    }

    // Tokens are stored in cookies. We persist only the access token expiry locally.
    // If access token is still valid, skip hitting refresh endpoint on mount.
    if (!isAccessTokenExpired()) {
      setAuthState({ isAuthenticated: true, hasChecked: true });
      setLoading(false);
      return;
    }

    setLoading(true);
    try {
      await refreshRequest();
      setAuthState({ isAuthenticated: true, hasChecked: true });
    } catch {
      setAuthState({ isAuthenticated: false, hasChecked: true });
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (restoreOnMount) restore();
  }, [restoreOnMount, restore]);

  const logout = useCallback(async () => {
    try {
      await logoutRequest();
    } finally {
      setAuthState({ isAuthenticated: false, hasChecked: true });
    }
  }, []);

  return {
    isAuthenticated: authState.isAuthenticated,
    loading,
    restore,
    logout,
    setIsAuthenticated: (v) => setAuthState({ isAuthenticated: Boolean(v), hasChecked: true }),
  };
}
