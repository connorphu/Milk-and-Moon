import { TemplateRef } from "@angular/core"

export interface FeedingTrackerData {
  bottleSize: number
  feedingType: string
  breastSide: 'left' | 'right'
  milkType: string[]
  milkConsumed: number
  startTime: Date | null
  endTime: Date | null
  notes: string
}

export interface FeedingStep {
  name: string
  templateRef?: TemplateRef<any>
}
