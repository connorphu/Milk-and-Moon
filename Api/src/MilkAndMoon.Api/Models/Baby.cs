public class Baby : BaseModel
{
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
