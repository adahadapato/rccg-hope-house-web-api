using RccgHopeHouse.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RccgHopeHouse.Core.Entities
{
    public class ChurchService : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public ServiceCategory Category { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public TimeSpan? StartTime { get; private set; }
        public TimeSpan? EndTime { get; private set; }
        public string? Description { get; private set; }
        public string? Location { get; private set; }
        public string? ZoomId { get; private set; }
        public string? ZoomPasscode { get; private set; }
        public bool IsActive { get; private set; } = true;
        public int DisplayOrder { get; private set; }
        public RecurrencePattern Recurrence { get; private set; } = RecurrencePattern.Weekly;
        public int? DayOfMonth { get; private set; }
        public string? AdditionalInfo { get; private set; }

        /// <summary>
        /// True for services held by this local parish; false for services
        /// broadcast/streamed from RCCG HQ (e.g. the monthly Holy
        /// Communion, Holy Ghost Service, and Thanksgiving Service from
        /// Lagos). Distinguishes services that otherwise share the same
        /// Category, DayOfWeek, and Recurrence — e.g. the local
        /// "Thanksgiving Sunday" and the HQ "Thanksgiving Service" both
        /// use ServiceCategory.ThanksgivingService + FirstOfMonth, but are
        /// genuinely different events with different times.
        /// </summary>
        public bool IsLocal { get; private set; } = true;

        private ChurchService() { }

        public static ChurchService Create(
        string name,
        ServiceCategory category,
        DayOfWeek dayOfWeek,
        TimeSpan? startTime,
        TimeSpan? endTime,
        string? description = null,
        RecurrencePattern recurrence = RecurrencePattern.Weekly,
        int? dayOfMonth = null,
        bool isLocal = true)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

            return new ChurchService
            {
                Name = name.Trim(),
                Category = category,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                Description = description?.Trim(),
                Recurrence = recurrence,
                DayOfMonth = dayOfMonth,
                IsLocal = isLocal,
                DisplayOrder = 0,
                IsActive = true
            };
        }

        public void UpdateSchedule(
        TimeSpan? startTime,
        TimeSpan? endTime,
        string? location,
        string? zoomId,
        string? zoomPasscode)
        {
            StartTime = startTime;
            EndTime = endTime;
            Location = location;
            ZoomId = zoomId;
            ZoomPasscode = zoomPasscode;
            MarkAsUpdated();
        }

        public void ToggleActive() => IsActive = !IsActive;

        public void Update(string name, ServiceCategory category,
            DayOfWeek dayOfWeek, TimeSpan? startTime, TimeSpan? endTime,
            string? description, RecurrencePattern recurrence, int? dayOfMonth,
            bool isLocal)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

            Name = name.Trim();
            Category = category;
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
            Description = description?.Trim();
            Recurrence = recurrence;
            DayOfMonth = dayOfMonth;
            IsLocal = isLocal;
            MarkAsUpdated();
        }

        public void SetDisplayOrder(int displayOrder)
        {
            DisplayOrder = displayOrder;
            MarkAsUpdated();
        }
    }
}