namespace RccgHopeHouse.Core.Entities
{
    public class ContactUs : BaseEntity
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public EmailAddress Email { get; private set; } = null!;
        public PhoneNumber? PhoneNumber { get; private set; }
        public ContactReason Reason { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public bool IsRead { get; private set; }
        public DateTime? RespondedAt { get; private set; }

        private ContactUs() { } // EF Core parameterless constructor

        public static ContactUs Create(
            string firstName,
            string lastName,
            string email,
            string message,
            ContactReason reason,
            string? phoneNumber = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));
            ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            return new ContactUs
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = EmailAddress.Create(email),
                PhoneNumber = PhoneNumber.CreateOrNull(phoneNumber),
                Reason = reason,
                Message = message.Trim(),
                IsRead = false
            };
        }

        public void MarkAsRead() => IsRead = true;

        public void MarkAsResponded()
        {
            RespondedAt = DateTime.UtcNow;
            IsRead = true;
        }
    }
}