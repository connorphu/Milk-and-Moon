import { DateTime } from "luxon"
import { DiaperTrackerData } from "./diaper-log"
import { FeedTrackerData } from "./feed-log"
import { PumpTrackerData } from "./pump-log"
import { SleepTrackerData } from "./sleep-log"

export interface BabyLogModel {
  id?: string
  trackerType: 'feed' | 'diaper' | 'pump' | 'sleep'
  createdAt: string
  updatedAt: string
  data: FeedTrackerData | DiaperTrackerData | PumpTrackerData | SleepTrackerData
}
