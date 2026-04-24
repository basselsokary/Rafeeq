import api from './axiosConfig'

export const addCity = async (cityData) => {
    const response = await api.post('/admin/cities', cityData)
    return response.data
}

export const getCities = async () => {
    const response = await api.get('/cities')
    return response.data
}

export const getCity = async (cityId) => {
    const response = await api.get(`/admin/cities/${cityId}`)
    return response.data
}

export const updateCity = async (cityId, cityData) => {
    const response = await api.put(`/admin/cities/${cityId}`, cityData)
    return response.data
}

export const deleteCity = async (cityId) => {
    const response = await api.delete(`/admin/cities/${cityId}`)
    return response.data
}

