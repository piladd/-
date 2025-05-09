// src/utils/crypto.ts

import { useAuthStore } from '@/store/auth'
import type { MessageDto } from '@/types/Message'
import { uploadPublicKey, getPublicKey } from '@/services/chat.service'

/** Нормализация Base64 (URL-safe → стандартный) */
function normalizeBase64(b64: string): string {
  let s = b64.replace(/-/g, '+').replace(/_/g, '/')
  while (s.length % 4 !== 0) s += '='
  return s
}

/** Base64 → ArrayBuffer */
export function base64ToBuffer(base64: string): ArrayBuffer {
  const normalized = normalizeBase64(base64)
  const bin = atob(normalized)
  const bytes = new Uint8Array(bin.length)
  for (let i = 0; i < bin.length; i++) bytes[i] = bin.charCodeAt(i)
  return bytes.buffer
}

/** ArrayBuffer/TypedArray → Base64 */
export function bufferToBase64(input: ArrayBuffer | ArrayBufferView): string {
  const bytes = input instanceof ArrayBuffer
    ? new Uint8Array(input)
    : new Uint8Array(input.buffer, input.byteOffset, input.byteLength)
  let bin = ''
  for (let i = 0; i < bytes.byteLength; i++) bin += String.fromCharCode(bytes[i])
  return btoa(bin)
}

/** Генерация пары RSA-OAEP (2048 бит, SHA-256) */
export async function generateKeyPair(): Promise<CryptoKeyPair> {
  return crypto.subtle.generateKey(
    { name: 'RSA-OAEP', modulusLength: 2048, publicExponent: new Uint8Array([1, 0, 1]), hash: 'SHA-256' },
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
    'spki', buff,
    { name: 'RSA-OAEP', hash: 'SHA-256' },
    true,
    ['encrypt']
  )
}

/** Экспорт RSA-приватного ключа в PKCS#8→Base64 */
export async function exportRsaPrivateKey(key: CryptoKey): Promise<string> {
  const pkcs8 = await crypto.subtle.exportKey('pkcs8', key)
  return bufferToBase64(pkcs8)
}

/** Сохранение приватного ключа в localStorage (PKCS#8 Base64) */
export async function storePrivateKey(key: CryptoKey): Promise<void> {
  const b64 = await exportRsaPrivateKey(key)
  localStorage.setItem('privateKeyBase64', b64)
}

/** Загрузка приватного ключа из localStorage */
export async function loadPrivateKey(): Promise<CryptoKey> {
  const b64 = localStorage.getItem('privateKeyBase64')
  if (!b64) throw new Error('Нет приватного ключа в хранилище')
  const buffer = base64ToBuffer(b64)
  return crypto.subtle.importKey(
    'pkcs8', buffer,
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

/** AES-GCM шифрование */
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

/** RSA-OAEP шифрование AES-ключа */
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

/** Получение публичного ключа (локально или из API) */
async function getPublicKeyBase64(userId: string | null): Promise<string> {
  if (!userId) throw new Error('userId отсутствует')
  const auth = useAuthStore()
  if (userId === auth.userId) {
    if (!auth.publicKey) await generateAndUploadKeyPair()
    return auth.publicKey!
  }
  const cacheKey = `publicKey_${userId}`
  const cached = localStorage.getItem(cacheKey)
  if (cached) return cached
  const keyBase64 = await getPublicKey(userId)
  localStorage.setItem(cacheKey, keyBase64)
  return keyBase64
}

/** Шифрование для получателя -> возвращает только данные шифрования */
export async function encryptForRecipient(
  recipientId: string,
  plaintext: string
): Promise<{ encryptedContent: string; encryptedAesKey: string; iv: string }> {
  const aesKey = await generateAesKey()
  const iv = generateIv()
  const cipher = await encryptMessageWithAes(plaintext, aesKey, iv)
  const keyB64 = await getPublicKeyBase64(recipientId)
  const rsaPub = await importRsaPublicKey(keyB64)
  const encAesKey = await encryptAesKeyWithRsa(aesKey, rsaPub)
  return {
    encryptedContent: bufferToBase64(cipher),
    encryptedAesKey: bufferToBase64(encAesKey),
    iv: bufferToBase64(iv)
  }
}

/** Дешифровка входящего сообщения */
// src/utils/crypto.ts

export async function decryptMessageContent(msg: MessageDto): Promise<string> {
  try {
    if (!msg.encryptedAesKey || !msg.encryptedContent || !msg.iv) {
      throw new Error('Поля сообщения отсутствуют');
    }

    const auth    = useAuthStore();
    const privKey = auth.privateKey ?? await loadPrivateKey();

    // Отладочные логи: убедимся, что base64-строки корректны
    console.log('🔑 Encrypted AES key (B64):', msg.encryptedAesKey);
    console.log('📜 Ciphertext (B64):',      msg.encryptedContent);
    console.log('🔬 IV (B64):',              msg.iv);

    // 1) Раскодируем и дешифруем AES-ключ через RSA-OAEP
    let aesRaw: ArrayBuffer;
    try {
      aesRaw = await decryptAesKeyWithRsa(
        base64ToBuffer(msg.encryptedAesKey),
        privKey
      );
      console.log('🔑 AES raw key bytes:', new Uint8Array(aesRaw));
    } catch (e) {
      console.error('❌ RSA-OAEP дешифровка не удалась:', e);
      throw e;  // чтобы попасть в общий catch и вернуть «[Ошибка расшифровки]»
    }

    // 2) Импортируем AES-ключ
    let aesKey: CryptoKey;
    try {
      aesKey = await crypto.subtle.importKey(
        'raw',
        aesRaw,
        { name: 'AES-GCM' },
        false,
        ['decrypt']
      );
    } catch (e) {
      console.error('❌ Ошибка при импорте AES-ключа:', e);
      throw e;
    }

    // 3) Подготовим IV и ciphertext
    const ivBuf     = new Uint8Array(base64ToBuffer(msg.iv));
    const cipherBuf = base64ToBuffer(msg.encryptedContent);

    console.log('🔬 IV bytes:',     ivBuf);
    console.log('📜 Cipher bytes:', new Uint8Array(cipherBuf));

    // 4) Дешифруем AES-GCM
    let plainBuf: ArrayBuffer;
    try {
      plainBuf = await decryptMessageWithAes(
        cipherBuf,
        aesKey,
        ivBuf
      );
    } catch (e) {
      console.error('❌ AES-GCM дешифровка не удалась:', e);
      throw e;
    }

    // 5) Успешно распознали плейн-текст
    return new TextDecoder().decode(plainBuf);

  } catch (err: any) {
    console.error('Ошибка при расшифровке сообщения:', err); // этот лог вы уже видели :contentReference[oaicite:0]{index=0}:contentReference[oaicite:1]{index=1}
    return '[Ошибка расшифровки]';
  }
}


/** Генерация RSA-OAEP пары и загрузка публичного ключа на сервер */
export async function generateAndUploadKeyPair(): Promise<void> {
  const keyPair = await generateKeyPair()
  const publicKeyBase64 = await exportRsaPublicKey(keyPair.publicKey)
  await uploadPublicKey(publicKeyBase64)
  await storePrivateKey(keyPair.privateKey)
  const auth = useAuthStore()
  auth.publicKey = publicKeyBase64
}
