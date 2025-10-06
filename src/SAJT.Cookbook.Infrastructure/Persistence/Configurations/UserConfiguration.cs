using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .ValueGeneratedNever();

        builder.Property(user => user.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(user => user.Name)
            .IsUnique();

        builder.Property(user => user.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(user => user.UpdatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();
    }
}
