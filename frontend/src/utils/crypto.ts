// frontend/src/utils/crypto.ts

import api from '@/services/api'
import { useAuthStore } from '@/store/auth'
import type { SendMessageRequest, MessageDto } from '@/types/Message'

/** Нормализация Base64 строки (URL-safe → стандартный) */
function normalizeBase64(b64: string): string {
  let s = b64.replace(/-/g, '+').replace(/_/g, '/')
  while (s.length % 4 !== 0) s += '='
  return s
}

/** Base64 → ArrayBuffer */
export function base64ToBuffer(base64: string): ArrayBuffer {
  const normalized = normalizeBase64(base64)
  const bin        = atob(normalized)
  const len        = bin.length
  const bytes      = new Uint8Array(len)
  for (let i = 0; i < len; i++) {
    bytes[i] = bin.charCodeAt(i)
  }
  return bytes.buffer
}

/** ArrayBuffer/TypedArray → Base64 */
export function bufferToBase64(input: ArrayBuffer | ArrayBufferView): string {
  const bytes = input instanceof ArrayBuffer
      ? new Uint8Array(input)
      : new Uint8Array(input.buffer, input.byteOffset, input.byteLength)
  let bin = ''
  for (let i = 0; i < bytes.byteLength; i++) {
    bin += String.fromCharCode(bytes[i])
  }
  return btoa(bin)
}

/** Генерация пары RSA-OAEP ключей (2048 бит, SHA-256) */
export async function generateKeyPair(): Promise<CryptoKeyPair> {
  return crypto.subtle.generateKey(
      {
        name: 'RSA-OAEP',
        modulusLength: 2048,
        publicExponent: new Uint8Array([1, 0, 1]),
        hash: 'SHA-256'
      },
      true,
      ['encrypt', 'decrypt']
  )
}

/** Экспорт RSA-публичного ключа в SPKI→Base64 */
export async function exportRsaPublicKey(key: CryptoKey): Promise<string> {
  const spki = await crypto.subtle.exportKey('spki', key)
  return bufferToBase64(spki)
}

/** Импорт RSA-публичного ключа из SPKI(Base64) */
export async function importRsaPublicKey(keyBase64: string): Promise<CryptoKey> {
  const buff = base64ToBuffer(keyBase64)
  return crypto.subtle.importKey(
      'spki',
      buff,
      { name: 'RSA-OAEP', hash: 'SHA-256' },
      true,
      ['encrypt']
  )
}

/** Экспорт приватного ключа в JWK и сохранение в localStorage */
export async function storePrivateKey(key: CryptoKey): Promise<void> {
  const jwk = await crypto.subtle.exportKey('jwk', key)
  localStorage.setItem('privateKeyJwk', JSON.stringify(jwk))
}

/** Импорт приватного ключа из JWK в localStorage */
export async function loadPrivateKey(): Promise<CryptoKey> {
  const raw = localStorage.getItem('privateKeyJwk')
  if (!raw) throw new Error('Нет приватного ключа в localStorage')
  return crypto.subtle.importKey(
      'jwk',
      JSON.parse(raw),
      { name: 'RSA-OAEP', hash: 'SHA-256' },
      true,
      ['decrypt']
  )
}

/** Генерация AES-GCM ключа (256 бит) */
export async function generateAesKey(): Promise<CryptoKey> {
  return crypto.subtle.generateKey(
      { name: 'AES-GCM', length: 256 },
      true,
      ['encrypt', 'decrypt']
  )
}

/** Генерация IV для AES-GCM (96 бит) */
export function generateIv(): Uint8Array {
  return crypto.getRandomValues(new Uint8Array(12))
}

/** AES-GCM шифрование текста */
export async function encryptMessageWithAes(
    plaintext: string,
    aesKey: CryptoKey,
    iv: Uint8Array
): Promise<ArrayBuffer> {
  const encoded = new TextEncoder().encode(plaintext)
  return crypto.subtle.encrypt({ name: 'AES-GCM', iv }, aesKey, encoded)
}

/** AES-GCM дешифрование */
export async function decryptMessageWithAes(
    ciphertext: ArrayBuffer,
    aesKey: CryptoKey,
    iv: Uint8Array
): Promise<ArrayBuffer> {
  return crypto.subtle.decrypt({ name: 'AES-GCM', iv }, aesKey, ciphertext)
}

/** RSA-OAEP шифрование raw AES-ключа */
export async function encryptAesKeyWithRsa(
    aesKey: CryptoKey,
    publicKey: CryptoKey
): Promise<ArrayBuffer> {
  const raw = await crypto.subtle.exportKey('raw', aesKey)
  return crypto.subtle.encrypt({ name: 'RSA-OAEP' }, publicKey, raw)
}

/** RSA-OAEP дешифрование AES-ключа */
export async function decryptAesKeyWithRsa(
    encryptedKey: ArrayBuffer,
    privateKey: CryptoKey
): Promise<ArrayBuffer> {
  return crypto.subtle.decrypt({ name: 'RSA-OAEP' }, privateKey, encryptedKey)
}

/** Получение publicKey (собственный из стора или чужой через кэш/API) */
async function getPublicKeyBase64(userId: string | null): Promise<string> {
  if (!userId) throw new Error('userId отсутствует')
  const auth = useAuthStore()

  if (userId === auth.userId) {
    if (!auth.publicKey) throw new Error('Нет publicKey в сторе')
    return auth.publicKey
  }

  const cacheKey = `publicKey_${userId}`
  const cached  = localStorage.getItem(cacheKey)
  if (cached) return cached

  const { data } = await api.get<{ keyBase64: string }>(`/api/keys/${userId}`)
  if (!data.keyBase64) {
    throw Object.assign(new Error('У получателя нет публичного ключа'), { code: 404 })
  }
  localStorage.setItem(cacheKey, data.keyBase64)
  return data.keyBase64
}

/**
 * Шифрование для получателя:
 * 1) AES-GCM → ciphertext
 * 2) RSA-OAEP → encrypted AES key
 */
export async function encryptForRecipient(
    recipientId: string,
    plaintext: string
): Promise<SendMessageRequest> {
  // Генерируем симметричный AES ключ и IV
  const aesKey = await generateAesKey()
  const iv     = generateIv()
  const cipher = await encryptMessageWithAes(plaintext, aesKey, iv)

  const keyB64    = await getPublicKeyBase64(recipientId)
  const rsaPub    = await importRsaPublicKey(keyB64)
  const encAesKey = await encryptAesKeyWithRsa(aesKey, rsaPub)

  return {
    encryptedContent: bufferToBase64(cipher),
    encryptedAesKey:  bufferToBase64(encAesKey),
    iv:               bufferToBase64(iv)
  }
}

/**
 * Дешифровка сообщения:
 * 1) RSA-OAEP decrypt AES key
 * 2) AES-GCM decrypt content
 */
export async function decryptMessageContent(msg: MessageDto): Promise<string> {
  try {
    if (!msg.encryptedAesKey || !msg.encryptedContent || !msg.iv) {
      throw new Error('Поля сообщения отсутствуют')
    }

    const auth    = useAuthStore()
    const privKey = auth.privateKey ?? await loadPrivateKey()

    // RSA-OAEP дешифровка AES-ключа
    const aesRaw = await decryptAesKeyWithRsa(
        base64ToBuffer(msg.encryptedAesKey),
        privKey
    )
    const aesKey = await crypto.subtle.importKey(
        'raw',
        aesRaw,
        { name: 'AES-GCM' },
        true,
        ['decrypt']
    )

    // AES-GCM дешифровка контента
    const plainBuf = await decryptMessageWithAes(
        base64ToBuffer(msg.encryptedContent),
        aesKey,
        new Uint8Array(base64ToBuffer(msg.iv))
    )

    return new TextDecoder().decode(plainBuf)
  } catch {
    return '[Ошибка расшифровки]'
  }
}