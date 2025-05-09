// src/services/api.ts
import axios, { InternalAxiosRequestConfig, AxiosRequestHeaders } from 'axios'
import { getToken } from '@/utils/token'

const api = axios.create({
  // Используем относительный путь, чтобы прокси Vite ловил все запросы
  baseURL: import.meta.env.VITE_API_BASE_URL ?? '/api',
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true // если нужны куки или авторизация через заголовки
})

api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = getToken()
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
