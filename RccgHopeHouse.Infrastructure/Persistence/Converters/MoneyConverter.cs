using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Infrastructure.Persistence.Converters;

/// <summary>
/// EF Core value converter for the <see cref="Money"/> Value Object.
/// Stores amount and currency as a single formatted string for simplicity.
/// Alternative: split into two columns (Amount decimal, CurrencyCode string).
/// </summary>
public class MoneyConverter : ValueConverter<Money?, string?>
{
    /// <summary>
    /// Initializes the converter with mapping functions for domain ↔ database translation.
    /// Format: "123.45:GBP" (amount:currencyCode).
    /// </summary>
    public MoneyConverter() : base(
        vo => vo != null ? $"{vo.Amount:F2}:{vo.CurrencyCode}" : null,
        value => ParseMoney(value))
    {
    }

    private static Money? ParseMoney(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var parts = value.Split(':');
        if (parts.Length != 2) return null;
        if (decimal.TryParse(parts[0], out var amount))
            return Money.Create(amount, parts[1]);
        return null;
    }
}