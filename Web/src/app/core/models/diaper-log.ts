import { TemplateRef } from "@angular/core"

export interface DiaperTrackerData {
  diaperType: string
  peeColor: string
  stoolColor: string[]
  stoolTexture: string[]
  rash: string
  rashLocation: string[]
  startTime: string
  notes: string
}

export interface DiaperStep {
  name: string
  templateRef?: TemplateRef<any>
}
