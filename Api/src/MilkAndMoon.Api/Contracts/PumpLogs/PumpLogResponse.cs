using MilkAndMoon.Api.Contracts.Logs;
using MilkAndMoon.Api.Models;

namespace MilkAndMoon.Api.Contracts.PumpLogs;

public record PumpLogResponse(
    Guid Id,
    Guid UserId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string Timezone,
    decimal LeftAmount,
    decimal RightAmount,
    string Notes,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime
) : LogResponse(StartTime)
{
    public static PumpLogResponse FromEntity(PumpLog p) =>
        new(
            p.Id,
            p.UserId,
            p.CreatedAt,
            p.UpdatedAt,
            p.Timezone,
            p.LeftAmount,
            p.RightAmount,
            p.Notes,
            p.StartTime,
            p.EndTime
        );
};
