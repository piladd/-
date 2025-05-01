import {createRouter, createWebHistory} from 'vue-router'
import ChatRoom from "@/pages/ChatRoom.vue"
import Login from "@/pages/LoginPage.vue"
import RegisterPage from "@/pages/RegisterPage.vue" // ✅ добавляем

const routes = [
    {path: '/', redirect: '/login'},
    {path: '/login', component: Login},
    {path: '/register', component: RegisterPage}, // ✅ маршрут регистрации
    {path: '/chat', component: ChatRoom}
]

export default createRouter({
    history: createWebHistory(),
    routes
})
