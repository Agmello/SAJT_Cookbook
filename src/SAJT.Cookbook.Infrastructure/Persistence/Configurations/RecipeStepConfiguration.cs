using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Infrastructure.Persistence.Configurations;

public class RecipeStepConfiguration : IEntityTypeConfiguration<RecipeStep>
{
    public void Configure(EntityTypeBuilder<RecipeStep> builder)
    {
        builder.ToTable("RecipeSteps");

        builder.HasKey(step => step.Id);

        builder.Property(step => step.Id)
            .ValueGeneratedOnAdd();

        builder.Property(step => step.StepNumber)
            .IsRequired();

        builder.Property(step => step.Instruction)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(step => step.DurationMinutes);

        builder.Property(step => step.MediaUrl)
            .HasMaxLength(500);

        builder.HasIndex(step => new { step.RecipeId, step.StepNumber })
            .IsUnique();
    }
}
