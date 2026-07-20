import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTimepickerModule } from '@angular/material/timepicker';
import { MatDialog } from '@angular/material/dialog';
import { FeedingTrackerDialog } from '../dialog/feeding-tracker-dialog';

@Component({
  selector: 'app-feeding-form',
  imports: [FormsModule, MatButtonModule, MatFormFieldModule, MatTimepickerModule],
  templateUrl: './feeding-tracker-form.html',
  styleUrl: './feeding-tracker-form.css',
})
export class FeedingTrackerForm {
  dialog = inject(MatDialog);

  openDialog(): void {
    this.dialog.open(FeedingTrackerDialog, {
      panelClass: 'tracker-dialog'
    });
  }
}


