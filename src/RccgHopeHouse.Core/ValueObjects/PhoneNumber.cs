
using System.Text.RegularExpressions;

namespace RccgHopeHouse.Core.ValueObjects;

/// <summary>
/// Represents a validated telephone number.
/// </summary>
/// <remarks>
/// Supports telephone numbers containing digits, spaces,
/// hyphens and parentheses, with an optional international
/// prefix.
///
/// Provides E.164 normalisation for external messaging services.
///
/// UK national telephone numbers beginning with zero are
/// converted to the +44 international format.
/// </remarks>
public sealed partial record PhoneNumber
{
    /// <summary>
    /// Defines the accepted telephone number format.
    /// </summary>
    public const string Pattern = @"^\+?[\d\s\-\(\)]{10,15}$";

    /// <summary>
    /// Gets the validated telephone number.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initialises a validated telephone number.
    /// </summary>
    /// <param name="value">The validated telephone number.</param>
    private PhoneNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Provides the compiled telephone number validation expression.
    /// </summary>
    [GeneratedRegex(Pattern, RegexOptions.Compiled, "en-US")]
    private static partial Regex ValidPattern();

    /// <summary>
    /// Provides the compiled E.164 validation expression.
    /// </summary>
    [GeneratedRegex(@"^\+[1-9][0-9]{7,14}$")]
    private static partial Regex E164Pattern();

    /// <summary>
    /// Creates a validated telephone number.
    /// </summary>
    /// <param name="phoneNumber">
    /// The telephone number to validate.
    /// </param>
    /// <returns>A validated PhoneNumber value object.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the telephone number is missing or invalid.
    /// </exception>
    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException(
                ValidationMessages.Required,
                nameof(phoneNumber));
        }

        var cleaned = phoneNumber.Trim();

        if (!ValidPattern().IsMatch(cleaned))
        {
            throw new ArgumentException(
                ValidationMessages.InvalidPhoneNumber,
                nameof(phoneNumber));
        }

        return new PhoneNumber(cleaned);
    }

    /// <summary>
    /// Creates a validated telephone number or returns null
    /// when no telephone number is supplied.
    /// </summary>
    /// <param name="phoneNumber">
    /// The optional telephone number.
    /// </param>
    /// <returns>
    /// A validated PhoneNumber or null.
    /// </returns>
    public static PhoneNumber? CreateOrNull(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return null;
        }

        return Create(phoneNumber);
    }

    /// <summary>
    /// Converts the telephone number into E.164 international format.
    /// </summary>
    /// <remarks>
    /// Numbers beginning with 00 are converted to an international
    /// prefix beginning with +.
    ///
    /// National-format numbers beginning with zero are assumed
    /// to belong to the United Kingdom and receive the +44 prefix.
    ///
    /// Other international numbers must already include their
    /// country code.
    /// </remarks>
    /// <returns>
    /// The telephone number in E.164 format.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the telephone number cannot be converted
    /// into a valid E.164 format.
    /// </exception>
    public string ToE164()
    {
        var cleaned = new string(
            Value
                .Where(c => char.IsAsciiDigit(c) || c == '+')
                .ToArray());

        if (cleaned.StartsWith("00", StringComparison.Ordinal))
        {
            cleaned = "+" + cleaned[2..];
        }
        else if (cleaned.StartsWith("0", StringComparison.Ordinal))
        {
            cleaned = "+44" + cleaned[1..];
        }

        if (!E164Pattern().IsMatch(cleaned))
        {
            throw new InvalidOperationException("The telephone number cannot be converted to E.164 format.");
        }

        return cleaned;
    }

    /// <summary>
    /// Returns the original validated telephone number.
    /// </summary>
    /// <returns>The telephone number as a string.</returns>
    public override string ToString()
    {
        return Value;
    }
}
