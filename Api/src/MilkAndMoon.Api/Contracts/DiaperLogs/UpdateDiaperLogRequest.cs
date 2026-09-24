namespace MilkAndMoon.Api.Contracts.DiaperLogs;

public record UpdateDiaperLogRequest(
    string? Timezone,
    string? DiaperType,
    string? PeeColor,
    string[]? StoolColor,
    string[]? StoolTexture,
    string? Rash,
    string[]? RashLocation,
    string? Notes
);
