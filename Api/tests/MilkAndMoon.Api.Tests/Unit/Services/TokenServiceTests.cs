using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Time.Testing;
using MilkAndMoon.Api.Models;
using MilkAndMoon.Api.Services;

namespace MilkAndMoon.Api.Tests.Unit.Services;

public class TokenServiceTests
{
    private const string SigningKey = "test-signing-key-that-is-at-least-32-bytes-long";

    private static TokenService CreateService(TimeProvider? timeProvider = null)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { { "Jwt:SigningKey", SigningKey } }
            )
            .Build();

        return new TokenService(configuration, timeProvider ?? TimeProvider.System);
    }

    [Fact]
    public void GenerateToken_ValidUser_SetsSubtoUserId()
    {
        User user = new() { Id = Guid.NewGuid(), Email = "test@example.com" };

        string token = CreateService().GenerateToken(user);

        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal(user.Id.ToString(), jwt.Subject);
    }

    [Fact]
    public void GenerateToken_ValidUser_ExpiresInFifteenMinutes()
    {
        User user = new() { Id = Guid.NewGuid(), Email = "test@example.com" };
        DateTimeOffset now = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        FakeTimeProvider fakeTimeProvider = new(now);

        string token = CreateService(fakeTimeProvider).GenerateToken(user);

        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal(jwt.ValidTo, now.AddMinutes(15));
    }

    [Fact]
    public void GenerateToken_MissingSigningKey_ThrowsException()
    {
        IConfiguration emptyConfig = new ConfigurationBuilder().Build();

        Assert.Throws<InvalidOperationException>(() =>
            new TokenService(emptyConfig, TimeProvider.System)
        );
    }
}
