public record CreateDiaperLogRequest(
    string Timezone,
    string DiaperType,
    string? PeeColor,
    string[] StoolColor,
    string[] StoolTexture,
    string? Rash,
    string[] RashLocation,
    string Notes
);
