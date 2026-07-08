import { Component, inject } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";4
import { MatSliderModule } from "@angular/material/slider";

export interface DialogData {
  name: string;
}

@Component({
  selector: 'app-feeding-form-dialog',
  imports: [
    MatFormFieldModule,
    FormsModule,
    MatSliderModule,
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
