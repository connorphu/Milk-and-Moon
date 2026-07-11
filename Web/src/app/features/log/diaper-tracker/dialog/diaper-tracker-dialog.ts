import { Component, inject, signal, TemplateRef, viewChild, viewChildren } from '@angular/core';
import { form } from '@angular/forms/signals';
import { NgTemplateOutlet } from '@angular/common';
import { MatDialogActions, MatDialogClose, MatDialogRef } from "@angular/material/dialog";
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { MatChipsModule } from '@angular/material/chips';
import { MatLabel } from '@angular/material/form-field';
import { TemplateName } from '../../../../shared/directives/template-name';

export interface DiaperTrackerData {
  type: string
  time: Date
  color: string
  texture: string
  rash: string
  notes: string
}

interface DiaperStep {
  name: string
  templateRef?: TemplateRef<any>
}

@Component({
  selector: 'app-diaper-tracker-dialog',
  imports: [
    NgTemplateOutlet,
    TemplateName,
    MatDialogActions,
    MatDialogClose,
    MatLabel,
    MatStepperModule,
    MatChipsModule
  ],
  templateUrl: './diaper-tracker-dialog.html',
  styleUrl: './diaper-tracker-dialog.css',
})
export class DiaperTrackerDialog {
  readonly dialogRef = inject(MatDialogRef<DiaperTrackerDialog>);
  readonly allTemplateRefs = viewChildren(TemplateName);
  readonly diaperStepper = viewChild.required<MatStepper>('stepper')

  // step order matters
  readonly breastSteps: DiaperStep[] = [
    { name: 'time' },
    { name: 'breastSide' },
    { name: 'notes' }
  ];
  readonly diaperModel = signal<DiaperTrackerData>({
    type: '',
    time: null,
    color: '',
    texture: '',
    rash: '',
    notes: ''
  } as unknown as DiaperTrackerData);

  protected diaperForm = form(this.diaperModel);
  protected stepType: DiaperStep[] = []

  goBack() {
    this.diaperStepper().previous()
  }

  goForward() {
    this.diaperStepper().next()
  }
}
