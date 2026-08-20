using Microsoft.EntityFrameworkCore;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Baby> Babies { get; set; } = null!;
    public DbSet<FeedLog> FeedLogs { get; set; } = null!;
    public DbSet<SleepLog> SleepLogs { get; set; } = null!;
    public DbSet<PumpLog> PumpLogs { get; set; } = null!;
    public DbSet<DiaperLog> DiaperLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}