using MilkAndMoon.Api.Contracts.Logs;
using MilkAndMoon.Api.Models;

namespace MilkAndMoon.Api.Contracts.FeedLogs;

public record FeedLogResponse(
    Guid Id,
    Guid BabyId,
    string BabyName,
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
) : LogResponse(StartTime)
{
    public static FeedLogResponse FromEntity(FeedLog f) =>
        new(
            f.Id,
            f.BabyId,
            f.Baby.Name,
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
