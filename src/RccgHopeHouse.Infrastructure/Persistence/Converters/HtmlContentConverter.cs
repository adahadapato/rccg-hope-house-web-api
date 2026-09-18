using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Infrastructure.Persistence.Converters;

public class HtmlContentConverter : ValueConverter<HtmlContent?, string?>
{
    public HtmlContentConverter() : base(
        vo => vo == null ? null : vo.Value,
        value => value != null ? HtmlContent.Create(value, false) : null)
    {
    }
}