import axios from 'axios';
import { setAuthState } from '../auth/authState';
import { clearTokenExpirations, persistTokenExpirations } from '../utils/tokenExpiry';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5143';

// Dedicated client for refresh (no interceptors to avoid loops)
const refreshClient = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
    'Accept-Language': 'en',
  },
});

let refreshPromise = null;

export const refreshTokens = () => {
  const authBase = import.meta.env.VITE_AUTH_BASE_PATH || '/api/auth';
  return refreshClient.post(`${authBase}/web/refresh`).then((res) => {
    persistTokenExpirations(res?.data?.value ?? res?.data);
    return res;
  });
};

const api = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true, // send backend-set cookies on every request
  headers: {
    'Content-Type': 'application/json',
    'Accept-Language': 'en',
  },
});

// ── Global error handling (cookie auth + refresh) ──
api.interceptors.response.use(
  (res) => res,
  async (err) => {
    const originalRequest = err?.config;
    const status = err?.response?.status;

    if (status !== 401) return Promise.reject(err);
    if (!originalRequest) return Promise.reject(err);
    if (originalRequest._retry) return Promise.reject(err);
    if (originalRequest.skipAuthRefresh) return Promise.reject(err);

    originalRequest._retry = true;

    try {
      if (!refreshPromise) {
        refreshPromise = refreshTokens();
      }

      await refreshPromise;
      refreshPromise = null;

      setAuthState({ isAuthenticated: true, hasChecked: true });

      return api(originalRequest);
    } catch (refreshErr) {
      console.error('Token refresh failed:', refreshErr);
      refreshPromise = null;

      setAuthState({ isAuthenticated: false, hasChecked: true });
      clearTokenExpirations();
      window.location.href = '/login';
      return Promise.reject(refreshErr);
    }
  }
);

export default api;