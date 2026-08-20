public class PumpLog : BaseLog
{
    public decimal LeftAmount { get; set; }
    public decimal RightAmount { get; set; }
    public DateTimeOffset? EndTime { get; set; }
}