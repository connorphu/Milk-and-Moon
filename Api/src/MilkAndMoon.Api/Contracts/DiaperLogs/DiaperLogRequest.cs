namespace MilkAndMoon.Api.Contracts.DiaperLogs;

public record DiaperLogRequest(
    string Timezone,
    string DiaperType,
    string? PeeColor,
    string[] StoolColor,
    string[] StoolTexture,
    string? Rash,
    string[] RashLocation,
    string Notes,
    DateTimeOffset StartTime
);
