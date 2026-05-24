import api from './axiosInstance';

const BASE = '/api/admins/dashboard';


export const getDashboardStats = () =>
  api.get(`${BASE}/stats`);
