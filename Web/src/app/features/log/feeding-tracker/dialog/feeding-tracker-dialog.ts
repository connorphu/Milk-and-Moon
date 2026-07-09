import { Component, inject } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSliderModule } from "@angular/material/slider";
import { MatChipsModule } from "@angular/material/chips";
import { MatTimepickerModule } from "@angular/material/timepicker";
import { MatIcon } from "@angular/material/icon";
import { provideNativeDateAdapter } from "@angular/material/core";

export interface DialogData {
  name: string;
}

@Component({
  selector: 'app-feeding-form-dialog',
  providers: [provideNativeDateAdapter()],
  imports: [
    MatInputModule,
    MatFormFieldModule,
    FormsModule,
    MatIcon,
    MatSliderModule,
    MatChipsModule,
    MatTimepickerModule
  ],
  templateUrl: './feeding-tracker-dialog.html',
  styleUrl: './feeding-tracker-dialog.css',
})
export class FeedingTrackerDialog {
  readonly dialogRef = inject(MatDialogRef<FeedingTrackerDialog>);
  readonly data = inject<DialogData>(MAT_DIALOG_DATA);

  onNoClick(): void {
    this.dialogRef.close();
  }
}

