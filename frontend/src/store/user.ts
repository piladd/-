// 📁 store/user.ts
import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'

export interface User {
    id: string
    username: string
    avatarUrl?: string
    isOnline?: boolean
}

export const useUserStore = defineStore('user', () => {
    const currentUser = ref<User | null>(null)
    const allUsers = ref<User[]>([])

    async function login(username: string, password: string) {
        const res = await axios.post('/api/auth/login', { username, password })
        currentUser.value = res.data.user
        localStorage.setItem('userId', res.data.user.id)
        axios.defaults.headers.common['Authorization'] = `Bearer ${res.data.token}`
    }

    async function register(username: string, password: string) {
        const res = await axios.post('/api/auth/register', { username, password })
        currentUser.value = res.data.user
        localStorage.setItem('userId', res.data.user.id)
        axios.defaults.headers.common['Authorization'] = `Bearer ${res.data.token}`
    }

    async function fetchUsers() {
        const res = await axios.get<User[]>('/api/users')
        allUsers.value = res.data
    }

    function getUserById(id: string): User | undefined {
        return allUsers.value.find(u => u.id === id)
    }

    function setOnlineStatus(userId: string, online: boolean) {
        const user = allUsers.value.find(u => u.id === userId)
        if (user) user.isOnline = online
    }

    return {
        currentUser,
        allUsers,
        login,
        register,
        fetchUsers,
        getUserById,
        setOnlineStatus
    }
})
