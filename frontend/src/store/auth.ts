// frontend/src/store/auth.ts
import { defineStore } from 'pinia'
import { login as loginApi, register as registerApi } from '@/services/auth.service'
import { saveToken, getToken, removeToken } from '@/utils/token'
import { fetchMe } from '@/services/user.service'
import wsService from '@/services/ws.service'
import { useMessageStore } from './message'
import type { UserDto } from '@/types/User'

interface AuthState {
  token: string
  isAuthenticated: boolean
  currentUser: UserDto | null
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    token: getToken() || '',
    isAuthenticated: !!getToken(),
    currentUser: null
  }),

  actions: {
    /** Логин: сохраняем токен, тянем профиль и стартуем real-time */
    async login(username: string, password: string): Promise<boolean> {
      try {
        const token = await loginApi({ username, password })
        console.log('💾 login got token:', token, typeof token)
        saveToken(token)
        this.token = token
        this.isAuthenticated = true

        await this.loadCurrentUser()

        if (this.currentUser?.id) {
          await wsService.connect(this.currentUser.id, token)
          const msgStore = useMessageStore()
          wsService.onMessage((msg) => {
            if (msg.senderId === msgStore.currentRecipientId) {
              msgStore.messages.push(msg)
            }
          })
        }

        return true
      } catch {
        return false
      }
    },

    /** Регистрация: аналогично login */
    async register(username: string, password: string): Promise<boolean> {
      try {
        const token = await registerApi({ username, password })
        console.log('💾 register got token:', token, typeof token)
        saveToken(token)
        this.token = token
        this.isAuthenticated = true

        await this.loadCurrentUser()

        if (this.currentUser?.id) {
          await wsService.connect(this.currentUser.id, token)
          const msgStore = useMessageStore()
          wsService.onMessage((msg) => {
            if (msg.senderId === msgStore.currentRecipientId) {
              msgStore.messages.push(msg)
            }
          })
        }

        return true
      } catch {
        return false
      }
    },

    /** Получение профиля */
    async loadCurrentUser() {
      try {
        this.currentUser = await fetchMe()
      } catch (error) {
        console.error('Ошибка получения текущего пользователя:', error)
        this.currentUser = null
      }
    },

    /** Логаут */
    async logout() {
      await wsService.disconnect()
      removeToken()
      this.token = ''
      this.isAuthenticated = false
      this.currentUser = null
    }
  }
})
