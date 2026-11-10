import { HttpErrorResponse } from '@angular/common/http'
import { DiaperLogResponse } from './diaper-log'
import { FeedLogResponse } from './feed-log'
import { PumpLogResponse } from './pump-log'
import { SleepLogResponse } from './sleep-log'

export type LogResponse = FeedLogResponse | DiaperLogResponse | SleepLogResponse | PumpLogResponse

export type LogsResult =
  | { result: 'ok'; logs: LogResponse[] }
  | { result: 'error'; error: HttpErrorResponse }
  | { result: 'loading' }
