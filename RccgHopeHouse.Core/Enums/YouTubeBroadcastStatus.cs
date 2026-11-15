namespace RccgHopeHouse.Core.Enums;

/// <summary>
/// Represents the broadcast state of a YouTube video.
/// </summary>
public enum YouTubeBroadcastStatus
{
    /// <summary>
    /// The video is not identified as a livestream broadcast.
    /// </summary>
    None = 0,

    /// <summary>
    /// The livestream is scheduled but has not yet started.
    /// </summary>
    Upcoming = 1,

    /// <summary>
    /// The livestream is currently live.
    /// </summary>
    Live = 2,

    /// <summary>
    /// The livestream has ended.
    /// </summary>
    Completed = 3
}
