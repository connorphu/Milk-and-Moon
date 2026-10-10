using System.Net;
using System.Net.Http.Json;
using MilkAndMoon.Api.Contracts.Babies;
using MilkAndMoon.Api.Contracts.DiaperLogs;
using static MilkAndMoon.Api.Tests.Infrastructure.Routes;
using static MilkAndMoon.Api.Tests.Infrastructure.TestToken;

namespace MilkAndMoon.Api.Tests.Infrastructure;

public static class HttpClientExtensions
{
    public static async Task<BabyResponse> CreateBabyAsync(
        this HttpClient client,
        string name = "Baby",
        DateOnly? dateOfBirth = null
    )
    {
        BabyRequest request = new(name, dateOfBirth ?? new DateOnly(2026, 1, 1));

        HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            BabiesUrl,
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        BabyResponse? baby = await createResponse.Content.ReadFromJsonAsync<BabyResponse>(
            TestCancellationToken
        );
        Assert.NotNull(baby);

        return baby;
    }

    public static async Task<TResponse> PostCreatedAsync<TRequest, TResponse>(
        this HttpClient client,
        string url,
        TRequest request
    )
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            url,
            request,
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        TResponse? body = await response.Content.ReadFromJsonAsync<TResponse>(
            TestCancellationToken
        );
        Assert.NotNull(body);

        return body;
    }

    public static Task<DiaperLogResponse> CreateDiaperLogAsync(
        this HttpClient client,
        Guid babyId,
        DiaperLogRequest? request = null
    ) =>
        client.PostCreatedAsync<DiaperLogRequest, DiaperLogResponse>(
            DiaperLogsUrl(babyId),
            request ?? TestRequests.DiaperLog
        );
}
