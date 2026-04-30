import axios from 'axios';

const baseURL = 'https://ggtmd2s8-5143.euw.devtunnels.ms/api/'

const api = axios.create({
    baseURL: baseURL
})

export default api;