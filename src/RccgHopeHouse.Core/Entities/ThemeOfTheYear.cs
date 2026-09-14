using RccgHopeHouse.Core.Exceptions;

namespace RccgHopeHouse.Core.Entities
{
    public class ThemeOfTheYear : BaseEntity
    {
        public int Year { get; private set; }
        public string ThemeTitle { get; private set; } = string.Empty;
        public string ScriptureText { get; private set; } = string.Empty;
        public string ScriptureReference { get; private set; } = string.Empty;
        public string PrimaryDescription { get; private set; } = string.Empty;
        public string? SecondaryDescription { get; private set; }
        public string? CallToActionText { get; private set; }

        private ThemeOfTheYear() { } // EF Core parameterless constructor

        public static ThemeOfTheYear Create(
            int year,
            string themeTitle,
            string scriptureText,
            string scriptureReference,
            string primaryDescription,
            string? secondaryDescription = null,
            string? callToActionText = null)
        {
            if (year < 2000 || year > 2100)
                throw new ArgumentOutOfRangeException(nameof(year), "Year must be a realistic calendar year.");

            ArgumentException.ThrowIfNullOrWhiteSpace(themeTitle, nameof(themeTitle));
            ArgumentException.ThrowIfNullOrWhiteSpace(scriptureText, nameof(scriptureText));
            ArgumentException.ThrowIfNullOrWhiteSpace(scriptureReference, nameof(scriptureReference));
            ArgumentException.ThrowIfNullOrWhiteSpace(primaryDescription, nameof(primaryDescription));

            return new ThemeOfTheYear
            {
                Year = year,
                ThemeTitle = themeTitle.Trim(),
                ScriptureText = scriptureText.Trim(),
                ScriptureReference = scriptureReference.Trim(),
                PrimaryDescription = primaryDescription.Trim(),
                SecondaryDescription = secondaryDescription?.Trim(),
                CallToActionText = callToActionText?.Trim()
            };
        }

        public void Update(
            string themeTitle,
            string scriptureText,
            string scriptureReference,
            string primaryDescription,
            string? secondaryDescription,
            string? callToActionText)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(themeTitle, nameof(themeTitle));
            ArgumentException.ThrowIfNullOrWhiteSpace(scriptureText, nameof(scriptureText));
            ArgumentException.ThrowIfNullOrWhiteSpace(scriptureReference, nameof(scriptureReference));
            ArgumentException.ThrowIfNullOrWhiteSpace(primaryDescription, nameof(primaryDescription));

            ThemeTitle = themeTitle.Trim();
            ScriptureText = scriptureText.Trim();
            ScriptureReference = scriptureReference.Trim();
            PrimaryDescription = primaryDescription.Trim();
            SecondaryDescription = secondaryDescription?.Trim();
            CallToActionText = callToActionText?.Trim();
            MarkAsUpdated();
        }
    }
}