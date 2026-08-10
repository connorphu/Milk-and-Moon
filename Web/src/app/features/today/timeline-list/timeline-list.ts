import { Component, computed, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { BabyLogModel } from '../../../core/models/baby-log';
import { DateTime } from 'luxon';

@Component({
  selector: 'app-timeline-list',
  imports: [MatIconModule],
  templateUrl: './timeline-list.html',
  styleUrl: './timeline-list.css',
})
export class TimelineList {
  public data = input<BabyLogModel[]>([]);

  readonly visibleData = computed(() => {
    const visibleData = this.data().map(log => {
      const id = log.id || ''
      const time = DateTime.fromISO(log.data.startTime).toFormat('HH:mm a')
      const type = log.trackerType.charAt(0).toUpperCase() + log.trackerType.slice(1)
      return {
        id,
        time,
        type,
        trackerType: log.trackerType
      }
    })
    return visibleData.sort((a, b) => a.time.localeCompare(b.time))
  })
}
