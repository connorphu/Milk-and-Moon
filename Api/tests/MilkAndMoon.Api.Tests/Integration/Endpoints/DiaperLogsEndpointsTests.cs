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
    [InlineData("PUT", FakeDiaperLogsByIdUrl)]
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
        DiaperLogRequest request = TestRequests.DiaperLog with
        {
            DiaperType = "both",
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
            BabiesByIdUrl(baby.Id),
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

    [Fact]
    public async Task GetDiaperLogs_HasLogs_ReturnsAllLogs()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        BabyResponse otherBaby = await client.CreateBabyAsync();
        DiaperLogResponse log1 = await client.CreateDiaperLogAsync(
            baby.Id,
            TestRequests.DiaperLog with
            {
                DiaperType = "both",
            }
        );
        DiaperLogResponse log2 = await client.CreateDiaperLogAsync(
            baby.Id,
            TestRequests.DiaperLog with
            {
                PeeColor = "light yellow",
            }
        );
        await client.CreateDiaperLogAsync(
            otherBaby.Id,
            TestRequests.DiaperLog with
            {
                Notes = "Other fellow",
            }
        );
        List<DiaperLogResponse> expected = [log1, log2];

        HttpResponseMessage response = await client.GetAsync(
            DiaperLogsUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<DiaperLogResponse>? logs = await response.Content.ReadFromJsonAsync<
            List<DiaperLogResponse>
        >(TestCancellationToken);
        Assert.NotNull(logs);
        Assert.Equivalent(expected, logs, true);
    }

    [Fact]
    public async Task GetDiaperLogs_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        DiaperLogResponse log = await client.CreateDiaperLogAsync(baby.Id);
        List<DiaperLogResponse> expectedBeforeBabyDelete = [log];

        HttpResponseMessage getLogsBeforeBabyDelete = await client.GetAsync(
            DiaperLogsUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getLogsBeforeBabyDelete.StatusCode);

        List<DiaperLogResponse>? logsBeforeBabyDelete =
            await getLogsBeforeBabyDelete.Content.ReadFromJsonAsync<List<DiaperLogResponse>>(
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
            DiaperLogsUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NotFound, getLogsAfterBabyDelete.StatusCode);
    }

    [Fact]
    public async Task GetDiaperLogs_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        await otherClient.CreateDiaperLogAsync(
            otherBaby.Id,
            TestRequests.DiaperLog with
            {
                PeeColor = "light yellow",
            }
        );

        HttpResponseMessage response = await callerClient.GetAsync(
            DiaperLogsUrl(otherBaby.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDiaperLogs_NonExistentBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.GetAsync(
            FakeDiaperLogsUrl,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDiaperLogById_OwnLog_ReturnsLog()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        DiaperLogResponse log = await client.CreateDiaperLogAsync(
            baby.Id,
            TestRequests.DiaperLog with
            {
                Timezone = "America/Chicago",
                DiaperType = "both",
                PeeColor = "light yellow",
                StoolColor = ["yellow"],
                StoolTexture = ["seedy"],
                Rash = "mild",
                RashLocation = ["back"],
                Notes = "test note",
            }
        );

        HttpResponseMessage response = await client.GetAsync(
            DiaperLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        DiaperLogResponse? retrievedLog =
            await response.Content.ReadFromJsonAsync<DiaperLogResponse>(TestCancellationToken);
        Assert.NotNull(retrievedLog);
        Assert.Equivalent(log, retrievedLog);
    }

    [Fact]
    public async Task GetDiaperLogById_NonExistentLog_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        await client.CreateDiaperLogAsync(baby.Id);

        HttpResponseMessage response = await client.GetAsync(
            DiaperLogsByIdUrl(baby.Id, Guid.NewGuid()),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDiaperLogById_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        DiaperLogResponse otherLog = await otherClient.CreateDiaperLogAsync(otherBaby.Id);

        HttpResponseMessage response = await callerClient.GetAsync(
            DiaperLogsByIdUrl(otherBaby.Id, otherLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDiaperLogById_MultipleBabies_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync("Oldest");
        BabyResponse otherBaby = await client.CreateBabyAsync("Youngest");
        DiaperLogResponse otherBabyLog = await client.CreateDiaperLogAsync(otherBaby.Id);

        HttpResponseMessage response = await client.GetAsync(
            DiaperLogsByIdUrl(baby.Id, otherBabyLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDiaperLogById_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        DiaperLogResponse log = await client.CreateDiaperLogAsync(baby.Id);

        HttpResponseMessage deleteBabyResponse = await client.DeleteAsync(
            BabiesByIdUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBabyResponse.StatusCode);

        HttpResponseMessage response = await client.GetAsync(
            DiaperLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDiaperLog_OwnLog_ReturnsUpdatedLog()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        DiaperLogResponse log = await client.CreateDiaperLogAsync(baby.Id);
        DiaperLogRequest updateRequest = new(
            "America/Denver",
            "both",
            "clear",
            ["yellow"],
            ["seedy"],
            "mild",
            ["back"],
            "test note",
            new DateTimeOffset(2026, 3, 2, 16, 30, 0, TimeSpan.Zero)
        );

        HttpResponseMessage updateResponse = await client.PutAsJsonAsync(
            DiaperLogsByIdUrl(baby.Id, log.Id),
            updateRequest,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        DiaperLogResponse? updatedLog =
            await updateResponse.Content.ReadFromJsonAsync<DiaperLogResponse>(
                TestCancellationToken
            );
        Assert.NotNull(updatedLog);

        HttpResponseMessage getResponse = await client.GetAsync(
            DiaperLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        DiaperLogResponse? retrievedLog =
            await getResponse.Content.ReadFromJsonAsync<DiaperLogResponse>(TestCancellationToken);

        DiaperLogResponse expected = log with
        {
            Timezone = "America/Denver",
            DiaperType = "both",
            PeeColor = "clear",
            StoolColor = ["yellow"],
            StoolTexture = ["seedy"],
            Rash = "mild",
            RashLocation = ["back"],
            Notes = "test note",
            StartTime = new DateTimeOffset(2026, 3, 2, 16, 30, 0, TimeSpan.Zero),
        };

        Assert.NotNull(retrievedLog);
        Assert.Equivalent(expected, updatedLog);
        Assert.Equivalent(expected, retrievedLog);
    }

    [Fact]
    public async Task UpdateDiaperLog_NonExistentLog_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        DiaperLogRequest updateRequest = TestRequests.UpdateDiaperLog;

        HttpResponseMessage response = await client.PutAsJsonAsync(
            DiaperLogsByIdUrl(baby.Id, Guid.NewGuid()),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDiaperLog_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        DiaperLogResponse otherLog = await otherClient.CreateDiaperLogAsync(otherBaby.Id);
        DiaperLogRequest updateRequest = TestRequests.UpdateDiaperLog;

        HttpResponseMessage response = await callerClient.PutAsJsonAsync(
            DiaperLogsByIdUrl(otherBaby.Id, otherLog.Id),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDiaperLog_MultipleBabies_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        BabyResponse otherBaby = await client.CreateBabyAsync();
        DiaperLogResponse otherLog = await client.CreateDiaperLogAsync(otherBaby.Id);
        DiaperLogRequest updateRequest = TestRequests.UpdateDiaperLog;

        HttpResponseMessage response = await client.PutAsJsonAsync(
            DiaperLogsByIdUrl(baby.Id, otherLog.Id),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDiaperLog_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        DiaperLogResponse log = await client.CreateDiaperLogAsync(baby.Id);
        DiaperLogRequest updateRequest = TestRequests.UpdateDiaperLog;

        HttpResponseMessage deleteBabyResponse = await client.DeleteAsync(
            BabiesByIdUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBabyResponse.StatusCode);

        HttpResponseMessage response = await client.PutAsJsonAsync(
            DiaperLogsByIdUrl(baby.Id, log.Id),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDiaperLog_OwnLog_ReturnsNoContent()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        DiaperLogResponse log = await client.CreateDiaperLogAsync(baby.Id);

        HttpResponseMessage deleteLogResponse = await client.DeleteAsync(
            DiaperLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteLogResponse.StatusCode);

        HttpResponseMessage response = await client.GetAsync(
            DiaperLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDiaperLog_NonExistentLog_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();

        HttpResponseMessage response = await client.DeleteAsync(
            DiaperLogsByIdUrl(baby.Id, Guid.NewGuid()),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDiaperLog_MultipleBabies_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        BabyResponse otherBaby = await client.CreateBabyAsync();
        DiaperLogResponse otherLog = await client.CreateDiaperLogAsync(otherBaby.Id);

        HttpResponseMessage response = await client.DeleteAsync(
            DiaperLogsByIdUrl(baby.Id, otherLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDiaperLog_OtherUsersBaby_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        DiaperLogResponse otherLog = await otherClient.CreateDiaperLogAsync(otherBaby.Id);

        HttpResponseMessage response = await callerClient.DeleteAsync(
            DiaperLogsByIdUrl(otherBaby.Id, otherLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDiaperLog_DeletedBaby_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        DiaperLogResponse log = await client.CreateDiaperLogAsync(baby.Id);

        HttpResponseMessage deleteBabyResponse = await client.DeleteAsync(
            BabiesByIdUrl(baby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteBabyResponse.StatusCode);

        HttpResponseMessage response = await client.DeleteAsync(
            DiaperLogsByIdUrl(baby.Id, log.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
