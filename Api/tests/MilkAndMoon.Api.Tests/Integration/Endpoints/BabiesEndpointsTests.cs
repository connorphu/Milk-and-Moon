using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MilkAndMoon.Api.Contracts.Babies;
using MilkAndMoon.Api.Data;
using MilkAndMoon.Api.Models;
using MilkAndMoon.Api.Services;
using MilkAndMoon.Api.Tests.Infrastructure;

namespace MilkAndMoon.Api.Tests.Integration.Endpoints;

public class BabiesEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string BabiesUrl = "/babies";
    private const string BabiesByIdUrl = $"{BabiesUrl}/00000000-0000-0000-0000-000000000001";

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        User user = new()
        {
            Name = "Test Parent",
            Email = $"{Guid.NewGuid()}@example.com",
            PasswordHash = "not-a-real-hash!",
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        string token = scope.ServiceProvider.GetRequiredService<TokenService>().GenerateToken(user);

        HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private async Task<BabyResponse> CreateBabyAsync(
        HttpClient client,
        string name = "Baby",
        DateOnly? dateOfBirth = null
    )
    {
        CreateBabyRequest request = new(name, dateOfBirth ?? new DateOnly(2026, 1, 1));

        HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            BabiesUrl,
            request,
            TestContext.Current.CancellationToken
        );
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        BabyResponse? baby = await createResponse.Content.ReadFromJsonAsync<BabyResponse>(
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(baby);

        return baby;
    }

    [Theory]
    [InlineData("GET", BabiesUrl)]
    [InlineData("GET", BabiesByIdUrl)]
    [InlineData("POST", BabiesUrl)]
    [InlineData("PATCH", BabiesByIdUrl)]
    [InlineData("DELETE", BabiesByIdUrl)]
    public async Task AnyEndpoint_NoToken_ReturnsUnauthorized(string method, string url)
    {
        HttpClient client = factory.CreateClient();

        HttpRequestMessage request = new(new HttpMethod(method), url);

        HttpResponseMessage response = await client.SendAsync(
            request,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateBaby_ValidRequest_ReturnsCreatedBaby()
    {
        HttpClient client = await CreateAuthenticatedClientAsync();
        CreateBabyRequest request = new("New Baby", new DateOnly(2026, 1, 1));

        HttpResponseMessage response = await client.PostAsJsonAsync(
            BabiesUrl,
            request,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        BabyResponse? baby = await response.Content.ReadFromJsonAsync<BabyResponse>(
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(baby);
        Assert.Equal(request.Name, baby.Name);
        Assert.Equal(request.DateOfBirth, baby.DateOfBirth);
        Assert.NotEqual(Guid.Empty, baby.Id);
        Assert.NotEqual(default, baby.CreatedAt);
        Assert.Equal($"{BabiesUrl}/{baby.Id}", response.Headers.Location?.OriginalString);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateBaby_EmptyOrWhitespaceName_ReturnsBadRequest(string? name)
    {
        HttpClient client = await CreateAuthenticatedClientAsync();
        CreateBabyRequest request = new(name!, new DateOnly(2026, 1, 1));

        HttpResponseMessage response = await client.PostAsJsonAsync(
            BabiesUrl,
            request,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string? message = await response.Content.ReadFromJsonAsync<string>(
            TestContext.Current.CancellationToken
        );
        Assert.Equal("Baby name is required.", message);
    }

    [Fact]
    public async Task CreateBaby_ValidRequest_BabyBelongsToCaller()
    {
        HttpClient client = await CreateAuthenticatedClientAsync();
        BabyResponse baby = await CreateBabyAsync(client);

        HttpResponseMessage getResponse = await client.GetAsync(
            $"{BabiesUrl}/{baby.Id}",
            TestContext.Current.CancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        BabyResponse? retrievedBaby = await getResponse.Content.ReadFromJsonAsync<BabyResponse>(
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(retrievedBaby);
        Assert.Equal(baby, retrievedBaby);
    }
}
