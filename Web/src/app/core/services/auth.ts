import { computed, inject, Service, signal } from '@angular/core'
import { environment } from '../../../environments/environment'
import { HttpClient } from '@angular/common/http'
import { RegisterData } from '../models/register'
import { LoginData, LoginResponse } from '../models/login'
import { switchMap, tap } from 'rxjs'

@Service()
export class AuthService {
  private readonly url = `${environment.apiUrl}/auth`
  private http = inject(HttpClient)
  private _token = signal<string>('')
  readonly token = this._token.asReadonly()
  readonly isLoggedIn = computed(() => !!this._token())

  register(data: RegisterData) {
    return this.http
      .post(`${this.url}/register`, data)
      .pipe(switchMap(() => this.login({ email: data.email, password: data.password })))
  }

  login(data: LoginData) {
    return this.http.post<LoginResponse>(`${this.url}/login`, data).pipe(
      tap((res) => {
        this._token.set(res.token)
      })
    )
  }

  logout() {
    this._token.set('')
  }
}
