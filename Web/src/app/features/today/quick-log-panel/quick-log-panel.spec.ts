import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuickLogPanel } from './quick-log-panel';

describe('QuickLogPanel', () => {
  let component: QuickLogPanel;
  let fixture: ComponentFixture<QuickLogPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuickLogPanel],
    }).compileComponents();

    fixture = TestBed.createComponent(QuickLogPanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
