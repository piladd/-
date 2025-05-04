// frontend/src/services/ws.service.ts
import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import type { MessageDto } from "@/types/Message";

class WsService {
    private connection: HubConnection | null = null;

    /**
     * Устанавливает SignalR-соединение.
     * @param userId – текущий userId
     * @param token – JWT-токен
     */
    async connect(userId: string, token: string): Promise<void> {
        if (this.connection) return; // уже подключены

        this.connection = new HubConnectionBuilder()
            .withUrl(`${import.meta.env.VITE_API_URL}/hubs/chat`, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Warning)
            .build();

        this.connection.onreconnecting(err => {
            console.warn("SignalR reconnecting:", err);
        });
        this.connection.onreconnected(cid => {
            console.log("SignalR reconnected, id:", cid);
        });
        this.connection.onclose(err => {
            console.log("SignalR closed:", err);
            this.connection = null;
        });

        await this.connection.start();
        console.log("🟢 SignalR connected");
    }

    /**
     * Отправляет сообщение на сервер.
     */
    async send(payload: {
        chatId: string;
        receiverId: string;
        encryptedContent: string;
        encryptedAesKey: string;
        iv: string;
        content: string;
        type: number;
    }): Promise<void> {
        if (!this.connection) throw new Error("SignalR is not connected");
        await this.connection.invoke(
            "SendMessage",
            payload.chatId,
            payload.receiverId,
            payload.encryptedContent,
            payload.encryptedAesKey,
            payload.iv,
            payload.content,
            payload.type
        );
    }

    /**
     * Подписывает колбэк на все входящие сообщения.
     */
    onMessage(cb: (msg: MessageDto) => void): void {
        if (!this.connection) throw new Error("SignalR is not connected");
        this.connection.on("ReceiveMessage", cb);
    }

    /**
     * Отключается от хаба.
     */
    async disconnect(): Promise<void> {
        if (this.connection) {
            await this.connection.stop();
            this.connection = null;
            console.log("🔴 SignalR disconnected");
        }
    }
}

export default new WsService();
