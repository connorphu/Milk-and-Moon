namespace MilkAndMoon.Api.Contracts.SleepLogs;

public record CreateSleepLogRequest(
    string Timezone,
    string? Location,
    string[] WakeReasons,
    string Notes,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime
);
