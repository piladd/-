<template>
  <form @submit.prevent="send" class="input-wrapper">
    <input
        v-model="message"
        type="text"
        placeholder="Введите сообщение"
        class="message-input"
    />
    <button type="submit">➤</button>
  </form>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const message = ref('')

const emit = defineEmits<{
  (e: 'send', text: string): void
}>()

function send() {
  if (message.value.trim()) {
    emit('send', message.value)
    message.value = ''
  }
}
</script>

<style scoped>
.input-wrapper {
  display: flex;
  padding: 8px;
  border-top: 1px solid #ddd;
  background: #fff;
}
.message-input {
  flex: 1;
  padding: 10px;
  font-size: 16px;
  border: none;
  outline: none;
}
button {
  background: #42b983;
  color: white;
  border: none;
  padding: 0 16px;
  cursor: pointer;
  font-size: 18px;
}
</style>
