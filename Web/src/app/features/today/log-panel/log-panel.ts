import { Component } from '@angular/core';
import { FeedTrackerForm } from '../../trackers/feed-tracker/form/feed-tracker-form';
import { DiaperTrackerForm } from '../../trackers/diaper-tracker/form/diaper-tracker-form';
import { PumpTrackerForm } from '../../trackers/pump-tracker/form/pump-tracker-form';
import { SleepTrackerForm } from '../../trackers/sleep-tracker/form/sleep-tracker-form';

@Component({
  selector: 'app-log-panel',
  imports: [FeedTrackerForm, DiaperTrackerForm, SleepTrackerForm, PumpTrackerForm],
  templateUrl: './log-panel.html',
  styleUrl: './log-panel.css',
})
export class LogPanel{}
