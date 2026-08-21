public class FeedLog : BaseLog
{
    public decimal BottleSize { get; set; }
    public string FeedType { get; set; } = null!;
    public string[] BreastSide { get; set; } = null!;
    public string[] MilkType { get; set; } = null!;
    public decimal MilkConsumed { get; set; }
    public DateTimeOffset? EndTime { get; set; }
}
