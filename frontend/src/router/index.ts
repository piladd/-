import { createRouter, createWebHistory } from 'vue-router'
import ChatRoom from "@/pages/ChatRoom.vue";
import Login from "@/pages/Login.vue";

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: Login },
  { path: '/chat', component: ChatRoom }
]

export default createRouter({
  history: createWebHistory(),
  routes
})
