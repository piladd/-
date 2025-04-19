// 📁 store/message.ts
import { defineStore } from 'pinia'
import { ref, watch } from 'vue'
import { useChatStore } from './chat'
import { getMessages as apiGetMessages } from '@/services/chat.service'
import { KeyManagerService } from '@/services/key-manager.service'
import { hybridDecrypt } from '@/services/crypto.service'
import { sendMessage as sendEncryptedMessage } from '@/services/websocket'
import type { Message } from '@/types/Message'

export const useMessageStore = defineStore('message', () => {
    const messages = ref<Message[]>([]) // ✅ определено
    const chatStore = useChatStore()

    async function fetchMessages(chatId: string): Promise<void> {
        const res = await apiGetMessages(chatId)
        messages.value = res // ✅ корректно, res: Message[]
    }

    async function sendMessage(text: string) {
        const chatId = chatStore.activeChatId
        const receiverId = chatStore.getReceiverId(chatId!)
        if (!chatId || !receiverId) return
        await sendEncryptedMessage(text, chatId, receiverId)
    }

    async function receiveMessage(msg: any) {
        const { encryptedKey, encryptedData, iv } = msg

        if (encryptedKey && encryptedData && iv) {
            const privateKey = await KeyManagerService.ensurePrivateKey()
            const decryptedText = await hybridDecrypt({ encryptedKey, encryptedData, iv }, privateKey)

            messages.value.push({
                id: msg.id,
                chatId: msg.chatId,
                senderId: msg.senderId,
                text: decryptedText,
                timestamp: msg.timestamp,
                isMine: msg.senderId === localStorage.getItem('userId'),
            })
        } else {
            messages.value.push(msg)
        }
    }

    watch(
        () => chatStore.activeChatId,
        (chatId) => {
            if (chatId) fetchMessages(chatId)
        },
        { immediate: true }
    )

    return { messages, fetchMessages, receiveMessage, sendMessage }
})
