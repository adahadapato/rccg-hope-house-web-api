using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Infrastructure.Persistence.Converters;

/// <summary>
/// EF Core value converter for the <see cref="HtmlContent"/> Value Object.
/// Converts between domain VO and database string representation.
/// </summary>
public class HtmlContentConverter : ValueConverter<HtmlContent?, string?>
{
    /// <summary>
    /// Initializes the converter with mapping functions for domain ↔ database translation.
    /// </summary>
    public HtmlContentConverter() : base(
        // Domain → Database: extract the raw string value
        vo => vo.Value,
        // Database → Domain: reconstruct the Value Object with validation
        value => value != null ? HtmlContent.Create(value, false) : null)
    {
    }
}