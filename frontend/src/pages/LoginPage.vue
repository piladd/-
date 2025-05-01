<template>
  <AuthLayout>
    <div class="auth-box">
      <h2>Вход в аккаунт</h2>

      <form @submit.prevent="handleLogin">
        <input
            v-model="username"
            type="text"
            placeholder="Введите логин"
            required
            minlength="3"
        />
        <input
            v-model="password"
            type="password"
            placeholder="Введите пароль"
            required
            minlength="6"
        />

        <button type="submit" :disabled="isLoading">
          {{ isLoading ? 'Загрузка...' : 'Войти' }}
        </button>
      </form>

      <p class="hint">
        Нет аккаунта?
        <router-link to="/register">Зарегистрироваться</router-link>
      </p>

      <p v-if="error" class="error-msg">{{ error }}</p>
    </div>
  </AuthLayout>
</template>

<script setup lang="ts">
import {ref} from 'vue'
import {useRouter} from 'vue-router'
import {useAuthStore} from '@/store/auth'
import AuthLayout from '@/layouts/AuthLayout.vue'

const username = ref('')
const password = ref('')
const isLoading = ref(false)
const error = ref('')
const router = useRouter()
const auth = useAuthStore()

const handleLogin = async () => {
  error.value = ''
  isLoading.value = true

  const success = await auth.login(username.value.trim(), password.value)
  isLoading.value = false

  if (success) {
    router.push('/chat')
  } else {
    error.value = 'Неверный логин или пароль.'
  }
}
</script>

<style scoped>
.auth-box {
  width: 320px;
  background: #2a2e35;
  padding: 32px;
  border-radius: 12px;
  color: white;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.2);
}

h2 {
  margin-bottom: 20px;
  text-align: center;
}

input {
  width: 100%;
  padding: 12px;
  margin-bottom: 16px;
  border-radius: 8px;
  border: none;
  background: #393c43;
  color: white;
}

input::placeholder {
  color: #bbb;
}

button {
  width: 100%;
  padding: 12px;
  border-radius: 8px;
  border: none;
  background: #3399ff;
  font-weight: bold;
  color: white;
  cursor: pointer;
  transition: background 0.3s;
}

button:disabled {
  background: #297ecc;
  cursor: wait;
}

button:hover:not(:disabled) {
  background: #267bd7;
}

.hint {
  margin-top: 14px;
  font-size: 0.9em;
  text-align: center;
  color: #ccc;
}

.error-msg {
  color: #ff7b7b;
  margin-top: 12px;
  text-align: center;
  font-size: 0.9em;
}
</style>
