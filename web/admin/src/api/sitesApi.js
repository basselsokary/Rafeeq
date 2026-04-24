import api from './axiosConfig'

export const citySites = async (cityId) => {
    const response = await api.get(`/sites?city=${cityId}`)
    return response.data
}

export const getSite = async (siteId) => {
    const response = await api.get(`/admin/sites/${siteId}`)
    return response.data
}

export const addSite = async (siteData) => {
    const response = await api.post('/admin/sites', siteData)
    return response.data
}

export const updateSite = async (siteId, siteData) => {
    const response = await api.put(`/admin/sites/${siteId}`, siteData)
    return response.data
}

export const deleteSite = async (siteId) => {
    const response = await api.delete(`/admin/sites/${siteId}`)
    return response.data
}

