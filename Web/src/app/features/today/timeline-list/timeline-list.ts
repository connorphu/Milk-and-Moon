import { Component, inject, computed } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop'
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { DateTime } from 'luxon';
import { BabyLog } from '../../../core/services/baby-log';
import { BabyLogModel } from '../../../core/models/baby-log';

@Component({
  selector: 'app-timeline-list',
  imports: [MatListModule, MatCardModule],
  templateUrl: './timeline-list.html',
  styleUrl: './timeline-list.css',
})
export class TimelineList {
  private babyLogService = inject(BabyLog)
  private data = toSignal(this.babyLogService.loadData('', { params: { createdAt_like: DateTime.utc().toISODate() } }), { initialValue: []})

  readonly visibleData = computed(() => {
    return this.data().map(log => {
      const id = log.id || ''
      const createdAt = DateTime.fromISO(log.createdAt).toFormat('HH:mm a')
      const type = log.trackerType.charAt(0).toUpperCase() + log.trackerType.slice(1)
      return {
        id,
        createdAt,
        type
      }
    })
  })
}
