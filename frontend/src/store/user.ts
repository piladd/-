import {defineStore} from 'pinia'
import type {UserDto} from '@/types/User'
import {searchUsers} from '@/services/user.service'

export const useUserStore = defineStore('user', {
    state: () => ({
        users: [] as UserDto[],
        currentUser: null as UserDto | null
    }),

    actions: {
        async fetchUsers(query: string = '') {
            this.users = await searchUsers(query)
        },

        setCurrentUser(user: UserDto) {
            this.currentUser = user
        }
    }
})
