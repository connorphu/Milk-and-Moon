using MilkAndMoon.Api.Contracts.DiaperLogs;
using MilkAndMoon.Api.Contracts.FeedLogs;
using MilkAndMoon.Api.Contracts.PumpLogs;
using MilkAndMoon.Api.Contracts.SleepLogs;

namespace MilkAndMoon.Api.Tests.Infrastructure;

public static class TestRequests
{
    public static DiaperLogRequest DiaperLog =>
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

    public static DiaperLogRequest FilledDiaperLog =>
        new(
            "America/New_York",
            "both",
            "light yellow",
            ["yellow"],
            ["seedy"],
            "mild",
            ["back"],
            "test note",
            new DateTimeOffset(2026, 4, 15, 20, 0, 0, TimeSpan.Zero)
        );

    public static FeedLogRequest FeedLog =>
        new(
            "America/Chicago",
            0,
            "bottle",
            [],
            [],
            0,
            "",
            new DateTimeOffset(2026, 11, 23, 14, 30, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 11, 23, 15, 30, 0, TimeSpan.Zero)
        );

    public static FeedLogRequest FilledFeedLog =>
        new(
            "America/New_York",
            5.5m,
            "breast",
            ["left", "right"],
            ["breastmilk"],
            3.5m,
            "new note",
            new DateTimeOffset(2026, 4, 15, 20, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 15, 20, 30, 0, TimeSpan.Zero)
        );

    public static PumpLogRequest PumpLog =>
        new(
            "America/Chicago",
            0,
            0,
            "",
            new DateTimeOffset(2026, 11, 23, 14, 30, 0, TimeSpan.Zero),
            null
        );

    public static PumpLogRequest FilledPumpLog =>
        new(
            "America/New_York",
            3.5m,
            4.5m,
            "new note",
            new DateTimeOffset(2026, 4, 15, 20, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 15, 20, 20, 0, TimeSpan.Zero)
        );

    public static SleepLogRequest SleepLog =>
        new(
            "America/Chicago",
            null,
            [],
            "",
            new DateTimeOffset(2026, 11, 23, 14, 30, 0, TimeSpan.Zero),
            null
        );

    public static SleepLogRequest FilledSleepLog =>
        new(
            "America/New_York",
            "crib",
            ["hungry", "diaper"],
            "new note",
            new DateTimeOffset(2026, 4, 15, 20, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 15, 22, 0, 0, TimeSpan.Zero)
        );
}
