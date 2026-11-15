using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Core.Entities
{
    /// <summary>
    /// Represents a church member and their membership information.
    /// </summary>
    public class Member : BaseEntity
    {
        /// <summary>
        /// Gets the member's first name.
        /// </summary>
        public string FirstName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the member's last name.
        /// </summary>
        public string LastName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the member's email address, if provided.
        /// </summary>
        public EmailAddress? Email { get; private set; }

        /// <summary>
        /// Gets the member's phone number, if provided.
        /// </summary>
        public PhoneNumber? PhoneNumber { get; private set; }

        /// <summary>
        /// Gets the member's postal address, if provided.
        /// </summary>
        public Address? Address { get; private set; }

        /// <summary>
        /// Gets the member's birthday, if provided.
        /// Month and day are required within the value object,
        /// while the birth year is optional.
        /// </summary>
        public Birthday? Birthday { get; private set; }

        /// <summary>
        /// Gets the member's marital status, if provided.
        /// </summary>
        public MaritalStatus? MaritalStatus { get; private set; }

        /// <summary>
        /// Gets the member's full wedding anniversary date.
        /// The anniversary may only be set when the member's
        /// marital status is Married.
        /// </summary>
        public DateOnly? WeddingAnniversary { get; private set; }

        /// <summary>
        /// Indicates whether the member has consented to
        /// receiving church communications.
        /// </summary>
        public bool ConsentToContact { get; private set; }

        /// <summary>
        /// Indicates whether the member has consented to their
        /// birthday information being published on the public website.
        /// </summary>
        public bool ConsentToBirthdayPublication { get; private set; }

        /// <summary>
        /// Gets the identifier of the gallery image selected as the
        /// member's birthday/profile photograph, if one has been selected.
        /// </summary>
        public Guid? PhotoId { get; private set; }

        /// <summary>
        /// Gets the gallery image selected as the member's
        /// birthday/profile photograph.
        /// </summary>
        public GalleryImage? Photo { get; private set; }

        /// <summary>
        /// Indicates whether the member is currently active.
        /// </summary>
        public bool IsActive { get; private set; } = true;

        /// <summary>
        /// Gets the date the member joined the church.
        /// If no date is supplied when the member is created,
        /// the date and time of record creation is used.
        /// </summary>
        public DateTime JoinedDate { get; private set; }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        private Member()
        {
        }

        /// <summary>
        /// Creates a new church member.
        /// </summary>
        /// <param name="firstName">
        /// The member's first name.
        /// </param>
        /// <param name="lastName">
        /// The member's last name.
        /// </param>
        /// <param name="email">
        /// The member's email address, if provided.
        /// </param>
        /// <param name="phoneNumber">
        /// The member's phone number, if provided.
        /// </param>
        /// <param name="address">
        /// The member's postal address, if provided.
        /// </param>
        /// <param name="birthday">
        /// The member's birthday, if provided.
        /// </param>
        /// <param name="maritalStatus">
        /// The member's marital status, if provided.
        /// </param>
        /// <param name="weddingAnniversary">
        /// The member's wedding anniversary, if applicable.
        /// </param>
        /// <param name="consentToContact">
        /// Indicates whether the member has consented to contact.
        /// </param>
        /// <param name="consentToBirthdayPublication">
        /// Indicates whether the member has consented to their
        /// birthday being published on the public website.
        /// </param>
        /// <param name="joinedDate">
        /// The date the member joined the church. When omitted,
        /// the current UTC date and time is used.
        /// </param>
        /// <returns>
        /// A newly created member.
        /// </returns>
        public static Member Create(
            string firstName,
            string lastName,
            string? email = null,
            string? phoneNumber = null,
            Address? address = null,
            Birthday? birthday = null,
            MaritalStatus? maritalStatus = null,
            DateOnly? weddingAnniversary = null,
            bool consentToContact = false,
            bool consentToBirthdayPublication = false,
            DateTime? joinedDate = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                firstName,
                nameof(firstName));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                lastName,
                nameof(lastName));

            ValidateWeddingAnniversary(
                maritalStatus,
                weddingAnniversary);

            return new Member
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),

                Email =
                    EmailAddress.CreateOrNull(
                        email),

                PhoneNumber =
                    PhoneNumber.CreateOrNull(
                        phoneNumber),

                Address = address,
                Birthday = birthday,

                MaritalStatus =
                    maritalStatus,

                WeddingAnniversary =
                    weddingAnniversary,

                ConsentToContact =
                    consentToContact,

                ConsentToBirthdayPublication =
                    consentToBirthdayPublication,

                JoinedDate =
                    joinedDate ??
                    DateTime.UtcNow,

                IsActive = true
            };
        }

        /// <summary>
        /// Updates the member's core profile information.
        /// </summary>
        /// <param name="firstName">
        /// The member's first name.
        /// </param>
        /// <param name="lastName">
        /// The member's last name.
        /// </param>
        /// <param name="email">
        /// The member's email address, if provided.
        /// </param>
        /// <param name="phoneNumber">
        /// The member's phone number, if provided.
        /// </param>
        public void UpdateProfile(
            string firstName,
            string lastName,
            string? email,
            string? phoneNumber)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                firstName,
                nameof(firstName));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                lastName,
                nameof(lastName));

            FirstName =
                firstName.Trim();

            LastName =
                lastName.Trim();

            Email =
                EmailAddress.CreateOrNull(
                    email);

            PhoneNumber =
                PhoneNumber.CreateOrNull(
                    phoneNumber);

            MarkAsUpdated();
        }

        /// <summary>
        /// Updates or removes the member's postal address.
        /// </summary>
        /// <param name="address">
        /// The new address, or null to remove the current address.
        /// </param>
        public void UpdateAddress(
            Address? address)
        {
            Address = address;

            MarkAsUpdated();
        }

        /// <summary>
        /// Updates or removes the member's birthday.
        /// </summary>
        /// <param name="birthday">
        /// The new birthday, or null to remove the current birthday.
        /// </param>
        public void UpdateBirthday(
            Birthday? birthday)
        {
            Birthday = birthday;

            MarkAsUpdated();
        }

        /// <summary>
        /// Updates the member's marital information.
        /// </summary>
        /// <param name="maritalStatus">
        /// The member's marital status.
        /// </param>
        /// <param name="weddingAnniversary">
        /// The wedding anniversary, if applicable.
        /// </param>
        public void UpdateMaritalInfo(
            MaritalStatus? maritalStatus,
            DateOnly? weddingAnniversary)
        {
            ValidateWeddingAnniversary(
                maritalStatus,
                weddingAnniversary);

            MaritalStatus =
                maritalStatus;

            WeddingAnniversary =
                weddingAnniversary;

            MarkAsUpdated();
        }

        /// <summary>
        /// Sets whether the member has consented to
        /// receiving church communications.
        /// </summary>
        /// <param name="consent">
        /// True when contact consent has been given.
        /// </param>
        public void SetConsentToContact(
            bool consent)
        {
            if (ConsentToContact == consent)
            {
                return;
            }

            ConsentToContact = consent;

            MarkAsUpdated();
        }

        /// <summary>
        /// Sets whether the member has consented to their birthday
        /// information being published on the public website.
        /// </summary>
        /// <param name="consent">
        /// True when birthday publication consent has been given.
        /// </param>
        public void SetBirthdayPublicationConsent(
            bool consent)
        {
            if (ConsentToBirthdayPublication == consent)
            {
                return;
            }

            ConsentToBirthdayPublication =
                consent;

            MarkAsUpdated();
        }

        /// <summary>
        /// Assigns an existing gallery image as the member's
        /// birthday/profile photograph.
        /// </summary>
        /// <param name="photoId">
        /// The identifier of the gallery image to assign.
        /// </param>
        public void SetPhoto(
            Guid photoId)
        {
            if (photoId == Guid.Empty)
            {
                throw new ArgumentException(
                    "A valid gallery image ID is required.",
                    nameof(photoId));
            }

            if (PhotoId == photoId)
            {
                return;
            }

            PhotoId = photoId;

            MarkAsUpdated();
        }

        /// <summary>
        /// Removes the gallery image currently assigned as the
        /// member's birthday/profile photograph.
        /// </summary>
        public void RemovePhoto()
        {
            if (!PhotoId.HasValue)
            {
                return;
            }

            PhotoId = null;

            MarkAsUpdated();
        }

        /// <summary>
        /// Updates the date the member joined the church.
        /// </summary>
        /// <param name="joinedDate">
        /// The corrected membership joining date.
        /// </param>
        public void UpdateJoinedDate(
            DateTime joinedDate)
        {
            JoinedDate = joinedDate;

            MarkAsUpdated();
        }

        /// <summary>
        /// Deactivates the member while retaining their record.
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;

            MarkAsUpdated();
        }

        /// <summary>
        /// Reactivates a previously inactive member.
        /// </summary>
        public void Reactivate()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            MarkAsUpdated();
        }

        /// <summary>
        /// Validates the relationship between marital status
        /// and wedding anniversary.
        /// </summary>
        /// <param name="maritalStatus">
        /// The member's marital status.
        /// </param>
        /// <param name="weddingAnniversary">
        /// The wedding anniversary being assigned.
        /// </param>
        private static void ValidateWeddingAnniversary(
            MaritalStatus? maritalStatus,
            DateOnly? weddingAnniversary)
        {
            if (weddingAnniversary.HasValue &&
                maritalStatus !=
                Enums.MaritalStatus.Married)
            {
                throw new ArgumentException(
                    "A wedding anniversary can only be set when marital status is Married.",
                    nameof(weddingAnniversary));
            }
        }
    }
}