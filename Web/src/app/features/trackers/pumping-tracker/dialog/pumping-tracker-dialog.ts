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
import { QuickNote } from '../../../../core/models/model';
import { PumpingStep, PumpingTrackerData } from '../../../../core/models/pumping-log.model';

@Component({
  selector: 'app-pumping-tracker-dialog',
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
  templateUrl: './pumping-tracker-dialog.html',
  styleUrl: './pumping-tracker-dialog.css',
})
export class PumpingTrackerDialog implements AfterViewInit {
  readonly dialogRef = inject(MatDialogRef<PumpingTrackerDialog>);
  readonly allTemplateRefs = viewChildren(TemplateName);
  readonly pumpingStepper = viewChild.required<MatStepper>('stepper')

  // step order matters
  readonly allSteps: PumpingStep[] = [
    { name: 'time' },
    { name: 'amount' },
    { name: 'notes' }
  ];
  readonly quickNotes: QuickNote[] = [
    { description: 'Painful' },
    { description: 'Low amount' },
    { description: 'High amount' }
  ]
  readonly pumpingModel = signal<PumpingTrackerData>({
    startTime: null,
    endTime: null,
    leftAmount: 0,
    rightAmount: 0,
    notes: ''
  } as unknown as PumpingTrackerData);

  protected pumpingForm = form(this.pumpingModel);

  ngAfterViewInit(): void {
    this.allSteps.forEach(step => {
      step.templateRef = this.allTemplateRefs().find(template => template.templateName() === step.name)?.templateRef;
    });
  }

  goBack() {
    this.pumpingStepper().previous()
  }

  goForward() {
    this.pumpingStepper().next()
  }

  onQuickNotesChange(event: MatChipListboxChange) {
    if (!event.value.length) {
      this.pumpingForm.notes().value.set('')
      return
    }

    const quickNotes: string[] = event.value
    if (quickNotes) {
      this.pumpingForm.notes().value.set(quickNotes.reduce((finalNote, currentNote) => {
        return finalNote.concat(`, ${currentNote}`)
      }))
    }
  }

}
