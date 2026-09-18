using System.Text.RegularExpressions;
namespace RccgHopeHouse.Core.ValueObjects;
public partial record EmailAddress(string Value)
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled, "en-US")]
    private static partial Regex ValidPattern();

    public static EmailAddress Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        if (!ValidPattern().IsMatch(email))
            throw new ArgumentException("Invalid email format.", nameof(email));

        return new EmailAddress(email.Trim().ToLowerInvariant());
    }

    public static EmailAddress? CreateOrNull(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return Create(email);
    }
}