namespace MilkAndMoon.Api.Models;

public abstract class BaseLog : BaseModel
{
    public DateTimeOffset StartTime { get; set; }
    public string Timezone { get; set; } = null!;
    public string Notes { get; set; } = string.Empty;
}
