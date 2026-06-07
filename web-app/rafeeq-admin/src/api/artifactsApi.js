import api from './axiosInstance';

const BASE = '/api/admins/artifacts';

// ─── Dashboard ─────────────────────────────────────────────────────

export const getDashboardStats = () =>
  api.get(`${BASE}/dashboard`);

// ─── Core CRUD ─────────────────────────────────────────────────────

export const getArtifacts = (params = {}) =>
  api.get(BASE, { params });

export const getArtifactsBySiteId = (siteId, params = {}) =>
  api.get(`${BASE}/site/${siteId}`, { params });

export const getArtifactById = (id) =>
  api.get(`${BASE}/${id}`);

export const createArtifact = (body) =>
  api.post(BASE, body);

export const updateArtifact = (id, body) =>
  api.put(`${BASE}/${id}`, body);

export const deleteArtifact = (id) =>
  api.delete(`${BASE}/${id}`);

// ─── Images ────────────────────────────────────────────────────────

export const getImages = (artifactId) =>
  api.get(`${BASE}/${artifactId}/images`);

export const uploadImages = (artifactId, formData) =>
  api.post(`${BASE}/${artifactId}/images`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

export const deleteImages = (artifactId, body) =>
  api.delete(`${BASE}/${artifactId}/images`, { data: body });

// ─── Localized Contents ────────────────────────────────────────────

export const getLocalizedContents = (artifactId) =>
  api.get(`${BASE}/${artifactId}/localized-contents`);

export const addLocalizedContents = (artifactId, body) =>
  api.post(`${BASE}/${artifactId}/localized-contents`, body);

export const updateLocalizedContents = (artifactId, body) =>
  api.put(`${BASE}/${artifactId}/localized-contents`, body);
