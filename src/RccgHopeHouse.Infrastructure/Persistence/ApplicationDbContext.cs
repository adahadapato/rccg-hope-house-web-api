using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.ValueObjects;
using RccgHopeHouse.Infrastructure.Identity;

namespace RccgHopeHouse.Infrastructure.Persistence;

/// <summary>
/// Central EF Core DbContext for the RCCG Hope House application.
/// Manages domain entity mappings, Identity tables, value converters, and global query filters.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Initializes a new instance with the specified database provider options.
    /// </summary>
    /// <param name="options">The DbContextOptions configured in DependencyInjection.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // ==================== Domain DbSets ====================
    public DbSet<Sermon> Sermons => Set<Sermon>();
    public DbSet<PastorPost> PastorPosts => Set<PastorPost>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<GalleryCategory> GalleryCategories => Set<GalleryCategory>();
    public DbSet<GalleryTag> GalleryTags => Set<GalleryTag>();
    public DbSet<PrayerRequest> PrayerRequests => Set<PrayerRequest>();
    public DbSet<ChurchService> ChurchServices => Set<ChurchService>();
    public DbSet<ContactUs> ContactRequests => Set<ContactUs>();
    public DbSet<ThanksgivingService> ThanksgivingServices => Set<ThanksgivingService>();

    /// <summary>
    /// Configures the model schema, applies Fluent API configurations, and sets up value converters & global filters.
    /// </summary>
    /// <param name="builder">The ModelBuilder instance for schema configuration.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all IEntityTypeConfiguration implementations from this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure Value Object → Database column conversions
        ConfigureValueConverters(builder);

        // Configure many-to-many join relationships
        ConfigureManyToMany(builder);

        // Apply soft-delete global query filter to all BaseEntity-derived entities
        ConfigureGlobalFilters(builder);

        ConfigureAdditionalValueConverters(builder); // New method
    }

    /// <summary>
    /// Registers EF Core value converters for domain Value Objects.
    /// Ensures VOs are stored as primitive types but materialized as rich domain objects.
    /// </summary>
    private static void ConfigureValueConverters(ModelBuilder builder)
    {
        builder.Entity<PrayerRequest>()
            .Property(p => p.PhoneNumber)
            .HasConversion(
                vo => vo.Value,
                value => value != null ? Core.ValueObjects.PhoneNumber.CreateOrNull(value) : null);
    }

    /// <summary>
    /// Configures explicit many-to-many join entities for clean relationship management.
    /// </summary>
    private static void ConfigureManyToMany(ModelBuilder builder)
    {
        builder.Entity<GalleryImageTag>()
            .HasKey(t => new { t.ImageId, t.TagId });

        builder.Entity<GalleryImageTag>()
            .HasOne(t => t.Image)
            .WithMany(i => i.Tags)
            .HasForeignKey(t => t.ImageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GalleryImageTag>()
            .HasOne(t => t.Tag)
            .WithMany(tg => tg.Images)
            .HasForeignKey(t => t.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Applies a global soft-delete filter to all entities inheriting from <see cref="Core.Entities.BaseEntity"/>.
    /// Automatically excludes IsDeleted = true records from all queries unless explicitly overridden.
    /// </summary>
    private static void ConfigureGlobalFilters(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(Core.Entities.BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, "IsDeleted");
                var filter = System.Linq.Expressions.Expression.Lambda(
                    System.Linq.Expressions.Expression.Not(property), parameter);

                builder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    /// <summary>
    /// Registers additional EF Core value converters for domain Value Objects.
    /// Called from OnModelCreating to ensure proper database ↔ domain translation.
    /// </summary>
    private static void ConfigureAdditionalValueConverters(ModelBuilder builder)
    {
        // HtmlContent VO ↔ string
        builder.Entity<PastorPost>()
            .Property(p => p.Content)
            .HasConversion(
                vo => vo, // Store as string directly (since property is string)
                value => HtmlContent.Create(value, false).ToString());

        // SafeUrl VO ↔ string (for VideoUrl fields)
        builder.Entity<Sermon>()
            .Property(s => s.VideoUrl)
            .HasConversion(
                vo => vo, // Store as string directly (since property is string)
                value => value == null ? null : SafeUrl.Create(value, false).Value);

        // Add more converters as needed for other entities
    }
}