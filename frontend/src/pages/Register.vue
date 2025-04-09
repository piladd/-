<template>
  <div class="register-page">
    <h2>Регистрация</h2>
    <form @submit.prevent="register">
      <input
          v-model="username"
          placeholder="Введите имя пользователя"
          required
      />
      <button type="submit">Зарегистрироваться</button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/store/user'
import { KeyManagerService } from '@/services/key-manager.service'

const username = ref('')
const router = useRouter()
const userStore = useUserStore()

const register = async () => {
  const trimmed = username.value.trim()
  if (!trimmed) return

  // Сохраняем пользователя (пока в localStorage)
  localStorage.setItem('userId', trimmed)
  userStore.login(trimmed)

  // Генерируем и отправляем ключ
  await KeyManagerService.ensurePrivateKey()

  // Перенаправляем в чат
  await router.push('/chat')
}
</script>

<style scoped>
.register-page {
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
  background-color: #35495e;
  color: white;
  border: none;
  cursor: pointer;
}
</style>
