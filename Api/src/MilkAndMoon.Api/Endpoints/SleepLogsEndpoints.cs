using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MilkAndMoon.Api.Contracts.SleepLogs;
using MilkAndMoon.Api.Data;
using MilkAndMoon.Api.Models;

namespace MilkAndMoon.Api.Endpoints;

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

    public static async Task<IResult> GetSleepLogsAsync(
        Guid babyId,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        List<SleepLog> sleepLogs = await dbContext
            .SleepLogs.Where(s =>
                s.BabyId == babyId && s.Baby.UserId == userId && s.Baby.DeletedAt == null
            )
            .ToListAsync();

        SleepLogResponse[] sleepLogResponses = sleepLogs
            .Select(s => new SleepLogResponse(
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
            ))
            .ToArray();

        return Results.Ok(sleepLogResponses);
    }

    public static async Task<IResult> CreateSleepLogAsync(
        Guid babyId,
        CreateSleepLogRequest request,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Baby? baby = await dbContext.Babies.FirstOrDefaultAsync(b =>
            b.Id == babyId && b.UserId == userId && b.DeletedAt == null
        );

        if (baby is null)
        {
            return Results.NotFound();
        }

        SleepLog sleepLog = new()
        {
            BabyId = babyId,
            Timezone = request.Timezone,
            Location = request.Location,
            WakeReasons = request.WakeReasons,
            Notes = request.Notes,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
        };

        dbContext.SleepLogs.Add(sleepLog);
        await dbContext.SaveChangesAsync();

        SleepLogResponse sleepLogResponse = new(
            sleepLog.Id,
            sleepLog.BabyId,
            sleepLog.CreatedAt,
            sleepLog.UpdatedAt,
            sleepLog.Timezone,
            sleepLog.Location,
            sleepLog.WakeReasons,
            sleepLog.Notes,
            sleepLog.StartTime,
            sleepLog.EndTime
        );

        return Results.Created($"/babies/{babyId}/sleep-logs/{sleepLog.Id}", sleepLogResponse);
    }

    public static async Task<IResult> GetSleepLogByIdAsync(
        Guid babyId,
        Guid id,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        SleepLog? sleepLog = await dbContext.SleepLogs.FirstOrDefaultAsync(s =>
            s.Id == id && s.BabyId == babyId && s.Baby.UserId == userId && s.Baby.DeletedAt == null
        );

        if (sleepLog is null)
        {
            return Results.NotFound();
        }

        SleepLogResponse sleepLogResponse = new(
            sleepLog.Id,
            sleepLog.BabyId,
            sleepLog.CreatedAt,
            sleepLog.UpdatedAt,
            sleepLog.Timezone,
            sleepLog.Location,
            sleepLog.WakeReasons,
            sleepLog.Notes,
            sleepLog.StartTime,
            sleepLog.EndTime
        );

        return Results.Ok(sleepLogResponse);
    }

    public static async Task<IResult> UpdateSleepLogAsync(
        Guid babyId,
        Guid id,
        UpdateSleepLogRequest request,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        SleepLog? sleepLog = await dbContext.SleepLogs.FirstOrDefaultAsync(s =>
            s.Id == id && s.BabyId == babyId && s.Baby.UserId == userId && s.Baby.DeletedAt == null
        );

        if (sleepLog is null)
        {
            return Results.NotFound();
        }

        sleepLog.Timezone = request.Timezone ?? sleepLog.Timezone;
        sleepLog.Location = request.Location ?? sleepLog.Location;
        sleepLog.WakeReasons = request.WakeReasons ?? sleepLog.WakeReasons;
        sleepLog.Notes = request.Notes ?? sleepLog.Notes;
        sleepLog.StartTime = request.StartTime ?? sleepLog.StartTime;
        sleepLog.EndTime = request.EndTime ?? sleepLog.EndTime;

        await dbContext.SaveChangesAsync();

        SleepLogResponse sleepLogResponse = new(
            sleepLog.Id,
            sleepLog.BabyId,
            sleepLog.CreatedAt,
            sleepLog.UpdatedAt,
            sleepLog.Timezone,
            sleepLog.Location,
            sleepLog.WakeReasons,
            sleepLog.Notes,
            sleepLog.StartTime,
            sleepLog.EndTime
        );

        return Results.Ok(sleepLogResponse);
    }

    public static async Task<IResult> DeleteSleepLogAsync(
        Guid babyId,
        Guid id,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        SleepLog? sleepLog = await dbContext.SleepLogs.FirstOrDefaultAsync(s =>
            s.Id == id && s.BabyId == babyId && s.Baby.UserId == userId && s.Baby.DeletedAt == null
        );

        if (sleepLog is null)
        {
            return Results.NotFound();
        }

        dbContext.SleepLogs.Remove(sleepLog);
        await dbContext.SaveChangesAsync();

        return Results.NoContent();
    }
}
