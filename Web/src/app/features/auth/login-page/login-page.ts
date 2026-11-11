import { Component, inject, signal } from '@angular/core'
import { AuthService } from '../../../core/services/auth'
import { LoginRequest } from '../../../core/models/login'
import { email, form, FormField, required } from '@angular/forms/signals'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatInputModule } from '@angular/material/input'
import { Router, RouterLink } from '@angular/router'
import { MatButtonModule } from '@angular/material/button'
import { throwError } from 'rxjs'
import { HttpErrorResponse } from '@angular/common/http'

@Component({
  selector: 'app-login-page',
  templateUrl: 'login-page.html',
  styleUrl: 'login-page.css',
  imports: [MatFormFieldModule, MatInputModule, FormField, RouterLink, MatButtonModule]
})
export class LoginPageComponent {
  private authService = inject(AuthService)
  private router = inject(Router)

  readonly loginData = signal<LoginRequest>({
    email: '',
    password: ''
  })

  protected loginForm = form(this.loginData, (input) => {
    required(input.email, { message: 'Email is required' })
    email(input.email, { message: 'Enter a valid email address' })
    required(input.password, { message: 'Password is required' })
  })

  onLoginAccount(event: SubmitEvent) {
    event.preventDefault()
    this.authService.login(this.loginData()).subscribe({
      next: () => {
        this.router.navigate(['/today'])
      },
      error: (err: HttpErrorResponse) => throwError(err)
    })
  }
}
