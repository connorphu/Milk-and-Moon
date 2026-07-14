import { NgTemplateOutlet } from '@angular/common';
import { AfterViewInit, Component, inject, signal, TemplateRef, viewChild, viewChildren } from '@angular/core';
import { provideNativeDateAdapter } from '@angular/material/core';
import { form, FormField } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MatChipListboxChange, MatChipsModule } from '@angular/material/chips';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule, MatLabel } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { MatTimepickerModule } from '@angular/material/timepicker';
import { MatSliderModule } from '@angular/material/slider';
import { TemplateName } from '../../../../shared/directives/template-name';

export interface SleepTrackerData {
  startTime: Date
  endTime: Date
  location: string
  wakeReason: string
  notes: string
}

interface SleepStep {
  name: string
  templateRef?: TemplateRef<any>
}

interface QuickNote {
  description: string
}

@Component({
  selector: 'app-sleep-tracker-dialog',
  providers: [provideNativeDateAdapter()],
  imports: [
    NgTemplateOutlet,
    TemplateName,
    FormField,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule,
    MatLabel,
    MatStepperModule,
    MatChipsModule,
    MatTimepickerModule,
    MatSliderModule
  ],
  templateUrl: './sleep-tracker-dialog.html',
  styleUrl: './sleep-tracker-dialog.css',
})
export class SleepTrackerDialog implements AfterViewInit{
  readonly dialogRef = inject(MatDialogRef<SleepTrackerDialog>);
  readonly allTemplateRefs = viewChildren(TemplateName);
  readonly sleepStepper = viewChild.required<MatStepper>('stepper')

  // step order matters
  readonly allSteps: SleepStep[] = [
    { name: 'time' },
    { name: 'location' },
    { name: 'wakeReason' },
    { name: 'notes' }
  ];
  readonly quickNotes: QuickNote[] = [
    { description: 'Frequent wakes' },
    { description: 'Slept all night' },
  ]
  readonly sleepModel = signal<SleepTrackerData>({
    startTime: null,
    endTime: null,
    location: '',
    wakeReason: '',
    notes: ''
  } as unknown as SleepTrackerData);

  protected sleepForm = form(this.sleepModel);

  ngAfterViewInit(): void {
    this.allSteps.forEach(step => {
      step.templateRef = this.allTemplateRefs().find(template => template.templateName() === step.name)?.templateRef;
    });
  }

  goBack() {
    this.sleepStepper().previous()
  }

  goForward() {
    this.sleepStepper().next()
  }

  onLocationChange(event: MatChipListboxChange) {
    this.sleepForm.location().value.set(event.value ?? '')
  }

  onExplaination(event: InputEvent) {

  }

  onQuickNotesChange(event: MatChipListboxChange) {
    if (!event.value.length) {
      this.sleepForm.notes().value.set('')
      return
    }

    const quickNotes: string[] = event.value
    if (quickNotes) {
      this.sleepForm.notes().value.set(quickNotes.reduce((finalNote, currentNote) => {
        return finalNote.concat(`, ${currentNote}`)
      }))
    }
  }
}
