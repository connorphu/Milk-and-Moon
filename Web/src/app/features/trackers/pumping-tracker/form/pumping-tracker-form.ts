import { Component, inject } from '@angular/core';
import { PumpingTrackerDialog } from '../dialog/pumping-tracker-dialog';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-pumping-form',
  imports: [MatButtonModule],
  templateUrl: './pumping-tracker-form.html',
  styleUrl: './pumping-tracker-form.css',
})
export class PumpingTrackerForm {
  dialog = inject(MatDialog);

  openDialog(): void {
    this.dialog.open(PumpingTrackerDialog, {
      height: '100%',
      width: '100%',
    });
  }
}
