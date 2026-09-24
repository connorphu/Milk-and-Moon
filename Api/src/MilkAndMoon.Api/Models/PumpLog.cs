namespace MilkAndMoon.Api.Models;

public class PumpLog : BaseLog
{
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }
    public decimal LeftAmount { get; set; }
    public decimal RightAmount { get; set; }
    public DateTimeOffset? EndTime { get; set; }
}
