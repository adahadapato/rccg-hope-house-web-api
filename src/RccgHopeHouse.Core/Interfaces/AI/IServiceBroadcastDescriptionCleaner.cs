namespace RccgHopeHouse.Core.Interfaces.AI;

/// <summary>
/// Represents cleaned metadata extracted from a service broadcast.
/// </summary>
/// <param name="Description">
/// Cleaned description suitable for display on the church website.
/// </param>
/// <param name="Theme">
/// Service theme explicitly identified from the broadcast information,
/// or <see langword="null"/> when no theme can be identified.
/// </param>
public sealed record ServiceBroadcastCleaningResult(
    string? Description,
    string? Theme);

/// <summary>
/// Defines a service that cleans and extracts useful metadata from
/// service broadcast information before it is stored.
/// </summary>
public interface IServiceBroadcastDescriptionCleaner
{
    /// <summary>
    /// Cleans a broadcast description and extracts the service theme
    /// when the theme is explicitly present in the supplied information.
    /// </summary>
    /// <param name="title">
    /// The original YouTube video title.
    /// </param>
    /// <param name="description">
    /// The original YouTube video description.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The cleaned description and any explicitly identified service theme.
    /// </returns>
    Task<ServiceBroadcastCleaningResult> CleanAsync(
        string title,
        string? description,
        CancellationToken cancellationToken = default);
}