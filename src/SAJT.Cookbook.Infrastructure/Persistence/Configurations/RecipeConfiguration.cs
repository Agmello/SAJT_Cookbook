using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Infrastructure.Persistence.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("Recipes");

        builder.HasKey(recipe => recipe.Id);

        builder.Property(recipe => recipe.Id)
            .ValueGeneratedOnAdd();

        builder.Property(recipe => recipe.AuthorId)
            .IsRequired();

        builder.Property(recipe => recipe.Slug)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(recipe => recipe.Slug)
            .IsUnique();

        builder.Property(recipe => recipe.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(recipe => new { recipe.AuthorId, recipe.Title })
            .IsUnique();

        builder.Property(recipe => recipe.Description)
            .HasMaxLength(2000);

        builder.Property(recipe => recipe.PrepTimeMinutes)
            .IsRequired();

        builder.Property(recipe => recipe.CookTimeMinutes)
            .IsRequired();

        builder.Property(recipe => recipe.Servings)
            .HasDefaultValue((byte)0);

        builder.Property(recipe => recipe.Difficulty)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(recipe => recipe.IsPublished)
            .HasDefaultValue(false);

        builder.Property(recipe => recipe.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(recipe => recipe.UpdatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(recipe => recipe.RowVersion)
            .IsRowVersion();

        builder.Navigation(recipe => recipe.Steps)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(recipe => recipe.Ingredients)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(recipe => recipe.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(recipe => recipe.Steps)
            .WithOne(step => step.Recipe)
            .HasForeignKey(step => step.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(recipe => recipe.Ingredients)
            .WithOne(entry => entry.Recipe)
            .HasForeignKey(entry => entry.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(recipe => recipe.Tags)
            .WithOne(tag => tag.Recipe)
            .HasForeignKey(tag => tag.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(recipe => recipe.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}

