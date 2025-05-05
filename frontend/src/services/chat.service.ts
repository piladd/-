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
    // ✅ Проверка перед отправкой
    if (!data.encryptedAesKey || !data.encryptedContent || !data.iv) {
        console.error('❌ Ошибка: один из параметров зашифровки отсутствует', data)
        throw new Error('Encrypted message data is incomplete')
    }

    console.log('📤 Отправка зашифрованного сообщения:', {
        receiverId: recipientId,
        ...data
    })

    const response = await api.post('/api/chat/send', {
        receiverId: recipientId,
        encryptedContent: data.encryptedContent,
        encryptedAesKey: data.encryptedAesKey,
        iv: data.iv,
        content: data.content ?? '',
        type: data.type ?? 0
    })
    return response.data
}