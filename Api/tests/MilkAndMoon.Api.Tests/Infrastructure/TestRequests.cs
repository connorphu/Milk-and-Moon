using MilkAndMoon.Api.Contracts.DiaperLogs;

namespace MilkAndMoon.Api.Tests.Infrastructure;

public static class TestRequests
{
    public static CreateDiaperLogRequest DiaperLog =>
        new("America/Chicago", "wet", "clear", [], [], null, [], "");
}
