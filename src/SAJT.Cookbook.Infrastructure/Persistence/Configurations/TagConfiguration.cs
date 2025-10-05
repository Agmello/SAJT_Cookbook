using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Infrastructure.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(tag => tag.Id);

        builder.Property(tag => tag.Id)
            .ValueGeneratedOnAdd();

        builder.Property(tag => tag.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(tag => tag.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(tag => tag.Slug)
            .IsUnique();

        builder.Property(tag => tag.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(tag => tag.UpdatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Navigation(tag => tag.Recipes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
