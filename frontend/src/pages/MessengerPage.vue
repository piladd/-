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
import ChatList from '@/components/Chat/ChatList.vue'
import ChatWindow from '@/components/Chat/ChatWindow.vue'
import MessageInput from '@/components/Chat/MessageInput.vue'
import MessengerLayout from '@/layouts/MessengerLayout.vue'
</script>
