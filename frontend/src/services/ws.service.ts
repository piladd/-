// frontend/src/services/ws.service.ts
import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import type { MessageDto } from "@/types/Message";

// src/services/ws.service.ts
export class WsService {
    private connection: HubConnection | null = null
  
    async connect(userId: string, token: string) {
      if (this.connection) {
        // уже подключены
        return
      }
      this.connection = new HubConnectionBuilder()
        .withUrl(import.meta.env.VITE_WS_URL!, { accessTokenFactory: () => token })
        .withAutomaticReconnect()
        .build()
  
      await this.connection.start()
    }
  
    // новый метод — чтобы «войти» в группу конкретного чата
    async joinChat(chatId: string) {
      if (!this.connection) throw new Error('SignalR не подключён')
      await this.connection.invoke('JoinChat', chatId)
    }
  
    onMessage(cb: (msg: MessageDto) => void) {
      if (!this.connection) throw new Error('SignalR не подключён')
      // Сервер вызывает этот метод, когда у группы есть новое сообщение
      this.connection.on('ReceiveMessage', (payload: any) => {
        console.log('🔔 WS got:', payload)
        cb(payload as MessageDto)
      })
    }
  
    async send(msg: {
      chatId: string
      receiverId: string
      encryptedContent: string
      encryptedAesKey: string
      iv: string
      type: number
    }) {
      if (!this.connection) throw new Error('SignalR не подключён')
      await this.connection.invoke(
        'SendMessage',
        msg.chatId,
        msg.receiverId,
        msg.encryptedContent,
        msg.encryptedAesKey,
        msg.iv,
        msg.type
      )
    }
  
    async disconnect() {
      await this.connection?.stop()
      this.connection = null
    }
  }
  
  export default new WsService()
  