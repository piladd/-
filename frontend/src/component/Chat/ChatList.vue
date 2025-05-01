<template>
  <div class="chat-list">
    <div
        v-for="user in users"
        :key="user.id"
        :class="['chat-item', { active: user.id === selectedUserId }]"
        @click="selectUser(user.id)"
    >
      <img :src="user.avatarUrl || defaultAvatar" class="avatar"/>
      <div class="info">
        <div class="name">{{ user.username }}</div>
        <div class="status">{{ user.online ? '🟢 В сети' : '🔴 Не в сети' }}</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import {onMounted, computed} from 'vue'
import {useUserStore} from '@/store/user'
import {useChatStore} from '@/store/chat'
import defaultAvatar from '@/assets/default-avatar.png'

const userStore = useUserStore()
const chatStore = useChatStore()

const users = computed(() => userStore.users)
const selectedUserId = computed(() => chatStore.currentRecipientId)

const selectUser = (id: string) => {
  chatStore.setCurrentRecipient(id)
  chatStore.fetchMessages(id)
}

onMounted(() => {
  userStore.fetchUsers()
})
</script>

<style scoped>
.chat-list {
  display: flex;
  flex-direction: column;
  padding: 16px;
}

.chat-item {
  display: flex;
  align-items: center;
  padding: 12px;
  border-radius: 10px;
  transition: background 0.2s;
  cursor: pointer;
  margin-bottom: 8px;
}

.chat-item:hover {
  background: #393c41;
}

.chat-item.active {
  background: #4a4d55;
}

.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
  margin-right: 12px;
}

.info {
  display: flex;
  flex-direction: column;
}

.name {
  font-weight: bold;
}

.status {
  font-size: 0.8em;
  color: #aaa;
}
</style>
