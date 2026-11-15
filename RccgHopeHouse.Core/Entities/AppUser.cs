namespace RccgHopeHouse.Core.Entities
{
    public class AppUser : BaseEntity
    {
        public string Email { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime? LastLoginAt { get; private set; }


        // EF Core requires parameterless constructor
        private AppUser() { }

        public static AppUser Create(string email, string firstName, string lastName, string? phoneNumber = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));

            return new AppUser
            {
                Email = email.Trim().ToLowerInvariant(),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                PhoneNumber = phoneNumber?.Trim(),
                IsActive = true
            };
        }

        public void UpdateProfile(string firstName, string lastName, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            MarkAsUpdated();
        }

        public void RecordLogin() => LastLoginAt = DateTime.UtcNow;
        public void Deactivate() => IsActive = false;
        public void Reactivate() => IsActive = true;
    }
}
