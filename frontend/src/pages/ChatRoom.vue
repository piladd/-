<template>
  <div class="chat-room">
    <ChatList class="chat-list" />
    <div class="chat-content">
      <ChatWindow :messages="messages" />
      <MessageInput @send="handleSend" />
    </div>
  </div>
</template>

<script setup lang="ts">
import ChatList from '@/components/ChatList.vue'
import ChatWindow from '@/components/ChatWindow.vue'
import MessageInput from '@/components/MessageInput.vue'
import { ref } from 'vue'
import type { Message } from '@/types/Message'

const messages = ref<Message[]>([
  {
    id: '1',
    text: 'Привет!',
    timestamp: new Date().toLocaleTimeString(),
    isMine: false
  },
  {
    id: '2',
    text: 'Как дела?',
    timestamp: new Date().toLocaleTimeString(),
    isMine: true
  }
])

const handleSend = (text: string) => {
  messages.value.push({
    id: crypto.randomUUID(),
    text,
    timestamp: new Date().toLocaleTimeString(),
    isMine: true
  })
}
</script>

<style scoped>
.chat-room {
  display: flex;
  height: 100vh;
}
.chat-list {
  width: 250px;
  border-right: 1px solid #ddd;
}
.chat-content {
  flex: 1;
  display: flex;
  flex-direction: column;
}
</style>
