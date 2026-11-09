import { inject, Service } from '@angular/core'
import { LogResponse } from '../models/log-response'
import { HttpClient } from '@angular/common/http'
import { catchError, map, Observable, of, startWith, Subject, switchMap, tap } from 'rxjs'
import { DateTime } from 'luxon'
import { environment } from '../../../environments/environment'

@Service()
export class BabyLog {
  private readonly url = `${environment.apiUrl}/logs`
  private readonly refresh$ = new Subject<void>()
  private http = inject(HttpClient)

  loadData(id?: string, params?: Record<string, any>): Observable<LogResponse[]> {
    const endpoint = id ? `${this.url}/${id}` : this.url
    return this.refresh$.pipe(
      startWith(undefined),
      switchMap(() =>
        this.http.get<LogResponse[]>(endpoint, params).pipe(
          catchError((error) => {
            console.log(error)
            return of([])
          })
        )
      )
    )
  }

  editData(data: LogResponse) {
    return this.http.put(this.url, data).pipe(
      tap(() => this.refresh$.next()),
      map(() => true),
      catchError((error) => {
        console.log(error)
        return of(false)
      })
    )
  }

  track(type: 'feed' | 'diaper' | 'pump' | 'sleep', data: any) {
    const model: LogResponse = {
      trackerType: type,
      createdAt: DateTime.utc().toISO(),
      updatedAt: DateTime.utc().toISO(),
      timezone: DateTime.local().zoneName,
      ...data
    }
    return this.http.post(this.url, model).pipe(
      tap(() => this.refresh$.next()),
      map(() => true),
      catchError((error) => {
        console.log(error)
        return of(false)
      })
    )
  }

  deleteData(id: string) {
    return this.http.delete(`${this.url}/${id}`).pipe(
      tap(() => this.refresh$.next()),
      map(() => true),
      catchError((error) => {
        console.log(error)
        return of(false)
      })
    )
  }
}
