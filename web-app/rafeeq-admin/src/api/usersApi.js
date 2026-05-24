import api from './axiosInstance';

const BASE_URL = '/api/admins/users';

// ============================================
// USER LISTING & SEARCH
// ============================================

export const getUsers = async (params = {}) => {
  const response = await api.get(BASE_URL, { params });
  return response.data;
};

export const getUserById = async (userId) => {
  const response = await api.get(`${BASE_URL}/${userId}`);
  return response.data;
};

export const searchUsers = async (searchTerm, limit = 10) => {
  const response = await api.get(`${BASE_URL}/search`, {
    params: { searchTerm, limit }
  });
  return response.data;
};

// ============================================
// USER CREATION
// ============================================

export const createModerator = async (data) => {
  const response = await api.post(`${BASE_URL}/moderators`, data);
  return response.data;
};

// ============================================
// ROLE MANAGEMENT
// ============================================

export const promoteToAdmin = async (userId) => {
  const response = await api.post(`${BASE_URL}/${userId}/promote-to-admin`);
  return response.data;
};

export const demoteToModerator = async (userId) => {
  const response = await api.post(`${BASE_URL}/${userId}/demote-to-moderator`);
  return response.data;
};

export const updateUserRoles = async (userId, roles, reason) => {
  const response = await api.put(`${BASE_URL}/${userId}/roles`, {
    userId,
    roles,
    reason
  });
  return response.data;
};

// ============================================
// ACCOUNT STATUS
// ============================================

export const lockUserAccount = async (userId, reason, lockUntil = null) => {
  const response = await api.post(`${BASE_URL}/${userId}/lock`, {
    userId,
    reason,
    lockUntil
  });
  return response.data;
};

export const unlockUserAccount = async (userId) => {
  const response = await api.post(`${BASE_URL}/${userId}/unlock`);
  return response.data;
};

export const suspendUserAccount = async (userId, reason, suspendUntil, notifyUser = true) => {
  const response = await api.post(`${BASE_URL}/${userId}/suspend`, {
    userId,
    reason,
    suspendUntil,
    notifyUser
  });
  return response.data;
};

export const reactivateUserAccount = async (userId) => {
  const response = await api.post(`${BASE_URL}/${userId}/reactivate`);
  return response.data;
};

// ============================================
// PASSWORD MANAGEMENT
// ============================================

export const resetUserPassword = async (userId) => {
  const response = await api.post(`${BASE_URL}/${userId}/reset-password`);
  return response.data;
};

export const requirePasswordChange = async (userId) => {
  const response = await api.post(`${BASE_URL}/${userId}/require-password-change`);
  return response.data;
};

// ============================================
// EMAIL VERIFICATION
// ============================================

export const confirmUserEmail = async (userId) => {
  const response = await api.post(`${BASE_URL}/${userId}/confirm-email`);
  return response.data;
};

export const resendVerificationEmail = async (userId) => {
  const response = await api.post(`${BASE_URL}/${userId}/resend-verification-email`);
  return response.data;
};

// ============================================
// ACTIVITY & AUDIT
// ============================================

export const getUserActivity = async (userId, page = 1, pageSize = 20) => {
  const response = await api.get(`${BASE_URL}/${userId}/activity`, {
    params: { page, pageSize }
  });
  return response.data;
};

export const getUserLoginHistory = async (userId, page = 1, pageSize = 20) => {
  const response = await api.get(`${BASE_URL}/${userId}/login-history`, {
    params: { page, pageSize }
  });
  return response.data;
};

// ============================================
// STATISTICS
// ============================================

export const getUserStatistics = async () => {
  const response = await api.get(`${BASE_URL}/statistics`);
  return response.data;
};

export const exportUsers = async (params = {}) => {
  const response = await api.get(`${BASE_URL}/export`, {
    params,
    responseType: 'blob'
  });
  return response.data;
};

// ============================================
// USER DELETION
// ============================================

export const deleteUser = async (userId, reason, notifyUser = true) => {
  const response = await api.delete(`${BASE_URL}/${userId}`, {
    data: { userId, reason, notifyUser }
  });
  return response.data;
};

export const permanentlyDeleteUser = async (userId, reason, confirmDeletion = true, gdprRequestReference = null) => {
  const response = await api.delete(`${BASE_URL}/${userId}/permanent`, {
    data: { userId, reason, confirmDeletion, gdprRequestReference }
  });
  return response.data;
};
