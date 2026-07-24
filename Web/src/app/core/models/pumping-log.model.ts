import { TemplateRef } from "@angular/core"

export interface PumpingTrackerData {
  startTime: Date
  endTime: Date
  leftAmount: number
  rightAmount: number
  notes: string
}

export interface PumpingStep {
  name: string
  templateRef?: TemplateRef<any>
}
