import { inject, Service } from '@angular/core';
import { FeedingTrackerData } from '../models/feeding-log.model';
import { DiaperTrackerData } from '../models/diaper-log.model';
import { PumpingTrackerData } from '../models/pumping-log.model';
import { SleepTrackerData } from '../models/sleep-log.model';
import { HttpClient } from '@angular/common/http';

interface BabyLogModel {
  id: string
  trackerType: 'feeding' | 'diaper' | 'pumping' | 'sleep'
  createdAt: Date
  updatedAt: Date
  data: FeedingTrackerData | DiaperTrackerData | PumpingTrackerData | SleepTrackerData
}

@Service()
export class BabyLog {
  private http = inject(HttpClient)

  loadData() {
    return this.http.get('/assets/mock-data/baby-log.json')
  }
  // trackFeeding(data: FeedingTrackerData): BabyLogModel {

  // }

  // trackDiaper(data: DiaperTrackerData): BabyLogModel {

  // }

  // trackPumping(data: PumpingTrackerData): BabyLogModel {

  // }

  // trackSleep(data: SleepTrackerData): BabyLogModel {

  // }

}
