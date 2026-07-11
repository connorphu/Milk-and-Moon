import { Component, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DiaperTrackerDialog } from '../dialog/diaper-tracker-dialog';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-diaper-form',
  imports: [MatButtonModule],
  templateUrl: './diaper-tracker-form.html',
  styleUrl: './diaper-tracker-form.css',
})
export class DiaperTrackerForm {
  dialog = inject(MatDialog);

  openDialog(): void {
    const dialogRef = this.dialog.open(DiaperTrackerDialog, {
      height: '100%',
      width: '100%',
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed: ' + result);
    });
  }
}
