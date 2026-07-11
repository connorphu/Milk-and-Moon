import { Component } from '@angular/core';
import { LogPanel } from '../log-panel/log-panel';
import { DailySummary } from '../daily-summary/daily-summary';
import { TimelineList } from '../timeline-list/timeline-list';

@Component({
  selector: 'app-today-page',
  imports: [LogPanel, DailySummary, TimelineList],
  templateUrl: './today-page.html',
  styleUrl: './today-page.css',
})
export class TodayPage {


}
