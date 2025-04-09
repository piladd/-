<template>
  <div class="chat-list">
    <div
        v-for="chat in chats"
        :key="chat.id"
        class="chat-item"
        @click="$emit('select', chat.id)"
    >
      {{ chat.name }}
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  chats: { id: string; name: string }[]
}>()

defineEmits<{
  (e: 'select', id: string): void
}>()
</script>

<style scoped>
.chat-list {
  background: #fff;
  border-right: 1px solid #ddd;
  height: 100%;
}
.chat-item {
  padding: 12px;
  cursor: pointer;
}
.chat-item:hover {
  background: #f5f5f5;
}
</style>
