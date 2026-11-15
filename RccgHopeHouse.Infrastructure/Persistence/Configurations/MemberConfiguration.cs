using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for <see cref="Member"/>: church congregation
/// records used for admin management and automated birthday/anniversary
/// greetings.
/// </summary>
public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    /// <summary>
    /// Configures the database mapping for <see cref="Member"/>.
    /// </summary>
    /// <param name="builder">
    /// The entity type builder used to configure the member entity.
    /// </param>
    public void Configure(
        EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(
            member => member.Id);

        builder.Property(
                member => member.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(
                member => member.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(
                member => member.Email)
            .HasMaxLength(255)
            .HasConversion(
                valueObject =>
                    valueObject != null
                        ? valueObject.Value
                        : null,
                value =>
                    value != null
                        ? EmailAddress.CreateOrNull(
                            value)
                        : null);

        builder.Property(
                member => member.PhoneNumber)
            .HasMaxLength(20)
            .HasConversion(
                valueObject =>
                    valueObject != null
                        ? valueObject.Value
                        : null,
                value =>
                    value != null
                        ? PhoneNumber.CreateOrNull(
                            value)
                        : null);

        builder.Property(
                member =>
                    member.ConsentToBirthdayPublication)
            .IsRequired()
            .HasDefaultValue(false);

        ConfigureAddress(
            builder);

        ConfigureBirthday(
            builder);

        ConfigurePhoto(
            builder);

        builder.HasIndex(
            member =>
                member.WeddingAnniversary);

        builder.HasIndex(
            member =>
                member.IsActive);

        builder.HasIndex(
            member =>
                member.ConsentToBirthdayPublication);

        builder.HasIndex(
            member =>
                member.PhotoId);
    }

    /// <summary>
    /// Configures the member's optional postal address as an owned
    /// value object stored in the Members table.
    /// </summary>
    /// <param name="builder">
    /// The member entity type builder.
    /// </param>
    private static void ConfigureAddress(
        EntityTypeBuilder<Member> builder)
    {
        builder.OwnsOne(
            member => member.Address,
            address =>
            {
                address.Property(
                        value =>
                            value.AddressLine1)
                    .HasColumnName(
                        "AddressLine1")
                    .HasMaxLength(200);

                address.Property(
                        value =>
                            value.AddressLine2)
                    .HasColumnName(
                        "AddressLine2")
                    .HasMaxLength(200);

                address.Property(
                        value =>
                            value.City)
                    .HasColumnName(
                        "City")
                    .HasMaxLength(100);

                address.Property(
                        value =>
                            value.County)
                    .HasColumnName(
                        "County")
                    .HasMaxLength(100);

                address.Property(
                        value =>
                            value.Postcode)
                    .HasColumnName(
                        "Postcode")
                    .HasMaxLength(20);

                address.Property(
                        value =>
                            value.Country)
                    .HasColumnName(
                        "Country")
                    .HasMaxLength(100);
            });
    }

    /// <summary>
    /// Configures the member's optional birthday as an owned
    /// value object stored in the Members table.
    /// </summary>
    /// <param name="builder">
    /// The member entity type builder.
    /// </param>
    private static void ConfigureBirthday(
        EntityTypeBuilder<Member> builder)
    {
        builder.OwnsOne(
            member => member.Birthday,
            birthday =>
            {
                birthday.Property(
                        value =>
                            value.Month)
                    .HasColumnName(
                        "BirthMonth");

                birthday.Property(
                        value =>
                            value.Day)
                    .HasColumnName(
                        "BirthDay");

                birthday.Property(
                        value =>
                            value.Year)
                    .HasColumnName(
                        "BirthYear");

                // Supports the daily birthday lookup:
                // WHERE BirthMonth = X AND BirthDay = Y.
                birthday.HasIndex(
                    value => new
                    {
                        value.Month,
                        value.Day
                    });
            });
    }

    /// <summary>
    /// Configures the optional relationship between a member and the
    /// existing gallery image selected as their birthday/profile photo.
    /// </summary>
    /// <param name="builder">
    /// The member entity type builder.
    /// </param>
    private static void ConfigurePhoto(
        EntityTypeBuilder<Member> builder)
    {
        builder.HasOne(
                member =>
                    member.Photo)
            .WithMany()
            .HasForeignKey(
                member =>
                    member.PhotoId)
            .OnDelete(
                DeleteBehavior.SetNull);
    }
}