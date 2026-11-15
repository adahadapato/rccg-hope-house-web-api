using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Entities
{
    /// <summary>
    /// Represents a contact, website, messaging, or social media
    /// method associated with the church.
    /// </summary>
    public class ChurchContactMethod : BaseEntity
    {
        /// <summary>
        /// Gets the identifier of the church information record
        /// that owns this contact method.
        /// </summary>
        public Guid ChurchInfoId { get; private set; }

        /// <summary>
        /// Gets the church information record that owns this contact method.
        /// </summary>
        public ChurchInfo ChurchInfo { get; private set; } = null!;

        /// <summary>
        /// Gets the type of contact method.
        /// </summary>
        public ContactMethodType Type { get; private set; }

        /// <summary>
        /// Gets the contact value, such as a telephone number,
        /// email address, website URL, or social media URL.
        /// </summary>
        public string Value { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the optional public-facing label for this contact method,
        /// such as "Main Office" or "Join Our WhatsApp Group".
        /// </summary>
        public string? Label { get; private set; }

        /// <summary>
        /// Gets the order in which this contact method should be displayed.
        /// </summary>
        public int DisplayOrder { get; private set; }

        /// <summary>
        /// Parameterless constructor required by Entity Framework Core.
        /// </summary>
        private ChurchContactMethod()
        {
        }

        /// <summary>
        /// Creates a new church contact method.
        /// </summary>
        /// <param name="churchInfoId">
        /// The identifier of the church information record.
        /// </param>
        /// <param name="type">The type of contact method.</param>
        /// <param name="value">
        /// The contact value, such as a telephone number, email address,
        /// website URL, or social media URL.
        /// </param>
        /// <param name="label">An optional public-facing label.</param>
        /// <param name="displayOrder">The display order.</param>
        /// <returns>A new <see cref="ChurchContactMethod"/>.</returns>
        public static ChurchContactMethod Create(
            Guid churchInfoId,
            ContactMethodType type,
            string value,
            string? label = null,
            int displayOrder = 0)
        {
            if (churchInfoId == Guid.Empty)
            {
                throw new ArgumentException(
                    "A contact method must belong to a ChurchInfo.",
                    nameof(churchInfoId));
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(
                value,
                nameof(value));

            return new ChurchContactMethod
            {
                ChurchInfoId = churchInfoId,
                Type = type,
                Value = value.Trim(),
                Label = string.IsNullOrWhiteSpace(label)
                    ? null
                    : label.Trim(),
                DisplayOrder = displayOrder
            };
        }

        /// <summary>
        /// Updates the contact method.
        /// </summary>
        /// <param name="type">The contact method type.</param>
        /// <param name="value">The contact value.</param>
        /// <param name="label">The optional public-facing label.</param>
        /// <param name="displayOrder">The display order.</param>
        public void Update(
            ContactMethodType type,
            string value,
            string? label,
            int displayOrder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                value,
                nameof(value));

            Type = type;
            Value = value.Trim();
            Label = string.IsNullOrWhiteSpace(label)
                ? null
                : label.Trim();
            DisplayOrder = displayOrder;

            MarkAsUpdated();
        }
    }
}