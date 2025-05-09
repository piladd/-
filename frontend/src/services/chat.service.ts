// src/services/chat.service.ts

import api from './api'
import type { MessageDto, SendMessageRequest } from '@/types/Message'


/**
 * Получение истории сообщений с конкретным пользователем
 */
export const getMessages = async (chatId: string): Promise<MessageDto[]> => {
  const { data } = await api.get<MessageDto[]>(`/api/chat/history/${chatId}`)
  return data
}

/**
 * Отправка зашифрованного сообщения
 */
export const sendMessage = async (
  payload: SendMessageRequest
): Promise<MessageDto> => {
  const { data: message } = await api.post<MessageDto>(
    '/api/chat/send',
    payload
  )
  return message
}

/**
 * Загрузка публичного ключа текущего пользователя на сервер
 */
export const uploadPublicKey = async (keyBase64: string): Promise<void> => {
  await api.post<void>('/api/keys/upload', { keyBase64 })
}

/**
 * Получение чужого публичного ключа
 */
export const getPublicKey = async (userId: string): Promise<string> => {
  const { data } = await api.get<{ userId: string; keyBase64: string }>(`/api/keys/${userId}`)
  return data.keyBase64
}
