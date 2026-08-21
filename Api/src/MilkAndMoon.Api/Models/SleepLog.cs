public class SleepLog : BaseLog
{
    public string? Location { get; set; }
    public string[] WakeReasons { get; set; } = null!;
    public DateTimeOffset? EndTime { get; set; }
}
