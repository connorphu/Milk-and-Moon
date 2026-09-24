namespace MilkAndMoon.Api.Contracts.PumpLogs;

public record CreatePumpLogRequest(
    string Timezone,
    decimal LeftAmount,
    decimal RightAmount,
    string Notes,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime
);
