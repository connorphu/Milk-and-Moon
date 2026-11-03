import { ComponentFixture, TestBed } from '@angular/core/testing'

import { DiaperTrackerDialog } from './diaper-tracker-dialog'
import { MatDialogRef } from '@angular/material/dialog'

describe('DiaperTrackerDialog', () => {
  let component: DiaperTrackerDialog
  let fixture: ComponentFixture<DiaperTrackerDialog>

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DiaperTrackerDialog],
      providers: [{ provide: MatDialogRef, useValue: { close: () => {} } }]
    }).compileComponents()

    fixture = TestBed.createComponent(DiaperTrackerDialog)
    component = fixture.componentInstance
    await fixture.whenStable()
  })

  it('should create', () => {
    expect(component).toBeTruthy()
  })
})
