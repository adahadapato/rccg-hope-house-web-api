using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Entities
{
    public class ChurchContactMethod : BaseEntity
    {
        public Guid ChurchInfoId { get; private set; }
        public ChurchInfo ChurchInfo { get; private set; } = null!;

        public ContactMethodType Type { get; private set; }
        public string Value { get; private set; } = string.Empty;

        /// <summary>
        /// Optional label shown alongside the value, e.g. "Main Office",
        /// "Prayer Line", "Pastor's Direct".
        /// </summary>
        public string? Label { get; private set; }

        public int DisplayOrder { get; private set; }

        private ChurchContactMethod() { } // EF Core parameterless constructor

        public static ChurchContactMethod Create(
            Guid churchInfoId,
            ContactMethodType type,
            string value,
            string? label = null,
            int displayOrder = 0)
        {
            if (churchInfoId == Guid.Empty)
                throw new ArgumentException("A contact method must belong to a ChurchInfo.", nameof(churchInfoId));

            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

            return new ChurchContactMethod
            {
                ChurchInfoId = churchInfoId,
                Type = type,
                Value = value.Trim(),
                Label = label?.Trim(),
                DisplayOrder = displayOrder
            };
        }

        public void Update(string value, string? label, int displayOrder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

            Value = value.Trim();
            Label = label?.Trim();
            DisplayOrder = displayOrder;
            MarkAsUpdated();
        }
    }
}