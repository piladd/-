<template>
  <div class="chat-window">
    <div v-for="msg in messages" :key="msg.id" class="message">
      <MessageBubble :text="msg.text" :isMine="msg.isMine" />
    </div>
  </div>
</template>

<script setup lang="ts">
import MessageBubble from './MessageBubble.vue'

defineProps<{
  messages: { id: string; text: string; isMine: boolean }[]
}>()
</script>

<style scoped>
.chat-window {
  padding: 1rem;
  height: 100%;
  overflow-y: auto;
}
</style>
