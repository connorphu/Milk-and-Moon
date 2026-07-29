import { DiaperTrackerData } from "./diaper-log"
import { FeedTrackerData } from "./feed-log"
import { PumpingTrackerData } from "./pumping-log"
import { SleepTrackerData } from "./sleep-log"

export interface BabyLogModel {
  id?: string
  trackerType: 'feed' | 'diaper' | 'pumping' | 'sleep'
  createdAt: Date
  updatedAt: Date
  data: FeedTrackerData | DiaperTrackerData | PumpingTrackerData | SleepTrackerData
}
