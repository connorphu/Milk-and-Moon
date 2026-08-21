public class BaseLog : BaseModel
{
    public Baby Baby { get; set; } = null!;
    public Guid BabyId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public string Timezone { get; set; } = null!;
    public string? Notes { get; set; }
}
