import { inject, Service } from '@angular/core';
import { FeedingTrackerData } from '../models/feeding-log.model';
import { DiaperTrackerData } from '../models/diaper-log.model';
import { PumpingTrackerData } from '../models/pumping-log.model';
import { SleepTrackerData } from '../models/sleep-log.model';
import { HttpClient } from '@angular/common/http';
import { catchError, map, of } from 'rxjs';

interface BabyLogModel {
  id?: string
  trackerType: 'feeding' | 'diaper' | 'pumping' | 'sleep'
  createdAt: Date
  updatedAt: Date
  data: FeedingTrackerData | DiaperTrackerData | PumpingTrackerData | SleepTrackerData
}

@Service()
export class BabyLog {
  private readonly url = 'http://localhost:3000/baby-log'
  private http = inject(HttpClient)

  loadData(id?: string) {
    return this.http.get(`${this.url}${id ? `/${id}` : ''}`)
  }

  editData(data: any) {
    return this.http.put(this.url, data)
  }

  track(type: 'feeding' | 'diaper' | 'pumping' | 'sleep', data: any) {
    const log: BabyLogModel = {
      trackerType: type,
      createdAt: new Date(),
      updatedAt: new Date(),
      data: data
    }
    return this.http.post(this.url, log).pipe(
    map(() => true),
    catchError((error) => {
      console.log(error);
      return of(false);
    }))
  }
}
