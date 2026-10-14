using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MilkAndMoon.Api.Contracts.Babies;
using MilkAndMoon.Api.Data;
using MilkAndMoon.Api.Models;

namespace MilkAndMoon.Api.Endpoints;

public static class BabiesEndpoints
{
    public static void MapBabiesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/babies").RequireAuthorization();

        group.MapGet("/", GetBabiesAsync);
        group.MapPost("/", CreateBabyAsync);
        group.MapGet("/{id:guid}", GetBabyByIdAsync);
        group.MapPut("/{id:guid}", UpdateBabyAsync);
        group.MapDelete("/{id:guid}", DeleteBabyAsync);
    }

    private static async Task<IResult> GetBabiesAsync(AppDbContext dbContext, ClaimsPrincipal user)
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        List<Baby> babies = await dbContext
            .Babies.Where(b => b.UserId == userId && b.DeletedAt == null)
            .OrderBy(b => b.CreatedAt)
            .ThenBy(b => b.Id)
            .ToListAsync();

        List<BabyResponse> babyResponses = babies.Select(BabyResponse.FromEntity).ToList();

        return Results.Ok(babyResponses);
    }

    private static async Task<IResult> CreateBabyAsync(
        BabyRequest request,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
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

        BabyResponse babyResponse = BabyResponse.FromEntity(baby);

        return Results.Created($"/babies/{baby.Id}", babyResponse);
    }

    private static async Task<IResult> GetBabyByIdAsync(
        Guid id,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Baby? baby = await dbContext.Babies.FirstOrDefaultAsync(b =>
            b.Id == id && b.UserId == userId && b.DeletedAt == null
        );

        if (baby is null)
        {
            return Results.NotFound();
        }

        BabyResponse babyResponse = BabyResponse.FromEntity(baby);

        return Results.Ok(babyResponse);
    }

    private static async Task<IResult> UpdateBabyAsync(
        Guid id,
        BabyRequest request,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Results.BadRequest("Baby name cannot be empty.");
        }

        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Baby? existingBaby = await dbContext.Babies.FirstOrDefaultAsync(b =>
            b.Id == id && b.UserId == userId && b.DeletedAt == null
        );

        if (existingBaby is null)
        {
            return Results.NotFound();
        }

        existingBaby.Name = request.Name;
        existingBaby.DateOfBirth = request.DateOfBirth;

        await dbContext.SaveChangesAsync();

        BabyResponse babyResponse = BabyResponse.FromEntity(existingBaby);

        return Results.Ok(babyResponse);
    }

    private static async Task<IResult> DeleteBabyAsync(
        Guid id,
        AppDbContext dbContext,
        ClaimsPrincipal user
    )
    {
        Guid userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Baby? existingBaby = await dbContext.Babies.FirstOrDefaultAsync(b =>
            b.Id == id && b.UserId == userId && b.DeletedAt == null
        );

        if (existingBaby is null)
        {
            return Results.NotFound();
        }

        existingBaby.DeletedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync();

        return Results.NoContent();
    }
}
