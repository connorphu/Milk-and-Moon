import { Component, inject } from '@angular/core';
import { SleepTrackerDialog } from '../dialog/sleep-tracker-dialog';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-sleep-form',
  imports: [MatButtonModule],
  templateUrl: './sleep-tracker-form.html',
  styleUrl: './sleep-tracker-form.css',
})
export class SleepTrackerForm {
  dialog = inject(MatDialog);

  openDialog(): void {
    this.dialog.open(SleepTrackerDialog, {
      height: '100%',
      width: '100%',
    });
  }
}
