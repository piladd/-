import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'

export interface Chat {
    id: string
    name: string
    participants: string[]
}

export const useChatStore = defineStore('chat', () => {
    const chats = ref<Chat[]>([])
    const activeChatId = ref<string | null>(null)

    async function fetchChats() {
        const res = await axios.get('/api/chats')
        chats.value = res.data
    }

    function selectChat(id: string) {
        activeChatId.value = id
    }

    function getReceiverId(chatId: string): string | null {
        const chat = chats.value.find(c => c.id === chatId)
        const currentUserId = localStorage.getItem('userId')
        if (!chat || !currentUserId) return null
        return chat.participants.find(id => id !== currentUserId) || null
    }

    return { chats, activeChatId, fetchChats, selectChat, getReceiverId }
})
