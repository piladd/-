// frontend/src/types/auth.ts
export interface RegisterRequest {
  username:   string
  password:   string
  publicKey:  string
  privateKey: string     // ← добавлено
}

export interface LoginRequest {
  username: string
  password: string
}

export interface AuthResponse {
  token:      string
  userId:     string
  username:   string
  publicKey:  string
  privateKey?: string
}
