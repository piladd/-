const TOKEN_KEY = 'token'

export function saveToken(token: string) {
    console.log('💾 saveToken got:', token, ' type=', typeof token)
    localStorage.setItem(TOKEN_KEY, token)
  }
  
  export function getToken(): string | null {
    const t = localStorage.getItem(TOKEN_KEY)
    console.log('📥 getToken returns:', t, ' type=', typeof t)
    return t
  }
  

export const removeToken = () => {
    localStorage.removeItem(TOKEN_KEY)
}
