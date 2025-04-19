<template>
  <div class="register-page">
    <h2>Регистрация</h2>
    <form @submit.prevent="register">
      <input
          v-model="username"
          placeholder="Логин"
          required
      />
      <input
          v-model="password"
          type="password"
          placeholder="Пароль"
          required
      />
      <button type="submit">Зарегистрироваться</button>
    </form>

    <p class="login-link">
      Уже есть аккаунт?
      <router-link to="/login">Войти</router-link>
    </p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/store/user'
import { KeyManagerService } from '@/services/key-manager.service'

const username = ref('')
const password = ref('')
const router = useRouter()
const userStore = useUserStore()

const register = async () => {
  const trimmedUsername = username.value.trim()
  const trimmedPassword = password.value.trim()
  if (!trimmedUsername || !trimmedPassword) return

  try {
    await userStore.register(trimmedUsername, trimmedPassword)

    // Генерация ключей (для E2EE)
    await KeyManagerService.ensurePrivateKey()

    await router.push('/chat')
  } catch (e) {
    alert('Ошибка регистрации')
    console.error(e)
  }
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
.login-link {
  margin-top: 10px;
}
</style>
