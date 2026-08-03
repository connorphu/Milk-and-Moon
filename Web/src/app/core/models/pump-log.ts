import { TemplateRef } from "@angular/core"

export interface PumpTrackerData {
  startTime: string
  endTime: string
  leftAmount: number
  rightAmount: number
  notes: string
}

export interface PumpStep {
  name: string
  templateRef?: TemplateRef<any>
}
