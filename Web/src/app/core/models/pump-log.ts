import { TemplateRef } from "@angular/core"

export interface PumpTrackerData {
  startTime: Date
  endTime: Date
  leftAmount: number
  rightAmount: number
  notes: string
}

export interface PumpStep {
  name: string
  templateRef?: TemplateRef<any>
}
