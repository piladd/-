// frontend/src/services/auth.service.ts
import api from './api'
import type { RegisterRequest, LoginRequest, AuthResponse } from '@/types/auth'
import {
  generateKeyPair,
  exportRsaPublicKey,
  storePrivateKey,
  exportRsaPrivateKey
} from '@/utils/crypto'

export async function register(data: { username: string; password: string }): Promise<AuthResponse> {
  // 1) Генерируем пару ключей
  const { publicKey: pk, privateKey: sk } = await generateKeyPair()
  const publicKeyB64  = await exportRsaPublicKey(pk)
  const privateKeyB64 = await exportRsaPrivateKey(sk)    // ← экспорт приватного в строку
  await storePrivateKey(sk)

  // 2) Отправляем оба ключа на бекенд
  const payload: RegisterRequest = {
    username:   data.username,
    password:   data.password,
    publicKey:  publicKeyB64,
    privateKey: privateKeyB64                           // ← новое поле
  }
  const { data: auth } = await api.post<AuthResponse>('/api/auth/register', payload)
  return auth
}

export async function login(data: LoginRequest): Promise<AuthResponse> {
  const { data: auth } = await api.post<AuthResponse>('/api/auth/login', data)
  return auth
}
