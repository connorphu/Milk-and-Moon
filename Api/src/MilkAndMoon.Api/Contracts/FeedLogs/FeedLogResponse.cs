public record FeedLogResponse(
    Guid Id,
    Guid BabyId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string Timezone,
    decimal BottleSize,
    string FeedType,
    string[] BreastSide,
    string[] MilkType,
    decimal MilkConsumed,
    string? Notes,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime
);
