using System.Net;
using System.Net.Http.Json;
using MilkAndMoon.Api.Contracts.Babies;
using MilkAndMoon.Api.Contracts.DiaperLogs;
using MilkAndMoon.Api.Contracts.FeedLogs;
using MilkAndMoon.Api.Contracts.Logs;
using MilkAndMoon.Api.Contracts.PumpLogs;
using MilkAndMoon.Api.Contracts.SleepLogs;
using MilkAndMoon.Api.Tests.Infrastructure;
using static MilkAndMoon.Api.Tests.Infrastructure.Routes;
using static MilkAndMoon.Api.Tests.Infrastructure.TestToken;

namespace MilkAndMoon.Api.Tests.Integration.Endpoints;

[Collection(ApiCollection.Name)]
public class LogsEndpointsTests(ApiFactory factory)
{
    private const string Chicago = "America/Chicago";

    // 2026-10-11 in America/Chicago (CDT, UTC-5) spans [2026-10-11T05:00Z, 2026-10-12T05:00Z)
    private static readonly DateOnly Day = new(2026, 10, 11);
    private static readonly DateTimeOffset DayStartUtc = new(2026, 10, 11, 5, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset DayEndUtc = new(2026, 10, 12, 5, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task GetLogs_NoToken_ReturnsUnauthorized()
    {
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync(LogsUrl, TestCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetLogs_NoStartDate_ReturnsBadRequest()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.GetAsync(
            $"{LogsUrl}?timezone={Chicago}",
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetLogs_UnknownTimezone_ReturnsBadRequest()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.GetAsync(
            LogsQueryUrl(Day, null, "Not/AZone"),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetLogs_EndDateBeforeStartDate_ReturnsBadRequest()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();

        HttpResponseMessage response = await client.GetAsync(
            LogsQueryUrl(Day, Day.AddDays(-1), Chicago),
            TestCancellationToken
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetLogs_StartDateOnly_ReturnsSingleDayLogs()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();

        await client.CreateFeedLogAsync(baby.Id, FeedLogAt(DayStartUtc.AddMinutes(-1)));
        FeedLogResponse atStart = await client.CreateFeedLogAsync(baby.Id, FeedLogAt(DayStartUtc));
        // Already 2026-10-12 in UTC, but still 2026-10-11 in Chicago
        FeedLogResponse lateLocal = await client.CreateFeedLogAsync(
            baby.Id,
            FeedLogAt(new DateTimeOffset(2026, 10, 12, 3, 0, 0, TimeSpan.Zero))
        );
        await client.CreateFeedLogAsync(baby.Id, FeedLogAt(DayEndUtc));

        List<LogResponse> logs = await GetLogsAsync(client, LogsQueryUrl(Day, null, Chicago));

        Assert.Collection(
            logs,
            log => Assert.Equal(atStart.Id, Assert.IsType<FeedLogResponse>(log).Id),
            log => Assert.Equal(lateLocal.Id, Assert.IsType<FeedLogResponse>(log).Id)
        );
    }

    [Fact]
    public async Task GetLogs_StartAndEndDate_ReturnsLogsInRange()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        DateOnly endDate = Day.AddDays(2);
        DateTimeOffset rangeEndUtc = DayEndUtc.AddDays(2);

        await client.CreateFeedLogAsync(baby.Id, FeedLogAt(DayStartUtc.AddMinutes(-1)));
        FeedLogResponse firstDay = await client.CreateFeedLogAsync(
            baby.Id,
            FeedLogAt(DayStartUtc.AddHours(12))
        );
        FeedLogResponse middleDay = await client.CreateFeedLogAsync(
            baby.Id,
            FeedLogAt(DayStartUtc.AddDays(1).AddHours(12))
        );
        FeedLogResponse lastDay = await client.CreateFeedLogAsync(
            baby.Id,
            FeedLogAt(rangeEndUtc.AddMinutes(-1))
        );
        await client.CreateFeedLogAsync(baby.Id, FeedLogAt(rangeEndUtc));

        List<LogResponse> logs = await GetLogsAsync(client, LogsQueryUrl(Day, endDate, Chicago));

        Assert.Equal(
            [firstDay.Id, middleDay.Id, lastDay.Id],
            logs.Select(l => Assert.IsType<FeedLogResponse>(l).Id)
        );
    }

    [Fact]
    public async Task GetLogs_DaylightSavingEnds_UsesOffsetAtEachBoundary()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();

        // 2026-11-01 in America/Chicago falls back CDT -> CST, so the day is 25 hours:
        // [2026-11-01T05:00Z (UTC-5), 2026-11-02T06:00Z (UTC-6))
        DateOnly fallBackDay = new(2026, 11, 1);
        DateTimeOffset startUtc = new(2026, 11, 1, 5, 0, 0, TimeSpan.Zero);
        DateTimeOffset endUtc = new(2026, 11, 2, 6, 0, 0, TimeSpan.Zero);

        await client.CreateFeedLogAsync(baby.Id, FeedLogAt(startUtc.AddMinutes(-1)));
        FeedLogResponse atStart = await client.CreateFeedLogAsync(baby.Id, FeedLogAt(startUtc));
        // 23:30 CST on 2026-11-01; excluded if the end boundary reuses the start's UTC-5 offset
        FeedLogResponse extraHour = await client.CreateFeedLogAsync(
            baby.Id,
            FeedLogAt(endUtc.AddMinutes(-30))
        );
        await client.CreateFeedLogAsync(baby.Id, FeedLogAt(endUtc));

        List<LogResponse> logs = await GetLogsAsync(
            client,
            LogsQueryUrl(fallBackDay, null, Chicago)
        );

        Assert.Equal(
            [atStart.Id, extraHour.Id],
            logs.Select(l => Assert.IsType<FeedLogResponse>(l).Id)
        );
    }

    [Fact]
    public async Task GetLogs_AllLogTypes_ReturnsLogsOrderedByStartTime()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();

        // Created out of chronological order so ordering comes from the endpoint
        PumpLogResponse pump = await client.CreatePumpLogAsync(
            TestRequests.PumpLog with
            {
                StartTime = DayStartUtc.AddHours(4),
            }
        );
        FeedLogResponse feed = await client.CreateFeedLogAsync(
            baby.Id,
            FeedLogAt(DayStartUtc.AddHours(1))
        );
        SleepLogResponse sleep = await client.CreateSleepLogAsync(
            baby.Id,
            TestRequests.SleepLog with
            {
                StartTime = DayStartUtc.AddHours(3),
            }
        );
        DiaperLogResponse diaper = await client.CreateDiaperLogAsync(
            baby.Id,
            TestRequests.DiaperLog with
            {
                StartTime = DayStartUtc.AddHours(2),
            }
        );

        List<LogResponse> logs = await GetLogsAsync(client, LogsQueryUrl(Day, null, Chicago));

        // Full equivalence, not just Id, so a field dropped from /logs fails here
        Assert.Collection(
            logs,
            log => Assert.Equivalent(feed, Assert.IsType<FeedLogResponse>(log)),
            log => Assert.Equivalent(diaper, Assert.IsType<DiaperLogResponse>(log)),
            log => Assert.Equivalent(sleep, Assert.IsType<SleepLogResponse>(log)),
            log => Assert.Equivalent(pump, Assert.IsType<PumpLogResponse>(log))
        );
    }

    [Fact]
    public async Task GetLogs_MultipleBabies_ReturnsLogsForAllBabies()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse oldest = await client.CreateBabyAsync("Oldest");
        BabyResponse youngest = await client.CreateBabyAsync("Youngest");

        FeedLogResponse oldestFeed = await client.CreateFeedLogAsync(
            oldest.Id,
            FeedLogAt(DayStartUtc.AddHours(1))
        );
        DiaperLogResponse youngestDiaper = await client.CreateDiaperLogAsync(
            youngest.Id,
            TestRequests.DiaperLog with
            {
                StartTime = DayStartUtc.AddHours(2),
            }
        );

        List<LogResponse> logs = await GetLogsAsync(client, LogsQueryUrl(Day, null, Chicago));

        Assert.Collection(
            logs,
            log =>
            {
                FeedLogResponse feed = Assert.IsType<FeedLogResponse>(log);
                Assert.Equal(oldestFeed.Id, feed.Id);
                Assert.Equal(oldest.Id, feed.BabyId);
                Assert.Equal(oldest.Name, feed.BabyName);
            },
            log =>
            {
                DiaperLogResponse diaper = Assert.IsType<DiaperLogResponse>(log);
                Assert.Equal(youngestDiaper.Id, diaper.Id);
                Assert.Equal(youngest.Id, diaper.BabyId);
                Assert.Equal(youngest.Name, diaper.BabyName);
            }
        );
    }

    [Fact]
    public async Task GetLogs_NoBabies_ReturnsPumpLogs()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        PumpLogResponse pump = await client.CreatePumpLogAsync(
            TestRequests.PumpLog with
            {
                StartTime = DayStartUtc.AddHours(1),
            }
        );

        List<LogResponse> logs = await GetLogsAsync(client, LogsQueryUrl(Day, null, Chicago));

        LogResponse log = Assert.Single(logs);
        Assert.Equal(pump.Id, Assert.IsType<PumpLogResponse>(log).Id);
    }

    [Fact]
    public async Task GetLogs_DeletedBaby_ExcludesItsLogs()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse deletedBaby = await client.CreateBabyAsync("Delete");
        BabyResponse keptBaby = await client.CreateBabyAsync("Keep");

        await client.CreateFeedLogAsync(deletedBaby.Id, FeedLogAt(DayStartUtc.AddHours(1)));
        FeedLogResponse kept = await client.CreateFeedLogAsync(
            keptBaby.Id,
            FeedLogAt(DayStartUtc.AddHours(2))
        );

        HttpResponseMessage deleteResponse = await client.DeleteAsync(
            BabiesByIdUrl(deletedBaby.Id),
            TestCancellationToken
        );
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        List<LogResponse> logs = await GetLogsAsync(client, LogsQueryUrl(Day, null, Chicago));

        LogResponse log = Assert.Single(logs);
        Assert.Equal(kept.Id, Assert.IsType<FeedLogResponse>(log).Id);
    }

    [Fact]
    public async Task GetLogs_OtherUsersLogs_ReturnsEmpty()
    {
        HttpClient otherClient = await factory.CreateAuthenticatedClientAsync();
        BabyResponse otherBaby = await otherClient.CreateBabyAsync();
        await otherClient.CreateFeedLogAsync(otherBaby.Id, FeedLogAt(DayStartUtc.AddHours(1)));
        await otherClient.CreatePumpLogAsync(
            TestRequests.PumpLog with
            {
                StartTime = DayStartUtc.AddHours(2),
            }
        );

        HttpClient client = await factory.CreateAuthenticatedClientAsync();

        List<LogResponse> logs = await GetLogsAsync(client, LogsQueryUrl(Day, null, Chicago));

        Assert.Empty(logs);
    }

    [Fact]
    public async Task GetLogs_NoTimezone_DefaultsToUtc()
    {
        HttpClient client = await factory.CreateAuthenticatedClientAsync();
        BabyResponse baby = await client.CreateBabyAsync();
        DateTimeOffset utcMidnight = new(2026, 10, 11, 0, 0, 0, TimeSpan.Zero);

        await client.CreateFeedLogAsync(baby.Id, FeedLogAt(utcMidnight.AddMinutes(-1)));
        // Would be 2026-10-10 in Chicago, so only included when the day is UTC
        FeedLogResponse atStart = await client.CreateFeedLogAsync(baby.Id, FeedLogAt(utcMidnight));
        await client.CreateFeedLogAsync(baby.Id, FeedLogAt(utcMidnight.AddDays(1)));

        List<LogResponse> logs = await GetLogsAsync(client, LogsQueryUrl(Day, null, null));

        LogResponse log = Assert.Single(logs);
        Assert.Equal(atStart.Id, Assert.IsType<FeedLogResponse>(log).Id);
    }

    private static FeedLogRequest FeedLogAt(DateTimeOffset startTime) =>
        TestRequests.FeedLog with
        {
            StartTime = startTime,
            EndTime = startTime.AddMinutes(30),
        };

    private static async Task<List<LogResponse>> GetLogsAsync(HttpClient client, string url)
    {
        HttpResponseMessage response = await client.GetAsync(url, TestCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<LogResponse>? logs = await response.Content.ReadFromJsonAsync<List<LogResponse>>(
            TestCancellationToken
        );
        Assert.NotNull(logs);

        return logs;
    }
}
