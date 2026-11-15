namespace RccgHopeHouse.Core.Results.AI;

/// <summary>
/// Represents the outcome of a YouTube service broadcast synchronization run.
/// </summary>
/// <param name="VideosExamined">
/// The number of unique YouTube videos examined during synchronization.
/// </param>
/// <param name="BroadcastsCreated">
/// The number of new service broadcasts created.
/// </param>
/// <param name="BroadcastsUpdated">
/// The number of existing service broadcasts updated.
/// </param>
/// <param name="BroadcastsSkipped">
/// The number of videos that did not result in a database change.
/// </param>
public sealed record ServiceBroadcastSynchronizationResult(
    int VideosExamined,
    int BroadcastsCreated,
    int BroadcastsUpdated,
    int BroadcastsSkipped);
