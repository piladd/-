let socket: WebSocket | null = null

export const connectToWebSocket = (userId: string) => {
    const url = `ws://${window.location.host}/hub/chat?userId=${userId}`
    socket = new WebSocket(url)

    socket.onopen = () => {
        console.log('✅ WebSocket подключён')
    }

    socket.onmessage = (event) => {
        const data = JSON.parse(event.data)
        console.log('📨 WS Message:', data)

        // Тут можно вставить push в messageStore
    }

    socket.onerror = (err) => {
        console.error('❌ WS ошибка:', err)
    }

    socket.onclose = () => {
        console.warn('🔌 WS отключён')
    }
}

export const disconnectWebSocket = () => {
    if (socket) socket.close()
}
