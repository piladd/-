import {defineStore} from 'pinia'
import {login, register} from '@/services/auth.service'
import {saveToken, getToken, removeToken} from '@/utils/token'
import {fetchMe} from '@/services/user.service'
import type {UserDto} from '@/types/user'

export const useAuthStore = defineStore('auth', {
    state: () => ({
        isAuthenticated: !!getToken(),
        currentUser: null as UserDto | null
    }),

    actions: {
        async login(username: string, password: string) {
            const token = await login({username, password})
            if (token) {
                saveToken(token)
                this.isAuthenticated = true
                await this.loadCurrentUser()
                return true
            }
            return false
        },

        async register(username: string, password: string) {
            const token = await register({username, password})
            if (token) {
                saveToken(token)
                this.isAuthenticated = true
                await this.loadCurrentUser()
                return true
            }
            return false
        },

        async loadCurrentUser() {
            const user = await fetchMe()
            this.currentUser = user
        },

        logout() {
            removeToken()
            this.isAuthenticated = false
            this.currentUser = null
        }
    }
})
