using MilkAndMoon.Api.Contracts.Logs;
using MilkAndMoon.Api.Models;

namespace MilkAndMoon.Api.Contracts.DiaperLogs;

public record DiaperLogResponse(
    Guid Id,
    Guid BabyId,
    string BabyName,
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
) : LogResponse(StartTime)
{
    public static DiaperLogResponse FromEntity(DiaperLog d) =>
        new(
            d.Id,
            d.BabyId,
            d.Baby.Name,
            d.CreatedAt,
            d.UpdatedAt,
            d.Timezone,
            d.DiaperType,
            d.PeeColor,
            d.StoolColor,
            d.StoolTexture,
            d.Rash,
            d.RashLocation,
            d.Notes,
            d.StartTime
        );
};
