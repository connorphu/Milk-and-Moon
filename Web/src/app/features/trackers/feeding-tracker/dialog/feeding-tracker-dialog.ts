import { Component, inject, signal, TemplateRef, viewChild, viewChildren } from "@angular/core";
import { NgTemplateOutlet } from "@angular/common";
import { form, FormField } from "@angular/forms/signals";
import { MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSliderModule } from "@angular/material/slider";
import { MatChipListboxChange, MatChipsModule } from "@angular/material/chips";
import { MatTimepickerModule } from "@angular/material/timepicker";
import { MatStepper, MatStepperModule } from "@angular/material/stepper";
import { provideNativeDateAdapter } from "@angular/material/core";
import { MatButtonModule } from "@angular/material/button";
import { TemplateName } from "../../../../shared/directives/template-name";
import { FeedingStep, FeedingTrackerData } from "../../../../core/models/feeding-log.model";
import { QuickNote } from "../../../../core/models/model";

@Component({
  selector: 'app-feeding-form-dialog',
  providers: [provideNativeDateAdapter()],
  imports: [
    TemplateName,
    NgTemplateOutlet,
    MatDialogModule,
    MatInputModule,
    MatFormFieldModule,
    FormField,
    MatStepperModule,
    MatSliderModule,
    MatChipsModule,
    MatTimepickerModule,
    MatButtonModule
],
  templateUrl: './feeding-tracker-dialog.html',
  styleUrl: './feeding-tracker-dialog.css',
})
export class FeedingTrackerDialog {
  readonly dialogRef = inject(MatDialogRef<FeedingTrackerDialog>);
  readonly allTemplateRefs = viewChildren(TemplateName);
  readonly feedingStepper = viewChild.required<MatStepper>('stepper')

  // step order matters
  readonly breastSteps: FeedingStep[] = [
    { name: 'time' },
    { name: 'breastSide' },
    { name: 'notes' }
  ];
  readonly bottleSteps: FeedingStep[] = [
    { name: 'bottleSize' },
    { name: 'milkType' },
    { name: 'time' },
    { name: 'milkConsumed' },
    { name: 'notes' }
  ];
  readonly quickNotes: QuickNote[] = [
    { description: 'Fast feed' },
    { description: 'Slow feed' },
    { description: 'Spit up' },
    { description: 'No latch/refusal' }
  ]
  readonly feedingModel = signal<FeedingTrackerData>({
    bottleSize: 0,
    feedingType: '',
    breastSide: '',
    milkType: [],
    milkConsumed: 0,
    startTime: null,
    endTime: null,
    notes: ''
  } as unknown as FeedingTrackerData);

  protected feedingForm = form(this.feedingModel);
  protected allSteps: FeedingStep[] = []

  goBack() {
    this.feedingStepper().previous()
  }

  goForward() {
    this.feedingStepper().next()
  }

  onFeedingTypeChange(event: MatChipListboxChange) {
    this.feedingForm.feedingType().value.set(event.value ?? '')
    this.allSteps = event.value === 'breast' ? this.breastSteps
                    : event.value === 'bottle' ? this.bottleSteps
                    : [];
    this.allSteps.forEach(step => {
      step.templateRef = this.allTemplateRefs().find(template => template.templateName() === step.name)?.templateRef;
    });
  }

  onQuickNotesChange(event: MatChipListboxChange) {
    if (!event.value.length) {
      this.feedingForm.notes().value.set('')
      return
    }

    const quickNotes: string[] = event.value
    if (quickNotes) {
      this.feedingForm.notes().value.set(quickNotes.reduce((finalNote, currentNote) => {
        return finalNote.concat(`, ${currentNote}`)
      }))
    }
  }

  onNoClick(): void {
    this.dialogRef.close();
  }
}

