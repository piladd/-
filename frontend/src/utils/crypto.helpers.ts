// crypto.helpers.ts

/**
 * Генерация пары RSA ключей для шифрования RSA-OAEP
 */
export async function generateKeyPair(): Promise<CryptoKeyPair> {
    return await crypto.subtle.generateKey(
        {
            name: 'RSA-OAEP',
            modulusLength: 4096,
            publicExponent: new Uint8Array([1, 0, 1]),
            hash: 'SHA-256'
        },
        true,
        ['encrypt', 'decrypt']
    );
}

/**
 * Генерация AES-GCM ключа
 */
export async function generateAesKey(): Promise<CryptoKey> {
    return await crypto.subtle.generateKey(
        { name: 'AES-GCM', length: 256 },
        true,
        ['encrypt', 'decrypt']
    );
}

/**
 * Генерация вектора инициализации (IV) для AES-GCM
 */
export function generateIv(): Uint8Array {
    return crypto.getRandomValues(new Uint8Array(12));
}

/**
 * Шифрование текста симметричным AES-GCM ключом
 */
export async function encryptMessageWithAes(
    plaintext: string,
    aesKey: CryptoKey,
    iv: Uint8Array
): Promise<ArrayBuffer> {
    const encoded = new TextEncoder().encode(plaintext);
    return await crypto.subtle.encrypt({ name: 'AES-GCM', iv }, aesKey, encoded);
}

/**
 * Шифрование AES ключа с помощью RSA публичного ключа
 */
export async function encryptAesKeyWithRsa(
    aesKey: CryptoKey,
    publicKey: CryptoKey
): Promise<ArrayBuffer> {
    const rawKey = await crypto.subtle.exportKey('raw', aesKey);
    return await crypto.subtle.encrypt({ name: 'RSA-OAEP' }, publicKey, rawKey);
}

/**
 * Импорт RSA публичного ключа из Base64 (SPKI)
 * @param keyBase64 - строка Base64 без PEM-обёртки
 */
export async function importRsaPublicKey(keyBase64: string): Promise<CryptoKey> {
    // нормализуем URL-safe Base64 в стандартный
    const binary = base64ToBuffer(keyBase64);
    return await crypto.subtle.importKey(
        'spki',
        binary,
        { name: 'RSA-OAEP', hash: 'SHA-256' },
        true,
        ['encrypt']
    );
}

/**
 * Преобразование ArrayBuffer в Base64
 */
export function bufferToBase64(buffer: ArrayBuffer): string {
    const bytes = new Uint8Array(buffer);
    let binary = '';
    for (let b of bytes) binary += String.fromCharCode(b);
    return btoa(binary);
}

/**
 * Нормализация Base64 строки (URL-safe -> стандартный) и доп. символы '='
 */
function normalizeBase64(b64: string): string {
    let s = b64.replace(/-/g, '+').replace(/_/g, '/');
    while (s.length % 4 !== 0) s += '=';
    return s;
}

/**
 * Преобразование Base64 в ArrayBuffer
 */
export function base64ToBuffer(base64: string): ArrayBuffer {
    const normalized = normalizeBase64(base64);
    const binary = atob(normalized);
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return bytes.buffer;
}