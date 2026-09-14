namespace RccgHopeHouse.Core.Entities
{
    public class PrayerRequest : BaseEntity
    {
        public string RequesterName { get; private set; } = string.Empty;
        public bool IsAnonymous { get; private set; }
        public EmailAddress? RequesterEmail { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public PhoneNumber? PhoneNumber { get; private set; }
        public string? PastoralNote { get; private set; }
        public PrayerRequestStatus Status { get; private set; } = PrayerRequestStatus.Pending;
        public DateTime? RespondedAt { get; private set; }

        private PrayerRequest() { } // EF Core parameterless constructor

        public static PrayerRequest Create(
            string content,
            bool isAnonymous,
            string? requesterName = null,
            string? phoneNumber = null,
            string? requesterEmail = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

            if (!isAnonymous)
                ArgumentException.ThrowIfNullOrWhiteSpace(requesterName, nameof(requesterName));

            return new PrayerRequest
            {
                RequesterName = isAnonymous ? "Anonymous" : requesterName!.Trim(),
                IsAnonymous = isAnonymous,
                Content = content.Trim(),
                RequesterEmail = isAnonymous ? null : EmailAddress.CreateOrNull(requesterEmail),
                PhoneNumber = isAnonymous ? null : PhoneNumber.CreateOrNull(phoneNumber)
            };
        }

        public void MarkAsResolved() => UpdateStatus(PrayerRequestStatus.Resolved, PastoralNote);

        public void UpdateStatus(PrayerRequestStatus newStatus, string? pastoralNote)
        {
            Status = newStatus;
            PastoralNote = pastoralNote?.Trim();
            if (newStatus == PrayerRequestStatus.Resolved)
                RespondedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }
    }
}