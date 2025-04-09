import axios from 'axios'
import type { Chat } from '@/types/Chat'
import type { Message } from '@/types/Message'

const API = 'http://localhost:5000/api/chat'

export async function getChats(): Promise<Chat[]> {
    const response = await axios.get(`${API}/list`)
    return response.data
}

export async function getMessages(chatId: string): Promise<Message[]> {
    const response = await axios.get(`${API}/${chatId}/messages`)
    return response.data
}

export async function sendMessage(chatId: string, text: string): Promise<Message> {
    const response = await axios.post(`${API}/${chatId}/messages`, { text })
    return response.data
}
