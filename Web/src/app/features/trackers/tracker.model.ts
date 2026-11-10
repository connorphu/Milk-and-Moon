import { TemplateRef } from '@angular/core'

export interface TrackerStep {
  name: string
  templateRef?: TemplateRef<unknown>
}

export interface QuickNote {
  description: string
}
