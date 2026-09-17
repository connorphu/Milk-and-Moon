public record SleepLogResponse(
    Guid Id,
    Guid BabyId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string Timezone,
    string? Location,
    string[] WakeReasons,
    string Notes,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime
);
