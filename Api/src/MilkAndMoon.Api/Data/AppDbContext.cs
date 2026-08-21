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

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(u => u.Name).HasDefaultValue("");
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(u => u.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<Baby>(entity =>
        {
            entity.Property(b => b.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(b => b.Name).HasDefaultValue("");
            entity.Property(b => b.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(b => b.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<FeedLog>(entity =>
        {
            entity.Property(f => f.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(f => f.BottleSize).HasDefaultValue(0).HasPrecision(3, 1);
            entity.Property(f => f.BreastSide).HasDefaultValueSql("'{}'");
            entity.Property(f => f.MilkType).HasDefaultValueSql("'{}'");
            entity.Property(f => f.MilkConsumed).HasDefaultValue(0).HasPrecision(3, 1);
            entity.Property(f => f.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(f => f.UpdatedAt).HasDefaultValueSql("now()");

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("feed_logs_feed_type",
                    "feed_type IN ('bottle', 'breast')");

                t.HasCheckConstraint("feed_logs_breast_side",
                    "breast_side <@ ARRAY['left','right']::text[]");

                t.HasCheckConstraint("feed_logs_milk_type",
                    "milk_type <@ ARRAY['breastmilk','formula']::text[]");
            });
        });

        modelBuilder.Entity<SleepLog>(entity => 
        {
            entity.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(s => s.WakeReasons).HasDefaultValueSql("'{}'");
            entity.Property(s => s.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(s => s.UpdatedAt).HasDefaultValueSql("now()");

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("sleep_logs_locations",
                    "location IN ('bassinet','crib','contactNap','stroller','car','other')");
                t.HasCheckConstraint("sleep_logs_wake_reasons",
                    "wake_reasons <@ ARRAY['hungry','diaper','naturally','noise','moved','unknown']::text[]");
            });
        });

        modelBuilder.Entity<DiaperLog>(entity =>
        {
            entity.Property(d => d.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(d => d.StoolColor).HasDefaultValueSql("'{}'");
            entity.Property(d => d.StoolTexture).HasDefaultValueSql("'{}'");
            entity.Property(d => d.RashLocation).HasDefaultValueSql("'{}'");
            entity.Property(d => d.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(d => d.UpdatedAt).HasDefaultValueSql("now()");

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("diaper_logs_diaper_types",
                    "diaper_type IN ('wet','poopy','both')");

                t.HasCheckConstraint("diaper_logs_pee_colors",
                    "pee_color IN ('clear','light yellow','dark yellow','orange')");

                t.HasCheckConstraint("diaper_logs_stool_colors",
                    "stool_color <@ ARRAY['black','yellow','green','brown','red','white','other']::text[]");

                t.HasCheckConstraint("diaper_logs_stool_textures",
                    "stool_texture <@ ARRAY['seedy','soft','loose','watery','mucus','hard','other']::text[]");

                t.HasCheckConstraint("diaper_logs_rash",
                    "rash IN ('mild','moderate','severe')");

                t.HasCheckConstraint("diaper_logs_rash_locations",
                    "rash_location <@ ARRAY['front','back','folds','aroundAnus','other']::text[]");
            });
        });

        modelBuilder.Entity<PumpLog>(entity =>
        {
            entity.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(p => p.LeftAmount).HasDefaultValue(0).HasPrecision(3, 1);
            entity.Property(p => p.RightAmount).HasDefaultValue(0).HasPrecision(3, 1);
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(p => p.UpdatedAt).HasDefaultValueSql("now()");
        });
    }
}