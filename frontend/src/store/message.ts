import { defineStore } from 'pinia'
import type { Message } from '@/types/Message'

export const useMessageStore = defineStore('message', {
    state: () => ({
        messages: [] as Message[],
    }),
    actions: {
        setMessages(msgs: Message[]) {
            this.messages = msgs
        },
        addMessage(msg: Message) {
            this.messages.push(msg)
        },
        clearMessages() {
            this.messages = []
        },
    },
})
