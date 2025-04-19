<template>
  <div class="chat-room">
    <ChatList :chats="chatStore.chats" :selected-id="chatStore.activeChatId" @select="chatStore.selectChat" />

    <div class="main">
      <div class="messages">
        <MessageBubble
            v-for="msg in formattedMessages"
            :key="msg.id"
            :text="msg.text"
            :sender-id="msg.senderId"
            :sent-at="msg.timestamp"
            :is-mine="msg.isMine"
        />
        <div v-if="typingUser" class="typing-indicator">{{ typingUser }} печатает...</div>
      </div>

      <MessageInput />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import ChatList from '@/components/ChatList.vue'
import MessageBubble from '@/components/MessageBubble.vue'
import MessageInput from '@/components/MessageInput.vue'
import { useChatStore } from '@/store/chat'
import { useMessageStore } from '@/store/message'
import { useUserStore } from '@/store/user'
import { initWebSocket } from '@/services/websocket'

const chatStore = useChatStore()
const messageStore = useMessageStore()
const userStore = useUserStore()

onMounted(() => {
  chatStore.fetchChats()
  userStore.fetchUsers()
  initWebSocket()
})

const typingUser = computed(() => {
  const id = chatStore.typingBy
  return id ? userStore.getUserById(id)?.username : null
})

const formattedMessages = computed(() =>
    messageStore.messages.map(msg => ({
      ...msg,
      isMine: msg.senderId === userStore.currentUser?.id
    }))
)
</script>

<style scoped>
.chat-room {
  display: flex;
  height: 100vh;
}
.main {
  flex: 1;
  display: flex;
  flex-direction: column;
}
.messages {
  flex: 1;
  padding: 1rem;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}
.typing-indicator {
  font-size: 13px;
  color: #888;
  margin-top: 4px;
  font-style: italic;
}
</style>