// 📁 services/websocket.ts
import { useMessageStore } from '@/store/message'
import { useUserStore } from '@/store/user'
import { useChatStore } from '@/store/chat'

let socket: WebSocket | null = null

export function initWebSocket() {
    const messageStore = useMessageStore()
    const userStore = useUserStore()
    const chatStore = useChatStore()

    socket = new WebSocket('ws://localhost:5000/ws')

    socket.onmessage = (event) => {
        const data = JSON.parse(event.data)

        if (data.type === 'message') {
            messageStore.receiveMessage(data)
        }

        if (data.type === 'typing') {
            chatStore.data(data.userId)
        }

        if (data.type === 'online') {
            userStore.setOnlineStatus(data.userId, true)
        }

        if (data.type === 'offline') {
            userStore.setOnlineStatus(data.userId, false)
        }
    }

    socket.onopen = () => {
        const userId = localStorage.getItem('userId')
        if (userId) {
            socket?.send(JSON.stringify({ type: 'online', userId }))
        }
    }

    window.addEventListener('beforeunload', () => {
        const userId = localStorage.getItem('userId')
        if (socket?.readyState === WebSocket.OPEN && userId) {
            socket.send(JSON.stringify({ type: 'offline', userId }))
        }
    })
}

export function sendMessage(text: string, chatId: string, receiverId: string) {
    const userId = localStorage.getItem('userId')
    socket?.send(JSON.stringify({
        type: 'message',
        chatId,
        senderId: userId,
        receiverId,
        text,
        timestamp: new Date().toISOString()
    }))
}

export function sendTyping(chatId: string) {
    const userId = localStorage.getItem('userId')
    socket?.send(JSON.stringify({
        type: 'typing',
        chatId,
        userId
    }))
}