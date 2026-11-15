namespace RccgHopeHouse.Core.Interfaces
{
    public interface IThemeOfTheYearRepository
    {
        /// <summary>
        /// Gets the theme for a specific year, or null if none exists yet.
        /// </summary>
        Task<ThemeOfTheYear?> GetByYearAsync(int year, CancellationToken ct = default);

        /// <summary>
        /// Gets the theme for the current calendar year (server UTC year),
        /// or null if it hasn't been set up yet.
        /// </summary>
        Task<ThemeOfTheYear?> GetCurrentYearThemeAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the most recently created/updated theme regardless of year —
        /// useful as a fallback if the current year's theme hasn't been entered yet.
        /// </summary>
        Task<ThemeOfTheYear?> GetLatestAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets all themes, most recent year first — for an admin history/list view.
        /// </summary>
        Task<IReadOnlyList<ThemeOfTheYear>> GetAllAsync(CancellationToken ct = default);

        Task<ThemeOfTheYear?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task AddAsync(ThemeOfTheYear theme, CancellationToken ct = default);

        Task UpdateAsync(ThemeOfTheYear theme, CancellationToken ct = default);

        Task DeleteAsync(ThemeOfTheYear theme, CancellationToken ct = default);

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}