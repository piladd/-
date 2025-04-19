// 📁 services/crypto.service.ts
import { openDB } from 'idb'

// ======= КОНСТАНТЫ =======
const RSA_ALGO = {
    name: 'RSA-OAEP',
    modulusLength: 2048,
    publicExponent: new Uint8Array([1, 0, 1]),
    hash: 'SHA-256',
}

const DB_NAME = 'crypto-keys'
const DB_STORE = 'rsa-private'

// ======= IndexedDB для приватного ключа =======
async function getKeyDB() {
    return await openDB(DB_NAME, 1, {
        upgrade(db) {
            if (!db.objectStoreNames.contains(DB_STORE)) {
                db.createObjectStore(DB_STORE)
            }
        }
    })
}

export async function savePrivateKey(key: CryptoKey) {
    const db = await getKeyDB()
    const exported = await crypto.subtle.exportKey('pkcs8', key)
    await db.put(DB_STORE, exported, 'privateKey')
}

export async function loadPrivateKey(): Promise<CryptoKey | null> {
    const db = await getKeyDB()
    const exported = await db.get(DB_STORE, 'privateKey')
    if (!exported) return null

    return await crypto.subtle.importKey('pkcs8', exported, RSA_ALGO, true, ['decrypt'])
}

// ======= RSA ключи =======
export async function generateKeyPair(): Promise<CryptoKeyPair> {
    return await crypto.subtle.generateKey(RSA_ALGO, true, ['encrypt', 'decrypt'])
}

export async function exportPublicKey(key: CryptoKey): Promise<string> {
    const exported = await crypto.subtle.exportKey('spki', key)
    return bufferToBase64(exported)
}

export async function importPublicKey(keyStr: string): Promise<CryptoKey> {
    const buffer = base64ToBuffer(keyStr)
    return await crypto.subtle.importKey('spki', buffer, RSA_ALGO, true, ['encrypt'])
}

export async function encryptMessage(plaintext: string, publicKey: CryptoKey): Promise<string> {
    const encoded = new TextEncoder().encode(plaintext)
    const encrypted = await crypto.subtle.encrypt({ name: 'RSA-OAEP' }, publicKey, encoded)
    return bufferToBase64(encrypted)
}

export async function decryptMessage(encryptedBase64: string, privateKey: CryptoKey): Promise<string> {
    const encryptedBuffer = base64ToBuffer(encryptedBase64)
    const decrypted = await crypto.subtle.decrypt({ name: 'RSA-OAEP' }, privateKey, encryptedBuffer)
    return new TextDecoder().decode(decrypted)
}

// ======= Гибридное шифрование (AES + RSA) =======
export async function generateAESKey(): Promise<CryptoKey> {
    return await crypto.subtle.generateKey(
        { name: 'AES-GCM', length: 256 },
        true,
        ['encrypt', 'decrypt']
    )
}

export async function hybridEncrypt(message: string, rsaPublicKey: CryptoKey): Promise<{
    encryptedKey: string,
    encryptedData: string,
    iv: string
}> {
    const aesKey = await generateAESKey()
    const iv = crypto.getRandomValues(new Uint8Array(12))

    const encodedMessage = new TextEncoder().encode(message)
    const encryptedData = await crypto.subtle.encrypt(
        { name: 'AES-GCM', iv },
        aesKey,
        encodedMessage
    )

    const exportedAESKey = await crypto.subtle.exportKey('raw', aesKey)
    const encryptedAESKey = await crypto.subtle.encrypt(
        { name: 'RSA-OAEP' },
        rsaPublicKey,
        exportedAESKey
    )

    return {
        encryptedKey: bufferToBase64(encryptedAESKey),
        encryptedData: bufferToBase64(encryptedData),
        iv: bufferToBase64(iv),
    }
}

export async function hybridDecrypt(encrypted: {
    encryptedKey: string
    encryptedData: string
    iv: string
}, rsaPrivateKey: CryptoKey): Promise<string> {
    const aesKeyBuffer = await crypto.subtle.decrypt(
        { name: 'RSA-OAEP' },
        rsaPrivateKey,
        base64ToBuffer(encrypted.encryptedKey)
    )

    const aesKey = await crypto.subtle.importKey(
        'raw',
        aesKeyBuffer,
        { name: 'AES-GCM' },
        true,
        ['decrypt']
    )

    const decrypted = await crypto.subtle.decrypt(
        {
            name: 'AES-GCM',
            iv: base64ToBuffer(encrypted.iv),
        },
        aesKey,
        base64ToBuffer(encrypted.encryptedData)
    )

    return new TextDecoder().decode(decrypted)
}

// ======= Цифровая подпись =======
export async function generateSigningKeyPair(): Promise<CryptoKeyPair> {
    return await crypto.subtle.generateKey(
        {
            name: 'RSASSA-PKCS1-v1_5',
            modulusLength: 2048,
            publicExponent: new Uint8Array([1, 0, 1]),
            hash: 'SHA-256',
        },
        true,
        ['sign', 'verify']
    )
}

export async function signMessage(message: string, privateKey: CryptoKey): Promise<string> {
    const encoded = new TextEncoder().encode(message)
    const signature = await crypto.subtle.sign('RSASSA-PKCS1-v1_5', privateKey, encoded)
    return bufferToBase64(signature)
}

export async function verifySignature(message: string, signatureBase64: string, publicKey: CryptoKey): Promise<boolean> {
    const encoded = new TextEncoder().encode(message)
    const signature = base64ToBuffer(signatureBase64)
    return await crypto.subtle.verify('RSASSA-PKCS1-v1_5', publicKey, signature, encoded)
}

// ======= Утилиты =======
function bufferToBase64(buffer: ArrayBuffer): string {
    return btoa(String.fromCharCode(...new Uint8Array(buffer)))
}

function base64ToBuffer(base64: string): ArrayBuffer {
    const binary = atob(base64)
    const bytes = new Uint8Array(binary.length)
    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i)
    }
    return bytes.buffer
}
