import { ComponentFixture, TestBed } from '@angular/core/testing'
import { SleepTrackerForm } from './sleep-tracker-form'
import { SleepTrackerDialog } from '../dialog/sleep-tracker-dialog'
import { MatDialog } from '@angular/material/dialog'

describe('SleepTrackerForm', () => {
  let component: SleepTrackerForm
  let fixture: ComponentFixture<SleepTrackerForm>
  const mockDialog = { open: vi.fn() }

  beforeEach(async () => {
    mockDialog.open.mockClear()

    await TestBed.configureTestingModule({
      imports: [SleepTrackerForm],
      providers: [{ provide: MatDialog, useValue: mockDialog }]
    }).compileComponents()

    fixture = TestBed.createComponent(SleepTrackerForm)
    component = fixture.componentInstance
    await fixture.whenStable()
  })

  it('should create', () => {
    expect(component).toBeTruthy()
  })

  it('should open dialog', () => {
    component.openDialog()

    expect(mockDialog.open).toHaveBeenCalledExactlyOnceWith(SleepTrackerDialog, {
      panelClass: 'tracker-dialog'
    })
  })

  it('should call openDialog when clicked', () => {
    fixture.nativeElement.querySelector('button').click()

    expect(mockDialog.open).toHaveBeenCalledOnce()
  })
})
