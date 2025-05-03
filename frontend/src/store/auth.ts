// frontend/src/store/auth.ts
import { defineStore } from 'pinia'
import { login as loginApi, register as registerApi } from '@/services/auth.service'
import { saveToken, getToken, removeToken } from '@/utils/token'
import { fetchMe } from '@/services/user.service'
import type { UserDto } from '@/types/user'

interface AuthState {
  isAuthenticated: boolean
  currentUser: UserDto | null
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    isAuthenticated: !!getToken(),
    currentUser: null
  }),

  actions: {
    async login(username: string, password: string): Promise<boolean> {
      try {
        const token = await loginApi({ username, password })
        console.log('💾 login got token:', token, typeof token)
        saveToken(token)
        this.isAuthenticated = true
        await this.loadCurrentUser()
        return true
      } catch (error) {
        console.error('Ошибка входа:', error)
        return false
      }
    },

    async register(username: string, password: string): Promise<boolean> {
      try {
        const token = await registerApi({ username, password })
        console.log('💾 register got token:', token, typeof token)
        saveToken(token)
        this.isAuthenticated = true
        await this.loadCurrentUser()
        return true
      } catch (error) {
        console.error('Ошибка регистрации:', error)
        return false
      }
    },

    async loadCurrentUser() {
      try {
        this.currentUser = await fetchMe()
      } catch (error) {
        console.error('Ошибка получения текущего пользователя:', error)
        this.currentUser = null
      }
    },

    logout() {
      removeToken()
      this.isAuthenticated = false
      this.currentUser = null
    }
  }
})
