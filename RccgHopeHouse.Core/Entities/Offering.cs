using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a financial offering submitted through the Give Online feature.
///
/// An offering belongs to a <see cref="GivingType"/> and may either be
/// submitted anonymously or with the giver's details.
/// </summary>
public class Offering : BaseEntity
{
    /// <summary>
    /// Gets the identifier of the giving type selected for this offering.
    /// </summary>
    public Guid GivingTypeId { get; private set; }

    /// <summary>
    /// Gets the giving type associated with this offering.
    /// </summary>
    public GivingType GivingType { get; private set; } = null!;

    /// <summary>
    /// Gets the monetary amount of the offering.
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Gets the current payment status of the offering.
    /// </summary>
    public PaymentStatus PaymentStatus { get; private set; }

    /// <summary>
    /// Gets the payment reference used to associate this offering with
    /// a payment transaction.
    /// </summary>
    public string? PaymentReference { get; private set; }

    /// <summary>
    /// Gets the giver's full name.
    /// Null when the offering is anonymous.
    /// </summary>
    public string? FullName { get; private set; }

    /// <summary>
    /// Gets the giver's email address.
    /// Null when the offering is anonymous.
    /// </summary>
    public EmailAddress? Email { get; private set; }

    /// <summary>
    /// Gets whether the offering was submitted anonymously.
    /// </summary>
    public bool IsAnonymous { get; private set; }

    /// <summary>
    /// Gets an optional message or reference supplied by the giver.
    /// </summary>
    public string? MessageReference { get; private set; }

    /// <summary>
    /// Required by EF Core.
    /// </summary>
    private Offering()
    {
    }

    /// <summary>
    /// Creates a new offering in the Pending payment state.
    /// </summary>
    /// <param name="givingTypeId">The selected giving type.</param>
    /// <param name="amount">The amount being given.</param>
    /// <param name="isAnonymous">
    /// Indicates whether the giver wishes to remain anonymous.
    /// </param>
    /// <param name="fullName">
    /// The giver's full name. Required when the offering is not anonymous.
    /// </param>
    /// <param name="email">
    /// The giver's email address. Required when the offering is not anonymous.
    /// </param>
    /// <param name="messageReference">
    /// Optional message or reference supplied by the giver.
    /// </param>
    /// <returns>A new <see cref="Offering"/>.</returns>
    public static Offering Create(
        Guid givingTypeId,
        decimal amount,
        bool isAnonymous,
        string? fullName = null,
        string? email = null,
        string? messageReference = null)
    {
        if (givingTypeId == Guid.Empty)
            throw new ArgumentException(
                "A giving type is required.",
                nameof(givingTypeId));

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Offering amount must be greater than zero.");

        if (!isAnonymous)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                fullName,
                nameof(fullName));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                email,
                nameof(email));
        }

        return new Offering
        {
            GivingTypeId = givingTypeId,
            Amount = amount,
            PaymentStatus = PaymentStatus.Pending,
            PaymentReference = null,
            IsAnonymous = isAnonymous,

            FullName = isAnonymous
                ? null
                : fullName!.Trim(),

            Email = isAnonymous
                ? null
                : EmailAddress.CreateOrNull(email),

            MessageReference = NormalizeOptionalText(messageReference)
        };
    }

    /// <summary>
    /// Assigns the payment reference returned by the payment process.
    /// </summary>
    /// <param name="paymentReference">The payment transaction reference.</param>
    public void SetPaymentReference(string paymentReference)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            paymentReference,
            nameof(paymentReference));

        PaymentReference = paymentReference.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Marks the offering payment as successfully completed.
    /// </summary>
    public void MarkAsPaid()
    {
        if (PaymentStatus == PaymentStatus.Paid)
            return;

        PaymentStatus = PaymentStatus.Paid;
        MarkAsUpdated();
    }

    /// <summary>
    /// Marks the offering payment as failed.
    /// </summary>
    public void MarkAsFailed()
    {
        if (PaymentStatus == PaymentStatus.Failed)
            return;

        PaymentStatus = PaymentStatus.Failed;
        MarkAsUpdated();
    }

    /// <summary>
    /// Marks the offering payment as cancelled.
    /// </summary>
    public void Cancel()
    {
        if (PaymentStatus == PaymentStatus.Cancelled)
            return;

        PaymentStatus = PaymentStatus.Cancelled;
        MarkAsUpdated();
    }

    /// <summary>
    /// Marks a successfully paid offering as refunded.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to refund an offering that has not been paid.
    /// </exception>
    public void MarkAsRefunded()
    {
        if (PaymentStatus == PaymentStatus.Refunded)
            return;

        if (PaymentStatus != PaymentStatus.Paid)
            throw new InvalidOperationException(
                "Only a paid offering can be refunded.");

        PaymentStatus = PaymentStatus.Refunded;
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the optional message or reference supplied by the giver.
    /// </summary>
    /// <param name="messageReference">
    /// The new message or reference. Blank values are stored as null.
    /// </param>
    public void UpdateMessageReference(string? messageReference)
    {
        MessageReference = NormalizeOptionalText(messageReference);
        MarkAsUpdated();
    }

    /// <summary>
    /// Normalizes optional text by trimming surrounding whitespace
    /// and converting blank values to null.
    /// </summary>
    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}