using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Repositories.EF;

public class ScreenshotDbContext(DbContextOptions<ScreenshotDbContext> options, IOptions<StorageConfigurations> consifurations) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<ScreenshotEntity> Screenshots => Set<ScreenshotEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder);
        ConfigureScreenshot(modelBuilder);
        ConfigureCategories(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(builder =>
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.EncryptedData)
                   .IsRequired();

            builder.Property(u => u.EncKey)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.Property(u => u.Password)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.Property(u => u.Salt)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.OwnsOne(u => u.SubscriptionPlan, sp =>
            {
                sp.Property(p => p.Type)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

                sp.Property(p => p.ScreenshotLeft)
                   .IsRequired();
            });
        });
    }

    private void ConfigureScreenshot(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScreenshotEntity>(builder =>
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                   .HasMaxLength(32)
                   .IsRequired();

            builder.Property(s => s.WebsiteUrl)
                   .HasMaxLength(2048)
                   .IsRequired();

            builder.Property(s => s.UserId)
                   .IsRequired();

            builder.Property(s => s.CreatedAt)
                   .IsRequired();

            builder.Property(s => s.State)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(s => s.Type)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(s => s.Title)
                   .HasMaxLength(150);

            builder.Property(s => s.Description)
                   .HasMaxLength(500);

            builder.HasOne<UserEntity>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.UserId);

            builder.HasIndex(s => new { s.UserId, s.CreatedAt });

            builder.HasIndex(s => s.Title);
            builder.HasIndex(s => s.Description);

            if (consifurations.Value.Provider == Provider.Postgres)
            {
                builder.HasGeneratedTsVectorColumn(s => s.SearchVector, "simple", s => new { s.Title, s.Description })
                       .HasIndex(s => s.SearchVector)
                       .HasMethod("GIN");

                builder.HasGeneratedTsVectorColumn(s => s.TitleVector, "simple", s => new { s.Title })
                       .HasIndex(s => s.TitleVector)
                       .HasMethod("GIN");
            }

            // MANY-TO-MANY
            builder.HasMany(s => s.Categories)
                   .WithMany()
                   .UsingEntity<Dictionary<string, object>>(
                       "ScreenshotCategories",
                       j => j
                           .HasOne<CategoryEntity>()
                           .WithMany()
                           .HasForeignKey("CategoryId")
                           .OnDelete(DeleteBehavior.Restrict),

                       j => j
                           .HasOne<ScreenshotEntity>()
                           .WithMany()
                           .HasForeignKey("ScreenshotId")
                           .OnDelete(DeleteBehavior.Cascade),
                       j =>
                       {
                           j.HasKey("ScreenshotId", "CategoryId");
                           j.HasIndex("CategoryId");
                           j.HasIndex("ScreenshotId");
                       });
        });
    }

    public void ConfigureCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoryEntity>(builder =>
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(c => c.Color)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.HasIndex(c => new { c.UserId });
        });
    }
}