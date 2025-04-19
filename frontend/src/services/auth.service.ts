import axios from 'axios'

const API = 'http://localhost:5000/api/auth'

export async function login(username: string, password: string) {
    const response = await axios.post(`${API}/login`, { username, password })
    return response.data // { token, user }
}

export async function register(username: string, password: string) {
    const response = await axios.post(`${API}/register`, { username, password })
    return response.data
}

export async function logout() {
    await axios.post(`${API}/logout`)
}
