import { Component } from '@angular/core';
import { FeedingTrackerForm } from '../feeding-tracker/form/feeding-tracker-form';
import { DiaperTrackerForm } from '../diaper-tracker/diaper-form';
import { PumpingTrackerForm } from '../pumping-tracker/pumping-form';
import { SleepTrackerForm } from '../sleep-tracker/sleep-form';
@Component({
  selector: 'app-log-page',
  imports: [FeedingTrackerForm, DiaperTrackerForm, SleepTrackerForm, PumpingTrackerForm],
  templateUrl: './log-page.html',
  styleUrl: './log-page.css',
})
export class LogPage {}
