import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PumpingTrackerDialog } from './pumping-tracker-dialog';

describe('PumpingTrackerDialog', () => {
  let component: PumpingTrackerDialog;
  let fixture: ComponentFixture<PumpingTrackerDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PumpingTrackerDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(PumpingTrackerDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
