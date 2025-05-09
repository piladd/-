// src/services/auth.service.ts
import api from './api'
import type { RegisterRequest, LoginRequest, AuthResponse } from '@/types/auth'
import { saveToken } from '@/utils/token'
import {
  generateKeyPair,
  exportRsaPublicKey,
  storePrivateKey,
  exportRsaPrivateKey
} from '@/utils/crypto'

/**
 * Регистрация нового пользователя:
 * 1) Генерируем пару ключей RSA
 * 2) Храним приватный ключ в IndexedDB (через storePrivateKey)
 * 3) Отправляем данные на бэкенд
 * 4) Сохраняем JWT и, если вернулся, приватный ключ
 */
export async function register(data: { username: string; password: string }): Promise<AuthResponse> {
  // 1) Генерация пары ключей
  const { publicKey: pk, privateKey: sk } = await generateKeyPair()
  const publicKeyB64  = await exportRsaPublicKey(pk)
  const privateKeyB64 = await exportRsaPrivateKey(sk)

  // 2) Сохраняем приватный ключ в браузере (IndexedDB)
  await storePrivateKey(sk)

  // 3) Формируем и отправляем запрос
  const payload: RegisterRequest = {
    username:   data.username,
    password:   data.password,
    publicKey:  publicKeyB64,
    privateKey: privateKeyB64
  }
  const { data: auth } = await api.post<AuthResponse>('/api/auth/register', payload)

  // 4) Сохраняем JWT в localStorage
  if (auth.token) {
    saveToken(auth.token)
  }
  // И, если бэкенд вернул приватный ключ (строка Base64)
  if (auth.privateKey) {
    localStorage.setItem('privateKeyBase64', auth.privateKey)
  }

  return auth
}

/**
 * Логин существующего пользователя:
 * 1) Отправляем учётки на бэкенд
 * 2) Сохраняем JWT и приватный ключ (если вернулся)
 */
export async function login(data: LoginRequest): Promise<AuthResponse> {
  const { data: auth } = await api.post<AuthResponse>('/api/auth/login', data)

  // Сохраняем JWT в localStorage
  if (auth.token) {
    saveToken(auth.token)
  }
  // Сохраняем приватный ключ (Base64)
  if (auth.privateKey) {
    localStorage.setItem('privateKeyBase64', auth.privateKey)
  }

  return auth
}
