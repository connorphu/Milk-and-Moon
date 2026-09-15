using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public static class BabiesEndpoints
{
    public static void MapBabiesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/babies").RequireAuthorization();

        group.MapGet("/", GetBabiesAsync);
        group.MapPost("/", CreateBabyAsync);
        group.MapGet("/{id:guid}", GetBabyByIdAsync);
        group.MapPatch("/{id:guid}", UpdateBabyAsync);
        group.MapDelete("/{id:guid}", DeleteBabyAsync);
    }

    private static async Task<IResult> GetBabiesAsync(AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        List<Baby> babies = await dbContext.Babies
            .Where(b => b.UserId == userId && b.DeletedAt == null)
            .ToListAsync();

        List<BabyResponse> babyResponses = babies.Select(b => new BabyResponse(b.Id, b.Name, b.DateOfBirth, b.CreatedAt)).ToList();

        return Results.Ok(babyResponses);
    }

    private static async Task<IResult> CreateBabyAsync(CreateBabyRequest request, AppDbContext dbContext, ClaimsPrincipal user)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Results.BadRequest("Baby name is required.");
        }

        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Baby baby = new()
        {
            UserId = userId,
            Name = request.Name,
            DateOfBirth = request.DateOfBirth,
        };

        dbContext.Babies.Add(baby);
        await dbContext.SaveChangesAsync();

        BabyResponse babyResponse = new(baby.Id, baby.Name, baby.DateOfBirth, baby.CreatedAt);

        return Results.Created($"/babies/{baby.Id}", babyResponse);
    }

    private static async Task<IResult> GetBabyByIdAsync(Guid id, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Baby? baby = await dbContext.Babies
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId && b.DeletedAt == null);

        if (baby is null)
        {
            return Results.NotFound();
        }

        BabyResponse babyResponse = new(baby.Id, baby.Name, baby.DateOfBirth, baby.CreatedAt);

        return Results.Ok(babyResponse);
    }

    private static async Task<IResult> UpdateBabyAsync(Guid id, UpdateBabyRequest request, AppDbContext dbContext, ClaimsPrincipal user)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Results.BadRequest("Baby name is required.");
        }

        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Baby? existingBaby = await dbContext.Babies
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId && b.DeletedAt == null);

        if (existingBaby is null)
        {
            return Results.NotFound();
        }

        if (request.Name is not null)
        {
            existingBaby.Name = request.Name;
        }

        if (request.DateOfBirth is not null)
        {
            existingBaby.DateOfBirth = request.DateOfBirth.Value;
        }

        await dbContext.SaveChangesAsync();

        BabyResponse babyResponse = new(existingBaby.Id, existingBaby.Name, existingBaby.DateOfBirth, existingBaby.CreatedAt);

        return Results.Ok(babyResponse);
    }

    private static async Task<IResult> DeleteBabyAsync(Guid id, AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Baby? existingBaby = await dbContext.Babies
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId && b.DeletedAt == null);

        if (existingBaby is null)
        {
            return Results.NotFound();
        }

        existingBaby.DeletedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync();

        return Results.NoContent();
    }
}
