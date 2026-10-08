using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkAndMoon.Api.Contracts.Auth;
using MilkAndMoon.Api.Data;
using MilkAndMoon.Api.Models;
using MilkAndMoon.Api.Tests.Infrastructure;
using static MilkAndMoon.Api.Tests.Infrastructure.TestToken;

namespace MilkAndMoon.Api.Tests.Integration.Endpoints;

[Collection(ApiCollection.Name)]
public class AuthEndpointsTests(ApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();
    private const string RegisterPath = "/auth/register";
    private const string LoginPath = "/auth/login";

    private async Task<RegisterRequest> RegisterUserAsync()
    {
        RegisterRequest request = new(
            "Registered User",
            $"{Guid.NewGuid()}@example.com",
            "IRegistered123!"
        );

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            RegisterPath,
            request,
            TestCancellationToken
        );
        response.EnsureSuccessStatusCode();

        return request;
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsCreatedUser()
    {
        RegisterRequest request = new(
            "Test User",
            $"{Guid.NewGuid()}@example.com",
            "TestPassword123!"
        );

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            RegisterPath,
            request,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        UserResponse? body = await response.Content.ReadFromJsonAsync<UserResponse>(
            TestCancellationToken
        );
        Assert.NotNull(body);
        Assert.Equal(request.Name, body.Name);
        Assert.Equal(request.Email, body.Email);
        Assert.Equal($"/users/{body.Id}", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Register_ValidRequest_StoresHashedPassword()
    {
        RegisterRequest request = new(
            "Test User",
            $"{Guid.NewGuid()}@example.com",
            "TestPassword123!"
        );

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            RegisterPath,
            request,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using IServiceScope scope = factory.Services.CreateScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        User saved = await db.Users.SingleAsync(
            u => u.Email == request.Email,
            TestCancellationToken
        );

        Assert.NotEqual(request.Password, saved.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(request.Password, saved.PasswordHash));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Register_MissingName_ReturnsBadRequest(string? name)
    {
        RegisterRequest request = new(name!, $"{Guid.NewGuid()}@example.com", "TestPassword123!");

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            RegisterPath,
            request,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string? message = await response.Content.ReadFromJsonAsync<string>(TestCancellationToken);
        Assert.Equal("Name is required.", message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Register_MissingEmail_ReturnsBadRequest(string? email)
    {
        RegisterRequest request = new("Test User", email!, "TestPassword123!");

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            RegisterPath,
            request,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string? message = await response.Content.ReadFromJsonAsync<string>(TestCancellationToken);
        Assert.Equal("Email is required.", message);
    }

    [Fact]
    public async Task Register_PasswordTooShort_ReturnsBadRequest()
    {
        RegisterRequest request = new("Test User", $"{Guid.NewGuid()}@example.com", "1234567");

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            RegisterPath,
            request,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string? message = await response.Content.ReadFromJsonAsync<string>(TestCancellationToken);
        Assert.Equal("Password must be at least 8 characters long.", message);
    }

    [Fact]
    public async Task Register_PasswordExactlyMinLength_ReturnsCreatedUser()
    {
        RegisterRequest request = new("Test User", $"{Guid.NewGuid()}@example.com", "12345678");

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            RegisterPath,
            request,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidUser_ReturnsUserAndToken()
    {
        RegisterRequest registered = await RegisterUserAsync();
        LoginRequest request = new(registered.Email, registered.Password);

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            LoginPath,
            request,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        LoginResponse? body = await response.Content.ReadFromJsonAsync<LoginResponse>(
            TestCancellationToken
        );
        Assert.NotNull(body);
        Assert.Equal(registered.Name, body.User.Name);
        Assert.Equal(request.Email, body.User.Email);

        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(body.Token);
        Assert.Equal(body.User.Id.ToString(), jwt.Subject);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        RegisterRequest registered = await RegisterUserAsync();
        LoginRequest request = new(registered.Email, "wrong_password");

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            LoginPath,
            request,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_UnknownEmail_ReturnsUnauthorized()
    {
        RegisterRequest registered = await RegisterUserAsync();
        LoginRequest request = new($"{Guid.NewGuid()}@example.com", registered.Password);

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            LoginPath,
            request,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Login_MissingEmail_ReturnsBadRequest(string? email)
    {
        LoginRequest request = new(email!, "Password1234!");

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            LoginPath,
            request,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string? message = await response.Content.ReadFromJsonAsync<string>(TestCancellationToken);
        Assert.Equal("Email is required.", message);
    }
}
