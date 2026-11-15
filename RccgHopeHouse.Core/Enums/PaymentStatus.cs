namespace RccgHopeHouse.Core.Enums;

/// <summary>
/// Represents the current payment state of an offering.
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// The offering has been created but payment has not yet been confirmed.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Payment has been successfully completed and confirmed.
    /// </summary>
    Paid = 2,

    /// <summary>
    /// The payment attempt failed.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// The payment was cancelled before completion.
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// A previously completed payment has been refunded.
    /// </summary>
    Refunded = 5
}