<template>
  <form class="register-form" @submit.prevent="handleRegister">
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
      {{ isLoading ? 'Загрузка...' : 'Создать аккаунт' }}
    </button>

    <p class="hint">
      Уже есть аккаунт?
      <router-link to="/login">Войти</router-link>
    </p>

    <p v-if="error" class="error-msg">{{ error }}</p>
  </form>
</template>

<script setup lang="ts">
import {ref} from 'vue'
import {useRouter} from 'vue-router'
import {useAuthStore} from '@/store/auth'
import { generateAndUploadKeyPair } from '@/utils/crypto'

const username = ref('')
const password = ref('')
const isLoading = ref(false)
const error = ref('')
const router = useRouter()
const auth = useAuthStore()


const handleRegister = async () => {
  error.value     = ''
  isLoading.value = true

  const success = await auth.register(username.value.trim(), password.value)
  isLoading.value = false

  if (success) {
    await generateAndUploadKeyPair()
    router.push('/chat')
  } else {
    error.value = 'Не удалось зарегистрироваться. Попробуйте снова.'
  }
}

</script>

<style scoped>
.register-form {
  width: 320px;
  background: #2a2e35;
  padding: 32px;
  border-radius: 12px;
  color: white;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.2);
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
  background: #33cc66;
  font-weight: bold;
  color: white;
  cursor: pointer;
  transition: background 0.3s;
}

button:disabled {
  background: #2d9e51;
  cursor: wait;
}

button:hover:not(:disabled) {
  background: #28b85c;
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
