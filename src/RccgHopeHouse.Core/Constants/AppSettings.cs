namespace RccgHopeHouse.Core.Constants;

/// <summary>
/// Provides strongly typed constants for application configuration keys.
/// This prevents configuration paths from being duplicated as magic strings
/// throughout the application.
/// </summary>
public static class AppSettings
{
    /// <summary>
    /// Configuration keys for Open Heavens devotional integration.
    /// </summary>
    public static class OpenHeavens
    {
        public const string BaseUrl = "OpenHeavens:BaseUrl";
    }

    /// <summary>
    /// Contains configuration keys used for JSON Web Token (JWT)
    /// authentication and token generation.
    /// </summary>
    public static class Jwt
    {
        /// <summary>
        /// Gets the configuration key for the secret key used
        /// to sign and validate JWT tokens.
        /// </summary>
        public const string Key = "Jwt:Key";

        /// <summary>
        /// Gets the configuration key for the expected JWT issuer.
        /// </summary>
        public const string Issuer = "Jwt:Issuer";

        /// <summary>
        /// Gets the configuration key for the expected JWT audience.
        /// </summary>
        public const string Audience = "Jwt:Audience";

        /// <summary>
        /// Gets the configuration key for the JWT token lifetime,
        /// expressed in minutes.
        /// </summary>
        public const string ExpiryMinutes = "Jwt:ExpiryMinutes";
    }

    /// <summary>
    /// Contains configuration keys used by the application's
    /// file and object storage services.
    /// </summary>
    public static class Storage
    {
        /// <summary>
        /// Gets the configuration key identifying the active
        /// storage provider.
        /// </summary>
        public const string Provider = "Storage:Provider";

        /// <summary>
        /// Gets the configuration key for the Azure Blob Storage
        /// connection string.
        /// </summary>
        public const string AzureBlobConnectionString =
            "Storage:AzureBlob:ConnectionString";

        /// <summary>
        /// Gets the configuration key for the Azure Blob Storage
        /// container name.
        /// </summary>
        public const string AzureBlobContainer =
            "Storage:AzureBlob:ContainerName";
    }

    /// <summary>
    /// Contains configuration keys used by YouTube integrations,
    /// including video retrieval and automatic service broadcast
    /// synchronization.
    /// </summary>
    public static class YouTube
    {
        /// <summary>
        /// Gets the configuration key for the YouTube Data API key.
        /// The actual API key should be supplied through secure
        /// configuration such as user secrets or environment variables.
        /// </summary>
        public const string ApiKey =   "YouTube:ApiKey";

        /// <summary>
        /// Gets the configuration key for the existing Thanksgiving
        /// service YouTube playlist identifier.
        /// </summary>
        public const string ThanksgivingPlaylistId =    "YouTube:ThanksgivingPlaylistId";

        /// <summary>
        /// Gets the configuration section containing the approved
        /// YouTube channels that may be monitored for service broadcasts.
        /// </summary>
        public const string Channels =   "YouTube:Channels";
    }

    /// <summary>
    /// Contains configuration keys for application database
    /// and cache connection strings.
    /// </summary>
    public static class ConnectionStrings
    {
        /// <summary>
        /// Gets the configuration key for the application's
        /// primary database connection string.
        /// </summary>
        public const string Default =  "ConnectionStrings:DefaultConnection";

        /// <summary>
        /// Gets the configuration key for the Redis
        /// connection string.
        /// </summary>
        public const string Redis =     "ConnectionStrings:Redis";
    }

    /// <summary>
    /// Contains general application-level configuration keys.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Gets the configuration key for the administration
        /// dashboard URL.
        /// </summary>
        public const string AdminDashboardUrl =  "AppSettings:AdminDashboardUrl";
    }

    /// <summary>
    /// OpenAI configuration keys.
    /// </summary>
    public static class OpenAI
    {
        /// <summary>
        /// Configuration key for the OpenAI API key.
        /// </summary>
        public const string ApiKey = "OpenAI:ApiKey";

        /// <summary>
        /// Configuration key for the OpenAI model.
        /// </summary>
        public const string Model = "OpenAI:Model";
    }
}