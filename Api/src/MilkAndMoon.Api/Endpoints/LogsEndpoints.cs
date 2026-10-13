using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MilkAndMoon.Api.Contracts.DiaperLogs;
using MilkAndMoon.Api.Contracts.FeedLogs;
using MilkAndMoon.Api.Contracts.Logs;
using MilkAndMoon.Api.Contracts.PumpLogs;
using MilkAndMoon.Api.Contracts.SleepLogs;
using MilkAndMoon.Api.Data;

namespace MilkAndMoon.Api.Endpoints;

public static class LogsEndpoints
{
    public static void MapLogsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/logs").RequireAuthorization();

        group.MapGet("/", GetDaysAsync);
    }

    public static async Task<IResult> GetDaysAsync(
        AppDbContext dbContext,
        ClaimsPrincipal user,
        DateOnly startDate,
        DateOnly? endDate,
        string timezone = "UTC"
    )
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);

        DateTime startLocal = startDate.ToDateTime(TimeOnly.MinValue);
        DateTimeOffset rangeStart = new DateTimeOffset(startLocal, tz.GetUtcOffset(startLocal));

        DateTime endLocalExclusive = (endDate ?? startDate)
            .AddDays(1)
            .ToDateTime(TimeOnly.MinValue);
        DateTimeOffset rangeEnd = new DateTimeOffset(
            endLocalExclusive,
            tz.GetUtcOffset(endLocalExclusive)
        );

        List<Guid> babyIds = await dbContext
            .Babies.Where(b => b.UserId == userId && b.DeletedAt == null)
            .Select(b => b.Id)
            .ToListAsync();

        List<FeedLogResponse> feedLogs = (
            await dbContext
                .FeedLogs.Include(f => f.Baby)
                .Where(f =>
                    babyIds.Contains(f.BabyId)
                    && f.StartTime >= rangeStart
                    && f.StartTime < rangeEnd
                )
                .ToListAsync()
        )
            .Select(FeedLogResponse.FromEntity)
            .ToList();

        List<DiaperLogResponse> diaperLogs = (
            await dbContext
                .DiaperLogs.Include(d => d.Baby)
                .Where(d =>
                    babyIds.Contains(d.BabyId)
                    && d.StartTime >= rangeStart
                    && d.StartTime < rangeEnd
                )
                .ToListAsync()
        )
            .Select(DiaperLogResponse.FromEntity)
            .ToList();

        List<SleepLogResponse> sleepLogs = (
            await dbContext
                .SleepLogs.Include(s => s.Baby)
                .Where(s =>
                    babyIds.Contains(s.BabyId)
                    && s.StartTime >= rangeStart
                    && s.StartTime < rangeEnd
                )
                .ToListAsync()
        )
            .Select(SleepLogResponse.FromEntity)
            .ToList();

        List<PumpLogResponse> pumpLogs = (
            await dbContext
                .PumpLogs.Where(p =>
                    p.UserId == userId && p.StartTime >= rangeStart && p.StartTime < rangeEnd
                )
                .ToListAsync()
        )
            .Select(PumpLogResponse.FromEntity)
            .ToList();

        List<LogResponse> dayLogEntries = feedLogs
            .Concat<LogResponse>(diaperLogs)
            .Concat(sleepLogs)
            .Concat(pumpLogs)
            .OrderByDescending(l => l.StartTime)
            .ToList();

        return Results.Ok(dayLogEntries);
    }
}
