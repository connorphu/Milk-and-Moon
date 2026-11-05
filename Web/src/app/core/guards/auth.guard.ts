import { CanMatchFn, Router } from '@angular/router'
import { AuthService } from '../services/auth'
import { inject } from '@angular/core'

export const authGuard: CanMatchFn = () => {
  const auth = inject(AuthService)
  return auth.isLoggedIn()
}

export const guestGuard: CanMatchFn = () => {
  const auth = inject(AuthService)
  return !auth.isLoggedIn()
}
