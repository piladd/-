// src/store/message.ts
import { defineStore } from 'pinia'
import api from '@/services/api'
import { getMessages, sendMessage } from '@/services/chat.service'
import wsService from '@/services/ws.service'
import { encryptForRecipient, decryptMessageContent } from '@/utils/crypto'
import type { MessageDto, SendMessageRequest } from '@/types/Message'

interface MessageState {
  currentRecipientId: string | null
  currentChatId:       string | null
  messages:            MessageDto[]
  isLoading:           boolean
  error:               string | null
}

export const useMessageStore = defineStore('message', {
  state: (): MessageState => ({
    currentRecipientId: null,
    currentChatId:      null,
    messages:           [],
    isLoading:          false,
    error:              null,
  }),

  actions: {
    /** Создаём или получаем chatId и грузим историю */
    async startDialog(recipientId: string) {
      this.isLoading = true
      this.error     = null
      try {
        const { data } = await api.post<{ chatId: string }>(
          '/api/chat/start',
          { interlocutorId: recipientId }
        )
        this.currentChatId      = data.chatId
        this.currentRecipientId = recipientId
        await this.loadMessages(recipientId)
      } catch (e: any) {
        this.error = e.message || 'Ошибка при старте диалога'
      } finally {
        this.isLoading = false
      }
    },

    /** Грузим и расшифровываем историю */
    async loadMessages(recipientId: string) {
      if (!this.currentChatId || this.currentRecipientId !== recipientId) {
        await this.startDialog(recipientId)
        return
      }
      this.isLoading = true
      this.error     = null
      try {
        const msgs = await getMessages(this.currentChatId)
        this.messages = await Promise.all(
          msgs.map(async msg => ({
            ...msg,
            decryptedContent: await decryptMessageContent(msg)
          }))
        )
      } catch (e: any) {
        this.error = e.message || 'Ошибка при загрузке сообщений'
      } finally {
        this.isLoading = false
      }
    },

    /** Шифруем и отправляем сообщение */
    async sendEncryptedMessage(recipientId: string, plaintext: string) {
      // Убеждаемся, что есть chatId
      if (!this.currentChatId) {
        await this.startDialog(recipientId)
      }

      this.isLoading = true
      this.error     = null

      try {
        // 1) Шифруем текст **один раз**
        const { encryptedContent, encryptedAesKey, iv } =
          await encryptForRecipient(recipientId, plaintext)

        // 2) Формируем payload строго по C# DTO
        const payload: SendMessageRequest = {
          receiverId:       recipientId,         // Guid получателя
          chatId:           this.currentChatId!, // Guid чата
          encryptedContent,                     // Base64 зашифрованного текста
          encryptedAesKey,                      // Base64 зашифрованного AES-ключа
          iv,                                   // Base64 IV
          type:             0,                  // MessageType.Text
          content:          plaintext           // именно строка
        }

        // 3) Отправляем единым объектом
        const saved: MessageDto = await sendMessage(payload)

        // 4) Расшифровываем ответ и добавляем в стор
        const decrypted = await decryptMessageContent(saved)
        this.messages.push({ ...saved, decryptedContent: decrypted })

        // 5) Посылаем по WebSocket для собеседника
        await wsService.send({
          chatId:           this.currentChatId!,
          receiverId:       recipientId,
          encryptedContent,
          encryptedAesKey,
          iv,
          content:          plaintext,
          type:             0
        })
      } catch (e: any) {
        this.error = e.message || 'Ошибка при отправке сообщения'
      } finally {
        this.isLoading = false
      }
    }
  }
})
