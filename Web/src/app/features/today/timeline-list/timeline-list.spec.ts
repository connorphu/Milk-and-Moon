import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimelineList } from './timeline-list';

describe('TimelineList', () => {
  let component: TimelineList;
  let fixture: ComponentFixture<TimelineList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TimelineList],
    }).compileComponents();

    fixture = TestBed.createComponent(TimelineList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
