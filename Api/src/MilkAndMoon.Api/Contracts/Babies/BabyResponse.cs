using MilkAndMoon.Api.Models;

namespace MilkAndMoon.Api.Contracts.Babies;

public record BabyResponse(Guid Id, string Name, DateOnly DateOfBirth, DateTimeOffset CreatedAt)
{
    public static BabyResponse FromEntity(Baby b) => new(b.Id, b.Name, b.DateOfBirth, b.CreatedAt);
};
