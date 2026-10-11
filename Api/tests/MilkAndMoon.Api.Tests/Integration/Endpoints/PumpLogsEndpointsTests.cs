using System.Net;
using System.Net.Http.Json;
using MilkAndMoon.Api.Contracts.PumpLogs;
using MilkAndMoon.Api.Tests.Infrastructure;
using static MilkAndMoon.Api.Tests.Infrastructure.Routes;
using static MilkAndMoon.Api.Tests.Infrastructure.TestToken;

namespace MilkAndMoon.Api.Tests.Integration.Endpoints;

[Collection(ApiCollection.Name)]
public class PumpLogsEndpointsTests(ApiFactory factory)
{
    private const string FakeId = "00000000-0000-0000-0000-000000000001";
    private const string FakePumpLogsByIdUrl = $"{PumpLogsUrl}/{FakeId}";

    [Theory]
    [InlineData("GET", PumpLogsUrl)]
    [InlineData("GET", FakePumpLogsByIdUrl)]
    [InlineData("POST", PumpLogsUrl)]
    [InlineData("PUT", FakePumpLogsByIdUrl)]
    [InlineData("DELETE", FakePumpLogsByIdUrl)]
    public async Task AnyEndpoint_NoToken_ReturnsUnauthorized(string method, string url)
    {
        HttpClient client = factory.CreateClient();

        HttpRequestMessage request = new(new HttpMethod(method), url);

        HttpResponseMessage response = await client.SendAsync(request, TestCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePumpLog_ValidRequest_ReturnsCreatedPumpLog()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        PumpLogRequest request = TestRequests.FilledPumpLog;

        HttpResponseMessage response = await client.PostAsJsonAsync(
            PumpLogsUrl,
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        PumpLogResponse? pumpLog = await response.Content.ReadFromJsonAsync<PumpLogResponse>(
            TestCancellationToken
        );
        Assert.NotNull(pumpLog);
        Assert.Equivalent(request, pumpLog);
        Assert.NotEqual(Guid.Empty, pumpLog.Id);
        Assert.NotEqual(Guid.Empty, pumpLog.UserId);
        Assert.Equal(PumpLogsByIdUrl(pumpLog.Id), response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task GetPumpLogs_HasLogs_ReturnsAllLogs()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        PumpLogResponse log1 = await client.CreatePumpLogAsync();
        PumpLogResponse log2 = await client.CreatePumpLogAsync(TestRequests.FilledPumpLog);
        List<PumpLogResponse> expected = [log1, log2];

        HttpResponseMessage response = await client.GetAsync(PumpLogsUrl, TestCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<PumpLogResponse>? logs = await response.Content.ReadFromJsonAsync<
            List<PumpLogResponse>
        >(TestCancellationToken);
        Assert.NotNull(logs);
        Assert.Equivalent(expected, logs, true);
    }

    [Fact]
    public async Task GetPumpLogs_OtherUsersLogs_NotIncluded()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        PumpLogResponse callerLog = await callerClient.CreatePumpLogAsync();
        await otherClient.CreatePumpLogAsync(
            TestRequests.PumpLog with
            {
                Notes = "Other fellow",
            }
        );
        List<PumpLogResponse> expected = [callerLog];

        HttpResponseMessage response = await callerClient.GetAsync(
            PumpLogsUrl,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<PumpLogResponse>? logs = await response.Content.ReadFromJsonAsync<
            List<PumpLogResponse>
        >(TestCancellationToken);
        Assert.NotNull(logs);
        Assert.Equivalent(expected, logs, true);
    }

    [Fact]
    public async Task GetPumpLogById_OwnLog_ReturnsLog()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        PumpLogResponse log = await client.CreatePumpLogAsync(TestRequests.FilledPumpLog);

        HttpResponseMessage response = await client.GetAsync(
            PumpLogsByIdUrl(log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        PumpLogResponse? retrievedLog = await response.Content.ReadFromJsonAsync<PumpLogResponse>(
            TestCancellationToken
        );
        Assert.NotNull(retrievedLog);
        Assert.Equivalent(log, retrievedLog);
    }

    [Fact]
    public async Task GetPumpLogById_NonExistentLog_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        await client.CreatePumpLogAsync();

        HttpResponseMessage response = await client.GetAsync(
            PumpLogsByIdUrl(Guid.NewGuid()),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPumpLogById_OtherUsersLog_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        PumpLogResponse otherLog = await otherClient.CreatePumpLogAsync();

        HttpResponseMessage response = await callerClient.GetAsync(
            PumpLogsByIdUrl(otherLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePumpLog_OwnLog_ReturnsUpdatedLog()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        PumpLogResponse log = await client.CreatePumpLogAsync();
        PumpLogRequest updateRequest = TestRequests.FilledPumpLog;

        HttpResponseMessage updateResponse = await client.PutAsJsonAsync(
            PumpLogsByIdUrl(log.Id),
            updateRequest,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        PumpLogResponse? updatedLog =
            await updateResponse.Content.ReadFromJsonAsync<PumpLogResponse>(TestCancellationToken);
        Assert.NotNull(updatedLog);

        HttpResponseMessage getResponse = await client.GetAsync(
            PumpLogsByIdUrl(log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        PumpLogResponse? retrievedLog =
            await getResponse.Content.ReadFromJsonAsync<PumpLogResponse>(TestCancellationToken);

        PumpLogResponse expected = log with
        {
            Timezone = updateRequest.Timezone,
            LeftAmount = updateRequest.LeftAmount,
            RightAmount = updateRequest.RightAmount,
            Notes = updateRequest.Notes,
            StartTime = updateRequest.StartTime,
            EndTime = updateRequest.EndTime,
        };

        Assert.NotNull(retrievedLog);
        Assert.Equivalent(expected, updatedLog);
        Assert.Equivalent(expected, retrievedLog);
    }

    [Fact]
    public async Task UpdatePumpLog_NonExistentLog_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        PumpLogRequest updateRequest = TestRequests.FilledPumpLog;

        HttpResponseMessage response = await client.PutAsJsonAsync(
            PumpLogsByIdUrl(Guid.NewGuid()),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePumpLog_OtherUsersLog_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        PumpLogResponse otherLog = await otherClient.CreatePumpLogAsync();
        PumpLogRequest updateRequest = TestRequests.FilledPumpLog;

        HttpResponseMessage response = await callerClient.PutAsJsonAsync(
            PumpLogsByIdUrl(otherLog.Id),
            updateRequest,
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePumpLog_EmptyValues_ClearsPreviousValues()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        PumpLogResponse log = await client.CreatePumpLogAsync(TestRequests.FilledPumpLog);

        PumpLogRequest updateRequest = TestRequests.PumpLog with
        {
            LeftAmount = 0,
            RightAmount = 0,
            Notes = "",
            EndTime = null,
        };

        HttpResponseMessage updateResponse = await client.PutAsJsonAsync(
            PumpLogsByIdUrl(log.Id),
            updateRequest,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        HttpResponseMessage getResponse = await client.GetAsync(
            PumpLogsByIdUrl(log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        PumpLogResponse? retrievedLog =
            await getResponse.Content.ReadFromJsonAsync<PumpLogResponse>(TestCancellationToken);

        Assert.NotNull(retrievedLog);
        Assert.Equivalent(updateRequest, retrievedLog);
    }

    [Fact]
    public async Task DeletePumpLog_OwnLog_ReturnsNoContent()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        PumpLogResponse log = await client.CreatePumpLogAsync();

        HttpResponseMessage deleteLogResponse = await client.DeleteAsync(
            PumpLogsByIdUrl(log.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteLogResponse.StatusCode);

        HttpResponseMessage response = await client.GetAsync(
            PumpLogsByIdUrl(log.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletePumpLog_NonExistentLog_ReturnsNotFound()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.DeleteAsync(
            PumpLogsByIdUrl(Guid.NewGuid()),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletePumpLog_OtherUsersLog_ReturnsNotFound()
    {
        HttpClient callerClient = await factory.CreateAuthenticatedClientAsync();
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        PumpLogResponse otherLog = await otherClient.CreatePumpLogAsync();

        HttpResponseMessage response = await callerClient.DeleteAsync(
            PumpLogsByIdUrl(otherLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        HttpResponseMessage getResponse = await otherClient.GetAsync(
            PumpLogsByIdUrl(otherLog.Id),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }
}
