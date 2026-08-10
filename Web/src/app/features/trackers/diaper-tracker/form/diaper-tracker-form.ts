import { Component, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DiaperTrackerDialog } from '../dialog/diaper-tracker-dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-diaper-form',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './diaper-tracker-form.html',
  styleUrl: './diaper-tracker-form.css',
})
export class DiaperTrackerForm {
  dialog = inject(MatDialog);

  openDialog(): void {
    this.dialog.open(DiaperTrackerDialog, {
      panelClass: 'tracker-dialog'
    });
  }
}
