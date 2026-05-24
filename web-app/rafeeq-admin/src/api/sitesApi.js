import api from './axiosInstance';

const BASE_PUBLIC = '/api/sites';
const BASE = '/api/admins/sites';


// ─── Dashboard ─────────────────────────────────────────────────────

export const getSiteDashboardStats = () =>
  api.get(`${BASE}/dashboard`);

// ─── Core CRUD ─────────────────────────────────────────────────────────────

export const getSites = (params = {}) =>
  api.get(BASE_PUBLIC, { params });

export const searchSites = (params = {}) =>
  api.get(`${BASE}/search`, { params }); // For simple dropdown lists, not paginated. '?q=string'

export const getSiteById = (id) =>
  api.get(`${BASE}/${id}`);

export const createSite = (body) =>
  api.post(BASE, body);

export const updateSite = (id, body) =>
  api.put(`${BASE}/${id}`, body);

export const deleteSite = (id) =>
  api.delete(`${BASE}/${id}`);

export const updateSiteStatus = (id, body) =>
  api.patch(`${BASE}/${id}`, body);

// ─── Localized Contents ────────────────────────────────────────────────────

export const getLocalizedContents = (siteId) =>
  api.get(`${BASE}/${siteId}/localized-contents`);

export const getLocalizedContentById = (siteId, contentId) =>
  api.get(`${BASE}/${siteId}/localized-contents/${contentId}`);

export const addLocalizedContents = (siteId, body) =>
  api.post(`${BASE}/${siteId}/localized-contents`, body);

export const updateLocalizedContents = (siteId, body) =>
  api.put(`${BASE}/${siteId}/localized-contents`, body);

// ─── Facilities ────────────────────────────────────────────────────────────

export const getFacilities = (siteId) =>
  api.get(`${BASE}/${siteId}/facilities`);

export const addFacilities = (siteId, body) =>
  api.post(`${BASE}/${siteId}/facilities`, body);

export const deleteFacilities = (siteId, body) =>
  api.delete(`${BASE}/${siteId}/facilities`, { data: body });

// ─── Images ────────────────────────────────────────────────────────────────

export const getImages = (siteId) =>
  api.get(`${BASE}/${siteId}/images`);

export const getImageById = (siteId, imageId) =>
  api.get(`${BASE}/${siteId}/images/${imageId}`);

export const uploadImages = (siteId, formData) =>
  api.post(`${BASE}/${siteId}/images`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

export const deleteImages = (siteId, body) =>
  api.delete(`${BASE}/${siteId}/images`, { data: body });

export const setMainImage = (siteId, imageId) =>
  api.patch(`${BASE}/${siteId}/set-main-image/${imageId}`);

// ─── Opening Hours ─────────────────────────────────────────────────────────

export const addOpeningHours = (siteId, body) =>
  api.post(`${BASE}/${siteId}/opening-hours`, body);

export const deleteOpeningHours = (siteId, body) =>
  api.delete(`${BASE}/${siteId}/opening-hours`, { data: body });

export const getOpeningHours = (siteId) =>
  api.get(`${BASE}/${siteId}/opening-hours`);

// ─── Nearest Transportation ────────────────────────────────────────────────

export const getNearestTransportation = (siteId) =>
  api.get(`${BASE}/${siteId}/nearest-transportation`);

export const getTransportationById = (siteId, transportationId) =>
  api.get(`${BASE}/${siteId}/nearest-transportation/${transportationId}`);
