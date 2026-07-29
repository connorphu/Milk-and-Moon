import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PumpTrackerDialog } from './pump-tracker-dialog';

describe('PumpTrackerDialog', () => {
  let component: PumpTrackerDialog;
  let fixture: ComponentFixture<PumpTrackerDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PumpTrackerDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(PumpTrackerDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
