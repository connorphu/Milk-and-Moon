namespace MilkAndMoon.Api.Contracts.Logs;

public record LogResponse(
    Guid Id,
    string TrackerType,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string Timezone,
    object Data
);
