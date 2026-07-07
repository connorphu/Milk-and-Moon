import { Component, inject } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";4
import { MatInputModule } from "@angular/material/input";

export interface DialogData {
  name: string;
}

@Component({
  selector: 'app-feeding-form-dialog',
  imports: [
    MatFormFieldModule,
    FormsModule,
    MatButtonModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    MatInputModule
  ],
  templateUrl: './feeding-dialog.html',
  styleUrl: './feeding-dialog.css',
})
export class FeedingDialog {
  readonly dialogRef = inject(MatDialogRef<FeedingDialog>);
  readonly data = inject<DialogData>(MAT_DIALOG_DATA);

  onNoClick(): void {
    this.dialogRef.close();
  }
}
