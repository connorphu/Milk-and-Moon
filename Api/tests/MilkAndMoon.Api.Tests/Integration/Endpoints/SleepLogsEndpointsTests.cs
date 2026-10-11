using System.Net;
using System.Net.Http.Json;
using MilkAndMoon.Api.Contracts.Babies;
using MilkAndMoon.Api.Contracts.SleepLogs;
using MilkAndMoon.Api.Tests.Infrastructure;
using static MilkAndMoon.Api.Tests.Infrastructure.Routes;
using static MilkAndMoon.Api.Tests.Infrastructure.TestToken;

namespace MilkAndMoon.Api.Tests.Integration.Endpoints;

[Collection(ApiCollection.Name)]
public class SleepLogsEndpointsTests(ApiFactory factory)
{
    private const string FakeId = "00000000-0000-0000-0000-000000000001";
    private const string FakeSleepLogsUrl = $"{BabiesUrl}/{FakeId}/sleep-logs";
    private const string FakeSleepLogsByIdUrl = $"{FakeSleepLogsUrl}/{FakeId}";

    [Theory]
    [InlineData("GET", FakeSleepLogsUrl)]
    [InlineData("GET", FakeSleepLogsByIdUrl)]
    [InlineData("POST", FakeSleepLogsUrl)]
    [InlineData("PUT", FakeSleepLogsByIdUrl)]
    [InlineData("DELETE", FakeSleepLogsByIdUrl)]
    public async Task AnyEndpoint_NoToken_ReturnsUnauthorized(string method, string url)
    {
        HttpClient client = factory.CreateClient();

        HttpRequestMessage request = new(new HttpMethod(method), url);

        HttpResponseMessage response = await client.SendAsync(request, TestCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateSleepLog_ValidRequest_ReturnsCreatedSleepLog()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        SleepLogRequest request = TestRequests.SleepLog with
        {
            Location = "bassinet",
            WakeReasons = ["naturally"],
        };

        HttpResponseMessage response = await client.PostAsJsonAsync(
            SleepLogsUrl(baby.Id),
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        SleepLogResponse? sleepLog = await response.Content.ReadFromJsonAsync<SleepLogResponse>(
            TestCancellationToken
        );
        Assert.NotNull(sleepLog);
        Assert.Equivalent(request, sleepLog);
        Assert.NotEqual(Guid.Empty, sleepLog.Id);
        Assert.NotEqual(default, sleepLog.CreatedAt);
        Assert.Equal(
            SleepLogsByIdUrl(baby.Id, sleepLog.Id),
            response.Headers.Location?.OriginalString
        );
    }

    [Fact]
    public async Task CreateSleepLog_NonExistentBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            FakeSleepLogsUrl,
            TestRequests.SleepLog,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateSleepLog_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();

        HttpResponseMessage response = await callerClient.PostAsJsonAsync(
            SleepLogsUrl(otherBaby.Id),
            TestRequests.SleepLog,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateSleepLog_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();

        HttpResponseMessage deleteBabyResponse = await client.DeleteAsync(
            BabiesByIdUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBabyResponse.StatusCode);

        HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            SleepLogsUrl(baby.Id),
            TestRequests.SleepLog,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, createResponse.StatusCode);
    }

    [Fact]
    public async Task GetSleepLogs_HasLogs_ReturnsAllLogs()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        BabyResponse otherBaby = await client.CreateBabyAsync();
        SleepLogResponse log1 = await client.CreateSleepLogAsync(
            baby.Id,
            TestRequests.SleepLog with
            {
                Location = "crib",
                WakeReasons = ["hungry"],
            }
        );
        SleepLogResponse log2 = await client.CreateSleepLogAsync(
            baby.Id,
            TestRequests.SleepLog with
            {
                Location = "stroller",
                EndTime = new DateTimeOffset(2026, 11, 23, 15, 30, 0, TimeSpan.Zero),
            }
        );
        await client.CreateSleepLogAsync(
            otherBaby.Id,
            TestRequests.SleepLog with
            {
                Notes = "Other fellow",
            }
        );
        List<SleepLogResponse> expected = [log1, log2];

        HttpResponseMessage response = await client.GetAsync(
            SleepLogsUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<SleepLogResponse>? logs = await response.Content.ReadFromJsonAsync<
            List<SleepLogResponse>
        >(TestCancellationToken);
        Assert.NotNull(logs);
        Assert.Equivalent(expected, logs, true);
    }

    [Fact]
    public async Task GetSleepLogs_NonExistentBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.GetAsync(
            FakeSleepLogsUrl,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSleepLogs_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        await otherClient.CreateSleepLogAsync(otherBaby.Id);

        HttpResponseMessage response = await callerClient.GetAsync(
            SleepLogsUrl(otherBaby.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSleepLogs_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        SleepLogResponse log = await client.CreateSleepLogAsync(baby.Id);
        List<SleepLogResponse> expectedBeforeBabyDelete = [log];

        HttpResponseMessage getLogsBeforeBabyDelete = await client.GetAsync(
            SleepLogsUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getLogsBeforeBabyDelete.StatusCode);

        List<SleepLogResponse>? logsBeforeBabyDelete =
            await getLogsBeforeBabyDelete.Content.ReadFromJsonAsync<List<SleepLogResponse>>(
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
            SleepLogsUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NotFound, getLogsAfterBabyDelete.StatusCode);
    }

    [Fact]
    public async Task GetSleepLogById_OwnLog_ReturnsLog()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        SleepLogResponse log = await client.CreateSleepLogAsync(
            baby.Id,
            TestRequests.FilledSleepLog
        );

        HttpResponseMessage response = await client.GetAsync(
            SleepLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        SleepLogResponse? retrievedLog =
            await response.Content.ReadFromJsonAsync<SleepLogResponse>(TestCancellationToken);
        Assert.NotNull(retrievedLog);
        Assert.Equivalent(log, retrievedLog);
    }

    [Fact]
    public async Task GetSleepLogById_NonExistentLog_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        await client.CreateSleepLogAsync(baby.Id);

        HttpResponseMessage response = await client.GetAsync(
            SleepLogsByIdUrl(baby.Id, Guid.NewGuid()),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSleepLogById_MultipleBabies_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync("Oldest");
        BabyResponse otherBaby = await client.CreateBabyAsync("Youngest");
        SleepLogResponse otherBabyLog = await client.CreateSleepLogAsync(otherBaby.Id);

        HttpResponseMessage response = await client.GetAsync(
            SleepLogsByIdUrl(baby.Id, otherBabyLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSleepLogById_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        SleepLogResponse otherLog = await otherClient.CreateSleepLogAsync(otherBaby.Id);

        HttpResponseMessage response = await callerClient.GetAsync(
            SleepLogsByIdUrl(otherBaby.Id, otherLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSleepLogById_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        SleepLogResponse log = await client.CreateSleepLogAsync(baby.Id);

        HttpResponseMessage deleteBabyResponse = await client.DeleteAsync(
            BabiesByIdUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBabyResponse.StatusCode);

        HttpResponseMessage response = await client.GetAsync(
            SleepLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSleepLog_OwnLog_ReturnsUpdatedLog()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        SleepLogResponse log = await client.CreateSleepLogAsync(baby.Id);
        SleepLogRequest updateRequest = TestRequests.FilledSleepLog;

        HttpResponseMessage updateResponse = await client.PutAsJsonAsync(
            SleepLogsByIdUrl(baby.Id, log.Id),
            updateRequest,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        SleepLogResponse? updatedLog =
            await updateResponse.Content.ReadFromJsonAsync<SleepLogResponse>(TestCancellationToken);
        Assert.NotNull(updatedLog);

        HttpResponseMessage getResponse = await client.GetAsync(
            SleepLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        SleepLogResponse? retrievedLog =
            await getResponse.Content.ReadFromJsonAsync<SleepLogResponse>(TestCancellationToken);

        SleepLogResponse expected = log with
        {
            Timezone = updateRequest.Timezone,
            Location = updateRequest.Location,
            WakeReasons = updateRequest.WakeReasons,
            Notes = updateRequest.Notes,
            StartTime = updateRequest.StartTime,
            EndTime = updateRequest.EndTime,
        };

        Assert.NotNull(retrievedLog);
        Assert.Equivalent(expected, updatedLog);
        Assert.Equivalent(expected, retrievedLog);
    }

    [Fact]
    public async Task UpdateSleepLog_NonExistentLog_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        SleepLogRequest updateRequest = TestRequests.FilledSleepLog;

        HttpResponseMessage response = await client.PutAsJsonAsync(
            SleepLogsByIdUrl(baby.Id, Guid.NewGuid()),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSleepLog_MultipleBabies_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        BabyResponse otherBaby = await client.CreateBabyAsync();
        SleepLogResponse otherLog = await client.CreateSleepLogAsync(otherBaby.Id);
        SleepLogRequest updateRequest = TestRequests.FilledSleepLog;

        HttpResponseMessage response = await client.PutAsJsonAsync(
            SleepLogsByIdUrl(baby.Id, otherLog.Id),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSleepLog_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        SleepLogResponse otherLog = await otherClient.CreateSleepLogAsync(otherBaby.Id);
        SleepLogRequest updateRequest = TestRequests.FilledSleepLog;

        HttpResponseMessage response = await callerClient.PutAsJsonAsync(
            SleepLogsByIdUrl(otherBaby.Id, otherLog.Id),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSleepLog_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        SleepLogResponse log = await client.CreateSleepLogAsync(baby.Id);
        SleepLogRequest updateRequest = TestRequests.FilledSleepLog;

        HttpResponseMessage deleteBabyResponse = await client.DeleteAsync(
            BabiesByIdUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBabyResponse.StatusCode);

        HttpResponseMessage response = await client.PutAsJsonAsync(
            SleepLogsByIdUrl(baby.Id, log.Id),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSleepLog_EmptyValues_ClearsPreviousValues()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        SleepLogResponse log = await client.CreateSleepLogAsync(
            baby.Id,
            TestRequests.FilledSleepLog
        );

        SleepLogRequest updateRequest = TestRequests.SleepLog with
        {
            Location = null,
            WakeReasons = [],
            Notes = "",
            EndTime = null,
        };

        HttpResponseMessage updateResponse = await client.PutAsJsonAsync(
            SleepLogsByIdUrl(baby.Id, log.Id),
            updateRequest,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        HttpResponseMessage getResponse = await client.GetAsync(
            SleepLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        SleepLogResponse? retrievedLog =
            await getResponse.Content.ReadFromJsonAsync<SleepLogResponse>(TestCancellationToken);

        Assert.NotNull(retrievedLog);
        Assert.Equivalent(updateRequest, retrievedLog);
    }

    [Fact]
    public async Task DeleteSleepLog_OwnLog_ReturnsNoContent()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        SleepLogResponse log = await client.CreateSleepLogAsync(baby.Id);

        HttpResponseMessage deleteLogResponse = await client.DeleteAsync(
            SleepLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteLogResponse.StatusCode);

        HttpResponseMessage response = await client.GetAsync(
            SleepLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSleepLog_NonExistentLog_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();

        HttpResponseMessage response = await client.DeleteAsync(
            SleepLogsByIdUrl(baby.Id, Guid.NewGuid()),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSleepLog_MultipleBabies_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        BabyResponse otherBaby = await client.CreateBabyAsync();
        SleepLogResponse otherLog = await client.CreateSleepLogAsync(otherBaby.Id);

        HttpResponseMessage response = await client.DeleteAsync(
            SleepLogsByIdUrl(baby.Id, otherLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSleepLog_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        SleepLogResponse otherLog = await otherClient.CreateSleepLogAsync(otherBaby.Id);

        HttpResponseMessage response = await callerClient.DeleteAsync(
            SleepLogsByIdUrl(otherBaby.Id, otherLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSleepLog_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        SleepLogResponse log = await client.CreateSleepLogAsync(baby.Id);

        HttpResponseMessage deleteBabyResponse = await client.DeleteAsync(
            BabiesByIdUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBabyResponse.StatusCode);

        HttpResponseMessage response = await client.DeleteAsync(
            SleepLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
