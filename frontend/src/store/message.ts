// frontend/src/store/message.ts
import { defineStore } from 'pinia'
import { getMessages, sendMessage } from '@/services/chat.service'
import wsService from '@/services/ws.service'
import type { MessageDto, SendMessageRequest } from '@/types/Message'

export const useMessageStore = defineStore('message', {
    state: () => ({
        currentRecipientId: null as string | null,
        messages: [] as MessageDto[],
        isLoading: false,
        error: null as string | null
    }),

    actions: {
        /** Загружает всю историю чата по userId */
        async loadMessages(recipientId: string) {
            this.currentRecipientId = recipientId
            this.isLoading = true
            this.error = null
            try {
                this.messages = await getMessages(recipientId)
            } finally {
                this.isLoading = false
            }
        },

        /** Подключает real-time-подписку */
        initRealtime() {
            wsService.onMessage((msg) => {
                if (msg.senderId === this.currentRecipientId) {
                    this.messages.push(msg)
                }
            })
        },

        /** Отправляет сообщение: сначала REST, потом WS */
        async sendEncryptedMessage(recipientId: string, data: SendMessageRequest) {
            const saved = await sendMessage(recipientId, data) 
            this.messages.push(saved)
            await wsService.send({
                chatId: saved.id,  
                receiverId: recipientId,
                encryptedContent: data.encryptedContent,
                encryptedAesKey: data.encryptedAesKey,
                iv: data.iv,
                content: data.content || '',
                type: data.type ?? 0
            })
        }
    }
})
