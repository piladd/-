import axios from 'axios'
import {getToken} from '@/utils/token'

const api = axios.create({
    baseURL: '/api'
})

api.interceptors.request.use(config => {
    const token = getToken()
    if (token) {
        config.headers.Authorization = `Bearer ${token}`
    }
    return config
})

export default api
