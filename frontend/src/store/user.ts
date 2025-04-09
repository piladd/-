import { defineStore } from 'pinia'

export const useUserStore = defineStore('user', {
    state: () => ({
        username: '' as string,
        isAuthenticated: false,
    }),
    actions: {
        login(name: string) {
            this.username = name
            this.isAuthenticated = true
        },
        logout() {
            this.username = ''
            this.isAuthenticated = false
        },
    },
})
