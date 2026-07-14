import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SleepTrackerDialog } from './sleep-tracker-dialog';

describe('SleepTrackerDialog', () => {
  let component: SleepTrackerDialog;
  let fixture: ComponentFixture<SleepTrackerDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SleepTrackerDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(SleepTrackerDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
