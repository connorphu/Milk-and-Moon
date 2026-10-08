using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkAndMoon.Api.Contracts.Babies;
using MilkAndMoon.Api.Data;
using MilkAndMoon.Api.Models;
using MilkAndMoon.Api.Tests.Infrastructure;
using static MilkAndMoon.Api.Tests.Infrastructure.Routes;
using static MilkAndMoon.Api.Tests.Infrastructure.TestToken;

namespace MilkAndMoon.Api.Tests.Integration.Endpoints;

public class BabiesEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string BabiesByIdUrl = $"{BabiesUrl}/00000000-0000-0000-0000-000000000001";

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

        HttpResponseMessage response = await client.SendAsync(request, TestCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateBaby_ValidRequest_ReturnsCreatedBaby()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        CreateBabyRequest request = new("New Baby", new DateOnly(2026, 1, 1));

        HttpResponseMessage response = await client.PostAsJsonAsync(
            BabiesUrl,
            request,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        BabyResponse? baby = await response.Content.ReadFromJsonAsync<BabyResponse>(
            TestCancellationToken
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
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        CreateBabyRequest request = new(name!, new DateOnly(2026, 1, 1));

        HttpResponseMessage response = await client.PostAsJsonAsync(
            BabiesUrl,
            request,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string? message = await response.Content.ReadFromJsonAsync<string>(TestCancellationToken);
        Assert.Equal("Baby name is required.", message);
    }

    [Fact]
    public async Task GetBabies_HasBabies_ReturnsAllBabies()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby1 = await client.CreateBabyAsync("Baby1");
        BabyResponse baby2 = await client.CreateBabyAsync("Baby2");
        List<BabyResponse> expected = [baby1, baby2];

        HttpResponseMessage response = await client.GetAsync(BabiesUrl, TestCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<BabyResponse>? babies = await response.Content.ReadFromJsonAsync<List<BabyResponse>>(
            TestCancellationToken
        );
        Assert.NotNull(babies);
        Assert.Equal(expected, babies);
    }

    [Fact]
    public async Task GetBabies_NoBabies_ReturnsEmptyList()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.GetAsync(BabiesUrl, TestCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<BabyResponse>? babies = await response.Content.ReadFromJsonAsync<List<BabyResponse>>(
            TestCancellationToken
        );
        Assert.NotNull(babies);
        Assert.Empty(babies);
    }

    [Fact]
    public async Task GetBabies_MultipleUsers_ReturnsOnlyCallersBabies()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse callerBaby = await callerClient.CreateBabyAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        List<BabyResponse> expected = [callerBaby];

        HttpResponseMessage response = await callerClient.GetAsync(
            BabiesUrl,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<BabyResponse>? retrievedBabies = await response.Content.ReadFromJsonAsync<
            List<BabyResponse>
        >(TestCancellationToken);
        Assert.NotNull(retrievedBabies);
        Assert.Equal(expected, retrievedBabies);
        Assert.DoesNotContain(otherBaby, retrievedBabies);
    }

    [Fact]
    public async Task GetBabies_DeletedSome_ReturnsActiveBabies()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse deletedBaby = await client.CreateBabyAsync("Delete");
        BabyResponse keptBaby = await client.CreateBabyAsync("Keep");
        List<BabyResponse> expected = [keptBaby];

        HttpResponseMessage deleteResponse = await client.DeleteAsync(
            $"{BabiesUrl}/{deletedBaby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        HttpResponseMessage getResponse = await client.GetAsync(BabiesUrl, TestCancellationToken);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        List<BabyResponse>? retrievedBabies = await getResponse.Content.ReadFromJsonAsync<
            List<BabyResponse>
        >(TestCancellationToken);
        Assert.NotNull(retrievedBabies);
        Assert.Equal(expected, retrievedBabies);
    }

    [Fact]
    public async Task GetBabyById_OwnBaby_ReturnsBaby()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        await client.CreateBabyAsync("Decoy");
        BabyResponse expected = await client.CreateBabyAsync("GetMe");

        HttpResponseMessage response = await client.GetAsync(
            $"{BabiesUrl}/{expected.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        BabyResponse? retrievedBaby = await response.Content.ReadFromJsonAsync<BabyResponse>(
            TestCancellationToken
        );
        Assert.NotNull(retrievedBaby);
        Assert.Equal(expected, retrievedBaby);
    }

    [Fact]
    public async Task GetBabyById_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();

        HttpResponseMessage response = await callerClient.GetAsync(
            $"{BabiesUrl}/{otherBaby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetBabyById_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();

        HttpResponseMessage deleteResponse = await client.DeleteAsync(
            $"{BabiesUrl}/{baby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        HttpResponseMessage response = await client.GetAsync(
            $"{BabiesUrl}/{baby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateBaby_EmptyOrWhitespaceName_ReturnsBadRequest(string name)
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        UpdateBabyRequest request = new(name, null);

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            $"{BabiesUrl}/{baby.Id}",
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string? message = await response.Content.ReadFromJsonAsync<string>(TestCancellationToken);
        Assert.Equal("Baby name cannot be empty.", message);
    }

    [Fact]
    public async Task UpdateBaby_OnlyName_UpdatesNameKeepsDateOfBirth()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        UpdateBabyRequest request = new("New Name", null);

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            $"{BabiesUrl}/{baby.Id}",
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        BabyResponse? updatedBaby = await response.Content.ReadFromJsonAsync<BabyResponse>(
            TestCancellationToken
        );
        Assert.NotNull(updatedBaby);
        Assert.Equal(request.Name, updatedBaby.Name);
        Assert.Equal(baby.DateOfBirth, updatedBaby.DateOfBirth);
    }

    [Fact]
    public async Task UpdateBaby_OnlyDateOfBirth_UpdatesDateKeepsName()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        UpdateBabyRequest request = new(null, new DateOnly(2026, 4, 1));

        HttpResponseMessage updateResponse = await client.PatchAsJsonAsync(
            $"{BabiesUrl}/{baby.Id}",
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        BabyResponse? updatedBaby = await updateResponse.Content.ReadFromJsonAsync<BabyResponse>(
            TestCancellationToken
        );
        Assert.NotNull(updatedBaby);
        Assert.Equal(baby.Name, updatedBaby.Name);
        Assert.Equal(request.DateOfBirth, updatedBaby.DateOfBirth);
    }

    [Fact]
    public async Task UpdateBaby_ValidRequest_ReturnsUpdatedBaby()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        UpdateBabyRequest request = new("Valid Name", new DateOnly(2026, 3, 2));

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            $"{BabiesUrl}/{baby.Id}",
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        BabyResponse? updatedBaby = await response.Content.ReadFromJsonAsync<BabyResponse>(
            TestCancellationToken
        );
        Assert.NotNull(updatedBaby);

        HttpResponseMessage getResponse = await client.GetAsync(
            $"{BabiesUrl}/{baby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        BabyResponse? fetchedBaby = await getResponse.Content.ReadFromJsonAsync<BabyResponse>(
            TestCancellationToken
        );
        BabyResponse expected = baby with
        {
            Name = "Valid Name",
            DateOfBirth = new DateOnly(2026, 3, 2),
        };
        Assert.NotNull(fetchedBaby);
        Assert.Equal(expected, updatedBaby);
        Assert.Equal(expected, fetchedBaby);
    }

    [Fact]
    public async Task UpdateBaby_NonExistentId_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        UpdateBabyRequest request = new("Update", new DateOnly(2026, 9, 24));

        HttpResponseMessage response = await client.PatchAsJsonAsync(
            BabiesByIdUrl,
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBaby_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        UpdateBabyRequest request = new("Update", new DateOnly(2026, 5, 4));

        HttpResponseMessage response = await callerClient.PatchAsJsonAsync(
            $"{BabiesUrl}/{otherBaby.Id}",
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        HttpResponseMessage getResponse = await otherClient.GetAsync(
            $"{BabiesUrl}/{otherBaby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        BabyResponse? fetchedBaby = await getResponse.Content.ReadFromJsonAsync<BabyResponse>(
            TestCancellationToken
        );
        Assert.NotNull(fetchedBaby);
        Assert.Equal(otherBaby, fetchedBaby);
    }

    [Fact]
    public async Task UpdateBaby_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        UpdateBabyRequest request = new("Deleted", new DateOnly(2026, 1, 4));

        HttpResponseMessage deleteResponse = await client.DeleteAsync(
            $"{BabiesUrl}/{baby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        HttpResponseMessage patchResponse = await client.PatchAsJsonAsync(
            $"{BabiesUrl}/{baby.Id}",
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NotFound, patchResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteBaby_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();

        HttpResponseMessage deleteResponse = await callerClient.DeleteAsync(
            $"{BabiesUrl}/{otherBaby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);

        HttpResponseMessage getResponse = await otherClient.GetAsync(
            $"{BabiesUrl}/{otherBaby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        BabyResponse? fetchedBaby = await getResponse.Content.ReadFromJsonAsync<BabyResponse>(
            TestCancellationToken
        );
        Assert.NotNull(fetchedBaby);
        Assert.Equal(otherBaby, fetchedBaby);
    }

    [Fact]
    public async Task DeleteBaby_NonExistentId_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        HttpResponseMessage response = await client.DeleteAsync(
            BabiesByIdUrl,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBaby_AlreadyDeleted_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();

        HttpResponseMessage firstDelete = await client.DeleteAsync(
            $"{BabiesUrl}/{baby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, firstDelete.StatusCode);

        HttpResponseMessage secondDelete = await client.DeleteAsync(
            $"{BabiesUrl}/{baby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NotFound, secondDelete.StatusCode);
    }

    [Fact]
    public async Task DeleteBaby_OwnBaby_SoftDeletesBaby()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();

        HttpResponseMessage response = await client.DeleteAsync(
            $"{BabiesUrl}/{baby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using IServiceScope scope = factory.Services.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Baby? retrievedBaby = await dbContext.Babies.FirstOrDefaultAsync(
            b => b.Id == baby.Id,
            TestCancellationToken
        );
        Assert.NotNull(retrievedBaby);
        Assert.NotNull(retrievedBaby.DeletedAt);
    }
}
