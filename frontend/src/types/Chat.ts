import type { User } from './User'

export interface Chat {
    id: string
    name: string
    isGroup: boolean
    participants: User[]
}
