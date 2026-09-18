public record PumpLogResponse(
    Guid Id,
    Guid UserId,
    string Timezone,
    decimal LeftAmount,
    decimal RightAmount,
    string Notes,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime
);
