import api from './api'
import type {LoginRequest, RegisterRequest} from '@/types/auth'

export const login = async (data: LoginRequest): Promise<string | null> => {
    try {
        const response = await api.post('/api/auth/login', data)
        return response.data.token // ожидается, что backend возвращает токен
    } catch (error) {
        console.error('Ошибка при входе:', error)
        return null
    }
}

export const register = async (data: RegisterRequest): Promise<string | null> => {
    try {
        const response = await api.post('/api/auth/register', data)
        return response.data.token
    } catch (error) {
        console.error('Ошибка при регистрации:', error)
        return null
    }
}
