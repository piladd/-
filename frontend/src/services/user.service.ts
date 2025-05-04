import api from './api'
import type {UserDto} from '@/types/User'

export const fetchMe = async (): Promise<UserDto> => {
    const res = await api.get('/api/user/me')
    return res.data
}

export const searchUsers = async (username: string = ''): Promise<UserDto[]> => {
    const res = await api.get('/api/user/search', {
        params: {username}
    })
    return res.data
}

export const updateDisplayName = async (displayName: string) => {
    await api.put('/api/user/display-name', {displayName})
}

export const uploadAvatar = async (file: File) => {
    const form = new FormData()
    form.append('file', file)

    await api.post('/api/user/avatar', form, {
        headers: {'Content-Type': 'multipart/form-data'}
    })
}
export const getPublicKeyPem = async (userId: string): Promise<string> => {
    const response = await api.get(`/api/user/public-key/${userId}`)
    return response.data.publicKey // или response.data — зависит от backend
}

