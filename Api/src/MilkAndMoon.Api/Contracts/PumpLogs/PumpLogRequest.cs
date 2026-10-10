namespace MilkAndMoon.Api.Contracts.PumpLogs;

public record PumpLogRequest(
    string Timezone,
    decimal LeftAmount,
    decimal RightAmount,
    string Notes,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime
);
