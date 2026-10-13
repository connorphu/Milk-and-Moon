using System.Text.Json.Serialization;
using MilkAndMoon.Api.Contracts.DiaperLogs;
using MilkAndMoon.Api.Contracts.FeedLogs;
using MilkAndMoon.Api.Contracts.PumpLogs;
using MilkAndMoon.Api.Contracts.SleepLogs;

namespace MilkAndMoon.Api.Contracts.Logs;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "trackerType")]
[JsonDerivedType(typeof(FeedLogResponse), "feed")]
[JsonDerivedType(typeof(DiaperLogResponse), "diaper")]
[JsonDerivedType(typeof(SleepLogResponse), "sleep")]
[JsonDerivedType(typeof(PumpLogResponse), "pump")]
public abstract record LogResponse(DateTimeOffset StartTime);
