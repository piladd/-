import { createRouter, createWebHistory } from 'vue-router'
import Login from '@/pages/Login.vue'
import ChatRoom from '@/pages/ChatRoom.vue'

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: Login },
  { path: '/chat', component: ChatRoom }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
