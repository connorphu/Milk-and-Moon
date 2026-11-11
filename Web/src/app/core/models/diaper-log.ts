import { BaseBabyLog, BaseLogRequest, BaseLogResponse } from './base-log'

export interface DiaperLogRequest extends BaseLogRequest {
  diaperType: string
  peeColor: string | null
  stoolColor: string[]
  stoolTexture: string[]
  rash: string | null
  rashLocation: string[]
}

export interface DiaperLogResponse extends DiaperLogRequest, BaseLogResponse, BaseBabyLog {
  trackerType: 'diaper'
}
