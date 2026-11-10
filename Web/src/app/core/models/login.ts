export interface LoginRequest {
  email: string
  password: string
}

export interface User {
  id: string
  name: string
  email: string
  createdAt: string
}

export interface LoginResponse {
  user: User
  token: string
}
