import { Component, inject, signal } from "@angular/core";
import { form, FormField } from "@angular/forms/signals";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSliderModule } from "@angular/material/slider";
import { MatChipsModule } from "@angular/material/chips";
import { MatTimepickerModule } from "@angular/material/timepicker";
import { MatStepperModule } from "@angular/material/stepper";
import { provideNativeDateAdapter } from "@angular/material/core";

export interface FeedingTrackerData {
  bottleSize: number
  breastSide: 'left' | 'right'
  startTime: Date
  endTime: Date
  milkType: 'breastmilk' | 'formula' | 'donor milk'
  notes: string
}

@Component({
  selector: 'app-feeding-form-dialog',
  providers: [provideNativeDateAdapter()],
  imports: [
    MatInputModule,
    MatFormFieldModule,
    FormField,
    MatStepperModule,
    MatSliderModule,
    MatChipsModule,
    MatTimepickerModule,
  ],
  templateUrl: './feeding-tracker-dialog.html',
  styleUrl: './feeding-tracker-dialog.css',
})
export class FeedingTrackerDialog {
  readonly dialogRef = inject(MatDialogRef<FeedingTrackerDialog>);
  readonly feedingModel = signal<FeedingTrackerData>({
    bottleSize: 0,
    breastSide: null,
    startTime: null,
    endTime: null,
    milkType: null,
    notes: null
  } as unknown as FeedingTrackerData);

  protected feedingForm = form(this.feedingModel);

  onNoClick(): void {
    this.dialogRef.close();
  }
}

