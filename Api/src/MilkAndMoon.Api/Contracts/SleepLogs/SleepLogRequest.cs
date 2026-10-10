namespace MilkAndMoon.Api.Contracts.SleepLogs;

public record SleepLogRequest(
    string Timezone,
    string? Location,
    string[] WakeReasons,
    string Notes,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime
);
