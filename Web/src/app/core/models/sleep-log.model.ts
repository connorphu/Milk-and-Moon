import { TemplateRef } from "@angular/core"

export interface SleepTrackerData {
  startTime: Date
  endTime: Date
  location: string
  wakeReason: string
  notes: string
}

export interface SleepStep {
  name: string
  templateRef?: TemplateRef<any>
}
