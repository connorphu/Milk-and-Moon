using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public static class DiaperLogsEndpoints
{
    public static void MapDiaperLogsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("babies/{babyId:guid}/diaper-logs").RequireAuthorization();

        group.MapGet("/", GetDiaperLogsAsync);
        group.MapPost("/", CreateDiaperLogAsync);
        group.MapGet("/{id:guid}", GetDiaperLogByIdAsync);
        group.MapPatch("/{id:guid}", UpdateDiaperLogAsync);
        group.MapDelete("/{id:guid}", DeleteDiaperLogAsync);
    }

    public static async Task<IResult> GetDiaperLogsAsync(Guid babyId, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        List<DiaperLog> diaperLogs = await dbContext.DiaperLogs
            .Where(d => d.BabyId == babyId && d.Baby.UserId == userId && d.Baby.DeletedAt == null)
            .ToListAsync();

        List<DiaperLogResponse> diaperLogResponses = diaperLogs.Select(d => new DiaperLogResponse(d.Id, d.BabyId, d.CreatedAt, d.UpdatedAt, d.Timezone, d.DiaperType, d.PeeColor, d.StoolColor, d.StoolTexture, d.Rash, d.RashLocation, d.Notes, d.StartTime)).ToList();

        return Results.Ok(diaperLogResponses);
    }

    public static async Task<IResult> CreateDiaperLogAsync(Guid babyId, CreateDiaperLogRequest request, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Baby? baby = await dbContext.Babies
            .FirstOrDefaultAsync(b => b.Id == babyId && b.UserId == userId && b.DeletedAt == null);

        if (baby is null)
        {
            return Results.NotFound();
        }

        DiaperLog diaperLog = new()
        {
            BabyId = babyId,
            Timezone = request.Timezone,
            DiaperType = request.DiaperType,
            PeeColor = request.PeeColor,
            StoolColor = request.StoolColor,
            StoolTexture = request.StoolTexture,
            Rash = request.Rash,
            RashLocation = request.RashLocation,
            Notes = request.Notes
        };

        dbContext.DiaperLogs.Add(diaperLog);
        await dbContext.SaveChangesAsync();

        DiaperLogResponse diaperLogResponse = new(diaperLog.Id, diaperLog.BabyId, diaperLog.CreatedAt, diaperLog.UpdatedAt, diaperLog.Timezone, diaperLog.DiaperType, diaperLog.PeeColor, diaperLog.StoolColor, diaperLog.StoolTexture, diaperLog.Rash, diaperLog.RashLocation, diaperLog.Notes, diaperLog.StartTime);

        return Results.Created($"/babies/{babyId}/diaper-logs/{diaperLog.Id}", diaperLogResponse);
    }

    public static async Task<IResult> GetDiaperLogByIdAsync(Guid babyId, Guid id, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        DiaperLog? diaperLog = await dbContext.DiaperLogs
            .FirstOrDefaultAsync(d => d.Id == id && d.BabyId == babyId && d.Baby.UserId == userId && d.Baby.DeletedAt == null);

        if (diaperLog is null)
        {
            return Results.NotFound();
        }

        DiaperLogResponse diaperLogResponse = new(diaperLog.Id, diaperLog.BabyId, diaperLog.CreatedAt, diaperLog.UpdatedAt, diaperLog.Timezone, diaperLog.DiaperType, diaperLog.PeeColor, diaperLog.StoolColor, diaperLog.StoolTexture, diaperLog.Rash, diaperLog.RashLocation, diaperLog.Notes, diaperLog.StartTime);

        return Results.Ok(diaperLogResponse);
    }

    public static async Task<IResult> UpdateDiaperLogAsync(Guid babyId, Guid id, UpdateDiaperLogRequest request, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        DiaperLog? diaperLog = await dbContext.DiaperLogs
            .FirstOrDefaultAsync(d => d.Id == id && d.BabyId == babyId && d.Baby.UserId == userId && d.Baby.DeletedAt == null);

        if (diaperLog is null)
        {
            return Results.NotFound();
        }

        diaperLog.Timezone = request.Timezone ?? diaperLog.Timezone;
        diaperLog.DiaperType = request.DiaperType ?? diaperLog.DiaperType;
        diaperLog.PeeColor = request.PeeColor ?? diaperLog.PeeColor;
        diaperLog.StoolColor = request.StoolColor ?? diaperLog.StoolColor;
        diaperLog.StoolTexture = request.StoolTexture ?? diaperLog.StoolTexture;
        diaperLog.Rash = request.Rash ?? diaperLog.Rash;
        diaperLog.RashLocation = request.RashLocation ?? diaperLog.RashLocation;
        diaperLog.Notes = request.Notes ?? diaperLog.Notes;

        await dbContext.SaveChangesAsync();

        DiaperLogResponse diaperLogResponse = new(diaperLog.Id, diaperLog.BabyId, diaperLog.CreatedAt, diaperLog.UpdatedAt, diaperLog.Timezone, diaperLog.DiaperType, diaperLog.PeeColor, diaperLog.StoolColor, diaperLog.StoolTexture, diaperLog.Rash, diaperLog.RashLocation, diaperLog.Notes, diaperLog.StartTime);

        return Results.Ok(diaperLogResponse);
    }

    public static async Task<IResult> DeleteDiaperLogAsync(Guid babyId, Guid id, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        DiaperLog? diaperLog = await dbContext.DiaperLogs
            .FirstOrDefaultAsync(d => d.Id == id && d.BabyId == babyId && d.Baby.UserId == userId && d.Baby.DeletedAt == null);

        if (diaperLog is null)
        {
            return Results.NotFound();
        }

        dbContext.DiaperLogs.Remove(diaperLog);
        await dbContext.SaveChangesAsync();

        return Results.NoContent();
    }
}
