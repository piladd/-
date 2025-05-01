import {MessageDto} from "@/types/message";

export interface ChatDto {
    id: string
    participants: string[] // [userId1, userId2]
    lastMessage?: MessageDto
}
