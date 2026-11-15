using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for <see cref="FirstTimer"/>: people who
/// filled in the welcome form on their first visit.
/// </summary>
public class FirstTimerConfiguration : IEntityTypeConfiguration<FirstTimer>
{
    public void Configure(EntityTypeBuilder<FirstTimer> builder)
    {
        builder.ToTable("FirstTimers");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FirstName)
            .IsRequired()
            .HasMaxLength(FirstTimer.NameMaxLength);

        builder.Property(f => f.LastName)
            .IsRequired()
            .HasMaxLength(FirstTimer.NameMaxLength);

        builder.Property(f => f.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(f => f.NormalizedPhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(f => f.Email).HasMaxLength(255);
        builder.Property(f => f.AddressLine1).HasMaxLength(200);
        builder.Property(f => f.AddressLine2).HasMaxLength(200);
        builder.Property(f => f.City).HasMaxLength(100);
        builder.Property(f => f.Postcode).HasMaxLength(20);

        builder.Property(f => f.InvitedBy)
            .HasMaxLength(FirstTimer.InvitedByMaxLength);

        builder.Property(f => f.Comment)
            .HasMaxLength(FirstTimer.CommentMaxLength);

        builder.Property(f => f.FollowUpNote)
            .HasMaxLength(FirstTimer.FollowUpNoteMaxLength);

        builder.Property(f => f.VisitCount)
            .IsRequired()
            .HasDefaultValue(1);

        // The prayer request lives in the PrayerRequests table so that
        // only the prayer team can read it. If it is deleted there,
        // the first-timer record simply loses the link.
        builder.HasOne<PrayerRequest>()
            .WithMany()
            .HasForeignKey(f => f.PrayerRequestId)
            .OnDelete(DeleteBehavior.SetNull);

        // Set when an admin promotes the visitor to a member.
        builder.HasOne<Member>()
            .WithMany()
            .HasForeignKey(f => f.MemberId)
            .OnDelete(DeleteBehavior.SetNull);

        // Finds a repeat submission from the same phone number.
        builder.HasIndex(f => f.NormalizedPhoneNumber);

        builder.HasIndex(f => f.FollowUpStatus);
        builder.HasIndex(f => f.LastVisitDate);
    }
}
