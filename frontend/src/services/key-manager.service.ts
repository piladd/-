import {
    generateKeyPair,
    exportPublicKey,
    savePrivateKey,
    loadPrivateKey,
    importPublicKey,
} from './crypto.service'
import { openDB } from 'idb'
import axios from 'axios'

const API = 'http://localhost:5000/api/keys'

// эти же значения ты используешь в crypto.service.ts
const DB_NAME = 'crypto-keys'
const DB_STORE = 'rsa-private'

export class KeyManagerService {
    // Генерация и загрузка приватного ключа (если нет — создать)
    static async ensurePrivateKey(): Promise<CryptoKey> {
        let privateKey = await loadPrivateKey()

        if (!privateKey) {
            const { publicKey, privateKey: newPrivateKey } = await generateKeyPair()
            await savePrivateKey(newPrivateKey)

            const publicKeyStr = await exportPublicKey(publicKey)
            await this.uploadPublicKey(publicKeyStr)

            privateKey = newPrivateKey
        }

        return privateKey
    }

    // Отправка публичного ключа на сервер
    static async uploadPublicKey(publicKey: string): Promise<void> {
        const userId = this.getCurrentUserId()
        await axios.post(`${API}/upload`, {
            userId,
            publicKey,
        })
    }

    // Получить публичный ключ другого пользователя
    static async getUserPublicKey(userId: string): Promise<CryptoKey> {
        const response = await axios.get(`${API}/user/${userId}`)
        const publicKeyStr = response.data.publicKey
        return await importPublicKey(publicKeyStr)
    }

    // Удаление приватного ключа из IndexedDB
    static async deletePrivateKey(): Promise<void> {
        const db = await openDB(DB_NAME, 1)
        await db.delete(DB_STORE, 'privateKey')
    }

    // Получение текущего пользователя (пока через localStorage)
    static getCurrentUserId(): string {
        return localStorage.getItem('userId') || 'anonymous'
    }
}
