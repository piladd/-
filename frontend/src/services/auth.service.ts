import axios from 'axios'

const API = 'http://localhost:5000/api/auth'

export async function login(username: string) {
    const response = await axios.post(`${API}/login`, { username })
    return response.data
}

export async function logout() {
    await axios.post(`${API}/logout`)
}
