import { defineStore } from 'pinia'
import type { Chat } from '@/types/Chat'

export const useChatStore = defineStore('chat', {
    state: () => ({
        chats: [] as Chat[],
        selectedChatId: null as string | null,
    }),
    getters: {
        selectedChat(state) {
            return state.chats.find(chat => chat.id === state.selectedChatId) || null
        },
    },
    actions: {
        setChats(chats: Chat[]) {
            this.chats = chats
        },
        selectChat(id: string) {
            this.selectedChatId = id
        },
    },
})
