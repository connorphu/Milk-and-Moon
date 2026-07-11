import { Component } from '@angular/core';
import { FeedingTrackerForm } from '../../log/feeding-tracker/form/feeding-tracker-form';
import { DiaperTrackerForm } from '../../log/diaper-tracker/form/diaper-tracker-form';
import { PumpingTrackerForm } from '../../log/pumping-tracker/pumping-form';
import { SleepTrackerForm } from '../../log/sleep-tracker/sleep-form';

@Component({
  selector: 'app-log-panel',
  imports: [FeedingTrackerForm, DiaperTrackerForm, SleepTrackerForm, PumpingTrackerForm],
  templateUrl: './log-panel.html',
  styleUrl: './log-panel.css',
})
export class LogPanel{}
