namespace MilkAndMoon.Api.Contracts.Auth;

public record UserResponse(Guid Id, string Name, string Email, DateTimeOffset CreatedAt);
