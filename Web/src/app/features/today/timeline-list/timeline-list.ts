import { Component, inject, OnInit } from '@angular/core';
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
export class TimelineList implements OnInit {
  private babyLogService = inject(BabyLog)

  public data: BabyLogModel[] = []

  public ngOnInit(): void {
    const today = DateTime.now().toISODate()
    this.babyLogService.loadData('', { params: { createdAt_like: today } }).subscribe((res) => {
      if (res) {
        this.data = res as unknown as BabyLogModel[]
        console.log('data: ', this.data)
      }
    })
  }
}
