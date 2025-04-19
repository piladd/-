<template>
  <div class="chat-list">
    <div
        v-for="chat in chats"
        :key="chat.id"
        class="chat-item"
        :class="{ active: chat.id === selectedId }"
        @click="$emit('select', chat.id)"
    >
      <div class="avatar">
        {{ getInitials(chat.name) }}
      </div>

      <div class="chat-content">
        <div class="top">
          <span class="name">{{ chat.name }}</span>
          <span class="time">{{ formatTime(chat.lastMessage?.sentAt) }}</span>
        </div>
        <div class="last-message">
          {{ chat.lastMessage?.text || 'Без сообщений' }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue'

defineProps<{
  chats: {
    id: string
    name: string
    lastMessage?: {
      text: string
      sentAt: string
    }
  }[]
  selectedId?: string
}>()

defineEmits<{
  (e: 'select', id: string): void
}>()

function getInitials(name: string) {
  return name.slice(0, 2).toUpperCase()
}

function formatTime(time?: string) {
  if (!time) return ''
  const date = new Date(time)
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}
</script>

<style scoped>
.chat-list {
  background: #ffffff;
  border-right: 1px solid #e0e0e0;
  height: 100%;
  overflow-y: auto;
}

.chat-item {
  display: flex;
  gap: 12px;
  padding: 12px 16px;
  cursor: pointer;
  transition: background-color 0.2s ease;
  border-bottom: 1px solid #f0f0f0;
}

.chat-item:hover {
  background-color: #f8f8f8;
}

.chat-item.active {
  background-color: #e6f1ff;
}

.avatar {
  width: 42px;
  height: 42px;
  border-radius: 50%;
  background: #dbeafe;
  color: #1d4ed8;
  font-weight: bold;
  font-size: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.chat-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
  overflow: hidden;
}

.top {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
  margin-bottom: 2px;
}

.name {
  font-weight: bold;
  color: #111827;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.time {
  font-size: 12px;
  color: #6b7280;
  white-space: nowrap;
}

.last-message {
  font-size: 13px;
  color: #6b7280;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
</style>
