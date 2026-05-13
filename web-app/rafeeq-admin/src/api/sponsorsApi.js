import api from './axiosInstance';

const BASE        = '/api/admins/sponsors';
const BASE_PUBLIC = '/api/sponsors';

// ─── Dashboard ──────────────────────────────────────────────────────────────

export const getSponsorDashboard = () =>
  api.get(`${BASE}/dashboard`);

// ─── Core CRUD ──────────────────────────────────────────────────────────────

export const getSponsors = (params = {}) =>
  api.get(BASE_PUBLIC, { params });

export const getSponsorById = (id) =>
  api.get(`${BASE}/${id}`);

export const createSponsor = (body) =>
  api.post(BASE, body);

export const updateSponsor = (id, body) =>
  api.put(`${BASE}/${id}`, body);

export const deleteSponsor = (id) =>
  api.delete(`${BASE}/${id}`);

export const activateSponsor = (id, activate) =>
  api.patch(`${BASE}/${id}`, { activate });

// ─── Images ─────────────────────────────────────────────────────────────────

export const getSponsorImages = (id) =>
  api.get(`${BASE}/${id}/images`);

export const getSponsorImageById = (id, imageId) =>
  api.get(`${BASE}/${id}/images/${imageId}`);

export const uploadSponsorImages = (id, formData) =>
  api.post(`${BASE}/${id}/images`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

export const deleteSponsorImages = (id, imageIds) =>
  api.delete(`${BASE}/${id}/images`, { data: imageIds });

export const setMainSponsorImage = (id, imageId) =>
  api.patch(`${BASE}/${id}/set-main-image/${imageId}`);

// ─── Localized Contents ─────────────────────────────────────────────────────

export const getSponsorLocalizedContents = (id) =>
  api.get(`${BASE}/${id}/localized-contents`);

export const addSponsorLocalizedContents = (id, body) =>
  api.post(`${BASE}/${id}/localized-contents`, body);

export const updateSponsorLocalizedContents = (id, body) =>
  api.put(`${BASE}/${id}/localized-contents`, body);

// ─── Offers (read-only for now) ─────────────────────────────────────────────

export const getSponsorOffers = (id, params = {}) =>
  api.get(`${BASE}/${id}/offers`, { params });
