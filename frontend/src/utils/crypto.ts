import type {SendMessageRequest, MessageDto} from '@/types/message'
import {
    generateAesKey,
    generateIv,
    encryptMessageWithAes,
    encryptAesKeyWithRsa,
    importRsaPublicKey,
    bufferToBase64,
    base64ToBuffer
} from './crypto.helpers'

// ❗Заглушка — должен быть реальный вызов API
async function getPublicKeyPem(recipientId: string): Promise<string> {
    return `-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkq...==\n-----END PUBLIC KEY-----`
}

export async function encryptForRecipient(recipientId: string, plaintext: string): Promise<SendMessageRequest> {
    const aesKey = await generateAesKey()
    const iv = generateIv()
    const ciphertext = await encryptMessageWithAes(plaintext, aesKey, iv)

    const publicKeyPem = await getPublicKeyPem(recipientId)
    const publicKey = await importRsaPublicKey(publicKeyPem)
    const encryptedAesKey = await encryptAesKeyWithRsa(aesKey, publicKey)

    return {
        encryptedContent: bufferToBase64(ciphertext),
        encryptedAesKey: bufferToBase64(encryptedAesKey),
        iv: bufferToBase64(iv)
    }
}

export async function decryptMessageContent(msg: MessageDto): Promise<string> {
    try {
        return atob(msg.encryptedContent || '')
    } catch {
        return '[Ошибка расшифровки]'
    }
}
