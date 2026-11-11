import { inject, Service } from '@angular/core'
import { ApiResult, LogRequest, LogResponse, LogsResult } from '../models/log-contracts'
import { HttpClient, HttpErrorResponse } from '@angular/common/http'
import { catchError, map, Observable, of, startWith, Subject, switchMap, tap } from 'rxjs'
import { DateTime } from 'luxon'
import { environment } from '../../../environments/environment'

type AllLogsParams = {
  startDate: string
  endDate?: string
  timezone: string
}

@Service()
export class TrackerService {
  private readonly logsUrl = `${environment.apiUrl}/logs`
  private readonly pumpUrl = `${environment.apiUrl}/pump-logs`
  private readonly babyUrl = `${environment.apiUrl}/babies`
  private readonly refresh$ = new Subject<void>()
  private http = inject(HttpClient)

  private url(log: LogResponse) {
    return log.trackerType === 'pump'
      ? `${this.pumpUrl}/${log.id}`
      : `${this.babyUrl}/${log.babyId}/${log.trackerType}-logs/${log.id}`
  }

  getAllLogs(params: AllLogsParams): Observable<LogsResult> {
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
    const endpoint = this.url(log)
    return this.http.put<LogResponse>(endpoint, log).pipe(
      tap(() => this.refresh$.next()),
      map((data) => ({ result: 'ok', data }) as const),
      catchError((error: HttpErrorResponse) => of({ result: 'error', error } as const))
    )
  }

  track(
    type: 'feed' | 'diaper' | 'pump' | 'sleep',
    request: LogRequest,
    babyId?: string
  ): Observable<ApiResult<LogResponse>> {
    const endpoint = type === 'pump' ? this.pumpUrl : `${this.babyUrl}/${babyId}/${type}-logs`
    const body = { ...request, timezone: DateTime.local().zoneName }
    return this.http.post<LogResponse>(endpoint, body).pipe(
      tap(() => this.refresh$.next()),
      map((data) => ({ result: 'ok', data }) as const),
      catchError((error: HttpErrorResponse) => of({ result: 'error', error } as const))
    )
  }

  deleteLog(log: LogResponse): Observable<ApiResult<void>> {
    const endpoint = this.url(log)
    return this.http.delete(endpoint).pipe(
      tap(() => this.refresh$.next()),
      map(() => ({ result: 'ok', data: undefined }) as const),
      catchError((error: HttpErrorResponse) => of({ result: 'error', error } as const))
    )
  }
}
