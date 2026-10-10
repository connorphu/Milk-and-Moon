using System.Net;
using System.Net.Http.Json;
using MilkAndMoon.Api.Contracts.Babies;
using MilkAndMoon.Api.Contracts.FeedLogs;
using MilkAndMoon.Api.Tests.Infrastructure;
using static MilkAndMoon.Api.Tests.Infrastructure.Routes;
using static MilkAndMoon.Api.Tests.Infrastructure.TestToken;

namespace MilkAndMoon.Api.Tests.Integration.Endpoints;

[Collection(ApiCollection.Name)]
public class FeedLogsEndpointsTests(ApiFactory factory)
{
    private const string FakeId = "00000000-0000-0000-0000-000000000001";
    private const string FakeFeedLogsUrl = $"{BabiesUrl}/{FakeId}/feed-logs";
    private const string FakeFeedLogsByIdUrl = $"{FakeFeedLogsUrl}/{FakeId}";

    [Theory]
    [InlineData("GET", FakeFeedLogsUrl)]
    [InlineData("GET", FakeFeedLogsByIdUrl)]
    [InlineData("POST", FakeFeedLogsUrl)]
    [InlineData("PUT", FakeFeedLogsByIdUrl)]
    [InlineData("DELETE", FakeFeedLogsByIdUrl)]
    public async Task AnyEndpoint_NoToken_ReturnsUnauthorized(string method, string url)
    {
        HttpClient client = factory.CreateClient();

        HttpRequestMessage request = new(new HttpMethod(method), url);

        HttpResponseMessage response = await client.SendAsync(request, TestCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateFeedLog_ValidRequest_ReturnsCreatedFeedLog()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        FeedLogRequest request = TestRequests.FeedLog with
        {
            FeedType = "breast",
            BreastSide = ["left"],
            MilkType = ["breastmilk"],
        };

        HttpResponseMessage response = await client.PostAsJsonAsync(
            FeedLogsUrl(baby.Id),
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        FeedLogResponse? feedLog = await response.Content.ReadFromJsonAsync<FeedLogResponse>(
            TestCancellationToken
        );
        Assert.NotNull(feedLog);
        Assert.Equivalent(request, feedLog);
        Assert.NotEqual(Guid.Empty, feedLog.Id);
        Assert.NotEqual(default, feedLog.CreatedAt);
        Assert.Equal(
            FeedLogsByIdUrl(baby.Id, feedLog.Id),
            response.Headers.Location?.OriginalString
        );
    }

    [Fact]
    public async Task CreateFeedLog_NonExistentBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            FakeFeedLogsUrl,
            TestRequests.FeedLog,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateFeedLog_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();

        HttpResponseMessage response = await callerClient.PostAsJsonAsync(
            FeedLogsUrl(otherBaby.Id),
            TestRequests.FeedLog,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateFeedLog_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();

        HttpResponseMessage deleteBabyResponse = await client.DeleteAsync(
            BabiesByIdUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBabyResponse.StatusCode);

        HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            FeedLogsUrl(baby.Id),
            TestRequests.FeedLog,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, createResponse.StatusCode);
    }

    [Fact]
    public async Task GetFeedLogs_HasLogs_ReturnsAllLogs()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        BabyResponse otherBaby = await client.CreateBabyAsync();
        FeedLogResponse log1 = await client.CreateFeedLogAsync(
            baby.Id,
            TestRequests.FeedLog with
            {
                BottleSize = 4,
                MilkType = ["formula"],
                MilkConsumed = 3.5m,
            }
        );
        FeedLogResponse log2 = await client.CreateFeedLogAsync(
            baby.Id,
            TestRequests.FeedLog with
            {
                FeedType = "breast",
                BreastSide = ["right"],
            }
        );
        await client.CreateFeedLogAsync(
            otherBaby.Id,
            TestRequests.FeedLog with
            {
                Notes = "Other fellow",
            }
        );
        List<FeedLogResponse> expected = [log1, log2];

        HttpResponseMessage response = await client.GetAsync(
            FeedLogsUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<FeedLogResponse>? logs = await response.Content.ReadFromJsonAsync<
            List<FeedLogResponse>
        >(TestCancellationToken);
        Assert.NotNull(logs);
        Assert.Equivalent(expected, logs, true);
    }

    [Fact]
    public async Task GetFeedLogs_NonExistentBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.GetAsync(
            FakeFeedLogsUrl,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFeedLogs_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        await otherClient.CreateFeedLogAsync(otherBaby.Id);

        HttpResponseMessage response = await callerClient.GetAsync(
            FeedLogsUrl(otherBaby.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFeedLogs_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        FeedLogResponse log = await client.CreateFeedLogAsync(baby.Id);
        List<FeedLogResponse> expectedBeforeBabyDelete = [log];

        HttpResponseMessage getLogsBeforeBabyDelete = await client.GetAsync(
            FeedLogsUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getLogsBeforeBabyDelete.StatusCode);

        List<FeedLogResponse>? logsBeforeBabyDelete =
            await getLogsBeforeBabyDelete.Content.ReadFromJsonAsync<List<FeedLogResponse>>(
                TestCancellationToken
            );
        Assert.NotNull(logsBeforeBabyDelete);
        Assert.Equivalent(expectedBeforeBabyDelete, logsBeforeBabyDelete);

        HttpResponseMessage deleteBaby = await client.DeleteAsync(
            BabiesByIdUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBaby.StatusCode);

        HttpResponseMessage getLogsAfterBabyDelete = await client.GetAsync(
            FeedLogsUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NotFound, getLogsAfterBabyDelete.StatusCode);
    }

    [Fact]
    public async Task GetFeedLogById_OwnLog_ReturnsLog()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        FeedLogResponse log = await client.CreateFeedLogAsync(baby.Id, TestRequests.FilledFeedLog);

        HttpResponseMessage response = await client.GetAsync(
            FeedLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        FeedLogResponse? retrievedLog = await response.Content.ReadFromJsonAsync<FeedLogResponse>(
            TestCancellationToken
        );
        Assert.NotNull(retrievedLog);
        Assert.Equivalent(log, retrievedLog);
    }

    [Fact]
    public async Task GetFeedLogById_NonExistentLog_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        await client.CreateFeedLogAsync(baby.Id);

        HttpResponseMessage response = await client.GetAsync(
            FeedLogsByIdUrl(baby.Id, Guid.NewGuid()),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFeedLogById_MultipleBabies_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync("Oldest");
        BabyResponse otherBaby = await client.CreateBabyAsync("Youngest");
        FeedLogResponse otherBabyLog = await client.CreateFeedLogAsync(otherBaby.Id);

        HttpResponseMessage response = await client.GetAsync(
            FeedLogsByIdUrl(baby.Id, otherBabyLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFeedLogById_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        FeedLogResponse otherLog = await otherClient.CreateFeedLogAsync(otherBaby.Id);

        HttpResponseMessage response = await callerClient.GetAsync(
            FeedLogsByIdUrl(otherBaby.Id, otherLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFeedLogById_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        FeedLogResponse log = await client.CreateFeedLogAsync(baby.Id);

        HttpResponseMessage deleteBabyResponse = await client.DeleteAsync(
            BabiesByIdUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBabyResponse.StatusCode);

        HttpResponseMessage response = await client.GetAsync(
            FeedLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFeedLog_OwnLog_ReturnsUpdatedLog()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        FeedLogResponse log = await client.CreateFeedLogAsync(baby.Id);
        FeedLogRequest updateRequest = TestRequests.FilledFeedLog;

        HttpResponseMessage updateResponse = await client.PutAsJsonAsync(
            FeedLogsByIdUrl(baby.Id, log.Id),
            updateRequest,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        FeedLogResponse? updatedLog =
            await updateResponse.Content.ReadFromJsonAsync<FeedLogResponse>(TestCancellationToken);
        Assert.NotNull(updatedLog);

        HttpResponseMessage getResponse = await client.GetAsync(
            FeedLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        FeedLogResponse? retrievedLog =
            await getResponse.Content.ReadFromJsonAsync<FeedLogResponse>(TestCancellationToken);

        FeedLogResponse expected = log with
        {
            Timezone = updateRequest.Timezone,
            BottleSize = updateRequest.BottleSize,
            FeedType = updateRequest.FeedType,
            BreastSide = updateRequest.BreastSide,
            MilkType = updateRequest.MilkType,
            MilkConsumed = updateRequest.MilkConsumed,
            Notes = updateRequest.Notes,
            StartTime = updateRequest.StartTime,
            EndTime = updateRequest.EndTime,
        };

        Assert.NotNull(retrievedLog);
        Assert.Equivalent(expected, updatedLog);
        Assert.Equivalent(expected, retrievedLog);
    }

    [Fact]
    public async Task UpdateFeedLog_NonExistentLog_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        FeedLogRequest updateRequest = TestRequests.FilledFeedLog;

        HttpResponseMessage response = await client.PutAsJsonAsync(
            FeedLogsByIdUrl(baby.Id, Guid.NewGuid()),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFeedLog_MultipleBabies_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        BabyResponse otherBaby = await client.CreateBabyAsync();
        FeedLogResponse otherLog = await client.CreateFeedLogAsync(otherBaby.Id);
        FeedLogRequest updateRequest = TestRequests.FilledFeedLog;

        HttpResponseMessage response = await client.PutAsJsonAsync(
            FeedLogsByIdUrl(baby.Id, otherLog.Id),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFeedLog_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        FeedLogResponse otherLog = await otherClient.CreateFeedLogAsync(otherBaby.Id);
        FeedLogRequest updateRequest = TestRequests.FilledFeedLog;

        HttpResponseMessage response = await callerClient.PutAsJsonAsync(
            FeedLogsByIdUrl(otherBaby.Id, otherLog.Id),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFeedLog_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        FeedLogResponse log = await client.CreateFeedLogAsync(baby.Id);
        FeedLogRequest updateRequest = TestRequests.FilledFeedLog;

        HttpResponseMessage deleteBabyResponse = await client.DeleteAsync(
            BabiesByIdUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBabyResponse.StatusCode);

        HttpResponseMessage response = await client.PutAsJsonAsync(
            FeedLogsByIdUrl(baby.Id, log.Id),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFeedLog_EmptyValues_ClearsPreviousValues()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        FeedLogResponse log = await client.CreateFeedLogAsync(baby.Id, TestRequests.FilledFeedLog);

        FeedLogRequest updateRequest = TestRequests.FeedLog with
        {
            BottleSize = 0,
            FeedType = "bottle",
            BreastSide = [],
            MilkType = [],
            MilkConsumed = 0,
            Notes = "",
            EndTime = null,
        };

        HttpResponseMessage updateResponse = await client.PutAsJsonAsync(
            FeedLogsByIdUrl(baby.Id, log.Id),
            updateRequest,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        HttpResponseMessage getResponse = await client.GetAsync(
            FeedLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        FeedLogResponse? retrievedLog =
            await getResponse.Content.ReadFromJsonAsync<FeedLogResponse>(TestCancellationToken);

        Assert.NotNull(retrievedLog);
        Assert.Equivalent(updateRequest, retrievedLog);
    }

    [Fact]
    public async Task DeleteFeedLog_OwnLog_ReturnsNoContent()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        FeedLogResponse log = await client.CreateFeedLogAsync(baby.Id);

        HttpResponseMessage deleteLogResponse = await client.DeleteAsync(
            FeedLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteLogResponse.StatusCode);

        HttpResponseMessage response = await client.GetAsync(
            FeedLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFeedLog_NonExistentLog_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();

        HttpResponseMessage response = await client.DeleteAsync(
            FeedLogsByIdUrl(baby.Id, Guid.NewGuid()),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFeedLog_MultipleBabies_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        BabyResponse otherBaby = await client.CreateBabyAsync();
        FeedLogResponse otherLog = await client.CreateFeedLogAsync(otherBaby.Id);

        HttpResponseMessage response = await client.DeleteAsync(
            FeedLogsByIdUrl(baby.Id, otherLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFeedLog_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        FeedLogResponse otherLog = await otherClient.CreateFeedLogAsync(otherBaby.Id);

        HttpResponseMessage response = await callerClient.DeleteAsync(
            FeedLogsByIdUrl(otherBaby.Id, otherLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFeedLog_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        FeedLogResponse log = await client.CreateFeedLogAsync(baby.Id);

        HttpResponseMessage deleteBabyResponse = await client.DeleteAsync(
            BabiesByIdUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBabyResponse.StatusCode);

        HttpResponseMessage response = await client.DeleteAsync(
            FeedLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
