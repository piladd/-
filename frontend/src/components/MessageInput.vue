<template>
  <form class="input-wrapper" @submit.prevent="handleSend">
    <input
        v-model="text"
        class="input"
        type="text"
        placeholder="Написать сообщение..."
        @dragover.prevent="onDragOver"
        @dragleave.prevent="onDragLeave"
        @drop.prevent="onFileDrop"
    />

    <!-- Предпросмотр файла -->
    <div v-if="file" class="preview-box">
      <img v-if="isImage" :src="previewUrl" class="image-preview" />
      <span v-else class="file-name">{{ file.name }}</span>
      <span class="cancel" @click="clearFile">✖</span>
    </div>

    <!-- Прогресс загрузки -->
    <div v-if="isUploading" class="progress">
      Загрузка: {{ uploadProgress }}%
    </div>

    <button type="submit" class="send-button" :disabled="!text.trim() && !file">
      ➤
    </button>
  </form>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

const text = ref('')
const file = ref<File | null>(null)
const previewUrl = ref('')
const uploadProgress = ref(0)
const isUploading = ref(false)

const emit = defineEmits<{
  (e: 'sendMessage', text: string): void
  (e: 'sendFile', file: File): void
}>()

function handleSend() {
  if (file.value) {
    uploadFile(file.value)
    return
  }

  const value = text.value.trim()
  if (!value) return
  emit('sendMessage', value)
  text.value = ''
}

function uploadFile(f: File) {
  isUploading.value = true
  uploadProgress.value = 0

  const formData = new FormData()
  formData.append('file', f)

  const xhr = new XMLHttpRequest()
  xhr.open('POST', '/api/files/upload')

  xhr.upload.onprogress = (e) => {
    if (e.lengthComputable) {
      uploadProgress.value = Math.round((e.loaded / e.total) * 100)
    }
  }

  xhr.onload = () => {
    isUploading.value = false
    if (xhr.status === 200) {
      const response = JSON.parse(xhr.responseText)
      emit('sendMessage', response.url) // Отправляем как текст
    }
    clearFile()
  }

  xhr.onerror = () => {
    console.error('Ошибка загрузки файла')
    isUploading.value = false
  }

  xhr.send(formData)
}

function clearFile() {
  file.value = null
  previewUrl.value = ''
  uploadProgress.value = 0
}

function onDragOver() {}
function onDragLeave() {}
function onFileDrop(event: DragEvent) {
  const files = event.dataTransfer?.files
  if (!files || files.length === 0) return
  setFile(files[0])
}

function setFile(f: File) {
  file.value = f
  if (f.type.startsWith('image/')) {
    previewUrl.value = URL.createObjectURL(f)
  }
}

const isImage = computed(() => file.value?.type.startsWith('image/'))
</script>

<style scoped>
.input-wrapper {
  display: flex;
  padding: 12px 16px;
  background: #f9f9f9;
  border-top: 1px solid #ddd;
  align-items: flex-end;
  gap: 8px;
  flex-direction: column;
}

.input {
  width: 100%;
  padding: 10px 14px;
  border-radius: 24px;
  border: 1px solid #ccc;
  font-size: 14px;
  outline: none;
}

.send-button {
  background: #4f46e5;
  color: white;
  border: none;
  padding: 10px 14px;
  border-radius: 50%;
  font-size: 16px;
  cursor: pointer;
  align-self: flex-end;
}

.preview-box {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px;
  background: #eee;
  border-radius: 8px;
  width: 100%;
}

.image-preview {
  max-height: 100px;
  border-radius: 6px;
}

.file-name {
  font-size: 14px;
  color: #333;
}

.cancel {
  margin-left: auto;
  cursor: pointer;
  font-size: 16px;
  color: #999;
}

.progress {
  font-size: 12px;
  color: #888;
  align-self: flex-start;
}
</style>
