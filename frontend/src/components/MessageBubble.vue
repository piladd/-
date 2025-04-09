<template>
  <div :class="['bubble', isMine ? 'mine' : 'theirs']">
    {{ text }}
  </div>
</template>

<script setup lang="ts">
defineProps<{
  text: string
  isMine: boolean
}>()
</script>

<style scoped>
.bubble {
  padding: 10px 14px;
  margin: 6px 0;
  border-radius: 12px;
  max-width: 60%;
  word-break: break-word;
}
.mine {
  background-color: #cfe9ff;
  align-self: flex-end;
}
.theirs {
  background-color: #f1f1f1;
  align-self: flex-start;
}
</style>
