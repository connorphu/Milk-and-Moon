import { Component, DestroyRef, inject, signal } from '@angular/core'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatInputModule } from '@angular/material/input'
import { MatButtonModule } from '@angular/material/button'
import { Router, RouterLink } from '@angular/router'
import { RegisterRequest } from '../../../core/models/register'
import { email, form, FormField, minLength, required } from '@angular/forms/signals'
import { AuthService } from '../../../core/services/auth'
import { HttpErrorResponse } from '@angular/common/http'
import { throwError } from 'rxjs'
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'

@Component({
  selector: 'app-register-page',
  imports: [MatFormFieldModule, MatButtonModule, MatInputModule, RouterLink, FormField],
  templateUrl: './register-page.html',
  styleUrl: './register-page.css'
})
export class RegisterPageComponent {
  private readonly destroyRef = inject(DestroyRef)

  readonly registerModel = signal<RegisterRequest>({
    name: '',
    email: '',
    password: ''
  })

  protected registerForm = form(this.registerModel, (input) => {
    required(input.name, { message: 'Name is required' })
    required(input.email, { message: 'Email is required' })
    email(input.email, { message: 'Enter a valid email address' })
    required(input.password, { message: 'Password is required' })
    minLength(input.password, 8, { message: 'Password must be at least 8 characters long' })
  })

  private authService = inject(AuthService)
  private router = inject(Router)

  onCreateAccount(event: Event) {
    event.preventDefault()
    this.authService
      .register(this.registerForm().value())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => this.router.navigate(['/today']),
        error: (err: HttpErrorResponse) => throwError(err)
      })
  }
}
