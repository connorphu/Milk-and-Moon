import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DailySummary } from './daily-summary';

describe('DailySummary', () => {
  let component: DailySummary;
  let fixture: ComponentFixture<DailySummary>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DailySummary],
    }).compileComponents();

    fixture = TestBed.createComponent(DailySummary);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
