import {defineStore} from 'pinia'
import type {MessageDto} from '@/types/message'
import {getMessages, sendMessage} from '@/services/chat.service'
import {uploadAttachment} from '@/services/attachment.service'
import {encryptForRecipient, decryptMessageContent} from '@/utils/crypto'

export const useMessageStore = defineStore('message', {
    state: () => ({
        messages: [] as MessageDto[],
        isLoading: false,
        error: null as string | null
    }),

    actions: {
        /// <summary>Загружает сообщения для выбранного собеседника</summary>
        /// <param name="recipientId">ID получателя</param>
        async loadMessages(recipientId: string) {
            this.isLoading = true
            this.error = null
            try {
                const raw = await getMessages(recipientId)

                this.messages = await Promise.all(
                    raw.map(async (msg) => ({
                        ...msg,
                        decryptedContent: await this.tryDecrypt(msg)
                    }))
                )
            } catch (err: any) {
                this.error = 'Не удалось загрузить сообщения'
                console.error(err)
            } finally {
                this.isLoading = false
            }
        },

        /// <summary>Отправляет зашифрованное текстовое сообщение</summary>
        /// <param name="recipientId">ID получателя</param>
        /// <param name="text">Открытый текст</param>
        async sendEncryptedMessage(recipientId: string, text: string) {
            try {
                const payload = await encryptForRecipient(recipientId, text)
                const sent = await sendMessage(recipientId, payload)

                this.messages.push({
                    ...sent,
                    decryptedContent: text
                })
            } catch (err) {
                console.error('Ошибка при отправке сообщения:', err)
            }
        },

        /// <summary>Отправляет зашифрованный файл как вложение</summary>
        /// <param name="recipientId">ID получателя</param>
        /// <param name="file">Файл</param>
        async sendEncryptedAttachment(recipientId: string, file: File) {
            try {
                await uploadAttachment(file)
                // Можно расширить: создать сообщение о загрузке
            } catch (err) {
                console.error('Ошибка загрузки вложения:', err)
            }
        },

        /// <summary>Пытается расшифровать содержимое сообщения</summary>
        /// <param name="msg">Сообщение</param>
        async tryDecrypt(msg: MessageDto): Promise<string> {
            try {
                return await decryptMessageContent(msg)
            } catch {
                return '[Ошибка расшифровки]'
            }
        },

        /// <summary>Добавляет входящее сообщение из WebSocket в стор</summary>
        /// <param name="msg">MessageDto из WS</param>
        async pushFromSocket(msg: MessageDto) {
            const decrypted = await this.tryDecrypt(msg)
            this.messages.push({
                ...msg,
                decryptedContent: decrypted
            })
        }
    }
})
