using System.Net;
using System.Net.Http.Json;
using MilkAndMoon.Api.Contracts.Babies;
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
        CreateBabyRequest request = new(name, dateOfBirth ?? new DateOnly(2026, 1, 1));

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
}
