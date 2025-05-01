<template>
  <form class="login-form" @submit.prevent="onSubmit">
    <input v-model="username" placeholder="Логин" required/>
    <input v-model="password" type="password" placeholder="Пароль" required/>
    <button type="submit">Войти</button>
  </form>
</template>

<script setup lang="ts">
import {ref} from 'vue'
import {useAuthStore} from '@/store/auth'
import {useRouter} from 'vue-router'

const router = useRouter()
const auth = useAuthStore()
const username = ref('')
const password = ref('')

const onSubmit = async () => {
  const success = await auth.login(username.value, password.value)
  if (success) router.push('/chat')
}
</script>

<style scoped>
.login-form {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

input {
  padding: 10px;
  border-radius: 6px;
  border: none;
  background: #393c43;
  color: white;
}

button {
  background: #3399ff;
  color: white;
  border: none;
  padding: 10px;
  border-radius: 6px;
  font-weight: bold;
}
</style>
