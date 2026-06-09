using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Repositories.EF;

public static class ScreenshotSearch
{
    public const string TitleSearchVector = "TitleSearchVector";

    public const string FullSearchVector = "FullSearchVector";
}

public class ScreenshotDbContext(DbContextOptions<ScreenshotDbContext> options) : DbContext(options)
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

    public DbSet<AdminEntity> Admins => Set<AdminEntity>();

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
        ConfigureLog(modelBuilder);
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

                sp.Property(p => p.Points)
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

            if (Database.IsNpgsql())
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

            builder.HasIndex(b => b.UserId)
                   .IsUnique();

            builder.HasMany(b => b.Lines)
                   .WithOne()
                   .HasForeignKey(l => l.BasketId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BasketLineEntity>(builder =>
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.BasketId)
                   .IsRequired();

            builder.Property(l => l.ProductId)
                   .IsRequired();

            builder.Property(l => l.Quantity)
                   .IsRequired();

            builder.HasIndex(l => new { l.BasketId, l.ProductId })
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
                   .HasForeignKey(l => l.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.CreatedAt);
        });

        modelBuilder.Entity<OrderLineEntity>(builder =>
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.OrderId)
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

    private void ConfigureLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogEntity>(builder =>
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Message)
                   .IsRequired()
                   .HasMaxLength(4000);

            builder.Property(x => x.Created)
                   .IsRequired();

            builder.Property(x => x.Severity)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.Source)
                   .HasMaxLength(200);

            builder.Property(x => x.PropertiesJson);

            builder.HasIndex(x => x.Created);
            builder.HasIndex(x => x.Severity);
            builder.HasIndex(x => x.Source);

            if (Database.IsNpgsql())
            {
                builder.Property(x => x.Created)
                       .HasColumnType("timestamptz");

                builder.Property(x => x.PropertiesJson)
                       .HasColumnType("jsonb");

                builder.HasIndex(x => x.PropertiesJson)
                       .HasMethod("gin");

                builder.HasIndex(x => x.Created);
            }
            else if (Database.IsSqlite())
            {
                builder.Property(x => x.Created)
                       .HasColumnType("TEXT");

                builder.Property(x => x.PropertiesJson)
                       .HasColumnType("TEXT");
            }
        });
    }

    private void ConfigurePaymentAttempt(ModelBuilder modelBuilder)
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

            builder.Property(p => p.Refunded)
                    .IsRequired();

            builder.HasIndex(p => p.OrderId);
            builder.HasIndex(p => p.UserId);

            builder.HasIndex(p => p.ProviderPaymentId)
                   .IsUnique(false);

            if (Database.IsNpgsql())
            {
                builder.HasIndex(p => p.OrderId)
                       .IsUnique()
                       .HasFilter("\"IsPrimary\" = true");
            }
            else if (Database.IsSqlite())
            {
                builder.HasIndex(p => p.OrderId)
                       .IsUnique()
                       .HasFilter("IsPrimary = 1");
            }
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

            builder.Property(s => s.Amount)
                    .IsRequired();

            builder.Property(s => s.IsActive)
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

            entity.Property(x => x.TokenHash)
                   .HasMaxLength(128)
                   .IsRequired();

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.EncryptedData).IsRequired();

            entity.OwnsOne(x => x.TokenMetadata, meta =>
            {
                meta.Property(x => x.Issued)
                   .IsRequired();

                meta.OwnsOne(x => x.IssuedLocation, loc =>
                {
                    loc.Property(x => x.CountryCode)
                       .HasMaxLength(4);

                    loc.Property(x => x.Country)
                       .HasMaxLength(50);

                    loc.Property(x => x.City)
                       .HasMaxLength(100);
                });

                meta.Property(x => x.LastUsed);

                meta.OwnsOne(x => x.LastUsedLocation, loc =>
                {
                    loc.Property(x => x.CountryCode)
                       .HasMaxLength(4);

                    loc.Property(x => x.Country)
                       .HasMaxLength(50);

                    loc.Property(x => x.City)
                       .HasMaxLength(100);
                });

                meta.Property(x => x.Revoked);

                meta.OwnsOne(x => x.RevokeLocation, loc =>
                {
                    loc.Property(x => x.CountryCode)
                       .HasMaxLength(4);

                    loc.Property(x => x.Country)
                       .HasMaxLength(50);

                    loc.Property(x => x.City)
                       .HasMaxLength(100);
                });
            });
        });

        modelBuilder.Entity<RefreshTokenEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasIndex(x => x.FamilyId);
            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.Expires);

            entity.Property(x => x.TokenHash)
                  .IsRequired()
                  .HasMaxLength(128);

            entity.Property(x => x.FamilyId)
                  .IsRequired()
                  .HasMaxLength(64);

            entity.OwnsOne(x => x.TokenMetadata, meta =>
            {
                meta.OwnsOne(x => x.IssuedLocation, loc =>
                {
                    loc.Property(x => x.CountryCode)
                       .HasMaxLength(4);

                    loc.Property(x => x.Country)
                       .HasMaxLength(50);

                    loc.Property(x => x.City)
                       .HasMaxLength(100);
                });

                meta.Property(x => x.Revoked);

                meta.OwnsOne(x => x.RevokeLocation, loc =>
                {
                    loc.Property(x => x.CountryCode)
                       .HasMaxLength(4);

                    loc.Property(x => x.Country)
                       .HasMaxLength(50);

                    loc.Property(x => x.City)
                       .HasMaxLength(100);
                });
            });
        });
    }
}