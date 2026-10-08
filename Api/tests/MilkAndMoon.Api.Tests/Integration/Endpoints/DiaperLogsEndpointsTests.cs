using System.Net;
using System.Net.Http.Json;
using MilkAndMoon.Api.Contracts.Babies;
using MilkAndMoon.Api.Contracts.DiaperLogs;
using MilkAndMoon.Api.Tests.Infrastructure;
using static MilkAndMoon.Api.Tests.Infrastructure.Routes;
using static MilkAndMoon.Api.Tests.Infrastructure.TestToken;

namespace MilkAndMoon.Api.Tests.Integration.Endpoints;

[Collection(ApiCollection.Name)]
public class DiaperLogsEndpointsTests(ApiFactory factory)
{
    private const string FakeId = "00000000-0000-0000-0000-000000000001";
    private const string FakeDiaperLogsUrl = $"{BabiesUrl}/{FakeId}/diaper-logs";
    private const string FakeDiaperLogsByIdUrl = $"{FakeDiaperLogsUrl}/{FakeId}";

    [Theory]
    [InlineData("GET", FakeDiaperLogsUrl)]
    [InlineData("GET", FakeDiaperLogsByIdUrl)]
    [InlineData("POST", FakeDiaperLogsUrl)]
    [InlineData("PATCH", FakeDiaperLogsByIdUrl)]
    [InlineData("DELETE", FakeDiaperLogsByIdUrl)]
    public async Task AnyEndpoint_NoToken_ReturnsUnauthorized(string method, string url)
    {
        HttpClient client = factory.CreateClient();

        HttpRequestMessage request = new(new HttpMethod(method), url);

        HttpResponseMessage response = await client.SendAsync(request, TestCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateDiaperLog_ValidRequest_ReturnsCreatedDiaperLog()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        CreateDiaperLogRequest request = TestRequests.DiaperLog with
        {
            StoolColor = ["yellow"],
            StoolTexture = ["seedy"],
            Rash = "mild",
            RashLocation = ["back"],
            Notes = "test note",
        };

        HttpResponseMessage createdResponse = await client.PostAsJsonAsync(
            DiaperLogsUrl(baby.Id),
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);

        DiaperLogResponse? diaperLog =
            await createdResponse.Content.ReadFromJsonAsync<DiaperLogResponse>(
                TestCancellationToken
            );
        Assert.NotNull(diaperLog);
        Assert.Equivalent(request, diaperLog);
        Assert.NotEqual(Guid.Empty, diaperLog.Id);
        Assert.NotEqual(default, diaperLog.CreatedAt);
        Assert.Equal(
            DiaperLogsByIdUrl(baby.Id, diaperLog.Id),
            createdResponse.Headers.Location?.OriginalString
        );
    }

    [Fact]
    public async Task CreateDiaperLog_NonExistentBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            FakeDiaperLogsUrl,
            TestRequests.DiaperLog,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateDiaperLog_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();

        HttpResponseMessage response = await callerClient.PostAsJsonAsync(
            DiaperLogsUrl(otherBaby.Id),
            TestRequests.DiaperLog,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateDiaperLog_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();

        HttpResponseMessage deleteBabyResponse = await client.DeleteAsync(
            $"{BabiesUrl}/{baby.Id}",
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBabyResponse.StatusCode);

        HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            DiaperLogsUrl(baby.Id),
            TestRequests.DiaperLog,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, createResponse.StatusCode);
    }
}
