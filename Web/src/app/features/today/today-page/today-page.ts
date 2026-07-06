import { Component } from '@angular/core';
import { QuickLogPanel } from '../quick-log-panel/quick-log-panel';
import { DailySummary } from '../daily-summary/daily-summary';

@Component({
  selector: 'app-today-page',
  imports: [QuickLogPanel, DailySummary],
  templateUrl: './today-page.html',
  styleUrl: './today-page.css',
})
export class TodayPage {


}
