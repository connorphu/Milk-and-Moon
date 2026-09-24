namespace MilkAndMoon.Api.Contracts.FeedLogs;

public record UpdateFeedLogRequest(
    string? Timezone,
    decimal? BottleSize,
    string? FeedType,
    string[]? BreastSide,
    string[]? MilkType,
    decimal? MilkConsumed,
    string? Notes,
    DateTimeOffset? StartTime,
    DateTimeOffset? EndTime
);
