import api from './api'
import type {MessageDto, SendMessageRequest} from '@/types/Message'

export const getMessages = async (recipientId: string): Promise<MessageDto[]> => {
    const response = await api.get(`/api/chat/history/${recipientId}`)
    return response.data
}

export const sendMessage = async (
    recipientId: string,
    data: SendMessageRequest
): Promise<MessageDto> => {
    const response = await api.post('/api/chat/send', {
        receiverId: recipientId,
        ...data
    })
    return response.data
}
