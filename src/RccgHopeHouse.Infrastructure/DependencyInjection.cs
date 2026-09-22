using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RccgHopeHouse.Application;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Bible;
using RccgHopeHouse.Infrastructure.Identity;
using RccgHopeHouse.Infrastructure.Persistence;
using RccgHopeHouse.Infrastructure.Persistence.Repositories;
using RccgHopeHouse.Infrastructure.Services;

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
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Application layer
        services.AddApplication();

        // 2. EF Core
        services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "DefaultConnection"),
                    sql =>
                        sql.EnableRetryOnFailure(
                                5,
                                TimeSpan.FromSeconds(10),
                                null)
                            .MigrationsAssembly(
                                typeof(ApplicationDbContext)
                                    .Assembly
                                    .FullName)));

        // 3. ASP.NET Core Identity
        services.AddIdentityCore<ApplicationUser>(
            options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;

                options.User.RequireUniqueEmail = true;

                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(5);

                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // 4. Token service
        services.AddSingleton<JwtTokenService>();

        // 5. Repositories
        services.AddScoped<
            ISermonRepository,
            SermonRepository>();

        services.AddScoped<
            IPastorPostRepository,
            PastorPostRepository>();

        services.AddScoped<
            IGalleryRepository,
            GalleryRepository>();

        services.AddScoped<
            IGalleryCategoryRepository,
            GalleryCategoryRepository>();

        services.AddScoped<
            IPrayerRequestRepository,
            PrayerRequestRepository>();

        services.AddScoped<
            IChurchServiceRepository,
            ChurchServiceRepository>();

        services.AddScoped<
            IContactUsRepository,
            ContactUsRepository>();

        services.AddScoped<
            IThemeOfTheYearRepository,
            ThemeOfTheYearRepository>();

        services.AddScoped<
            IMemberRepository,
            MemberRepository>();

        services.AddScoped<
            IChurchInfoRepository,
            ChurchInfoRepository>();

        services.AddScoped<
            IServiceBroadcastRepository,
            ServiceBroadcastRepository>();

        services.AddScoped<
            IGivingTypeRepository,
            GivingTypeRepository>();

        services.AddScoped<
            IOfferingRepository,
            OfferingRepository>();

        // 6. Application-facing infrastructure services
        services.AddScoped<
            IAuthService,
            AuthService>();

        services.AddScoped<
            IEmailService,
            EmailService>();

        services.AddScoped<
            IImageStorageService,
            ImageStorageService>();

        services.AddScoped<
            IGalleryImageStorage,
            FileSystemGalleryImageStorageService>();

        services.AddScoped<
            ICurrentUserService,
            CurrentUserService>();

        // 7. YouTube API
        //
        // A typed HttpClient registration also registers
        // IYouTubeService, so a separate AddScoped registration
        // is unnecessary.
        services.AddHttpClient<
            IYouTubeService,
            YouTubeService>(
            client =>
            {
                client.BaseAddress =
                    new Uri(
                        "https://www.googleapis.com/youtube/v3/");

                client.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "RCCG-HopeHouse-Website/1.0");
            });

        // 8. API.Bible
        services.Configure<ApiBibleOptions>(
            configuration.GetSection(
                ApiBibleOptions.SectionName));

        services.AddHttpClient<
            IApiBibleService,
            ApiBibleService>(
            (serviceProvider, client) =>
            {
                var options =
                    serviceProvider
                        .GetRequiredService<
                            Microsoft.Extensions.Options
                                .IOptions<ApiBibleOptions>>()
                        .Value;

                client.BaseAddress =
                    new Uri(options.BaseUrl);

                client.DefaultRequestHeaders.Add(
                    "api-key",
                    options.ApiKey);

                client.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "RCCG-HopeHouse-Website/1.0");
            });

        // 9. Caching
        services.AddMemoryCache();

        // 10. Notification settings
        services.AddSingleton(
            new NotificationSettings(
                configuration[
                    "EmailSettings:AdminContactEmail"]
                    ?? "admin@rccghopehouse.org.uk",

                configuration[
                    "EmailSettings:AdminContactName"]
                    ?? "RCCG Hope House Team",

                configuration[
                    "AppSettings:AdminDashboardUrl"]
                    ?? "https://localhost:7153/admin"));

        return services;
    }
}