import api from './axiosConfig'

export const addCity = async (cityData) => {
    const response = await api.post('/admins/cities', cityData, {
        headers: {
            'Content-Type': 'multipart/form-data',
        },
    })

    return response.data
}

export const getCities = async () => {
    const response = await api.get('/cities')
    return response.data
}

export const getCity = async (cityId) => {
    const response = await api.get(`/admins/cities/${cityId}`)
    return response.data
}

export const updateCity = async (cityId, cityData) => {
    const response = await api.put(`/admins/cities/${cityId}`, cityData, {
        headers: {
            'Content-Type': 'multipart/form-data',
        },
    })
    return response.data
}

export const deleteCity = async (cityId) => {
    const response = await api.delete(`/admins/cities/${cityId}`)
    return response.data
}

export const updateLocalizedContent = async (cityId, contentData) => {
    const response = await api.put(`/admins/cities/${cityId}/localized-contents`, contentData, {
        headers: {
            'Content-Type': 'application/json',
        },
    })
    return response.data
}

