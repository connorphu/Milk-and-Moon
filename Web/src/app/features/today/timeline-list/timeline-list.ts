import { Component, inject } from '@angular/core';
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

  public data = toSignal(this.babyLogService.loadData('', { params: { createdAt_like: DateTime.now().toISODate() } }), { initialValue: []})
}
