import { Service, signal } from '@angular/core'

@Service()
export class AuthService {
  public isLoggedIn = signal<boolean>(false)
}
