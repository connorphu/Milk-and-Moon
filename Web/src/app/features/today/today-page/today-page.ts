import { Component } from '@angular/core';
import { QuickLogPanel } from '../quick-log-panel/quick-log-panel';
import { DailySummary } from '../daily-summary/daily-summary';
import { TimelineList } from '../timeline-list/timeline-list';

@Component({
  selector: 'app-today-page',
  imports: [QuickLogPanel, DailySummary, TimelineList],
  templateUrl: './today-page.html',
  styleUrl: './today-page.css',
})
export class TodayPage {


}
