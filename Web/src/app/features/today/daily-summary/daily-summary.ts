import { Component, computed, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { DateTime, Duration } from 'luxon';
import { BabyLogModel } from '../../../core/models/baby-log';
import { FeedTrackerData } from '../../../core/models/feed-log';
import { SleepTrackerData } from '../../../core/models/sleep-log';

@Component({
  selector: 'app-daily-summary',
  imports: [MatIconModule],
  templateUrl: './daily-summary.html',
  styleUrl: './daily-summary.css',
})
export class DailySummary {
  public data = input<BabyLogModel[]>([]);

  readonly feedData = computed(() => {
    let bottles = 0
    let breastFeeds = 0
    const feedData: BabyLogModel[] = this.data().filter(log => log.trackerType === 'feed')

    feedData.forEach(log => {
      const feedData = log.data as FeedTrackerData
      if (feedData.feedType === 'bottle') {
        bottles++
      } else {
        breastFeeds++
      }
    })

    return { bottles, breastFeeds }
  })

  readonly diaperData = computed(() => {
    return this.data().filter(log => log.trackerType === 'diaper')
  })

  readonly sleepData = computed(() => {
    const sleepData: BabyLogModel[] = this.data().filter(log => log.trackerType === 'sleep')
    let totalSeconds = 0

    sleepData.forEach(log => {
      const sleepData = log.data as SleepTrackerData
      const startTime = DateTime.fromISO(sleepData.startTime)
      const endTime = DateTime.fromISO(sleepData.endTime)
      totalSeconds += endTime.diff(startTime).as('seconds')
    })
    const duration = Duration.fromObject({ seconds: totalSeconds }).shiftTo('hours', 'minutes')
    return { totalHours: duration.hours, totalMinutes: duration.minutes }
  })

  readonly pumpData = computed(() => {
    return this.data().filter(log => log.trackerType === 'pump')
  })
}
