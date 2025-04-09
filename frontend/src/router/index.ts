import { createRouter, createWebHistory } from 'vue-router'

import Home from '@/pages/Home.vue'
import Login from '@/pages/Login.vue'
import ChatRoom from '@/pages/ChatRoom.vue'
import Register from '@/pages/Register.vue'


const routes = [
    { path: '/', name: 'Home', component: Home },
    { path: '/login', name: 'Login', component: Login },
    { path: '/chat', name: 'ChatRoom', component: ChatRoom },
    { path: '/register', name: 'Register', component: Register },

]

const router = createRouter({
    history: createWebHistory(),
    routes,
})

export default router
