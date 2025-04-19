<template>
  <div
      class="chat-window chat-drop-zone"
      @dragover.prevent="onDragOver"
      @dragleave.prevent="onDragLeave"
      @drop.prevent="onFileDrop"
      :class="{ 'drag-over': isDragOver }"
  >
    <div v-for="msg in messages" :key="msg.id" class="message">
      <MessageBubble
          :text="msg.text"
          :isMine="msg.isMine"
          :sender="msg.sender"
          :sentAt="msg.sentAt"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import MessageBubble from './MessageBubble.vue'

defineProps<{
  messages: {
    id: string
    text: string
    isMine: boolean
    sender: string
    sentAt: string
  }[]
}>()

const isDragOver = ref(false)

function onDragOver() {
  isDragOver.value = true
}

function onDragLeave() {
  isDragOver.value = false
}

function onFileDrop(event: DragEvent) {
  isDragOver.value = false

  const files = event.dataTransfer?.files
  if (!files || files.length === 0) return

  for (const file of files) {
    uploadFile(file)
  }
}

function uploadFile(file: File) {
  const formData = new FormData()
  formData.append('file', file)

  fetch('/api/files/upload', {
    method: 'POST',
    body: formData
  })
      .then(res => res.json())
      .then(data => {
        console.log('Файл загружен:', data)
        // Здесь можно вызвать emit для отправки ссылки как сообщения
      })
      .catch(console.error)
}
</script>

<style scoped>
.chat-window {
  padding: 1em;
  height: 100%;
  overflow-y: auto;
  position: relative;
}

.chat-drop-zone.drag-over {
  border: 2px dashed #4f46e5;
  background-color: rgba(79, 70, 229, 0.1);
}
</style>
