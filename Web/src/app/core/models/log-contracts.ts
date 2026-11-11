import { HttpErrorResponse } from '@angular/common/http'
import { DiaperLogRequest, DiaperLogResponse } from './diaper-log'
import { FeedLogRequest, FeedLogResponse } from './feed-log'
import { PumpLogRequest, PumpLogResponse } from './pump-log'
import { SleepLogRequest, SleepLogResponse } from './sleep-log'

export type LogRequest = FeedLogRequest | DiaperLogRequest | SleepLogRequest | PumpLogRequest

export type LogResponse = FeedLogResponse | DiaperLogResponse | SleepLogResponse | PumpLogResponse

export type ApiResult<T> = { result: 'ok'; data: T } | { result: 'error'; error: HttpErrorResponse }

export type LogsResult = ApiResult<LogResponse[]> | { result: 'loading' }
