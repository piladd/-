// src/services/api.ts
import axios, { InternalAxiosRequestConfig, AxiosRequestHeaders } from 'axios'
import { getToken } from '@/utils/token'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:5003',
  headers: { 'Content-Type': 'application/json' }
})

api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = getToken()
    console.log('🔑 Token in interceptor:', token, typeof token)
    if (token) {
      config.headers = (config.headers ?? {}) as AxiosRequestHeaders
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  error => Promise.reject(error)
)

api.interceptors.response.use(
  response => response,
  error => {
    console.error('❌ API response error', error.response?.status, error.message)
    return Promise.reject(error)
  }
)

export default api
