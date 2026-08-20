public class DiaperLog : BaseLog
{
    public string DiaperType { get; set; } = null!;
    public string? PeeColor { get; set; }
    public string[] StoolColor { get; set; } = null!;
    public string[] StoolTexture { get; set; } = null!;
    public string? Rash { get; set; }
    public string[] RashLocation { get; set; } = null!;
}