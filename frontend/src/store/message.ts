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
        async loadMessages(recipientId: string) {
            this.currentRecipientId = recipientId
            this.isLoading = true
            try {
                this.messages = await getMessages(recipientId)
            } finally {
                this.isLoading = false
            }
        },

        async sendEncryptedMessage(recipientId: string, data: SendMessageRequest) {
            // **Проверка**
            if (!data.encryptedContent || !data.encryptedAesKey || !data.iv) {
                throw new Error('Encrypted message data is incomplete')
            }

            // REST
            const saved = await sendMessage(recipientId, data)
            this.messages.push(saved)

            // WS
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
