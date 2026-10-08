using MilkAndMoon.Api.Contracts.DiaperLogs;

namespace MilkAndMoon.Api.Tests.Infrastructure;

public static class TestRequests
{
    public static CreateDiaperLogRequest DiaperLog =>
        new("American/Chicago", "wet", "clear", [], [], "", [], "");
}
