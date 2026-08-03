import { inject, Service } from '@angular/core';
import { BabyLogModel } from '../models/baby-log';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of, startWith, Subject, switchMap, tap } from 'rxjs';

@Service()
export class BabyLog {
  private readonly url = 'http://localhost:3000/baby-log'
  private readonly refresh$ = new Subject<void>()
  private http = inject(HttpClient)

  loadData(id?: string, params?: Record<string, any>): Observable<BabyLogModel[]> {
    const endpoint = id ? `${this.url}/${id}` : this.url
    return this.refresh$.pipe(
      startWith(undefined),
      switchMap(() => this.http.get<BabyLogModel[]>(endpoint, params).pipe(
        catchError((error) => {
          console.log(error);
          return of([]);
        })
      ))
    )
  }

  editData(data: BabyLogModel) {
    return this.http.put(this.url, data).pipe(
      tap(() => this.refresh$.next()),
      map(() => true),
      catchError((error) => {
        console.log(error);
        return of(false);
      }))
  }

  track(type: 'feed' | 'diaper' | 'pump' | 'sleep', data: any) {
    const model: BabyLogModel = {
      trackerType: type,
      createdAt: new Date(),
      updatedAt: new Date(),
      data: data
    }
    return this.http.post(this.url, model).pipe(
      tap(() => this.refresh$.next()),
      map(() => true),
      catchError((error) => {
        console.log(error);
        return of(false);
      }))
  }

  deleteData(id: string) {
    return this.http.delete(`${this.url}/${id}`).pipe(
      tap(() => this.refresh$.next()),
      map(() => true),
      catchError((error) => {
        console.log(error);
        return of(false);
      })
    )
  }
}
