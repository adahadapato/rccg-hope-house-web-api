namespace RccgHopeHouse.Core.ValueObjects
{
    /// <summary>
    /// Represents a member's postal address as an immutable value object.
    /// Equality is determined by the values of the address components.
    /// </summary>
    public partial record Address
    {
        public string AddressLine1 { get; private init; } = string.Empty;
        public string? AddressLine2 { get; private init; }
        public string City { get; private init; } = string.Empty;
        public string? County { get; private init; }
        public string Postcode { get; private init; } = string.Empty;
        public string Country { get; private init; } = string.Empty;

        private Address()
        {
        }

        /// <summary>
        /// Creates a validated postal address.
        /// </summary>
        /// <param name="addressLine1">The first line of the address.</param>
        /// <param name="addressLine2">The optional second line of the address.</param>
        /// <param name="city">The town or city.</param>
        /// <param name="county">The optional county, state, or region.</param>
        /// <param name="postcode">The postal or ZIP code.</param>
        /// <param name="country">The country.</param>
        /// <returns>A validated <see cref="Address"/> value object.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when a required address component is missing.
        /// </exception>
        public static Address Create(
            string addressLine1,
            string? addressLine2,
            string city,
            string? county,
            string postcode,
            string country)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                addressLine1,
                nameof(addressLine1));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                city,
                nameof(city));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                postcode,
                nameof(postcode));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                country,
                nameof(country));

            return new Address
            {
                AddressLine1 = addressLine1.Trim(),

                AddressLine2 = string.IsNullOrWhiteSpace(addressLine2)
                    ? null
                    : addressLine2.Trim(),

                City = city.Trim(),

                County = string.IsNullOrWhiteSpace(county)
                    ? null
                    : county.Trim(),

                Postcode = postcode.Trim(),

                Country = country.Trim()
            };
        }
    }
}