// src/store/message.ts
import { defineStore } from 'pinia'
import api from '@/services/api'
import { getMessages, sendMessage } from '@/services/chat.service'
import wsService from '@/services/ws.service'
import { encryptForRecipient, decryptMessageContent } from '@/utils/crypto'
import { useAuthStore } from '@/store/auth'
import type { MessageDto } from '@/types/Message'

interface MessageState {
  currentRecipientId: string | null
  currentChatId:       string | null
  messages:            (MessageDto & { decryptedContent?: string })[]
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
    /** Создаём или получаем chatId, грузим историю и подписываемся на WS */
    async startDialog(recipientId: string) {
      this.isLoading = true
      this.error     = null

      try {
        // ——— 1) HTTP: получить/создать чат ———
        const { data } = await api.post<{ chatId: string }>(
          '/api/chat/start',
          { interlocutorId: recipientId }
        )
        this.currentChatId      = data.chatId
        this.currentRecipientId = recipientId

        // ——— 2) HTTP: загрузить и расшифровать историю ———
        const msgs = await getMessages(this.currentChatId)
        this.messages = await Promise.all(
          msgs.map(async msg => ({
            ...msg,
            decryptedContent: await decryptMessageContent(msg)
          }))
        )

        // ——— 3) WS: коннект + joinChat + onMessage ———
        const auth = useAuthStore()
        // если ещё не подключены — подключаемся
        await wsService.connect(auth.userId!, auth.token)
        // **заходим в группу чата** — иначе сервера не будет шлёть ReceiveMessage
        await wsService.joinChat(this.currentChatId!)
        // подписываемся на новые
        wsService.onMessage(async (msg: MessageDto) => {
          if (msg.chatId === this.currentChatId) {
            const clear = await decryptMessageContent(msg)
            this.messages.push({
              ...msg,
              decryptedContent: clear
            })
          }
        })
      } catch (e: any) {
        this.error = e.message || 'Ошибка при старте диалога'
      } finally {
        this.isLoading = false
      }
    },

    /** Грузим и расшифровываем историю сообщений */
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
      if (!this.currentChatId) {
        await this.startDialog(recipientId)
      }

      this.isLoading = true
      this.error     = null

      try {
        // 1) Шифруем текст
        const { encryptedContent, encryptedAesKey, iv } =
          await encryptForRecipient(recipientId, plaintext)

        // 2) Отправляем на бэк
        const msgDto = await sendMessage({
          chatId:           this.currentChatId!,
          receiverId:       recipientId,
          encryptedContent,
          encryptedAesKey,
          iv,
          type:             0
        })

        // 3) Добавляем своё сообщение в UI сразу (plaintext)
        this.messages.push({
          ...msgDto,
          decryptedContent: plaintext
        })

        // 4) Рассылаем по WebSocket для собеседника
        await wsService.send({
          chatId:           this.currentChatId!,
          receiverId:       recipientId,
          encryptedContent,
          encryptedAesKey,
          iv,
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
