// frontend/src/services/auth.service.ts
import api from './api'
import type { LoginRequest, RegisterRequest, AuthResponse } from '@/types/auth'

export async function login(data: LoginRequest): Promise<string> {
  const { data: auth } = await api.post<AuthResponse>('/api/auth/login', data)
  console.log('⚙ auth.service login response.data:', auth)   // ← добавь эту строку для отладки
  return auth.token
}

export async function register(data: RegisterRequest): Promise<string> {
  const { data: auth } = await api.post<AuthResponse>('/api/auth/register', data)
  console.log('⚙ auth.service register response.data:', auth)
  return auth.token
}
