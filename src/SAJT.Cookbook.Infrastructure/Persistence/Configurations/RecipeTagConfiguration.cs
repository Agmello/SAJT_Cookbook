using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Infrastructure.Persistence.Configurations;

public class RecipeTagConfiguration : IEntityTypeConfiguration<RecipeTag>
{
    public void Configure(EntityTypeBuilder<RecipeTag> builder)
    {
        builder.ToTable("RecipeTags");

        builder.HasKey(tag => new { tag.RecipeId, tag.TagId });

        builder.HasOne(tag => tag.Recipe)
            .WithMany(recipe => recipe.Tags)
            .HasForeignKey(tag => tag.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tag => tag.Tag)
            .WithMany(tag => tag.Recipes)
            .HasForeignKey(tag => tag.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
