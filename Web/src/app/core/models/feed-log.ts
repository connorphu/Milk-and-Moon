import { TemplateRef } from "@angular/core"

export interface FeedTrackerData {
  bottleSize: number
  feedType: string
  breastSide: 'left' | 'right'
  milkType: string[]
  milkConsumed: number
  startTime: string
  endTime: string
  notes: string
}

export interface FeedStep {
  name: string
  templateRef?: TemplateRef<any>
}
