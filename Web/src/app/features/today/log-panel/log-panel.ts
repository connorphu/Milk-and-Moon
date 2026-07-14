import { Component } from '@angular/core';
import { FeedingTrackerForm } from '../../trackers/feeding-tracker/form/feeding-tracker-form';
import { DiaperTrackerForm } from '../../trackers/diaper-tracker/form/diaper-tracker-form';
import { PumpingTrackerForm } from '../../trackers/pumping-tracker/form/pumping-tracker-form';
import { SleepTrackerForm } from '../../trackers/sleep-tracker/form/sleep-tracker-form';

@Component({
  selector: 'app-log-panel',
  imports: [FeedingTrackerForm, DiaperTrackerForm, SleepTrackerForm, PumpingTrackerForm],
  templateUrl: './log-panel.html',
  styleUrl: './log-panel.css',
})
export class LogPanel{}
