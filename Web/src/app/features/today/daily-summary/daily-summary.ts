import { Component, computed, input } from '@angular/core'
import { MatIconModule } from '@angular/material/icon'
import { DateTime, Duration } from 'luxon'
import { LogResponse } from '../../../core/models/log-contracts'
import { FeedLogRequest, FeedLogResponse } from '../../../core/models/feed-log'
import { SleepLogResponse } from '../../../core/models/sleep-log'

@Component({
  selector: 'app-daily-summary',
  imports: [MatIconModule],
  templateUrl: './daily-summary.html',
  styleUrl: './daily-summary.css'
})
export class DailySummary {
  public data = input<LogResponse[]>([])

  readonly feedData = computed(() => {
    const feedLogs: FeedLogResponse[] = this.data().filter((log) => log.trackerType === 'feed')
    const bottles = feedLogs.filter((log) => log.feedType === 'bottle').length
    const breastFeeds = feedLogs.filter((log) => log.feedType === 'breast').length
    return { bottles, breastFeeds }
  })

  readonly diaperData = computed(() => {
    return this.data().filter((log) => log.trackerType === 'diaper')
  })

  readonly sleepData = computed(() => {
    const sleepLogs: SleepLogResponse[] = this.data().filter((log) => log.trackerType === 'sleep')
    let totalSeconds = 0

    sleepLogs.forEach((log) => {
      if (!log.endTime) return

      const startTime = DateTime.fromISO(log.startTime)
      const endTime = DateTime.fromISO(log.endTime)
      totalSeconds += endTime.diff(startTime).as('seconds')
    })
    const duration = Duration.fromObject({ seconds: totalSeconds }).shiftTo('hours', 'minutes')
    return { totalHours: duration.hours, totalMinutes: duration.minutes }
  })

  readonly pumpData = computed(() => {
    return this.data().filter((log) => log.trackerType === 'pump')
  })
}
