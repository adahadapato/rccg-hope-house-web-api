using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Core.Entities
{
    public class Member : BaseEntity
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public EmailAddress? Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }

        /// <summary>
        /// Birth month (1-12). Null if the member hasn't shared a birthday at all.
        /// </summary>
        public int? BirthMonth { get; private set; }

        /// <summary>
        /// Birth day of month. Always set together with BirthMonth.
        /// </summary>
        public int? BirthDay { get; private set; }

        /// <summary>
        /// Birth year — optional and independent of BirthMonth/BirthDay.
        /// A member may share their birthday (month + day, for greeting
        /// purposes) without sharing the year. Never used as a signal for
        /// "unknown"; the presence/absence of BirthMonth is that signal.
        /// </summary>
        public int? BirthYear { get; private set; }

        public MaritalStatus? MaritalStatus { get; private set; }

        /// <summary>
        /// Full wedding anniversary date, including year — required in full
        /// (not month/day-only like birthdays) since anniversary messages
        /// reference the number of years married.
        /// </summary>
        public DateOnly? WeddingAnniversary { get; private set; }

        public bool ConsentToContact { get; private set; }

        public bool IsActive { get; private set; } = true;
        public DateTime JoinedDate { get; private set; }

        private Member() { } // EF Core parameterless constructor

        public static Member Create(
            string firstName,
            string lastName,
            string? email = null,
            string? phoneNumber = null,
            int? birthMonth = null,
            int? birthDay = null,
            int? birthYear = null,
            MaritalStatus? maritalStatus = null,
            DateOnly? weddingAnniversary = null,
            bool consentToContact = false,
            DateTime? joinedDate = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));

            ValidateBirthday(birthMonth, birthDay, birthYear);

            if (weddingAnniversary.HasValue && maritalStatus != Enums.MaritalStatus.Married)
                throw new ArgumentException(
                    "A wedding anniversary can only be set when marital status is Married.",
                    nameof(weddingAnniversary));

            return new Member
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = EmailAddress.CreateOrNull(email),
                PhoneNumber = PhoneNumber.CreateOrNull(phoneNumber),
                BirthMonth = birthMonth,
                BirthDay = birthDay,
                BirthYear = birthYear,
                MaritalStatus = maritalStatus,
                WeddingAnniversary = weddingAnniversary,
                ConsentToContact = consentToContact,
                JoinedDate = joinedDate ?? DateTime.UtcNow,
                IsActive = true
            };
        }

        public void UpdateProfile(string firstName, string lastName, string? email, string? phoneNumber)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = EmailAddress.CreateOrNull(email);
            PhoneNumber = PhoneNumber.CreateOrNull(phoneNumber);
            MarkAsUpdated();
        }

        public void UpdateBirthday(int? birthMonth, int? birthDay, int? birthYear)
        {
            ValidateBirthday(birthMonth, birthDay, birthYear);
            BirthMonth = birthMonth;
            BirthDay = birthDay;
            BirthYear = birthYear;
            MarkAsUpdated();
        }

        public void UpdateMaritalInfo(MaritalStatus? maritalStatus, DateOnly? weddingAnniversary)
        {
            if (weddingAnniversary.HasValue && maritalStatus != Enums.MaritalStatus.Married)
                throw new ArgumentException(
                    "A wedding anniversary can only be set when marital status is Married.",
                    nameof(weddingAnniversary));

            MaritalStatus = maritalStatus;
            WeddingAnniversary = weddingAnniversary;
            MarkAsUpdated();
        }

        public void SetConsentToContact(bool consent)
        {
            ConsentToContact = consent;
            MarkAsUpdated();
        }

        public void Deactivate() => IsActive = false;
        public void Reactivate() => IsActive = true;

        /// <summary>
        /// Validates BirthMonth/BirthDay/BirthYear together: month and day
        /// must be provided as a pair (or not at all), month must be 1-12,
        /// and day must be valid for that month. Uses a fixed leap year
        /// (2000) to validate day-of-month ranges so Feb 29 is accepted
        /// even when no real BirthYear is supplied.
        /// </summary>
        private static void ValidateBirthday(int? birthMonth, int? birthDay, int? birthYear)
        {
            if (birthMonth is null && birthDay is null)
                return; // no birthday shared at all — valid

            if (birthMonth is null || birthDay is null)
                throw new ArgumentException("Birth month and day must both be provided together, or both omitted.");

            if (birthMonth < 1 || birthMonth > 12)
                throw new ArgumentException("Birth month must be between 1 and 12.", nameof(birthMonth));

            var daysInMonth = DateTime.DaysInMonth(2000, birthMonth.Value); // 2000 is a leap year
            if (birthDay < 1 || birthDay > daysInMonth)
                throw new ArgumentException($"Birth day is not valid for the given month.", nameof(birthDay));

            if (birthYear.HasValue && (birthYear < 1900 || birthYear > DateTime.UtcNow.Year))
                throw new ArgumentException("Birth year must be a realistic past year.", nameof(birthYear));
        }
    }
}