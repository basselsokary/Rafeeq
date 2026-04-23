import axios from 'axios';

const baseURL = ''

const api = axios.create({
    baseURL: baseURL,
    headers: {
        'Content-Type': 'application/json',
    },
})

export default api;