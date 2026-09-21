using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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

        List<LogResponse> feedLogs = await dbContext
            .FeedLogs.Where(f =>
                babyIds.Contains(f.BabyId) && f.StartTime >= rangeStart && f.StartTime < rangeEnd
            )
            .Select(f => new LogResponse(
                f.Id,
                "feed",
                f.CreatedAt,
                f.UpdatedAt,
                f.Timezone,
                new
                {
                    BabyName = f.Baby.Name,
                    f.FeedType,
                    f.BreastSide,
                    f.MilkType,
                    f.MilkConsumed,
                    f.Notes,
                    f.StartTime,
                    f.EndTime,
                }
            ))
            .ToListAsync();

        List<LogResponse> diaperLogs = await dbContext
            .DiaperLogs.Where(d =>
                babyIds.Contains(d.BabyId) && d.StartTime >= rangeStart && d.StartTime < rangeEnd
            )
            .Select(d => new LogResponse(
                d.Id,
                "diaper",
                d.CreatedAt,
                d.UpdatedAt,
                d.Timezone,
                new
                {
                    BabyName = d.Baby.Name,
                    d.DiaperType,
                    d.PeeColor,
                    d.StoolColor,
                    d.StoolTexture,
                    d.Rash,
                    d.RashLocation,
                    d.Notes,
                    d.StartTime,
                }
            ))
            .ToListAsync();

        List<LogResponse> sleepLogs = await dbContext
            .SleepLogs.Where(s =>
                babyIds.Contains(s.BabyId) && s.StartTime >= rangeStart && s.StartTime < rangeEnd
            )
            .Select(s => new LogResponse(
                s.Id,
                "sleep",
                s.CreatedAt,
                s.UpdatedAt,
                s.Timezone,
                new
                {
                    BabyName = s.Baby.Name,
                    s.Location,
                    s.WakeReasons,
                    s.Notes,
                    s.StartTime,
                    s.EndTime,
                }
            ))
            .ToListAsync();

        List<LogResponse> pumpLogs = await dbContext
            .PumpLogs.Where(p =>
                p.UserId == userId && p.StartTime >= rangeStart && p.StartTime < rangeEnd
            )
            .Select(p => new LogResponse(
                p.Id,
                "pump",
                p.CreatedAt,
                p.UpdatedAt,
                p.Timezone,
                new
                {
                    p.LeftAmount,
                    p.RightAmount,
                    p.StartTime,
                    p.EndTime,
                    p.Notes,
                }
            ))
            .ToListAsync();

        List<LogResponse> dayLogEntries = feedLogs
            .Concat(diaperLogs)
            .Concat(sleepLogs)
            .Concat(pumpLogs)
            .OrderByDescending(l => l.CreatedAt)
            .ToList();

        return Results.Ok(dayLogEntries);
    }
}
