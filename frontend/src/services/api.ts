import axios from 'axios'
import { getToken } from '@/utils/token'

const api = axios.create({
  baseURL: 'https://localhost:5003',   // убрал «/api» и теперь https
})

api.interceptors.request.use(config => {
  const token = getToken()
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export default api
