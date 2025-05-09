// src/store/message.ts

import { defineStore } from 'pinia'
import { getMessages, sendMessage } from '@/services/chat.service'
import wsService from '@/services/ws.service'
import { encryptForRecipient } from '@/utils/crypto'
import type { MessageDto } from '@/types/Message'

export const useMessageStore = defineStore('message', {
  state: () => ({
    currentRecipientId: null as string | null,
    messages: [] as MessageDto[],
    isLoading: false,
    error: null as string | null
  }),

  actions: {
    /** Загружает историю с получателем */
    async loadMessages(recipientId: string) {
      this.currentRecipientId = recipientId
      this.isLoading = true
      try {
        this.messages = await getMessages(recipientId)
      } catch (e: any) {
        this.error = e.message || 'Ошибка загрузки сообщений'
      } finally {
        this.isLoading = false
      }
    },

    /**
     * Шифрует plainText для текущего получателя и отправляет его
     * @param plainText — незашифрованный текст сообщения
     */
    async sendEncryptedMessage(plainText: string) {
      if (!this.currentRecipientId) {
        throw new Error('Не указан получатель для сообщения')
      }

      // 1) Шифрование и загрузка ключей (если нужно)
      const { encryptedContent, encryptedAesKey, iv } = 
        await encryptForRecipient(this.currentRecipientId, plainText)

      // 2) REST-запрос на отправку
      const saved: MessageDto = await sendMessage(this.currentRecipientId, {
        encryptedContent,
        encryptedAesKey,
        iv,
        content: plainText
      })

      // 3) Добавляем в локальный стор
      this.messages.push(saved)

      // 4) Публикуем по WebSocket
      await wsService.send({
        chatId: saved.id,
        receiverId: this.currentRecipientId,
        encryptedContent,
        encryptedAesKey,
        iv,
        content: plainText,
        type: 0
      })
    }
  }
})
