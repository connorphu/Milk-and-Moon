namespace MilkAndMoon.Api.Contracts.Babies;

public record UpdateBabyRequest(string? Name, DateOnly? DateOfBirth);
