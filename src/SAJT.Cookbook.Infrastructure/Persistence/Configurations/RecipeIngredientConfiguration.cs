using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Infrastructure.Persistence.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("RecipeIngredients");

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id)
            .ValueGeneratedOnAdd();

        builder.Property(entry => entry.Amount)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(entry => entry.Unit)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(entry => entry.Note)
            .HasMaxLength(200);

        builder.HasIndex(entry => new { entry.RecipeId, entry.IngredientId, entry.Unit })
            .IsUnique();
    }
}
