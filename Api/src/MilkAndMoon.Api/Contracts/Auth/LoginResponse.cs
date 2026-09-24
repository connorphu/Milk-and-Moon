namespace MilkAndMoon.Api.Contracts.Auth;

public record LoginResponse(UserResponse User, string token);
