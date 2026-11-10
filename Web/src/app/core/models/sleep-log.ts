import { BaseBabyLog, BaseLogRequest, BaseLogResponse } from './base-log'

export interface SleepLogRequest extends BaseLogRequest {
  location: string | null
  wakeReasons: string[]
  endTime: string | null
}

export interface SleepLogResponse extends SleepLogRequest, BaseLogResponse, BaseBabyLog {
  trackerType: 'sleep'
}
