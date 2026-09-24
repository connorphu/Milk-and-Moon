namespace MilkAndMoon.Api.Contracts.SleepLogs;

public record UpdateSleepLogRequest(
    string? Timezone,
    string? Location,
    string[]? WakeReasons,
    string? Notes,
    DateTimeOffset? StartTime,
    DateTimeOffset? EndTime
);
