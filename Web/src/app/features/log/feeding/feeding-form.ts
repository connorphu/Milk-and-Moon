import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTimepickerModule } from '@angular/material/timepicker';
import { MatDialog } from '@angular/material/dialog';
import { FeedingDialog } from './dialog/feeding-dialog';

@Component({
  selector: 'app-feeding-form',
  imports: [FormsModule, MatButtonModule, MatFormFieldModule, MatTimepickerModule, FeedingDialog],
  templateUrl: './feeding-form.html',
  styleUrl: './feeding-form.css',
})
export class FeedingForm {
  dialog = inject(MatDialog);

  openDialog(): void {
    const dialogRef = this.dialog.open(FeedingDialog, {
      data: {name: 'Populata'},
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed: ' + result);
    });
  }
}


