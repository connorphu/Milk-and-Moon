using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MilkAndMoon.Api.Contracts.PumpLogs;
using MilkAndMoon.Api.Data;
using MilkAndMoon.Api.Models;

namespace MilkAndMoon.Api.Endpoints;

public static class PumpLogsEndpoints
{
    public static void MapPumpLogsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/pump-logs").RequireAuthorization();

        group.MapGet("/", GetPumpLogsAsync);
        group.MapPost("/", CreatePumpLogAsync);
        group.MapGet("/{id:guid}", GetPumpLogByIdAsync);
        group.MapPatch("/{id:guid}", UpdatePumpLogAsync);
        group.MapDelete("/{id:guid}", DeletePumpLogAsync);
    }

    public static async Task<IResult> GetPumpLogsAsync(AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid currentUserId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        List<PumpLog> pumpLogs = await dbContext
            .PumpLogs.Where(p => p.UserId == currentUserId && p.User.DeletedAt == null)
            .ToListAsync();

        List<PumpLogResponse> pumpLogResponses = pumpLogs
            .Select(p => new PumpLogResponse(
                p.Id,
                p.UserId,
                p.Timezone,
                p.LeftAmount,
                p.RightAmount,
                p.Notes,
                p.StartTime,
                p.EndTime
            ))
            .ToList();

        return Results.Ok(pumpLogResponses);
    }

    public static async Task<IResult> CreatePumpLogAsync(
        CreatePumpLogRequest request,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
    {
        Guid currentUserId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        PumpLog pumpLog = new()
        {
            UserId = currentUserId,
            Timezone = request.Timezone,
            LeftAmount = request.LeftAmount,
            RightAmount = request.RightAmount,
            Notes = request.Notes,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
        };

        dbContext.PumpLogs.Add(pumpLog);
        await dbContext.SaveChangesAsync();

        PumpLogResponse pumpLogResponse = new(
            pumpLog.Id,
            pumpLog.UserId,
            pumpLog.Timezone,
            pumpLog.LeftAmount,
            pumpLog.RightAmount,
            pumpLog.Notes,
            pumpLog.StartTime,
            pumpLog.EndTime
        );

        return Results.Created($"/pump-logs/{pumpLog.Id}", pumpLogResponse);
    }

    public static async Task<IResult> GetPumpLogByIdAsync(
        Guid id,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
    {
        Guid currentUserId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        PumpLog? pumpLog = await dbContext.PumpLogs.FirstOrDefaultAsync(p =>
            p.Id == id && p.UserId == currentUserId && p.User.DeletedAt == null
        );

        if (pumpLog is null)
        {
            return Results.NotFound();
        }

        PumpLogResponse pumpLogResponse = new(
            pumpLog.Id,
            pumpLog.UserId,
            pumpLog.Timezone,
            pumpLog.LeftAmount,
            pumpLog.RightAmount,
            pumpLog.Notes,
            pumpLog.StartTime,
            pumpLog.EndTime
        );

        return Results.Ok(pumpLogResponse);
    }

    public static async Task<IResult> UpdatePumpLogAsync(
        Guid id,
        UpdatePumpLogRequest request,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
    {
        Guid currentUserId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        PumpLog? pumpLog = await dbContext.PumpLogs.FirstOrDefaultAsync(p =>
            p.Id == id && p.UserId == currentUserId && p.User.DeletedAt == null
        );

        if (pumpLog is null)
        {
            return Results.NotFound();
        }

        pumpLog.Timezone = request.Timezone ?? pumpLog.Timezone;
        pumpLog.LeftAmount = request.LeftAmount ?? pumpLog.LeftAmount;
        pumpLog.RightAmount = request.RightAmount ?? pumpLog.RightAmount;
        pumpLog.Notes = request.Notes ?? pumpLog.Notes;
        pumpLog.StartTime = request.StartTime ?? pumpLog.StartTime;
        pumpLog.EndTime = request.EndTime ?? pumpLog.EndTime;

        await dbContext.SaveChangesAsync();

        PumpLogResponse pumpLogResponse = new(
            pumpLog.Id,
            pumpLog.UserId,
            pumpLog.Timezone,
            pumpLog.LeftAmount,
            pumpLog.RightAmount,
            pumpLog.Notes,
            pumpLog.StartTime,
            pumpLog.EndTime
        );

        return Results.Ok(pumpLogResponse);
    }

    public static async Task<IResult> DeletePumpLogAsync(
        Guid id,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
    {
        Guid currentUserId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        PumpLog? pumpLog = await dbContext.PumpLogs.FirstOrDefaultAsync(p =>
            p.Id == id && p.UserId == currentUserId && p.User.DeletedAt == null
        );

        if (pumpLog is null)
        {
            return Results.NotFound();
        }

        dbContext.PumpLogs.Remove(pumpLog);
        await dbContext.SaveChangesAsync();

        return Results.NoContent();
    }
}
