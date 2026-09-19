namespace RccgHopeHouse.Core.Entities
{
    /// <summary>
    /// Represents a category or purpose for financial giving within
    /// RCCG Hope House.
    ///
    /// Examples include Tithe, Offering, Seed Offering,
    /// Thanksgiving Offering, Building Fund and Welfare.
    ///
    /// Giving types are stored as entities rather than enums so that
    /// authorised administrators can add, update, activate, deactivate
    /// and reorder them without requiring changes to application code.
    /// </summary>
    public class GivingType : BaseEntity
    {
        // ==================== Properties ====================

        /// <summary>
        /// The display name of the giving type.
        /// For example: "Tithe", "Offering" or "Seed Offering".
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Optional description explaining the purpose of this
        /// giving type.
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Determines whether this giving type is currently available
        /// for selection on the public Give Online form.
        ///
        /// Giving types should normally be deactivated rather than
        /// deleted so that historical giving records can continue to
        /// reference them.
        /// </summary>
        public bool IsActive { get; private set; } = true;

        /// <summary>
        /// Determines the order in which this giving type appears
        /// in the Give Online form and administrative interfaces.
        /// Lower values appear before higher values.
        /// </summary>
        public int DisplayOrder { get; private set; }


        // ==================== Constructor ====================

        /// <summary>
        /// Parameterless constructor required by Entity Framework Core.
        /// GivingType instances should otherwise be created through
        /// the Create factory method.
        /// </summary>
        private GivingType() { }


        // ==================== Factory Method ====================

        /// <summary>
        /// Creates a new giving type.
        /// </summary>
        /// <param name="name">
        /// The name displayed to users, such as "Tithe" or
        /// "Seed Offering".
        /// </param>
        /// <param name="displayOrder">
        /// The position of the giving type in displayed lists.
        /// Must be zero or greater.
        /// </param>
        /// <param name="description">
        /// Optional description of the giving purpose.
        /// </param>
        /// <returns>
        /// A valid <see cref="GivingType"/> instance.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the name is null, empty or whitespace.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when displayOrder is negative.
        /// </exception>
        public static GivingType Create(
            string name,
            int displayOrder,
            string? description = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                name,
                nameof(name));

            if (displayOrder < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(displayOrder),
                    "Display order cannot be negative.");
            }

            return new GivingType
            {
                Name = name.Trim(),
                Description = NormalizeOptionalText(description),
                DisplayOrder = displayOrder,
                IsActive = true
            };
        }


        // ==================== Update Details ====================

        /// <summary>
        /// Updates the editable details of the giving type.
        /// </summary>
        /// <param name="name">
        /// The new display name.
        /// </param>
        /// <param name="description">
        /// Optional description of the giving purpose.
        /// </param>
        /// <param name="displayOrder">
        /// The new display position.
        /// </param>
        public void Update(
            string name,
            string? description,
            int displayOrder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                name,
                nameof(name));

            if (displayOrder < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(displayOrder),
                    "Display order cannot be negative.");
            }

            Name = name.Trim();
            Description = NormalizeOptionalText(description);
            DisplayOrder = displayOrder;

            MarkAsUpdated();
        }


        // ==================== Activation ====================

        /// <summary>
        /// Makes the giving type available for use.
        /// </summary>
        public void Activate()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            MarkAsUpdated();
        }


        // ==================== Deactivation ====================

        /// <summary>
        /// Prevents the giving type from being offered for new
        /// online giving transactions.
        ///
        /// Deactivation does not delete the entity, allowing existing
        /// historical giving records to retain their relationship to it.
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


        // ==================== Display Order ====================

        /// <summary>
        /// Changes the position of the giving type in displayed lists.
        /// </summary>
        /// <param name="displayOrder">
        /// The new display position. Must be zero or greater.
        /// </param>
        public void ChangeDisplayOrder(int displayOrder)
        {
            if (displayOrder < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(displayOrder),
                    "Display order cannot be negative.");
            }

            if (DisplayOrder == displayOrder)
            {
                return;
            }

            DisplayOrder = displayOrder;

            MarkAsUpdated();
        }


        // ==================== Helpers ====================

        /// <summary>
        /// Normalises optional text values by trimming surrounding
        /// whitespace and converting empty or whitespace-only values
        /// to null.
        /// </summary>
        private static string? NormalizeOptionalText(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}