using MilkAndMoon.Api.Models;

namespace MilkAndMoon.Api.Contracts.Auth;

public record UserResponse(Guid Id, string Name, string Email, DateTimeOffset CreatedAt)
{
    public static UserResponse FromEntity(User u) => new(u.Id, u.Name, u.Email, u.CreatedAt);
};
