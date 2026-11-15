using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Infrastructure.Persistence.Converters;

public class SafeUrlConverter : ValueConverter<SafeUrl?, string?>
{
    public SafeUrlConverter() : base(
        vo => vo == null ? null : vo.Value,
        value => value != null ? SafeUrl.Create(value, false) : null)
    {
    }
}