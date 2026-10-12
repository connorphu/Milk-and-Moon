using MilkAndMoon.Api.Models;

namespace MilkAndMoon.Api.Contracts.FeedLogs;

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
    string Notes,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime
)
{
    public static FeedLogResponse FromEntity(FeedLog f) =>
        new(
            f.Id,
            f.BabyId,
            f.CreatedAt,
            f.UpdatedAt,
            f.Timezone,
            f.BottleSize,
            f.FeedType,
            f.BreastSide,
            f.MilkType,
            f.MilkConsumed,
            f.Notes,
            f.StartTime,
            f.EndTime
        );
};
