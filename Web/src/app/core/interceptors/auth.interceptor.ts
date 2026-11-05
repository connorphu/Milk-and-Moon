import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http'
import { AuthService } from '../services/auth'
import { inject } from '@angular/core'
import { environment } from '../../../environments/environment'
import { catchError, throwError } from 'rxjs'
import { Router } from '@angular/router'

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router)
  const auth = inject(AuthService)
  const token = auth.token()
  const isApiCall = req.url.startsWith(environment.apiUrl)

  if (!token || !isApiCall) {
    return next(req)
  }

  return next(req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        auth.logout()
        router.navigate(['/login'])
      }
      return throwError(() => error)
    })
  )
}
