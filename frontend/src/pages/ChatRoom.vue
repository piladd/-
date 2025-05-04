<template>
  <MessengerLayout>
    <template #sidebar>
      <ChatList/>
    </template>
    <template #main>
      <ChatWindow/>
      <MessageInput/>
    </template>
  </MessengerLayout>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useAuthStore } from '@/store/auth'
import wsService from '@/services/ws.service'

import MessengerLayout from '@/layouts/MessengerLayout.vue'
import ChatList from '@/component/Chat/ChatList.vue'
import ChatWindow from '@/component/Chat/ChatWindow.vue'
import MessageInput from '@/component/Chat/MessageInput.vue'

const auth = useAuthStore()

onMounted(async () => {
  if (auth.currentUser?.id && auth.token) {
    try {
      await wsService.connect(auth.currentUser.id, auth.token)
      // по желанию можно сразу подписаться на входящие здесь,
      // либо это уже настроено в store/auth при логине
    } catch (e) {
      console.error('Не удалось подключиться к SignalR:', e)
    }
  }
})
</script>
