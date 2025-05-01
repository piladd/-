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
    )
}

export async function generateAesKey(): Promise<CryptoKey> {
    return await crypto.subtle.generateKey(
        {
            name: 'AES-GCM',
            length: 256
        },
        true,
        ['encrypt', 'decrypt']
    )
}

export function generateIv(): Uint8Array {
    return crypto.getRandomValues(new Uint8Array(12)) // 96 бит
}

export async function encryptMessageWithAes(
    plaintext: string,
    aesKey: CryptoKey,
    iv: Uint8Array
): Promise<ArrayBuffer> {
    const encoded = new TextEncoder().encode(plaintext)
    return await crypto.subtle.encrypt({name: 'AES-GCM', iv}, aesKey, encoded)
}

export async function encryptAesKeyWithRsa(
    aesKey: CryptoKey,
    publicKey: CryptoKey
): Promise<ArrayBuffer> {
    const rawKey = await crypto.subtle.exportKey('raw', aesKey)
    return await crypto.subtle.encrypt({name: 'RSA-OAEP'}, publicKey, rawKey)
}

export async function importRsaPublicKey(pem: string): Promise<CryptoKey> {
    const binary = base64ToBuffer(
        pem.replace(/-----BEGIN PUBLIC KEY-----|-----END PUBLIC KEY-----|\\n/g, '')
    )
    return await crypto.subtle.importKey(
        'spki',
        binary,
        {
            name: 'RSA-OAEP',
            hash: 'SHA-256'
        },
        true,
        ['encrypt']
    )
}

export function bufferToBase64(buffer: ArrayBuffer): string {
    return btoa(String.fromCharCode(...new Uint8Array(buffer)))
}

export function base64ToBuffer(base64: string): ArrayBuffer {
    const binary = atob(base64)
    const bytes = new Uint8Array(binary.length)
    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i)
    }
    return bytes.buffer
}
