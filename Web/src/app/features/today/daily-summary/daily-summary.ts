import { Component, computed, input } from '@angular/core'
import { MatIconModule } from '@angular/material/icon'
import { DateTime, Duration } from 'luxon'
import { LogResponse } from '../../../core/models/log-response'
import { FeedTrackerData } from '../../../core/models/feed-log'
import { SleepTrackerData } from '../../../core/models/sleep-log'

@Component({
  selector: 'app-daily-summary',
  imports: [MatIconModule],
  templateUrl: './daily-summary.html',
  styleUrl: './daily-summary.css'
})
export class DailySummary {
  public data = input<LogResponse[]>([])

  readonly feedData = computed(() => {
    let bottles = 0
    let breastFeeds = 0
    const feedData: LogResponse[] = this.data().filter((log) => log.trackerType === 'feed')

    feedData.forEach((log) => {
      const feedData = log as FeedTrackerData
      if (feedData.feedType === 'bottle') {
        bottles++
      } else {
        breastFeeds++
      }
    })

    return { bottles, breastFeeds }
  })

  readonly diaperData = computed(() => {
    return this.data().filter((log) => log.trackerType === 'diaper')
  })

  readonly sleepData = computed(() => {
    const sleepData: LogResponse[] = this.data().filter((log) => log.trackerType === 'sleep')
    let totalSeconds = 0

    sleepData.forEach((log) => {
      const sleepData = log as SleepTrackerData
      const startTime = DateTime.fromISO(sleepData.startTime)
      const endTime = DateTime.fromISO(sleepData.endTime)
      totalSeconds += endTime.diff(startTime).as('seconds')
    })
    const duration = Duration.fromObject({ seconds: totalSeconds }).shiftTo('hours', 'minutes')
    return { totalHours: duration.hours, totalMinutes: duration.minutes }
  })

  readonly pumpData = computed(() => {
    return this.data().filter((log) => log.trackerType === 'pump')
  })
}
