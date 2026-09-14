namespace RccgHopeHouse.Core.ValueObjects
{
    public partial record TimeSlot(TimeSpan Start, TimeSpan End)
    {
        public string DisplayFormat => $"{FormatTime(Start)} - {FormatTime(End)}";

        private static string FormatTime(TimeSpan time)
        {
            var hour = time.Hours > 12 ? time.Hours - 12 : time.Hours;
            var period = time.Hours >= 12 ? "pm" : "am";
            return $"{hour}:{time.Minutes:D2}{period}";
        }

        public bool Overlaps(TimeSlot other) =>
            Start < other.End && End > other.Start;
    }
}
