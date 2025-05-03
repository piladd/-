// frontend/src/types/auth.ts
export interface LoginRequest  { username: string; password: string }
export interface RegisterRequest { username: string; password: string }
export interface AuthResponse {
  token: string
  userId: string
  username: string
}
