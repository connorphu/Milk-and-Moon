import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTimepickerModule } from '@angular/material/timepicker';
import { MatDialog } from '@angular/material/dialog';
import { FeedTrackerDialog } from '../dialog/feed-tracker-dialog';

@Component({
  selector: 'app-feed-form',
  imports: [FormsModule, MatButtonModule, MatFormFieldModule, MatTimepickerModule],
  templateUrl: './feed-tracker-form.html',
  styleUrl: './feed-tracker-form.css',
})
export class FeedTrackerForm {
  dialog = inject(MatDialog);

  openDialog(): void {
    this.dialog.open(FeedTrackerDialog, {
      panelClass: 'tracker-dialog'
    });
  }
}


