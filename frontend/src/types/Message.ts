export interface MessageDto {
    id: string
    senderId: string
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
}
