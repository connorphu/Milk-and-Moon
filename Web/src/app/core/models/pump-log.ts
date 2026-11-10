import { BaseLogRequest, BaseLogResponse, BaseUserLog } from './base-log'

export interface PumpLogRequest extends BaseLogRequest {
  leftAmount: number
  rightAmount: number
  endTime: string | null
}

export interface PumpLogResponse extends PumpLogRequest, BaseLogResponse, BaseUserLog {
  trackerType: 'pump'
}
