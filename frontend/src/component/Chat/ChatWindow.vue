<template>
  <div class="chat-window" ref="chatWindow">
    <div v-if="!currentRecipientId" class="empty">
      Выберите пользователя, чтобы начать переписку
    </div>

    <div v-else class="messages">
      <MessageBubble
        v-for="msg in messages"
        :key="msg.id"
        :message="msg"
        :isMine="msg.senderId === currentUser?.id"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick } from 'vue'
import { useMessageStore } from '@/store/message'
import { useUserStore } from '@/store/user'
import MessageBubble from './MessageBubble.vue'

const chatWindow = ref<HTMLElement | null>(null)

const messageStore = useMessageStore()
const userStore = useUserStore()

const currentUser = computed(() => userStore.currentUser)
const currentRecipientId = computed(() => messageStore.currentRecipientId)
const messages = computed(() => messageStore.messages)

watch(messages, async () => {
  await nextTick()
  if (chatWindow.value) {
    chatWindow.value.scrollTop = chatWindow.value.scrollHeight
  }
})
</script>


<style scoped>
.chat-window {
  flex: 1;
  overflow-y: auto;
  padding: 16px;
  display: flex;
  flex-direction: column;
  background: #222428;
}

.empty {
  margin: auto;
  color: #aaa;
  font-size: 1.2em;
  text-align: center;
}

.messages {
  display: flex;
  flex-direction: column;
  gap: 10px;
}
</style>
