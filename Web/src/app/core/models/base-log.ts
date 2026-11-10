export interface BaseLogRequest {
  timezone: string
  notes: string
  startTime: string
}

export interface BaseLogResponse {
  id: string
  createdAt: string
  updatedAt: string
}

export interface BaseBabyLog {
  babyId: string
  babyName: string
}

export interface BaseUserLog {
  userId: string
}
