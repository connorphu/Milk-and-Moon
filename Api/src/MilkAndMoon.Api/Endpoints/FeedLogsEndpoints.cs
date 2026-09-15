using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public static class FeedLogsEndpoints
{
    public static void MapFeedLogsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("babies/{babyId:guid}/feed-logs").RequireAuthorization();

        group.MapGet("/", GetFeedLogsAsync);
        group.MapPost("/", CreateFeedLogAsync);
        group.MapGet("/{id:guid}", GetFeedLogByIdAsync);
        group.MapPatch("/{id:guid}", UpdateFeedLogAsync);
        group.MapDelete("/{id:guid}", DeleteFeedLogAsync);
    }

    public static async Task<IResult> GetFeedLogsAsync(Guid babyId, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        List<FeedLog> feedLogs = await dbContext.FeedLogs
            .Where(f => f.BabyId == babyId && f.Baby.UserId == userId && f.Baby.DeletedAt == null)
            .ToListAsync();

        List<FeedLogResponse> feedLogResponses = feedLogs.Select(f => new FeedLogResponse(f.Id, f.BabyId, f.CreatedAt, f.UpdatedAt, f.Timezone, f.BottleSize, f.FeedType, f.BreastSide, f.MilkType, f.MilkConsumed, f.Notes, f.StartTime, f.EndTime)).ToList();

        return Results.Ok(feedLogResponses);
    }

    public static async Task<IResult> CreateFeedLogAsync(Guid babyId, CreateFeedLogRequest request, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Baby? baby = await dbContext.Babies
            .FirstOrDefaultAsync(b => b.Id == babyId && b.UserId == userId && b.DeletedAt == null);

        if (baby is null)
        {
            return Results.NotFound();
        }

        FeedLog feedLog = new()
        {
            BabyId = babyId,
            Timezone = request.Timezone,
            BottleSize = request.BottleSize,
            FeedType = request.FeedType,
            BreastSide = request.BreastSide,
            MilkType = request.MilkType,
            MilkConsumed = request.MilkConsumed,
            Notes = request.Notes,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        dbContext.FeedLogs.Add(feedLog);
        await dbContext.SaveChangesAsync();

        FeedLogResponse feedLogResponse = new(feedLog.Id, feedLog.BabyId, feedLog.CreatedAt, feedLog.UpdatedAt, feedLog.Timezone, feedLog.BottleSize, feedLog.FeedType, feedLog.BreastSide, feedLog.MilkType, feedLog.MilkConsumed, feedLog.Notes, feedLog.StartTime, feedLog.EndTime);

        return Results.Created($"/babies/{babyId}/feed-logs/{feedLog.Id}", feedLogResponse);
    }

    public static async Task<IResult> GetFeedLogByIdAsync(Guid babyId, Guid id, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        FeedLog? feedLog = await dbContext.FeedLogs
            .FirstOrDefaultAsync(f => f.Id == id && f.BabyId == babyId && f.Baby.UserId == userId && f.Baby.DeletedAt == null);

        if (feedLog is null)
        {
            return Results.NotFound();
        }

        FeedLogResponse feedLogResponse = new(feedLog.Id, feedLog.BabyId, feedLog.CreatedAt, feedLog.UpdatedAt, feedLog.Timezone, feedLog.BottleSize, feedLog.FeedType, feedLog.BreastSide, feedLog.MilkType, feedLog.MilkConsumed, feedLog.Notes, feedLog.StartTime, feedLog.EndTime);

        return Results.Ok(feedLogResponse);
    }

    public static async Task<IResult> UpdateFeedLogAsync(Guid babyId, Guid id, UpdateFeedLogRequest request, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        FeedLog? feedLog = await dbContext.FeedLogs
            .FirstOrDefaultAsync(f => f.Id == id && f.BabyId == babyId && f.Baby.UserId == userId && f.Baby.DeletedAt == null);

        if (feedLog is null)
        {
            return Results.NotFound();
        }

        if (request.Timezone is not null)
        {
            feedLog.Timezone = request.Timezone;
        }

        if (request.BottleSize is decimal bottleSize)
        {
            feedLog.BottleSize = bottleSize;
        }

        if (request.FeedType is not null)
        {
            feedLog.FeedType = request.FeedType;
        }

        if (request.BreastSide is not null)
        {
            feedLog.BreastSide = request.BreastSide;
        }

        if (request.MilkType is not null)
        {
            feedLog.MilkType = request.MilkType;
        }

        if (request.MilkConsumed is decimal milkConsumed)
        {
            feedLog.MilkConsumed = milkConsumed;
        }
        
        if (request.Notes is not null)
        {
            feedLog.Notes = request.Notes;
        }

        if (request.StartTime is DateTimeOffset startTime)
        {
            feedLog.StartTime = startTime;
        }

        if (request.EndTime is DateTimeOffset endTime)
        {
            feedLog.EndTime = endTime;
        }

        await dbContext.SaveChangesAsync();

        FeedLogResponse feedLogResponse = new(feedLog.Id, feedLog.BabyId, feedLog.CreatedAt, feedLog.UpdatedAt, feedLog.Timezone, feedLog.BottleSize, feedLog.FeedType, feedLog.BreastSide, feedLog.MilkType, feedLog.MilkConsumed, feedLog.Notes, feedLog.StartTime, feedLog.EndTime);

        return Results.Ok(feedLogResponse);
    }

    public static async Task<IResult> DeleteFeedLogAsync(Guid babyId, Guid id, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        FeedLog? feedLog = await dbContext.FeedLogs
            .FirstOrDefaultAsync(f => f.Id == id && f.BabyId == babyId && f.Baby.UserId == userId && f.Baby.DeletedAt == null);

        if (feedLog is null)
        {
            return Results.NotFound();
        }

        dbContext.FeedLogs.Remove(feedLog);
        await dbContext.SaveChangesAsync();

        return Results.NoContent();
    }
}
