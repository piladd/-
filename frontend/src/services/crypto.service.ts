// высокоуровневая логика для использования в messageStore
import {
    generateAesKey,
    generateIv,
    encryptAesKeyWithRsa,
    encryptMessageWithAes,
    importRsaPublicKey,
    bufferToBase64
} from '@/utils/crypto.helpers'
import {getPublicKeyPem} from './user.service' // должен быть реализован


export const encryptForRecipient = async (recipientId: string, plaintext: string) => {
    const aesKey = await generateAesKey()
    const iv = generateIv()
    const encryptedContent = await encryptMessageWithAes(plaintext, aesKey, iv)

    const publicKeyPem = await getPublicKeyPem(recipientId)
    const publicKey = await importRsaPublicKey(publicKeyPem)
    const encryptedAesKey = await encryptAesKeyWithRsa(aesKey, publicKey)

    return {
        encryptedContent: bufferToBase64(encryptedContent),
        encryptedAesKey: bufferToBase64(encryptedAesKey),
        iv: bufferToBase64(iv)
    }
}
