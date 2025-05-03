// frontend/src/services/ws.service.ts

class WsService {
    private socket: WebSocket | null = null
  
    connect(userId: string): void {
      const protocol = window.location.protocol === 'https:' ? 'wss' : 'ws'
      const host = window.location.host
      const url = `${protocol}://${host}/hub/chat?userId=${userId}`
  
      this.socket = new WebSocket(url)
  
      this.socket.onopen = () => {
        console.log('🟢 WS подключен:', url)
      }
  
      this.socket.onerror = (event: Event) => {
        console.error('❌ WS ошибка:', event)
      }
  
      this.socket.onclose = (event: CloseEvent) => {
        console.log(`🔌 WS отключен (код ${event.code}, причина: ${event.reason})`)
      }
  
      this.socket.onmessage = (msgEvent: MessageEvent) => {
        try {
          const data = JSON.parse(msgEvent.data)
          console.log('📨 WS сообщение:', data)
          // TODO: передать дальше полученные данные
        } catch {
          console.warn('Не смог распарсить WS-сообщение:', msgEvent.data)
        }
      }
    }
  
    send(message: any): void {
      if (this.socket && this.socket.readyState === WebSocket.OPEN) {
        this.socket.send(JSON.stringify(message))
      } else {
        console.error('WS не подключен — сообщение не отправлено:', message)
      }
    }
  
    disconnect(): void {
      if (this.socket) {
        this.socket.close()
        this.socket = null
      }
    }
  }
  
  const wsService = new WsService()
  
  /**
   * Открыть WS-соединение
   */
  export function connectToWebSocket(userId: string): void {
    wsService.connect(userId)
  }
  
  /**
   * Отправить сообщение по WS
   */
  export function sendWsMessage(message: any): void {
    wsService.send(message)
  }
  
  /**
   * Закрыть WS-соединение
   */
  export function disconnectWebSocket(): void {
    wsService.disconnect()
  }
  
  // Если кому-то нужен сам сервис
  export default wsService
  