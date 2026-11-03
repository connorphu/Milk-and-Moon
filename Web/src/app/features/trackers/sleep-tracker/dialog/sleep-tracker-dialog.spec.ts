import { ComponentFixture, TestBed } from '@angular/core/testing'

import { SleepTrackerDialog } from './sleep-tracker-dialog'
import { MatDialogRef } from '@angular/material/dialog'

describe('SleepTrackerDialog', () => {
  let component: SleepTrackerDialog
  let fixture: ComponentFixture<SleepTrackerDialog>

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SleepTrackerDialog],
      providers: [{ provide: MatDialogRef, useValue: { close: () => {} } }]
    }).compileComponents()

    fixture = TestBed.createComponent(SleepTrackerDialog)
    component = fixture.componentInstance
    await fixture.whenStable()
  })

  it('should create', () => {
    expect(component).toBeTruthy()
  })
})
