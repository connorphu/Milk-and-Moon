using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.Tokens;
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
    public void GenerateToken_ValidUser_ContainsExpectedClaims()
    {
        User user = new() { Id = Guid.NewGuid(), Email = "test@example.com" };
        Dictionary<string, string> expected = new()
        {
            { JwtRegisteredClaimNames.Sub, user.Id.ToString() },
            { JwtRegisteredClaimNames.Email, user.Email },
        };

        string token = CreateService().GenerateToken(user);

        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Dictionary<string, string> actual = jwt
            .Claims.Where(c => c.Type != JwtRegisteredClaimNames.Exp)
            .ToDictionary(c => c.Type, c => c.Value);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GenerateToken_ValidUser_ExpiresInFifteenMinutes()
    {
        User user = new() { Id = Guid.NewGuid(), Email = "test@example.com" };
        DateTimeOffset now = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        FakeTimeProvider fakeTimeProvider = new(now);

        string token = CreateService(fakeTimeProvider).GenerateToken(user);

        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal(now.AddMinutes(15).UtcDateTime, jwt.ValidTo);
    }

    [Fact]
    public void GenerateToken_ValidUser_IsSignedWithConfiguredKey()
    {
        User user = new() { Id = Guid.NewGuid(), Email = "test@example.com" };
        TokenValidationParameters validationParameters = new()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
        };

        string token = CreateService().GenerateToken(user);

        Exception? exception = Record.Exception(() =>
            new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _)
        );
        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_MissingSigningKey_Throws()
    {
        IConfiguration emptyConfig = new ConfigurationBuilder().Build();

        Assert.Throws<InvalidOperationException>(() =>
            new TokenService(emptyConfig, TimeProvider.System)
        );
    }
}
