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
import { MatIconModule } from '@angular/material/icon';
import { TemplateName } from '../../../../shared/directives/template-name';
import { QuickNote } from '../../../../core/models/model';
import { PumpStep, PumpTrackerData } from '../../../../core/models/pump-log';
import { BabyLog } from '../../../../core/services/baby-log';

@Component({
  selector: 'app-pump-tracker-dialog',
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
    MatSliderModule,
    MatIconModule
  ],
  templateUrl: './pump-tracker-dialog.html',
  styleUrl: './pump-tracker-dialog.css',
})
export class PumpTrackerDialog implements AfterViewInit {
  readonly dialogRef = inject(MatDialogRef<PumpTrackerDialog>);
  readonly allTemplateRefs = viewChildren(TemplateName);
  readonly pumpStepper = viewChild.required<MatStepper>('stepper')

  // step order matters
  readonly allSteps: PumpStep[] = [
    { name: 'time' },
    { name: 'amount' },
    { name: 'notes' }
  ];
  readonly quickNotes: QuickNote[] = [
    { description: 'Painful' },
    { description: 'Low amount' },
    { description: 'High amount' }
  ]
  readonly pumpModel = signal<PumpTrackerData>({
    startTime: null,
    endTime: null,
    leftAmount: 0,
    rightAmount: 0,
    notes: ''
  } as unknown as PumpTrackerData);

  protected pumpForm = form(this.pumpModel);

  private babyLogService = inject(BabyLog)

  ngAfterViewInit(): void {
    this.allSteps.forEach(step => {
      step.templateRef = this.allTemplateRefs().find(template => template.templateName() === step.name)?.templateRef;
    });
  }

  goBack() {
    this.pumpStepper().previous()
  }

  goForward() {
    this.pumpStepper().next()
  }

  onQuickNotesChange(event: MatChipListboxChange) {
    if (!event.value.length) {
      this.pumpForm.notes().value.set('')
      return
    }

    const quickNotes: string[] = event.value
    if (quickNotes) {
      this.pumpForm.notes().value.set(quickNotes.reduce((finalNote, currentNote) => {
        return finalNote.concat(`, ${currentNote}`)
      }))
    }
  }

  onSave() {
    this.babyLogService.track('pump', this.pumpForm().value()).subscribe(succeed => {
      // TODO: add toasts?
    })
  }
}
