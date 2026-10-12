using MilkAndMoon.Api.Models;

namespace MilkAndMoon.Api.Contracts.SleepLogs;

public record SleepLogResponse(
    Guid Id,
    Guid BabyId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string Timezone,
    string? Location,
    string[] WakeReasons,
    string Notes,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime
)
{
    public static SleepLogResponse FromEntity(SleepLog s) =>
        new(
            s.Id,
            s.BabyId,
            s.CreatedAt,
            s.UpdatedAt,
            s.Timezone,
            s.Location,
            s.WakeReasons,
            s.Notes,
            s.StartTime,
            s.EndTime
        );
};
