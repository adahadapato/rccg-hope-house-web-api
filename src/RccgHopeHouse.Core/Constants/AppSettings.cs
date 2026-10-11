
namespace RccgHopeHouse.Core.Constants;

/// <summary>
/// Provides strongly typed constants for application configuration keys.
/// Centralises configuration paths to avoid duplicated magic strings
/// throughout the application.
/// </summary>
public static class AppSettings
{
    // ==================== Serilog ====================

    /// <summary>
    /// Application logging configuration.
    /// </summary>
    public static class Serilog
    {
        public const string MinimumLevel = "Serilog:MinimumLevel";

        public const string DefaultMinimumLevel = "Serilog:MinimumLevel:Default";

        public const string MicrosoftMinimumLevel = "Serilog:MinimumLevel:Override:Microsoft";
        public const string SystemMinimumLevel = "Serilog:MinimumLevel:Override:System";

        public const string WriteTo = "Serilog:WriteTo";
        public const string Enrich =  "Serilog:Enrich";
    }

    // ==================== Connection Strings ====================

    /// <summary>
    /// Database and cache connection string configuration.
    /// </summary>
    public static class ConnectionStrings
    {
        public const string Default = "ConnectionStrings:DefaultConnection";

        public const string Redis = "ConnectionStrings:Redis";
    }

    // ==================== JWT ====================

    /// <summary>
    /// JSON Web Token authentication configuration.
    /// </summary>
    public static class Jwt
    {
        public const string Key = "Jwt:Key";

        public const string Issuer = "Jwt:Issuer";
        public const string Audience = "Jwt:Audience";

        public const string ExpiryMinutes = "Jwt:ExpiryMinutes";
    }

    // ==================== Application ====================

    /// <summary>
    /// General application and website configuration.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Backend API base URL.
        /// </summary>
        public const string ApiBaseUrl = "AppSettings:ApiBaseUrl";

        /// <summary>
        /// Public website base URL used for SEO,
        /// canonical URLs, sitemaps and website links.
        /// </summary>
        public const string WebsiteBaseUrl = "AppSettings:WebsiteBaseUrl";

        /// <summary>
        /// Administration dashboard URL.
        /// Retained for compatibility with existing code.
        /// </summary>
        public const string AdminDashboardUrl = "AppSettings:AdminDashboardUrl";
    }

    // ==================== Email ====================

    /// <summary>
    /// SMTP and email account configuration.
    /// </summary>
    public static class Email
    {
        public const string AdminContactEmail = "EmailSettings:AdminContactEmail";

        public const string AdminContactName = "EmailSettings:AdminContactName";
        public const string SmtpHost = "EmailSettings:SmtpHost";

        public const string SmtpPort = "EmailSettings:SmtpPort";
        public const string AdminUsername = "EmailSettings:AdminUsername";

        public const string AdminPassword = "EmailSettings:AdminPassword";
        public const string InfoUsername = "EmailSettings:InfoUsername";

        public const string InfoPassword = "EmailSettings:InfoPassword";
        public const string DefaultFromAddress = "EmailSettings:DefaultFromAddress";

        public const string DefaultFromName = "EmailSettings:DefaultFromName";
    }

    // ==================== YouTube ====================

    /// <summary>
    /// YouTube Data API and broadcast configuration.
    /// </summary>
    public static class YouTube
    {
        public const string ApiKey = "YouTube:ApiKey";

        public const string ThanksgivingPlaylistId = "YouTube:ThanksgivingPlaylistId";
        public const string Channels = "YouTube:Channels";
    }

    // ==================== ImageSharp ====================

    /// <summary>
    /// ImageSharp licensing configuration.
    /// </summary>
    public static class ImageSharp
    {
        public const string LicenseKey =  "ImageSharp:LicenseKey";
    }

    // ==================== Seed Data ====================

    /// <summary>
    /// Initial administrator account configuration.
    /// </summary>
    public static class SeedData
    {
        public const string AdminEmail = "SeedData:AdminEmail";

        public const string AdminPassword = "SeedData:AdminPassword";
    }

    // ==================== API Bible ====================

    /// <summary>
    /// API.Bible integration configuration.
    /// </summary>
    public static class ApiBible
    {
        public const string BaseUrl = "ApiBible:BaseUrl";

        public const string ApiKey = "ApiBible:ApiKey";
    }

    // ==================== OpenAI ====================

    /// <summary>
    /// OpenAI configuration shared by AI-powered features,
    /// including devotionals and Smart SEO.
    /// </summary>
    public static class OpenAI
    {
        public const string ApiKey = "OpenAI:ApiKey";
        public const string BaseUrl = "OpenAI:BaseUrl";
        public const string Model = "OpenAI:Model";
    }

    // ==================== Open Heavens ====================

    /// <summary>
    /// Open Heavens devotional source configuration.
    /// </summary>
    public static class OpenHeavens
    {
        public const string BaseUrl = "OpenHeavens:BaseUrl";
    }

    // ==================== Storage ====================

    /// <summary>
    /// File and object storage configuration.
    /// Retained for compatibility with existing services.
    /// </summary>
    public static class Storage
    {
        public const string Provider = "Storage:Provider";

        public const string AzureBlobConnectionString = "Storage:AzureBlob:ConnectionString";
        public const string AzureBlobContainer = "Storage:AzureBlob:ContainerName";
    }

    // ==================== Hosting ====================

    /// <summary>
    /// ASP.NET Core hosting configuration.
    /// </summary>
    public static class Hosting
    {
        public const string AllowedHosts = "AllowedHosts";
    }
}
