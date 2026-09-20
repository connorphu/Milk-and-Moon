using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public static class SleepLogsEndpoints
{
    public static void MapSleepLogsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("babies/{babyId:guid}/sleep-logs").RequireAuthorization();

        group.MapGet("/", GetSleepLogsAsync);
        group.MapPost("/", CreateSleepLogAsync);
        group.MapGet("/{id:guid}", GetSleepLogByIdAsync);
        group.MapPatch("/{id:guid}", UpdateSleepLogAsync);
        group.MapDelete("/{id:guid}", DeleteSleepLogAsync);
    }

    public static Task<IResult> GetSleepLogsAsync(Guid babyId, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        List<SleepLog> sleepLogs = dbContext.SleepLogs
            .Where(s => s.BabyId == babyId && s.Baby.UserId == userId && s.Baby.DeletedAt == null)
            .ToList();

        SleepLogResponse[] sleepLogResponses = sleepLogs.Select(s => new SleepLogResponse(s.Id, s.BabyId, s.CreatedAt, s.UpdatedAt, s.Timezone, s.Location, s.WakeReasons, s.Notes, s.StartTime, s.EndTime)).ToArray();

        return Task.FromResult(Results.Ok(sleepLogResponses));
    }

    public static Task<IResult> CreateSleepLogAsync(Guid babyId, CreateSleepLogRequest request, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Baby? baby = dbContext.Babies
            .FirstOrDefault(b => b.Id == babyId && b.UserId == userId && b.DeletedAt == null);

        if (baby is null)
        {
            return Task.FromResult(Results.NotFound());
        }

        SleepLog sleepLog = new()
        {
            BabyId = babyId,
            Timezone = request.Timezone,
            Location = request.Location,
            WakeReasons = request.WakeReasons,
            Notes = request.Notes,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        dbContext.SleepLogs.Add(sleepLog);
        dbContext.SaveChanges();

        SleepLogResponse sleepLogResponse = new(sleepLog.Id, sleepLog.BabyId, sleepLog.CreatedAt, sleepLog.UpdatedAt, sleepLog.Timezone, sleepLog.Location, sleepLog.WakeReasons, sleepLog.Notes, sleepLog.StartTime, sleepLog.EndTime);

        return Task.FromResult(Results.Created($"/babies/{babyId}/sleep-logs/{sleepLog.Id}", sleepLogResponse));
    }

    public static Task<IResult> GetSleepLogByIdAsync(Guid babyId, Guid id, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        SleepLog? sleepLog = dbContext.SleepLogs
            .FirstOrDefault(s => s.Id == id && s.BabyId == babyId && s.Baby.UserId == userId && s.Baby.DeletedAt == null);

        if (sleepLog is null)
        {
            return Task.FromResult(Results.NotFound());
        }

        SleepLogResponse sleepLogResponse = new(sleepLog.Id, sleepLog.BabyId, sleepLog.CreatedAt, sleepLog.UpdatedAt, sleepLog.Timezone, sleepLog.Location, sleepLog.WakeReasons, sleepLog.Notes, sleepLog.StartTime, sleepLog.EndTime);

        return Task.FromResult(Results.Ok(sleepLogResponse));
    }

    public static Task<IResult> UpdateSleepLogAsync(Guid babyId, Guid id, UpdateSleepLogRequest request, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        SleepLog? sleepLog = dbContext.SleepLogs
            .FirstOrDefault(s => s.Id == id && s.BabyId == babyId && s.Baby.UserId == userId && s.Baby.DeletedAt == null);

        if (sleepLog is null)
        {
            return Task.FromResult(Results.NotFound());
        }

        sleepLog.Timezone = request.Timezone ?? sleepLog.Timezone;
        sleepLog.Location = request.Location ?? sleepLog.Location;
        sleepLog.WakeReasons = request.WakeReasons ?? sleepLog.WakeReasons;
        sleepLog.Notes = request.Notes ?? sleepLog.Notes;
        sleepLog.StartTime = request.StartTime ?? sleepLog.StartTime;
        sleepLog.EndTime = request.EndTime ?? sleepLog.EndTime;

        dbContext.SaveChanges();

        SleepLogResponse sleepLogResponse = new(sleepLog.Id, sleepLog.BabyId, sleepLog.CreatedAt, sleepLog.UpdatedAt, sleepLog.Timezone, sleepLog.Location, sleepLog.WakeReasons, sleepLog.Notes, sleepLog.StartTime, sleepLog.EndTime);

        return Task.FromResult(Results.Ok(sleepLogResponse));
    }

    public static Task<IResult> DeleteSleepLogAsync(Guid babyId, Guid id, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        SleepLog? sleepLog = dbContext.SleepLogs
            .FirstOrDefault(s => s.Id == id && s.BabyId == babyId && s.Baby.UserId == userId && s.Baby.DeletedAt == null);

        if (sleepLog is null)
        {
            return Task.FromResult(Results.NotFound());
        }

        dbContext.SleepLogs.Remove(sleepLog);
        dbContext.SaveChanges();

        return Task.FromResult(Results.NoContent());
    }
}
