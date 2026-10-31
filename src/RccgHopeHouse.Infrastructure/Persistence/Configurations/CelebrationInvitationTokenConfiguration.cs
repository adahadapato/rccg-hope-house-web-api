
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures persistence, relationships, indexes and
/// optimistic concurrency for secure celebration invitation tokens.
/// </summary>
/// <remarks>
/// Invitation tokens allow church members to respond to birthday
/// and wedding anniversary publication invitations without
/// requiring an authenticated website account.
///
/// Only the SHA-256 hash of each invitation token is persisted.
/// Plaintext invitation tokens are never stored.
///
/// SQL Server rowversion protects invitation records against
/// conflicting concurrent database updates.
/// </remarks>
public sealed class CelebrationInvitationTokenConfiguration : IEntityTypeConfiguration<CelebrationInvitationToken>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CelebrationInvitationToken> builder)
    {
        // ==================== Table ====================

        builder.ToTable("CelebrationInvitationTokens");

        // ==================== Primary Key ====================

        builder.HasKey(x => x.Id);

        // ==================== Celebration Relationship ====================

        builder.Property(x => x.CelebrationId)
            .IsRequired();

        builder.HasOne(x => x.Celebration)
            .WithMany()
            .HasForeignKey(x => x.CelebrationId)
            .OnDelete(DeleteBehavior.Restrict);

        // ==================== Token Hash ====================

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(64)
            .IsUnicode(false);

        // ==================== Invitation Timestamps ====================

        builder.Property(x => x.IssuedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(x => x.ExpiresAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(x => x.UsedAt)
            .IsRequired(false)
            .HasColumnType("datetime2");

        builder.Property(x => x.RevokedAt)
            .IsRequired(false)
            .HasColumnType("datetime2");

        // ==================== Optimistic Concurrency ====================

        // SQL Server generates a new rowversion whenever
        // the invitation record is updated.
        //
        // EF Core includes the original rowversion in the
        // UPDATE statement's WHERE condition.
        //
        // If another request has already updated the row,
        // the second update affects zero rows and EF Core
        // raises DbUpdateConcurrencyException.

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        // ==================== Computed Domain Properties ====================

        builder.Ignore(x => x.IsUsed);

        builder.Ignore(x => x.IsRevoked);

        // ==================== Indexes ====================

        builder.HasIndex(x => x.TokenHash)
            .IsUnique()
            .HasDatabaseName(
                "IX_CelebrationInvitationTokens_TokenHash");

        builder.HasIndex(x => x.CelebrationId)
            .HasDatabaseName(
                "IX_CelebrationInvitationTokens_CelebrationId");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName(
                "IX_CelebrationInvitationTokens_ExpiresAt");
    }
}
