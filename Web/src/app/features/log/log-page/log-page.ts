import { Component } from '@angular/core';
import { FeedingForm } from '../feeding-form/feeding-form';
import { DiaperForm } from '../diaper-form/diaper-form';
import { SleepForm } from '../sleep-form/sleep-form';
import { PumpingForm } from '../pumping-form/pumping-form';
@Component({
  selector: 'app-log-page',
  imports: [FeedingForm, DiaperForm, SleepForm, PumpingForm],
  templateUrl: './log-page.html',
  styleUrl: './log-page.css',
})
export class LogPage {}
