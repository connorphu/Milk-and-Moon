using MilkAndMoon.Api.Models;

namespace MilkAndMoon.Api.Contracts.PumpLogs;

public record PumpLogResponse(
    Guid Id,
    Guid UserId,
    string Timezone,
    decimal LeftAmount,
    decimal RightAmount,
    string Notes,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime
)
{
    public static PumpLogResponse FromEntity(PumpLog p) =>
        new(
            p.Id,
            p.UserId,
            p.Timezone,
            p.LeftAmount,
            p.RightAmount,
            p.Notes,
            p.StartTime,
            p.EndTime
        );
};
