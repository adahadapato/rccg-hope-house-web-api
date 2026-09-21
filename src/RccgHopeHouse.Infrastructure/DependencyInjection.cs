using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
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
/// This is the technical implementation composition root: EF Core, Identity, repositories, and external APIs.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all infrastructure services and composes the Application layer.
    /// Call this from Program.cs or API startup to wire the complete dependency graph.
    /// </summary>
    /// <param name="services">The IServiceCollection to configure.</param>
    /// <param name="configuration">Application configuration for connection strings, JWT secrets, and API keys.</param>
    /// <returns>The modified IServiceCollection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Compose Application Layer (MediatR + Validation)
        services.AddApplication();

        // 2. EF Core Database Context
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql
                    .EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
                    .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

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
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
        //.AddDefaultTokenProviders();

        // Add these registrations to the existing AddInfrastructure method

        // Register JwtTokenService for dedicated token operations
        services.AddSingleton<JwtTokenService>();

        // Register Value Converters (if using explicit registration instead of automatic)
        // services.AddScoped<HtmlContentConverter>();
        // services.AddScoped<SafeUrlConverter>();

        // 4. Repository Implementations
        services.AddScoped<ISermonRepository, SermonRepository>();
        services.AddScoped<IPastorPostRepository, PastorPostRepository>();
        services.AddScoped<IGalleryRepository, GalleryRepository>();
        services.AddScoped<IPrayerRequestRepository, PrayerRequestRepository>();
        services.AddScoped<IChurchServiceRepository, ChurchServiceRepository>();
        services.AddScoped<IContactUsRepository, ContactUsRepository>();
        services.AddScoped<IThemeOfTheYearRepository, ThemeOfTheYearRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IChurchInfoRepository, ChurchInfoRepository>();
        services.AddScoped<IServiceBroadcastRepository, ServiceBroadcastRepository>();
        services.AddScoped<IGivingTypeRepository, GivingTypeRepository>();
        services.AddScoped<IOfferingRepository, OfferingRepository>();
        services.AddScoped<IGalleryCategoryRepository, GalleryCategoryRepository>();


        // 5. Service Implementations
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IYouTubeService, YouTubeService>();
        services.AddScoped<IImageStorageService, ImageStorageService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // 6. HTTP Clients for External APIs
        services.AddHttpClient<IYouTubeService, YouTubeService>(client =>
        {
            client.BaseAddress = new Uri("https://www.googleapis.com/youtube/v3/");
            client.DefaultRequestHeaders.Add("User-Agent", "RCCG-HopeHouse-Website/1.0");
        });

        // 7. Caching & Settings
        services.AddMemoryCache();
        
        services.AddSingleton(new NotificationSettings(
            configuration["EmailSettings:AdminContactEmail"] ?? "admin@rccghopehouse.org.uk",
            configuration["EmailSettings:AdminContactName"] ?? "RCCG Hope House Team",
            configuration["AppSettings:AdminDashboardUrl"] ?? "https://localhost:7153/admin"));


        // YouTube API
        services.AddHttpClient<IYouTubeService, YouTubeService>(client =>
        {
            client.BaseAddress = new Uri("https://www.googleapis.com/youtube/v3/");
            client.DefaultRequestHeaders.Add(
                "User-Agent",
                "RCCG-HopeHouse-Website/1.0");
        });

        // API.Bible configuration
        services.Configure<ApiBibleOptions>(
            configuration.GetSection(ApiBibleOptions.SectionName));

        // API.Bible HTTP client
        services.AddHttpClient<IApiBibleService, ApiBibleService>(
            (serviceProvider, client) =>
            {
                var options = serviceProvider
            .GetRequiredService<
                Microsoft.Extensions.Options.IOptions<ApiBibleOptions>>()
            .Value;

                client.BaseAddress = new Uri(options.BaseUrl);

                client.DefaultRequestHeaders.Add(
            "api-key",
            options.ApiKey);

                client.DefaultRequestHeaders.Add(
            "User-Agent",
            "RCCG-HopeHouse-Website/1.0");
            });

        return services;
    }
}