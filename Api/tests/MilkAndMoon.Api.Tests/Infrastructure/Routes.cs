namespace MilkAndMoon.Api.Tests.Infrastructure;

public static class Routes
{
    public const string BabiesUrl = "/babies";

    public static string DiaperLogsUrl(Guid babyId) => $"{BabiesUrl}/{babyId}/diaper-logs";

    public static string DiaperLogsByIdUrl(Guid babyId, Guid id) => $"{DiaperLogsUrl(babyId)}/{id}";
}
