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
import {onMounted} from 'vue'
import {useAuthStore} from '@/store/auth'
import {connectToWebSocket} from '@/services/ws.service'

import MessengerLayout from '@/layouts/MessengerLayout.vue'
import ChatList from '@/component/Chat/ChatList.vue'
import ChatWindow from '@/component/Chat/ChatWindow.vue'
import MessageInput from '@/component/Chat/MessageInput.vue'

const auth = useAuthStore()

onMounted(() => {
  if (auth.currentUser?.id) {
    connectToWebSocket(auth.currentUser.id)
  }
})
</script>
