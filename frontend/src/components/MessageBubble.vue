<template>
  <div :class="['bubble', isMine ? 'mine' : 'theirs']">
    <div class="meta">
      <span class="sender" v-if="!isMine">{{ sender }}</span>
      <span class="time">{{ formattedTime }}</span>
    </div>

    <div class="text">
      <!-- Текст сообщения -->
      <div v-if="text" class="text-content">
        {{ text }}
      </div>

      <!-- Превью изображения -->
      <img
          v-if="isImage"
          :src="fileUrl"
          class="image-preview"
          alt="file image"
      />

      <!-- Скачивание файла -->
      <a
          v-else-if="fileUrl"
          :href="fileUrl"
          target="_blank"
          rel="noopener noreferrer"
          class="file-download"
      >
        📎 Скачать файл
      </a>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  text: string
  isMine: boolean
  sender: string
  sentAt: string
  fileUrl?: string
}>()

const formattedTime = computed(() => {
  const date = new Date(props.sentAt)
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
})

const isImage = computed(() =>
    props.fileUrl?.match(/\.(jpg|jpeg|png|webp|gif)$/i)
)
</script>

<style scoped>
.bubble {
  padding: 10px 14px;
  margin: 6px 0;
  border-radius: 12px;
  max-width: 70%;
  word-break: break-word;
  display: flex;
  flex-direction: column;
}

.mine {
  background-color: #cfe9ff;
  align-self: flex-end;
  text-align: right;
}

.theirs {
  background-color: #f1f1f1;
  align-self: flex-start;
  text-align: left;
}

.meta {
  display: flex;
  justify-content: space-between;
  font-size: 12px;
  margin-bottom: 4px;
  color: #555;
}

.sender {
  font-weight: bold;
}

.text-content {
  margin-bottom: 6px;
  white-space: pre-line;
}

.image-preview {
  max-width: 100%;
  max-height: 200px;
  border-radius: 8px;
  margin-top: 6px;
}

.file-download {
  display: inline-block;
  margin-top: 6px;
  font-size: 14px;
  text-decoration: underline;
  color: #2b6cb0;
}
</style>
