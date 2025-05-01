import {defineStore} from 'pinia'
import type {UserDto} from '@/types/user'
import {searchUsers} from '@/services/user.service'

export const useUserStore = defineStore('user', {
    state: () => ({
        users: [] as UserDto[],
        currentUser: null as UserDto | null
    }),

    actions: {
        async fetchUsers(query: string = '') {
            const result = await searchUsers(query)
            this.users = result
        },

        setCurrentUser(user: UserDto) {
            this.currentUser = user
        }
    }
})
