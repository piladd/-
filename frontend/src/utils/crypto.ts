import api from '@/services/api'
import type { SendMessageRequest, MessageDto } from '@/types/message'
import {
  generateAesKey,
  generateIv,
  encryptMessageWithAes,
  encryptAesKeyWithRsa,
  importRsaPublicKey,
  bufferToBase64
} from './crypto.helpers'

/**
 * Запрашивает у API чистую Base64-строку публичного ключа пользователя
 */
async function getPublicKeyBase64(recipientId: string): Promise<string> {
  // ожидаем, что API отдаст plain Base64 без PEM-обёртки
  const response = await api.get<string>(`/api/keys/${recipientId}`)
  return response.data
}

/**
 * Подготавливает и возвращает объект для отправки зашифрованного сообщения
 */
export async function encryptForRecipient(
  recipientId: string,
  plaintext: string
): Promise<SendMessageRequest> {
  // Генерируем симметричный AES ключ и IV
  const aesKey = await generateAesKey()
  const iv = generateIv()
  // Шифруем сообщение симметричным ключом
  const ciphertext = await encryptMessageWithAes(plaintext, aesKey, iv)

  // Получаем публичный ключ получателя (Base64 SPKI)
  const keyBase64 = await getPublicKeyBase64(recipientId)
  const publicKey = await importRsaPublicKey(keyBase64)
  // Шифруем AES ключ асимметричным RSA
  const encryptedAesKey = await encryptAesKeyWithRsa(aesKey, publicKey)

  return {
    encryptedContent: bufferToBase64(ciphertext),
    encryptedAesKey: bufferToBase64(encryptedAesKey),
    iv: bufferToBase64(iv.buffer as ArrayBuffer)
  }
}

/**
 * Простая расшифровка контента (для просмотра в демо)
 */
export async function decryptMessageContent(msg: MessageDto): Promise<string> {
  try {
    // Здесь можно применить AES-расшифровку при наличии ключа
    return atob(msg.encryptedContent || '')
  } catch {
    return '[Ошибка расшифровки]'
  }
}