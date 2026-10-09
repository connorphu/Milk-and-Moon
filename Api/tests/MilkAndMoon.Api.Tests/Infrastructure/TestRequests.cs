using MilkAndMoon.Api.Contracts.DiaperLogs;

namespace MilkAndMoon.Api.Tests.Infrastructure;

public static class TestRequests
{
    public static CreateDiaperLogRequest DiaperLog =>
        new(
            "America/Chicago",
            "wet",
            "clear",
            [],
            [],
            null,
            [],
            "",
            new DateTimeOffset(2026, 11, 23, 14, 30, 0, TimeSpan.Zero)
        );

    public static UpdateDiaperLogRequest UpdateDiaperLog =>
        new(
            "America/Chicago",
            "both",
            "light yellow",
            ["yellow"],
            ["seedy"],
            "mild",
            ["back"],
            "test note"
        );
}
