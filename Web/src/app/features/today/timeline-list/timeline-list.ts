import { Component, inject } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { BabyLog } from '../../../core/services/baby-log';

@Component({
  selector: 'app-timeline-list',
  imports: [MatListModule, MatCardModule],
  templateUrl: './timeline-list.html',
  styleUrl: './timeline-list.css',
})
export class TimelineList {
  private babyLogService = inject(BabyLog)

  public data = {}

  constructor() {
    this.babyLogService.loadData().subscribe(res => {
      this.data = res
      console.log('data: ', this.data)
    })
  }
}
