public record DiaperLogResponse(
    Guid Id,
    Guid BabyId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
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
