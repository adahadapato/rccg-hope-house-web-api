using System.Globalization;

namespace RccgHopeHouse.Core.ValueObjects;

/// <summary>
/// Value object for monetary amounts.
/// Ensures precision, prevents negative values, and handles currency formatting.
/// Use for donation amounts, building fund targets, etc.
/// </summary>
public record Money(decimal Amount, string CurrencyCode = "GBP")
{
    /// <summary>
    /// Default currency for the church (UK-based).
    /// </summary>
    public const string DefaultCurrency = "GBP";

    /// <summary>
    /// Creates a new Money instance with validation.
    /// </summary>
    /// <param name="amount">Monetary amount (must be non-negative)</param>
    /// <param name="currencyCode">ISO 4217 currency code (default: GBP)</param>
    /// <returns>Validated Money</returns>
    /// <exception cref="ArgumentException">Thrown when amount is negative or currency is invalid</exception>
    public static Money Create(decimal amount, string currencyCode = DefaultCurrency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Length != 3)
            throw new ArgumentException("Currency code must be a 3-letter ISO code.", nameof(currencyCode));

        // Round to 2 decimal places for currency precision
        var rounded = Math.Round(amount, 2, MidpointRounding.AwayFromZero);

        return new Money(rounded, currencyCode.ToUpperInvariant());
    }

    /// <summary>
    /// Adds another Money amount (must be same currency).
    /// </summary>
    public Money Add(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
            throw new InvalidOperationException($"Cannot add different currencies: {CurrencyCode} + {other.CurrencyCode}");

        return Create(Amount + other.Amount, CurrencyCode);
    }

    /// <summary>
    /// Formats the amount with currency symbol (e.g., "£10.50").
    /// </summary>
    public string ToString(string? cultureName = null)
    {
        var culture = cultureName is not null
            ? new CultureInfo(cultureName)
            : CultureInfo.CurrentCulture;

        return Amount.ToString("C", culture);
    }

    /// <summary>
    /// Returns the amount as a decimal (for calculations).
    /// </summary>
    public decimal ToDecimal() => Amount;
}