import { Component } from '@angular/core';
import { FeedTrackerForm } from '../../trackers/feed-tracker/form/feed-tracker-form';
import { DiaperTrackerForm } from '../../trackers/diaper-tracker/form/diaper-tracker-form';
import { PumpingTrackerForm } from '../../trackers/pump-tracker/form/pumping-tracker-form';
import { SleepTrackerForm } from '../../trackers/sleep-tracker/form/sleep-tracker-form';

@Component({
  selector: 'app-log-panel',
  imports: [FeedTrackerForm, DiaperTrackerForm, SleepTrackerForm, PumpingTrackerForm],
  templateUrl: './log-panel.html',
  styleUrl: './log-panel.css',
})
export class LogPanel{}
