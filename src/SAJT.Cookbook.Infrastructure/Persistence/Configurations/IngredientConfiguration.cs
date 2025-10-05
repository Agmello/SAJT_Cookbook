using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Infrastructure.Persistence.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("Ingredients");

        builder.HasKey(ingredient => ingredient.Id);

        builder.Property(ingredient => ingredient.Id)
            .ValueGeneratedOnAdd();

        builder.Property(ingredient => ingredient.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(ingredient => ingredient.Name)
            .IsUnique();

        builder.Property(ingredient => ingredient.PluralName)
            .HasMaxLength(150);

        builder.Property(ingredient => ingredient.DefaultUnit)
            .HasConversion<byte?>();

        builder.Property(ingredient => ingredient.IsActive)
            .HasDefaultValue(true);

        builder.Property(ingredient => ingredient.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(ingredient => ingredient.UpdatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Navigation(ingredient => ingredient.RecipeIngredients)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
