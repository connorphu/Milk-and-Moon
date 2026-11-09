export type LogResponse = FeedLogResponse | DiaperLogResponse | SleepLogResponse | PumpLogResponse

export interface BaseLog {
  id: string
  createdAt: string
  updatedAt: string
  timezone: string
  notes: string
  startTime: string
}

export interface BaseBabyLog extends BaseLog {
  babyId: string
  babyName: string
}

export interface FeedLogResponse extends BaseBabyLog {
  trackerType: 'feed'
  bottleSize: number
  feedType: string
  breastSide: string[]
  milkType: string[]
  milkConsumed: number
  endTime: string | null
}

export interface DiaperLogResponse extends BaseBabyLog {
  trackerType: 'diaper'
  diaperType: string
  peeColor: string | null
  stoolColor: string[]
  stoolTexture: string[]
  rash: string | null
  rashLocation: string[]
}

export interface SleepLogResponse extends BaseBabyLog {
  trackerType: 'sleep'
  location: string | null
  wakeReasons: string[]
  endTime: string | null
}

export interface PumpLogResponse extends BaseLog {
  trackerType: 'pump'
  userId: string
  leftAmount: number
  rightAmount: number
  endTime: string | null
}
