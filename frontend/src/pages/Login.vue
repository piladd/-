<template>
  <div class="login-page">
    <h2>Вход</h2>
    <form @submit.prevent="login">
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
      <button type="submit">Войти</button>
    </form>

    <p class="register-link">
      Нет аккаунта?
      <router-link to="/register">Зарегистрироваться</router-link>
    </p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useUserStore } from '@/store/user'
import { useRouter } from 'vue-router'
import { KeyManagerService } from '@/services/key-manager.service'

const username = ref('')
const password = ref('')
const userStore = useUserStore()
const router = useRouter()

const login = async () => {
  const trimmedUsername = username.value.trim()
  const trimmedPassword = password.value.trim()
  if (!trimmedUsername || !trimmedPassword) return

  try {
    await userStore.login(trimmedUsername, trimmedPassword)

    // Генерация ключей после входа
    await KeyManagerService.ensurePrivateKey()

    await router.push('/chat')
  } catch (e) {
    alert('Неверный логин или пароль')
    console.error(e)
  }
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
.register-link {
  margin-top: 10px;
}
</style>
