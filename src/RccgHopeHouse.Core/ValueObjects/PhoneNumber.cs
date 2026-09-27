using System.Text.RegularExpressions;

namespace RccgHopeHouse.Core.ValueObjects
{
    public partial record PhoneNumber(string Value)
    {
        public const string Pattern = @"^\+?[\d\s\-\(\)]{10,15}$";

        [GeneratedRegex(Pattern, RegexOptions.Compiled, "en-US")]
        private static partial Regex ValidPattern();

        public static PhoneNumber Create(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException(ValidationMessages.Required, nameof(phoneNumber));

            var cleaned = phoneNumber.Trim();
            if (!ValidPattern().IsMatch(cleaned))
                throw new ArgumentException(ValidationMessages.InvalidPhoneNumber, nameof(phoneNumber));

            return new PhoneNumber(cleaned);
        }

        public static PhoneNumber? CreateOrNull(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            return Create(phoneNumber);
        }
    }
}