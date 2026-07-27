import { DiaperTrackerData } from "./diaper-log"
import { FeedingTrackerData } from "./feeding-log"
import { PumpingTrackerData } from "./pumping-log"
import { SleepTrackerData } from "./sleep-log"

export interface BabyLogModel {
  id?: string
  trackerType: 'feeding' | 'diaper' | 'pumping' | 'sleep'
  createdAt: Date
  updatedAt: Date
  data: FeedingTrackerData | DiaperTrackerData | PumpingTrackerData | SleepTrackerData
}
