namespace MilkAndMoon.Api.Contracts.Babies;

public record BabyResponse(Guid Id, string Name, DateOnly DateOfBirth, DateTimeOffset CreatedAt);
