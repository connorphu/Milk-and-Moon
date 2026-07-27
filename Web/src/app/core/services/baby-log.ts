import { inject, Service } from '@angular/core';
import { BabyLogModel } from '../models/baby-log';
import { HttpClient } from '@angular/common/http';
import { catchError, map, of } from 'rxjs';

@Service()
export class BabyLog {
  private readonly url = 'http://localhost:3000/baby-log'
  private http = inject(HttpClient)

  loadData(id?: string, opts?: any) {
    return this.http.get(`${this.url}${id ? `/${id}` : ''}`, opts).pipe(
    catchError((error) => {
      console.log(error);
      return of(false);
    }))
  }

  editData(data: BabyLogModel) {
    return this.http.put(this.url, data).pipe(
    map(() => true),
    catchError((error) => {
      console.log(error);
      return of(false);
    }))
  }

  track(type: 'feeding' | 'diaper' | 'pumping' | 'sleep', data: any) {
    const model: BabyLogModel = {
      trackerType: type,
      createdAt: new Date(),
      updatedAt: new Date(),
      data: data
    }
    return this.http.post(this.url, model).pipe(
    map(() => true),
    catchError((error) => {
      console.log(error);
      return of(false);
    }))
  }

  deleteData(id: string) {
    return this.http.delete(`${this.url}/${id}`).pipe(
      map(() => true),
      catchError((error) => {
        console.log(error);
        return of(false);
      })
    )
  }
}
