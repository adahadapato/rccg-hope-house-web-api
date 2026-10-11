using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RccgHopeHouse.Application;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Interfaces.Admin;
using RccgHopeHouse.Core.Interfaces.AI;
using RccgHopeHouse.Infrastructure.Identity;
using RccgHopeHouse.Infrastructure.Persistence;
using RccgHopeHouse.Infrastructure.Persistence.Repositories;
using RccgHopeHouse.Infrastructure.Persistence.Repositories.Admin;
using RccgHopeHouse.Infrastructure.Services;
using RccgHopeHouse.Infrastructure.Services.AI;

namespace RccgHopeHouse.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure-layer services.
/// This is the technical implementation composition root:
/// EF Core, Identity, repositories, storage services and external APIs.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all infrastructure services and composes
    /// the Application layer.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Application layer
        services.AddApplication();

        // 2. EF Core
        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration[AppSettings.ConnectionStrings.Default], sql =>
                        sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
                            .MigrationsAssembly(
                                typeof(ApplicationDbContext)
                                    .Assembly.FullName)));

        // 3. ASP.NET Core Identity
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // 4. Token service
        services.AddSingleton<JwtTokenService>();

        // 5. Repositories
        services.AddScoped<ISermonRepository, SermonRepository>();
        services.AddScoped<IPastorPostRepository, PastorPostRepository>();
        services.AddScoped<IGalleryRepository, GalleryRepository>();
        services.AddScoped<IGalleryCategoryRepository, GalleryCategoryRepository>();
        services.AddScoped<IPrayerRequestRepository, PrayerRequestRepository>();
        services.AddScoped<IChurchServiceRepository, ChurchServiceRepository>();
        services.AddScoped<IContactUsRepository, ContactUsRepository>();
        services.AddScoped<IThemeOfTheYearRepository, ThemeOfTheYearRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IChurchInfoRepository, ChurchInfoRepository>();
        services.AddScoped<IServiceBroadcastRepository, ServiceBroadcastRepository>();
        services.AddScoped<IGivingTypeRepository, GivingTypeRepository>();
        services.AddScoped<IOfferingRepository, OfferingRepository>();
        services.AddScoped<IProphecyYearRepository, ProphecyYearRepository>();
        services.AddScoped<IProphecyCategoryRepository, ProphecyCategoryRepository>();
        services.AddScoped<IProphecyRepository, ProphecyRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IAnnualPrayerRepository, AnnualPrayerRepository>();
        services.AddScoped<IChurchEventRepository, ChurchEventRepository>();
        services.AddScoped<IDevotionalRepository, DevotionalRepository>();
        services.AddScoped<IBibleReferenceService, BibleReferenceService>();
        services.AddScoped<ISeoMetadataRepository, SeoMetadataRepository>();

        // 6. Application-facing infrastructure services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IImageStorageService, ImageStorageService>();
        services.AddScoped<IGalleryImageStorage, FileSystemGalleryImageStorageService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAccountService, AccountService>();

        // 7. YouTube API
        services.AddScoped<IYouTubeService, YouTubeService>();

        // 8. AI services
        services.AddHttpClient<IServiceBroadcastDescriptionCleaner,  ServiceBroadcastDescriptionCleaner>();
        services.AddHttpClient<IOpenHeavensDevotionalService, OpenHeavensDevotionalService>();
        services.AddHttpClient<ISeoService, SeoService>();

        // 9. API.Bible
        services.AddHttpClient<IApiBibleService, ApiBibleService>((serviceProvider, client) =>
        {
            var baseUrl = configuration[AppSettings.ApiBible.BaseUrl];
            var apiKey = configuration[AppSettings.ApiBible.ApiKey];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("API.Bible BaseUrl is not configured.");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("API.Bible ApiKey is not configured.");

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("api-key", apiKey);
            client.DefaultRequestHeaders.Add("User-Agent", "RCCG-HopeHouse-Website/1.0");
        });

        // 10. Caching
        services.AddMemoryCache();

        // 11. Notification settings
        services.AddSingleton(
            new NotificationSettings(
                configuration[AppSettings.Email.AdminContactEmail] ?? "admin@rccghopehouse.org.uk",
                configuration[AppSettings.Email.AdminContactName] ?? "RCCG Hope House Team",
                configuration[AppSettings.Application.AdminDashboardUrl] ?? "https://localhost:7153/admin"));

        return services;
    }
}