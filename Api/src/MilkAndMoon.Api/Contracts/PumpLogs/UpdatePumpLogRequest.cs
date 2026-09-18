public record UpdatePumpLogRequest(
    string? Timezone,
    decimal? LeftAmount,
    decimal? RightAmount,
    string? Notes,
    DateTimeOffset? StartTime,
    DateTimeOffset? EndTime
);
