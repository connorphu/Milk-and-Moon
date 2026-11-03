import { ComponentFixture, TestBed } from '@angular/core/testing'

import { PumpTrackerDialog } from './pump-tracker-dialog'
import { MatDialogRef } from '@angular/material/dialog'

describe('PumpTrackerDialog', () => {
  let component: PumpTrackerDialog
  let fixture: ComponentFixture<PumpTrackerDialog>

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PumpTrackerDialog],
      providers: [{ provide: MatDialogRef, useValue: { close: () => {} } }]
    }).compileComponents()

    fixture = TestBed.createComponent(PumpTrackerDialog)
    component = fixture.componentInstance
    await fixture.whenStable()
  })

  it('should create', () => {
    expect(component).toBeTruthy()
  })
})
