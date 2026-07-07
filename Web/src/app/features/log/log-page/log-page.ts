import { Component } from '@angular/core';
import { FeedingForm } from '../feeding/feeding-form';
import { DiaperForm } from '../diaper/diaper-form';
import { SleepForm } from '../sleep/sleep-form';
import { PumpingForm } from '../pumping/pumping-form';
@Component({
  selector: 'app-log-page',
  imports: [FeedingForm, DiaperForm, SleepForm, PumpingForm],
  templateUrl: './log-page.html',
  styleUrl: './log-page.css',
})
export class LogPage {}
