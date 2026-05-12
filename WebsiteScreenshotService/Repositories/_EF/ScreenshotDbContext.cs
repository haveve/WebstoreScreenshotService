using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using NpgsqlTypes;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Repositories.EF;

public static class ScreenshotSearch
{
    public const string TitleSearchVector = "TitleSearchVector";

    public const string FullSearchVector = "FullSearchVector";
}

public class ScreenshotDbContext(DbContextOptions<ScreenshotDbContext> options, IOptions<StorageConfigurations> consifurations) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();

    public DbSet<ScreenshotEntity> Screenshots => Set<ScreenshotEntity>();

    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();

    public DbSet<BasketEntity> Baskets => Set<BasketEntity>();

    public DbSet<BasketLineEntity> BasketLines => Set<BasketLineEntity>();

    public DbSet<OrderEntity> Orders => Set<OrderEntity>();

    public DbSet<OrderLineEntity> OrderLines => Set<OrderLineEntity>();

    public DbSet<PaymentAttemptEntity> PaymentAttempts => Set<PaymentAttemptEntity>();

    public DbSet<SubscriptionEntity> Subscriptions => Set<SubscriptionEntity>();

    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    public DbSet<ApiTokenEntity> ApiTokens => Set<ApiTokenEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder);
        ConfigureScreenshot(modelBuilder);
        ConfigureCategories(modelBuilder);
        ConfigureBasket(modelBuilder);
        ConfigureOrder(modelBuilder);
        ConfigurePaymentAttempt(modelBuilder);
        ConfigureSubscription(modelBuilder);
        ConfigureAuth(modelBuilder);
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

            builder.Property(u => u.NickNameHash)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.Property(u => u.PasswordHash)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.Property(u => u.Salt)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.HasIndex(u => u.NickNameHash)
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
                builder.Property<NpgsqlTsVector>(ScreenshotSearch.TitleSearchVector)
                    .HasComputedColumnSql(@"
                    to_tsvector('simple', coalesce(""Title"", ''))
                ", stored: true);

                builder.HasIndex(ScreenshotSearch.TitleSearchVector)
                    .HasMethod("GIN");

                builder.Property<NpgsqlTsVector>(ScreenshotSearch.FullSearchVector)
                    .HasComputedColumnSql(@"
                    to_tsvector('simple',
                        coalesce(""Title"", '') || ' ' || coalesce(""Description"", '')
                    )
                ", stored: true);

                builder.HasIndex(ScreenshotSearch.FullSearchVector)
                    .HasMethod("GIN");
            }

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

    private static void ConfigureBasket(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BasketEntity>(builder =>
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.UserId)
                   .IsRequired();

            // Enforce ONE basket per user
            builder.HasIndex(b => b.UserId)
                   .IsUnique();

            builder.HasMany(b => b.Lines)
                   .WithOne()
                   .HasForeignKey("BasketId")
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BasketLineEntity>(builder =>
        {
            builder.HasKey(l => l.Id);

            builder.Property<Guid>("BasketId") // shadow FK
                   .IsRequired();

            builder.Property(l => l.ProductId)
                   .IsRequired();

            builder.Property(l => l.Quantity)
                   .IsRequired();

            // Optional: prevent duplicate product in same basket
            builder.HasIndex("BasketId", nameof(BasketLineEntity.ProductId))
                   .IsUnique();
        });
    }

    private static void ConfigureOrder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderEntity>(builder =>
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserId)
                   .IsRequired();

            builder.Property(o => o.TotalAmount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(o => o.Currency)
                   .HasMaxLength(10)
                   .IsRequired();

            builder.Property(o => o.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(o => o.CreatedAt)
                   .IsRequired();

            builder.HasMany(o => o.Lines)
                   .WithOne()
                   .HasForeignKey("OrderId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.CreatedAt);
        });

        modelBuilder.Entity<OrderLineEntity>(builder =>
        {
            builder.HasKey(l => l.Id);

            builder.Property<Guid>("OrderId")
                   .IsRequired();

            builder.Property(l => l.ProductId)
                   .IsRequired();

            builder.Property(l => l.Quantity)
                   .IsRequired();

            builder.Property(l => l.UnitPrice)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(l => l.ProductName)
                   .HasMaxLength(200)
                   .IsRequired();
        });
    }

    private static void ConfigurePaymentAttempt(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentAttemptEntity>(builder =>
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.OrderId)
                   .IsRequired();

            builder.Property(p => p.UserId)
                   .IsRequired();

            builder.Property(p => p.Amount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(p => p.Provider)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(p => p.ProviderPaymentId)
                   .HasMaxLength(200);

            builder.Property(p => p.CreatedAt)
                   .IsRequired();

            builder.HasIndex(p => p.OrderId);
            builder.HasIndex(p => p.UserId);

            builder.HasIndex(p => p.ProviderPaymentId)
                   .IsUnique(false);

            builder.HasIndex(p => p.OrderId)
                   .IsUnique()
                   .HasFilter("\"IsPrimary\" = true");
        });
    }

    private static void ConfigureSubscription(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SubscriptionEntity>(builder =>
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.UserId)
                   .IsRequired();

            builder.Property(s => s.Type)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(s => s.Provider)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(s => s.ProviderSubscriptionId)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(s => s.SubscriptionPeriod)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(s => s.CurrentPeriodEnd)
                   .IsRequired();

            builder.Property(s => s.CreatedAt)
                   .IsRequired();

            builder.Property(s => s.EncryptedData)
                   .IsRequired();

            builder.HasIndex(s => s.UserId);
            builder.HasIndex(s => s.ProviderSubscriptionId)
                   .IsUnique();
        });
    }

    private static void ConfigureAuth(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApiTokenEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasIndex(x => x.Expires);
            entity.HasIndex(x => x.UserId);

            entity.Property(x => x.TokenHash).IsRequired();
            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.EncryptedData).IsRequired();

            entity.OwnsOne(x => x.TokenMetadata, meta =>
            {
                meta.Property(x => x.Issued);

                meta.OwnsOne(x => x.IssuedLocation);

                meta.Property(x => x.LastUsed);

                meta.OwnsOne(x => x.LastUsedLocation);

                meta.Property(x => x.Revoked);

                meta.OwnsOne(x => x.RevokeLocation);
            });
        });

        modelBuilder.Entity<RefreshTokenEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasIndex(x => x.FamilyId);
            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.Expires);

            entity.Property(x => x.TokenHash).IsRequired();
            entity.Property(x => x.FamilyId).IsRequired();

            entity.OwnsOne(x => x.TokenMetadata, meta =>
            {
                meta.Property(x => x.Issued);

                meta.OwnsOne(x => x.IssuedLocation);

                meta.Property(x => x.Revoked);

                meta.OwnsOne(x => x.RevokeLocation);
            });
        });
    }
}