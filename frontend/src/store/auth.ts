// frontend/src/store/auth.ts
import { defineStore } from 'pinia'
import { login as loginApi, register as registerApi } from '@/services/auth.service'
import { saveToken, removeToken } from '@/utils/token'
import { loadPrivateKey } from '@/utils/crypto'
import type { AuthResponse } from '@/types/auth'
import { fetchMe } from '@/services/user.service'
import wsService from '@/services/ws.service'
import { useMessageStore } from './message'
import type { UserDto } from '@/types/User'

interface AuthState {
  token:       string
  userId:      string | null
  username:    string | null
  publicKey:   string | null
  privateKey:  CryptoKey | null
  isAuthenticated: boolean
  currentUser: UserDto | null
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    token:           localStorage.getItem('token')      || '',
    userId:          localStorage.getItem('userId')     || null,
    username:        localStorage.getItem('username')   || null,
    publicKey:       localStorage.getItem('publicKey')  || null,
    privateKey:      null,
    isAuthenticated: !!localStorage.getItem('token'),
    currentUser:     null
  }),

  actions: {
    /** Логин: сохраняем токен, тянем профиль и стартуем real-time */
    async login(username: string, password: string): Promise<boolean> {
      try {
        const auth: AuthResponse = await loginApi({ username, password })
        // сохраняем всё
        saveToken(auth.token)
        this.token           = auth.token
        this.userId          = auth.userId
        this.username        = auth.username
        this.publicKey       = auth.publicKey
        localStorage.setItem('userId', auth.userId)
        localStorage.setItem('username', auth.username)
        localStorage.setItem('publicKey', auth.publicKey)

        this.privateKey = await loadPrivateKey()

        this.isAuthenticated = true

        await this.loadCurrentUser()

        if (this.currentUser?.id) {
          await wsService.connect(this.currentUser.id, this.token)
          const msgStore = useMessageStore()
          wsService.onMessage(msg => {
            if (msg.senderId === msgStore.currentRecipientId) {
              msgStore.messages.push(msg)
            }
          })
        }

        return true
      } catch (e) {
        console.error('Login error:', e)
        return false
      }
    },

    /** Регистрация: аналогично login */
    async register(username: string, password: string): Promise<boolean> {
      try {
        const auth: AuthResponse = await registerApi({ username, password })
        saveToken(auth.token)
        this.token           = auth.token
        this.userId          = auth.userId
        this.username        = auth.username
        this.publicKey       = auth.publicKey
        localStorage.setItem('userId', auth.userId)
        localStorage.setItem('username', auth.username)
        localStorage.setItem('publicKey', auth.publicKey)

        this.privateKey = await loadPrivateKey()

        this.isAuthenticated = true

        await this.loadCurrentUser()

        if (this.currentUser?.id) {
          await wsService.connect(this.currentUser.id, this.token)
          const msgStore = useMessageStore()
          wsService.onMessage(msg => {
            if (msg.senderId === msgStore.currentRecipientId) {
              msgStore.messages.push(msg)
            }
          })
        }

        return true
      } catch (e) {
        console.error('Register error:', e)
        return false
      }
    },

    /** Получение профиля */
    async loadCurrentUser() {
      try {
        this.currentUser = await fetchMe()
      } catch (error) {
        console.error('Ошибка получения пользователя:', error)
        this.currentUser = null
      }
    },

    /** Логаут */
    async logout() {
      await wsService.disconnect()
      removeToken()
      this.token           = ''
      this.userId          = null
      this.username        = null
      this.publicKey       = null
      this.privateKey      = null
      this.isAuthenticated = false
      this.currentUser     = null
      localStorage.clear()
    }
  }
})
