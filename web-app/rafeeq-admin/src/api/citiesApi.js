import api from './axiosInstance';

const BASE_PUBLIC = '/api/cities';
const BASE = '/api/admins/cities';

// ─── Public ────────────────────────────────────────────────────────

export const getCities = () =>
  api.get(BASE_PUBLIC);

export const getCitySummaries = () =>
  api.get(`${BASE_PUBLIC}/summaries`);

// ─── Dashboard ─────────────────────────────────────────────────────

export const getDashboardStats = () =>
  api.get(`${BASE}/dashboard`);

// ─── Core CRUD ─────────────────────────────────────────────────────

export const getCityById = (id) =>
  api.get(`${BASE}/${id}`);

export const createCity = (formData) =>
  api.post(BASE, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

export const updateCity = (id, formData) =>
  api.put(`${BASE}/${id}`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

export const deleteCity = (id) =>
  api.delete(`${BASE}/${id}`);

export const updateCityImage = (id, formData) =>
  api.patch(`${BASE}/${id}/image`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

// ─── Localized Contents ────────────────────────────────────────────

export const getLocalizedContents = (cityId) =>
  api.get(`${BASE}/${cityId}/localized-contents`);

export const addLocalizedContents = (cityId, body) =>
  api.post(`${BASE}/${cityId}/localized-contents`, body);

export const updateLocalizedContents = (cityId, body) =>
  api.put(`${BASE}/${cityId}/localized-contents`, body);