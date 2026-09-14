using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Infrastructure.Persistence.Converters;

/// <summary>
/// EF Core value converter for the <see cref="SafeUrl"/> Value Object.
/// Ensures URL validation occurs at the domain boundary, not in the database.
/// </summary>
public class SafeUrlConverter : ValueConverter<SafeUrl?, string?>
{
    /// <summary>
    /// Initializes the converter with mapping functions for domain ↔ database translation.
    /// </summary>
    public SafeUrlConverter() : base(
        vo => vo.Value,
        value => value != null ? SafeUrl.Create(value, false) : null)
    {
    }
}