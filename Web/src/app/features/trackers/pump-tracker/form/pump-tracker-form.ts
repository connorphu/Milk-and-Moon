import { Component, inject } from '@angular/core';
import { PumpTrackerDialog as PumpTrackerDialog } from '../dialog/pump-tracker-dialog';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-pump-form',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './pump-tracker-form.html',
  styleUrl: './pump-tracker-form.css',
})
export class PumpTrackerForm {
  dialog = inject(MatDialog);

  openDialog(): void {
    this.dialog.open(PumpTrackerDialog, {
      panelClass: 'tracker-dialog'
    });
  }
}
