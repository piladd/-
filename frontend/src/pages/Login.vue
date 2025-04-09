<template>
  <div class="login-page">
    <h2>Вход</h2>
    <form @submit.prevent="login">
      <input
          v-model="username"
          placeholder="Введите имя пользователя"
          required
      />
      <button type="submit">Войти</button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useUserStore } from '@/store/user'
import { useRouter } from 'vue-router'
import { KeyManagerService } from '@/services/key-manager.service'

const username = ref('')
const userStore = useUserStore()
const router = useRouter()

const login = async () => {
  const trimmed = username.value.trim()
  if (!trimmed) return

  // Сохраняем локально
  localStorage.setItem('userId', trimmed)
  userStore.login(trimmed)

  // Генерация ключей (если нужно)
  await KeyManagerService.ensurePrivateKey()

  // Редирект
  await router.push('/chat')
}
</script>

<style scoped>
.login-page {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-top: 20vh;
}
form {
  display: flex;
  flex-direction: column;
  width: 300px;
}
input {
  padding: 10px;
  margin-bottom: 12px;
  font-size: 16px;
}
button {
  padding: 10px;
  font-size: 16px;
  background-color: #42b983;
  color: white;
  border: none;
  cursor: pointer;
}
</style>
