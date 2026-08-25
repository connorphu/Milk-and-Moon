public abstract class BabyLog : BaseLog
{
    public Baby Baby { get; set; } = null!;
    public Guid BabyId { get; set; }
}