import { TemplateRef } from "@angular/core"

export interface DiaperTrackerData {
  type: string
  peeColor: string
  stoolColor: string
  stoolTexture: string
  rash: string
  rashLocation: string
  time: Date
  notes: string
}

export interface DiaperStep {
  name: string
  templateRef?: TemplateRef<any>
}
