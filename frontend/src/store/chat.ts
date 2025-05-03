import {defineStore} from 'pinia'
import {useMessageStore} from './message'

export const useChatStore = defineStore('chat', {
    state: () => ({
        currentRecipientId: null as string | null
    }),

    getters: {
        /// <summary>Проверка: выбран ли собеседник</summary>
        isChatSelected: (state) => !!state.currentRecipientId
    },

    actions: {
        /// <summary>Устанавливает текущего собеседника и загружает историю сообщений</summary>
        /// <param name="id">ID пользователя</param>
        async setCurrentRecipient(id: string) {
            this.currentRecipientId = id

            const messageStore = useMessageStore()
            await messageStore.loadMessages(id)
        },

        /// <summary>Сбрасывает активного получателя</summary>
        clearChat() {
            this.currentRecipientId = null
        },

        /// <summary>Проверяет, открыт ли чат с указанным ID</summary>
        /// <param name="id">ID пользователя</param>
        isActive(id: string): boolean {
            return this.currentRecipientId === id
        },

        async fetchMessages(id: string) {
            const messageStore = useMessageStore()
            await messageStore.loadMessages(id)
          }
          
    }
})
