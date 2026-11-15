namespace RccgHopeHouse.Core.Entities
{
    public class ChurchInfo : BaseEntity
    {
        // ==================== Address ====================
        public string AddressLine1 { get; private set; } = string.Empty;
        public string? AddressLine2 { get; private set; }
        public string City { get; private set; } = string.Empty;
        public string? PostCode { get; private set; }
        public string Country { get; private set; } = string.Empty;

        // ==================== About Section ====================
        public string ParishName { get; private set; } = string.Empty;
        public int EstablishedYear { get; private set; }
        public string Tagline { get; private set; } = string.Empty;
        public string AboutLead { get; private set; } = string.Empty;
        public string AboutText { get; private set; } = string.Empty;

        /// <summary>
        /// Display string for the multi-cultural stat, e.g. "100%". Kept as
        /// a manually-set field — unlike church family size (now a live
        /// count from Member) or years of ministry (computed from
        /// EstablishedYear), "multi-cultural" isn't reliably derivable from
        /// stored data without a real nationality/ethnicity field on Member,
        /// and is more of a qualitative claim than a precise count.
        /// </summary>
        public string MultiCulturalStat { get; private set; } = string.Empty;

        public ICollection<ChurchContactMethod> ContactMethods { get; private set; } = new List<ChurchContactMethod>();

        private ChurchInfo() { } // EF Core parameterless constructor

        public static ChurchInfo Create(
            string addressLine1,
            string city,
            string country,
            string parishName,
            int establishedYear,
            string tagline,
            string aboutLead,
            string aboutText,
            string multiCulturalStat,
            string? addressLine2 = null,
            string? postCode = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(addressLine1, nameof(addressLine1));
            ArgumentException.ThrowIfNullOrWhiteSpace(city, nameof(city));
            ArgumentException.ThrowIfNullOrWhiteSpace(country, nameof(country));
            ArgumentException.ThrowIfNullOrWhiteSpace(parishName, nameof(parishName));
            ArgumentException.ThrowIfNullOrWhiteSpace(tagline, nameof(tagline));
            ArgumentException.ThrowIfNullOrWhiteSpace(aboutLead, nameof(aboutLead));
            ArgumentException.ThrowIfNullOrWhiteSpace(aboutText, nameof(aboutText));
            ArgumentException.ThrowIfNullOrWhiteSpace(multiCulturalStat, nameof(multiCulturalStat));

            if (establishedYear < 1900 || establishedYear > DateTime.UtcNow.Year)
                throw new ArgumentOutOfRangeException(nameof(establishedYear), "Established year must be a realistic past year.");

            return new ChurchInfo
            {
                AddressLine1 = addressLine1.Trim(),
                AddressLine2 = addressLine2?.Trim(),
                City = city.Trim(),
                PostCode = postCode?.Trim(),
                Country = country.Trim(),
                ParishName = parishName.Trim(),
                EstablishedYear = establishedYear,
                Tagline = tagline.Trim(),
                AboutLead = aboutLead.Trim(),
                AboutText = aboutText.Trim(),
                MultiCulturalStat = multiCulturalStat.Trim()
            };
        }

        public void UpdateAddress(
            string addressLine1,
            string city,
            string country,
            string? addressLine2,
            string? postCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(addressLine1, nameof(addressLine1));
            ArgumentException.ThrowIfNullOrWhiteSpace(city, nameof(city));
            ArgumentException.ThrowIfNullOrWhiteSpace(country, nameof(country));

            AddressLine1 = addressLine1.Trim();
            AddressLine2 = addressLine2?.Trim();
            City = city.Trim();
            PostCode = postCode?.Trim();
            Country = country.Trim();
            MarkAsUpdated();
        }

        public void UpdateAboutSection(
            string parishName,
            int establishedYear,
            string tagline,
            string aboutLead,
            string aboutText,
            string multiCulturalStat)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(parishName, nameof(parishName));
            ArgumentException.ThrowIfNullOrWhiteSpace(tagline, nameof(tagline));
            ArgumentException.ThrowIfNullOrWhiteSpace(aboutLead, nameof(aboutLead));
            ArgumentException.ThrowIfNullOrWhiteSpace(aboutText, nameof(aboutText));
            ArgumentException.ThrowIfNullOrWhiteSpace(multiCulturalStat, nameof(multiCulturalStat));

            if (establishedYear < 1900 || establishedYear > DateTime.UtcNow.Year)
                throw new ArgumentOutOfRangeException(nameof(establishedYear), "Established year must be a realistic past year.");

            ParishName = parishName.Trim();
            EstablishedYear = establishedYear;
            Tagline = tagline.Trim();
            AboutLead = aboutLead.Trim();
            AboutText = aboutText.Trim();
            MultiCulturalStat = multiCulturalStat.Trim();
            MarkAsUpdated();
        }

        public int YearsOfMinistry => DateTime.UtcNow.Year - EstablishedYear;

        public void AddContactMethod(ChurchContactMethod method)
        {
            ContactMethods.Add(method);
            MarkAsUpdated();
        }

        public void RemoveContactMethod(Guid contactMethodId)
        {
            var method = ContactMethods.FirstOrDefault(m => m.Id == contactMethodId);
            if (method != null)
            {
                ContactMethods.Remove(method);
                MarkAsUpdated();
            }
        }
    }
}