<template>
  <form class="message-input" @submit.prevent="handleSend">
    <input
        v-model="message"
        type="text"
        placeholder="Написать сообщение..."
        @keydown.enter.exact.prevent="handleSend"
    />

    <label class="upload-btn">
      📎
      <input type="file" hidden @change="handleFileChange" />
    </label>

    <button type="submit" :disabled="!message.trim()">➡</button>
  </form>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useMessageStore } from '@/store/message'
import { useChatStore } from '@/store/chat'
import { encryptForRecipient } from '@/utils/crypto'

const message = ref('')
const messageStore = useMessageStore()
const chatStore    = useChatStore()

/**
 * При отправке:
 * 1) шифруем текст для получателя
 * 2) POST → /api/chat/send
 * 3) пушим локально и отправляем по WS
 */
const handleSend = async () => {
  const text        = message.value.trim()
  const recipientId = chatStore.currentRecipientId
  if (!text || !recipientId) return

  try {
    // шифруем
    const encrypted = await encryptForRecipient(recipientId, text)

    // отправляем на REST
    await messageStore.sendEncryptedMessage(recipientId, {
      ...encrypted,
      content: text,
      type: 0
    })

    // очищаем поле
    message.value = ''
  } catch (e: any) {
    // если у получателя нет ключа на сервере → 404
    if (e.response?.status === 404 || e.code === 404) {
      alert('❌ У получателя нет публичного ключа. Нельзя зашифровать сообщение.')
    } else {
      console.error('Ошибка при шифровании/отправке:', e)
      alert('❌ Не удалось отправить сообщение.')
    }
  }
}

/**
 * Файловые вложения шифруются в другой функции
 */
const handleFileChange = (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (file && chatStore.currentRecipientId) {
    messageStore.sendEncryptedAttachment(chatStore.currentRecipientId, file)
  }
}
</script>

<style scoped>
.message-input {
  display: flex;
  align-items: center;
  padding: 12px;
  background: #2a2c31;
  border-top: 1px solid #444;
}

input[type="text"] {
  flex: 1;
  padding: 10px 12px;
  border-radius: 8px;
  background: #393b40;
  border: none;
  color: white;
  margin-right: 10px;
}

input[type="text"]::placeholder {
  color: #aaa;
}

.upload-btn {
  font-size: 1.4em;
  cursor: pointer;
  margin-right: 10px;
}

button {
  background: #3399ff;
  color: white;
  border: none;
  padding: 8px 14px;
  border-radius: 8px;
  font-weight: bold;
  cursor: pointer;
  transition: background 0.3s;
}

button:hover:not(:disabled) {
  background: #2788e6;
}

button:disabled {
  opacity: 0.5;
  cursor: default;
}
</style>
