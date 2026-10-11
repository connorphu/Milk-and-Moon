namespace MilkAndMoon.Api.Tests.Infrastructure;

public static class Routes
{
    public const string BabiesUrl = "/babies";

    public static string BabiesByIdUrl(Guid babyId) => $"{BabiesUrl}/{babyId}";

    public static string DiaperLogsUrl(Guid babyId) => $"{BabiesUrl}/{babyId}/diaper-logs";

    public static string DiaperLogsByIdUrl(Guid babyId, Guid id) => $"{DiaperLogsUrl(babyId)}/{id}";

    public static string FeedLogsUrl(Guid babyId) => $"{BabiesUrl}/{babyId}/feed-logs";

    public static string FeedLogsByIdUrl(Guid babyId, Guid id) => $"{FeedLogsUrl(babyId)}/{id}";

    public const string PumpLogsUrl = "/pump-logs";

    public static string PumpLogsByIdUrl(Guid id) => $"{PumpLogsUrl}/{id}";
}
