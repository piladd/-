export interface Message {
    id: string
    chatId: string
    senderId: string
    text: string
    timestamp: string
    isMine: boolean
}
