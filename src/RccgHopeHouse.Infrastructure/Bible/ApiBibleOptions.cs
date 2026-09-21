namespace RccgHopeHouse.Infrastructure.Bible;

public sealed class ApiBibleOptions
{
    public const string SectionName = "ApiBible";

    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;
}
