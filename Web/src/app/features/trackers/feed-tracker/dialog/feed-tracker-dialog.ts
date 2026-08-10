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
import { MatIconModule } from "@angular/material/icon";
import { TemplateName } from "../../../../shared/directives/template-name";
import { FeedStep, FeedTrackerData } from "../../../../core/models/feed-log";
import { QuickNote } from "../../../../core/models/model";
import { BabyLog } from "../../../../core/services/baby-log";

@Component({
  selector: 'app-feed-form-dialog',
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
    MatButtonModule,
    MatIconModule
],
  templateUrl: './feed-tracker-dialog.html',
  styleUrl: './feed-tracker-dialog.css',
})
export class FeedTrackerDialog {
  readonly dialogRef = inject(MatDialogRef<FeedTrackerDialog>);
  readonly allTemplateRefs = viewChildren(TemplateName);
  readonly feedStepper = viewChild.required<MatStepper>('stepper')

  // step order matters
  readonly breastSteps: FeedStep[] = [
    { name: 'time' },
    { name: 'breastSide' },
    { name: 'notes' }
  ];
  readonly bottleSteps: FeedStep[] = [
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
  readonly feedModel = signal<FeedTrackerData>({
    bottleSize: 0,
    feedType: '',
    breastSide: [],
    milkType: [],
    milkConsumed: 0,
    startTime: '',
    endTime: '',
    notes: ''
  } as unknown as FeedTrackerData);

  protected feedForm = form(this.feedModel);
  protected allSteps: FeedStep[] = []

  private babyLogService = inject(BabyLog)

  goBack() {
    this.feedStepper().previous()
  }

  goForward() {
    this.feedStepper().next()
  }

  onFeedTypeChange(event: MatChipListboxChange) {
    this.feedForm.feedType().value.set(event.value ?? '')
    this.allSteps = event.value === 'breast' ? this.breastSteps
                    : event.value === 'bottle' ? this.bottleSteps
                    : [];
    this.allSteps.forEach(step => {
      step.templateRef = this.allTemplateRefs().find(template => template.templateName() === step.name)?.templateRef;
    });
  }

  onQuickNotesChange(event: MatChipListboxChange) {
    if (!event.value.length) {
      this.feedForm.notes().value.set('')
      return
    }

    const quickNotes: string[] = event.value
    if (quickNotes) {
      this.feedForm.notes().value.set(quickNotes.reduce((finalNote, currentNote) => {
        return finalNote.concat(`, ${currentNote}`)
      }))
    }
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  onSave() {
    this.babyLogService.track('feed', this.feedForm().value()).subscribe(succeed => {
      // TODO: add toasts?
    })
  }
}

