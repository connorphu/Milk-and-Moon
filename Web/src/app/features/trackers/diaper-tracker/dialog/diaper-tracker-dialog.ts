import { Component, inject, signal, TemplateRef, viewChild, viewChildren } from '@angular/core';
import { form, FormField } from '@angular/forms/signals';
import { NgTemplateOutlet } from '@angular/common';
import { MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { MatChipListboxChange, MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule, MatLabel } from '@angular/material/form-field';
import { TemplateName } from '../../../../shared/directives/template-name';
import { MatButtonModule } from '@angular/material/button';
import { MatTimepickerModule } from '@angular/material/timepicker';
import { MatInputModule } from '@angular/material/input';
import { provideNativeDateAdapter } from '@angular/material/core';
import { DiaperStep, DiaperTrackerData } from '../../../../core/models/diaper-log.model';
import { QuickNote } from '../../../../core/models/model';

@Component({
  selector: 'app-diaper-tracker-dialog',
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
    MatTimepickerModule
  ],
  templateUrl: './diaper-tracker-dialog.html',
  styleUrl: './diaper-tracker-dialog.css',
})
export class DiaperTrackerDialog {
  readonly dialogRef = inject(MatDialogRef<DiaperTrackerDialog>);
  readonly allTemplateRefs = viewChildren(TemplateName);
  readonly diaperStepper = viewChild.required<MatStepper>('stepper')

  // step order matters
  readonly wetSteps: DiaperStep[] = [
    { name: 'peeColor' }
  ];
  readonly poopySteps: DiaperStep[] = [
    { name: 'stoolColor' },
    { name: 'stoolTexture' }
  ]
  readonly bothTypeSteps: DiaperStep[] = [
    ...this.wetSteps,
    ...this.poopySteps,
  ]
  readonly staticSteps: DiaperStep[] = [
    { name: 'rash' },
    { name: 'time' },
    { name: 'notes' }
  ]
  readonly quickNotes: QuickNote[] = [
    { description: 'Weird smell' },
    { description: 'Weird mark/bruise' },
    { description: 'Bleeding '}
  ]
  readonly diaperModel = signal<DiaperTrackerData>({
    type: '',
    peeColor: '',
    stoolColor: [],
    stoolTexture: [],
    rash: [],
    rashLocation: [],
    time: null,
    notes: ''
  } as unknown as DiaperTrackerData);

  protected diaperForm = form(this.diaperModel);
  protected allSteps: DiaperStep[] = []

  goBack() {
    this.diaperStepper().previous()
  }

  goForward() {
    this.diaperStepper().next()
  }

  updateSteps() {
    this.allSteps.forEach(step => {
      step.templateRef = this.allTemplateRefs().find(template => template.templateName() === step.name)?.templateRef;
    });
  }

  onDiaperTypeChange(event: MatChipListboxChange) {
    this.diaperForm.type().value.set(event.value ?? '')
    const dynamicSteps = event.value === 'wet' ? this.wetSteps
                    : event.value === 'poopy' ? this.poopySteps
                    : event.value === 'both' ? this.bothTypeSteps
                    : [];
    this.allSteps = [...dynamicSteps, ...this.staticSteps]
    this.updateSteps()
  }

  onPeeColorChange(event: MatChipListboxChange) {
    this.diaperForm.peeColor().value.set(event.value ?? '')
  }

  onRashChange(event: MatChipListboxChange) {
    this.diaperForm.rash().value.set(event.value ?? '')
    const rashLocationStep = this.allSteps.find(step => step.name === 'rashLocation')

    if (this.diaperForm.rash().value() && !rashLocationStep) {
      const rashStepIndex = this.allSteps.findIndex(step => step.name === 'rash')
      this.allSteps.splice(rashStepIndex+1, 0, { name: 'rashLocation'})
      this.updateSteps()
    }

    if (!this.diaperForm.rash().value() && rashLocationStep) {
      this.allSteps = this.allSteps.filter(step => step.name !== 'rashLocation')
      this.diaperForm.rashLocation().value.set('')
    }
  }

  onQuickNotesChange(event: MatChipListboxChange) {
    if (!event.value.length) {
      this.diaperForm.notes().value.set('')
      return
    }

    const quickNotes: string[] = event.value
    if (quickNotes) {
      this.diaperForm.notes().value.set(quickNotes.reduce((finalNote, currentNote) => {
        return finalNote.concat(`, ${currentNote}`)
      }))
    }
  }
}
