export interface MessageDto {
    id: string
    chatId?: string
    senderId: string
    receiverId: string
    senderName: string
    senderAvatar: string | null
    encryptedContent: string | null
    encryptedAesKey: string | null
    iv: string | null
    createdAt: string
    decryptedContent?: string | null
}
export interface SendMessageRequest {
    encryptedContent: string
    encryptedAesKey: string
    iv: string
    content?: string
    type?: number
}
