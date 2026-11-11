import { BaseBabyLog, BaseLogRequest, BaseLogResponse } from './base-log'

export interface FeedLogRequest extends BaseLogRequest {
  bottleSize: number
  feedType: string
  breastSide: string[]
  milkType: string[]
  milkConsumed: number
  endTime: string | null
}

export interface FeedLogResponse extends FeedLogRequest, BaseLogResponse, BaseBabyLog {
  trackerType: 'feed'
}
