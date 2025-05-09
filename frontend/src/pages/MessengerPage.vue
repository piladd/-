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
import ChatList from '@/component/Chat/ChatList.vue'
import ChatWindow from '@/component/Chat/ChatWindow.vue'
import MessageInput from '@/component/Chat/MessageInput.vue'
import MessengerLayout from '@/layouts/MessengerLayout.vue'
</script>
