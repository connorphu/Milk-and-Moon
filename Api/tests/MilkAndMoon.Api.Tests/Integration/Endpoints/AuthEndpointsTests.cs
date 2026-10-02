using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkAndMoon.Api.Contracts.Auth;
using MilkAndMoon.Api.Data;
using MilkAndMoon.Api.Models;
using MilkAndMoon.Api.Tests.Infrastructure;

namespace MilkAndMoon.Api.Tests.Integration.Endpoints;

public class AuthEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private const string registerURI = "/auth/register";

    [Fact]
    public async Task Register_ValidRequest_ReturnsCreatedUser()
    {
        RegisterRequest request = new(
            "Test User",
            $"{Guid.NewGuid()}@example.com",
            "TestPassword123!"
        );

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            registerURI,
            request,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        UserResponse? body = await response.Content.ReadFromJsonAsync<UserResponse>(
            TestContext.Current.CancellationToken
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
            registerURI,
            request,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using IServiceScope scope = factory.Services.CreateScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        User saved = await db.Users.SingleAsync(
            u => u.Email == request.Email,
            TestContext.Current.CancellationToken
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
            registerURI,
            request,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string? message = await response.Content.ReadFromJsonAsync<string>(
            TestContext.Current.CancellationToken
        );
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
            registerURI,
            request,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string? message = await response.Content.ReadFromJsonAsync<string>(
            TestContext.Current.CancellationToken
        );
        Assert.Equal("Email is required.", message);
    }

    [Fact]
    public async Task Register_PasswordTooShort_ReturnsBadRequest()
    {
        RegisterRequest request = new("Test User", $"{Guid.NewGuid()}@example.com", "1234567");

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            registerURI,
            request,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string? message = await response.Content.ReadFromJsonAsync<string>(
            TestContext.Current.CancellationToken
        );
        Assert.Equal("Password must be at least 8 characters long.", message);
    }

    [Fact]
    public async Task Register_PasswordExactlyMinLength_ReturnsCreatedUser()
    {
        RegisterRequest request = new("Test User", $"{Guid.NewGuid()}@example.com", "12345678");

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            registerURI,
            request,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
