export interface LoginRequest  { username: string; password: string }
export interface RegisterRequest {
  username:  string
  password:  string
  publicKey: string   // добавляем
}
export interface AuthResponse {
  token:     string
  userId:    string
  username:  string
  publicKey: string    // добавляем
}
