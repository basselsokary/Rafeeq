import api from './axiosInstance';

const BASE = '/api/admins/attractions';

// ─── Dashboard ─────────────────────────────────────────────────────

export const getDashboardStats = () =>
  api.get(`${BASE}/dashboard`);

// ─── Core CRUD ─────────────────────────────────────────────────────

export const getAttractions = (params = {}) =>
  api.get(BASE, { params });

export const getAttractionsBySiteId = (siteId, params = {}) =>
  api.get(`${BASE}/site/${siteId}`, { params });

export const getAttractionById = (id) =>
  api.get(`${BASE}/${id}`);

export const createAttraction = (body) =>
  api.post(BASE, body);

export const updateAttraction = (id, body) =>
  api.put(`${BASE}/${id}`, body);

export const deleteAttraction = (id) =>
  api.delete(`${BASE}/${id}`);

export const markAsFeatured = (id, body) =>
  api.patch(`${BASE}/${id}`, body);

// ─── Images ────────────────────────────────────────────────────────

export const getImages = (attractionId) =>
  api.get(`${BASE}/${attractionId}/images`);

export const getImageById = (attractionId, imageId) =>
  api.get(`${BASE}/${attractionId}/images/${imageId}`);

export const uploadImages = (attractionId, formData) =>
  api.post(`${BASE}/${attractionId}/images`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

export const deleteImages = (attractionId, body) =>
  api.delete(`${BASE}/${attractionId}/images`, { data: body });

// ─── Localized Contents ────────────────────────────────────────────

export const getLocalizedContents = (attractionId) =>
  api.get(`${BASE}/${attractionId}/localized-contents`);

export const addLocalizedContents = (attractionId, body) =>
  api.post(`${BASE}/${attractionId}/localized-contents`, body);

export const updateLocalizedContents = (attractionId, body) =>
  api.put(`${BASE}/${attractionId}/localized-contents`, body);
