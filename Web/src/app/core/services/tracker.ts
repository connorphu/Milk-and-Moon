import { inject, Service } from '@angular/core'
import { ApiResult, LogRequest, LogResponse } from '../models/log-contracts'
import { HttpClient, HttpErrorResponse } from '@angular/common/http'
import { catchError, map, Observable, of, startWith, Subject, switchMap, tap } from 'rxjs'
import { DateTime } from 'luxon'
import { environment } from '../../../environments/environment'

@Service()
export class TrackerService {
  private readonly logsUrl = `${environment.apiUrl}/logs`
  private readonly pumpUrl = `${environment.apiUrl}/pump-logs`
  private readonly babyUrl = `${environment.apiUrl}/babies`
  private readonly refresh$ = new Subject<void>()
  private http = inject(HttpClient)

  getAllLogs(params?: Record<string, any>): Observable<ApiResult<LogResponse[]>> {
    return this.refresh$.pipe(
      startWith(undefined),
      switchMap(() =>
        this.http.get<LogResponse[]>(this.logsUrl, { params }).pipe(
          map((data) => ({ result: 'ok', data }) as const),
          catchError((error: HttpErrorResponse) => of({ result: 'error', error } as const))
        )
      )
    )
  }

  editLog(log: LogResponse): Observable<ApiResult<LogResponse>> {
    const endpoint =
      log.trackerType === 'pump'
        ? `${this.pumpUrl}/${log.id}`
        : `${this.babyUrl}/${log.babyId}/${log.trackerType}-logs/${log.id}`
    return this.http.put<LogResponse>(endpoint, log).pipe(
      tap(() => this.refresh$.next()),
      map((data) => ({ result: 'ok', data }) as const),
      catchError((error: HttpErrorResponse) => of({ result: 'error', error } as const))
    )
  }

  track(type: 'feed' | 'diaper' | 'pump' | 'sleep', babyId: string, data: LogRequest) {
    const endpoint = type === 'pump' ? this.pumpUrl : `${this.babyUrl}/${babyId}/${type}-logs`
    const request = { ...data, timezone: DateTime.local().zoneName }
    return this.http.post(endpoint, request).pipe(
      tap(() => this.refresh$.next()),
      map((log) => ({ result: 'ok', log }) as const),
      catchError((error: HttpErrorResponse) => of({ result: 'error', error } as const))
    )
  }

  deleteLog(log: LogResponse): Observable<ApiResult<void>> {
    const endpoint =
      log.trackerType === 'pump'
        ? `${this.pumpUrl}/${log.id}`
        : `${this.babyUrl}/${log.babyId}/${log.trackerType}-logs/${log.id}`
    return this.http.delete(endpoint).pipe(
      tap(() => this.refresh$.next()),
      map(() => ({ result: 'ok' }) as const),
      catchError((error: HttpErrorResponse) => of({ result: 'error', error } as const))
    )
  }
}
