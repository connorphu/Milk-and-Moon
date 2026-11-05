import { Routes } from '@angular/router'
import { authGuard, guestGuard } from './core/guards/auth.guard'

export const routes: Routes = [
  {
    path: '',
    canMatch: [guestGuard],
    children: []
  },
  {
    path: '',
    canMatch: [authGuard],
    children: [
      {
        path: 'today',
        loadComponent: () =>
          import('./features/today/today-page/today-page').then((m) => m.TodayPage)
      },
      {
        path: 'trends',
        loadComponent: () =>
          import('./features/trends/trends-page/trends-page').then((m) => m.TrendsPage)
      },
      {
        path: 'baby',
        loadComponent: () =>
          import('./features/baby-profile/baby-profile-page/baby-profile-page').then(
            (m) => m.BabyProfilePage
          )
      },
      {
        path: '',
        redirectTo: 'today',
        pathMatch: 'full'
      },
      {
        path: '**',
        redirectTo: 'today'
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
]
