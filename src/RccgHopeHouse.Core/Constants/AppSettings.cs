namespace RccgHopeHouse.Core.Constants;

/// <summary>
/// Strongly-typed configuration key constants.
/// Prevents magic strings in appsettings.json access.
/// </summary>
public static class AppSettings
{
    public static class Jwt
    {
        public const string Key = "Jwt:Key";
        public const string Issuer = "Jwt:Issuer";
        public const string Audience = "Jwt:Audience";
        public const string ExpiryMinutes = "Jwt:ExpiryMinutes";
    }

    public static class Storage
    {
        public const string Provider = "Storage:Provider";
        public const string AzureBlobConnectionString = "Storage:AzureBlob:ConnectionString";
        public const string AzureBlobContainer = "Storage:AzureBlob:ContainerName";
    }

    public static class YouTube
    {
        public const string ApiKey = "YouTube:ApiKey";
        public const string ThanksgivingPlaylistId = "YouTube:ThanksgivingPlaylistId";
    }

    public static class ConnectionStrings
    {
        public const string Default = "ConnectionStrings:DefaultConnection";
        public const string Redis = "ConnectionStrings:Redis";
    }
}