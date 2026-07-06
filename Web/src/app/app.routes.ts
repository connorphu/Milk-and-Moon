import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'today',
    pathMatch: 'full'
  },
  {
    path: 'today',
    loadComponent: () => import('./features/today/today-page/today-page').then(m => m.TodayPage)
  },
  {
    path: 'log',
    loadComponent: () => import('./features/log/log-page/log-page').then(m => m.LogPage)
  },
  {
    path: 'trends',
    loadComponent: () => import('./features/trends/trends-page/trends-page').then(m => m.TrendsPage)
  },
  {
    path: 'baby',
    loadComponent: () => import('./features/baby-profile/baby-profile-page/baby-profile-page').then(m => m.BabyProfilePage)
  },
  {
    path: '**',
    redirectTo: 'today'
  }
];
