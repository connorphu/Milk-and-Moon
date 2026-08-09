import { Component, inject } from '@angular/core';
import { LogPanel } from '../log-panel/log-panel';
import { DailySummary } from '../daily-summary/daily-summary';
import { TimelineList } from '../timeline-list/timeline-list';
import { BabyLog } from '../../../core/services/baby-log';
import { toSignal } from '@angular/core/rxjs-interop';
import { DateTime } from 'luxon';

@Component({
  selector: 'app-today-page',
  imports: [LogPanel, DailySummary, TimelineList],
  templateUrl: './today-page.html',
  styleUrl: './today-page.css',
})
export class TodayPage {
  private babyLogService = inject(BabyLog)

  readonly data = toSignal(this.babyLogService.loadData('', { params: { createdAt_like: DateTime.utc().toISODate() } }), { initialValue: []})
}
