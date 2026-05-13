import api from './axiosInstance';

const BASE = '/api/dashboard';


export const getDashboardStats = () =>
  api.get(`${BASE}/stats`);
