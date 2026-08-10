import { Component, inject } from '@angular/core';
import { SleepTrackerDialog } from '../dialog/sleep-tracker-dialog';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-sleep-form',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './sleep-tracker-form.html',
  styleUrl: './sleep-tracker-form.css',
})
export class SleepTrackerForm {
  dialog = inject(MatDialog);

  openDialog(): void {
    this.dialog.open(SleepTrackerDialog, {
      panelClass: 'tracker-dialog'
    });
  }
}
