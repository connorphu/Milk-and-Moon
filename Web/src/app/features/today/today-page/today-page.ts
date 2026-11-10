import { Component, computed, inject } from '@angular/core'
import { LogPanel } from '../log-panel/log-panel'
import { DailySummary } from '../daily-summary/daily-summary'
import { TimelineList } from '../timeline-list/timeline-list'
import { TrackerService } from '../../../core/services/tracker'
import { toSignal } from '@angular/core/rxjs-interop'
import { DateTime } from 'luxon'
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'

@Component({
  selector: 'app-today-page',
  imports: [LogPanel, DailySummary, TimelineList, MatProgressSpinnerModule],
  templateUrl: './today-page.html',
  styleUrl: './today-page.css'
})
export class TodayPage {
  private trackerService = inject(TrackerService)

  readonly data = toSignal(
    this.trackerService.loadData('', { params: { createdAt: DateTime.utc().toISODate() } }),
    { initialValue: { result: 'loading' } }
  )
}
