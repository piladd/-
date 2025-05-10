// frontend/src/services/ws.service.ts
import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import type { MessageDto } from "@/types/Message";

export class WsService {
    private connection: HubConnection | null = null;

    async connect(userId: string, token: string) {
        if (this.connection) return;

        const url = import.meta.env.VITE_WS_URL!;
        console.log("🛰️ SignalR connecting to:", url);
        if (!url) throw new Error("VITE_WS_URL не задан в .env");

        this.connection = new HubConnectionBuilder()
            .withUrl(url, { accessTokenFactory: () => token })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        await this.connection.start();
        console.log("🛰️ SignalR connected");
    }

    async joinChat(chatId: string) {
        if (!this.connection) throw new Error("SignalR не подключён");
        await this.connection.invoke("JoinChat", chatId);
    }

    onMessage(cb: (msg: MessageDto) => void) {
        if (!this.connection) throw new Error("SignalR не подключён")

        this.connection.on("ReceiveMessage", payload => {
            console.log("🔔 WS got (ReceiveMessage):", payload);
            cb(payload as MessageDto);
        });
    }

    async send(msg: {
        chatId: string;
        receiverId: string;
        encryptedContent: string;
        encryptedAesKey: string;
        iv: string;
        type: number;
    }) {
        if (!this.connection) throw new Error("SignalR не подключён");

        // Вызываем SendMessage с ровно СЕСТЬЮ аргументами,
        // как у вас теперь на сервере:
        await this.connection.invoke(
            "SendMessage",
            msg.chatId,
            msg.receiverId,
            msg.encryptedContent,
            msg.encryptedAesKey,
            msg.iv,
            msg.type
        );
    }

    async disconnect() {
        await this.connection?.stop();
        this.connection = null;
    }
}

export default new WsService();
