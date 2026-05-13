import api, { refreshTokens } from './axiosInstance';
import { clearTokenExpirations, persistTokenExpirations } from '../utils/tokenExpiry';

const BASE = import.meta.env.VITE_AUTH_BASE_PATH || 'api/auth';

export const login = (body) =>
  api.post(`${BASE}/login`, body).then((res) => {
    persistTokenExpirations(res?.data?.value ?? res?.data);
    return res;
  });

export const logout = () =>
  api.post(`${BASE}/logout`).finally(() => {
    clearTokenExpirations();
  });

export const refresh = () =>
  refreshTokens().then((res) => {
    persistTokenExpirations(res?.data?.value ?? res?.data);
    return res;
  });
