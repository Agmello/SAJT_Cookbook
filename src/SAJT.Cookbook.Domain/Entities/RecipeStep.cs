using System;

namespace SAJT.Cookbook.Domain.Entities;

public sealed class RecipeStep
{
    private RecipeStep()
    {
    }

    private RecipeStep(Recipe recipe, int stepNumber, string instruction, int? durationMinutes, string? mediaUrl)
    {
        Recipe = recipe;
        RecipeId = recipe.Id;
        StepNumber = stepNumber;
        Instruction = instruction;
        DurationMinutes = durationMinutes;
        MediaUrl = mediaUrl;
    }

    public long Id { get; private set; }

    public long RecipeId { get; private set; }

    public Recipe Recipe { get; private set; } = null!;

    public int StepNumber { get; private set; }

    public string Instruction { get; private set; } = string.Empty;

    public int? DurationMinutes { get; private set; }

    public string? MediaUrl { get; private set; }

    public static RecipeStep Create(Recipe recipe, int stepNumber, string instruction, int? durationMinutes = null, string? mediaUrl = null)
    {
        if (recipe is null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        if (stepNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stepNumber), "Step number must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(instruction))
        {
            throw new ArgumentException("Instruction cannot be empty.", nameof(instruction));
        }

        if (instruction.Length > 2000)
        {
            throw new ArgumentException("Instruction cannot exceed 2000 characters.", nameof(instruction));
        }

        if (durationMinutes is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(durationMinutes));
        }

        mediaUrl = mediaUrl?.Trim();
        if (mediaUrl is { Length: > 500 })
        {
            throw new ArgumentException("Media URL cannot exceed 500 characters.", nameof(mediaUrl));
        }

        return new RecipeStep(recipe, stepNumber, instruction.Trim(), durationMinutes, mediaUrl);
    }

    public void Update(string instruction, int? durationMinutes = null, string? mediaUrl = null)
    {
        if (string.IsNullOrWhiteSpace(instruction))
        {
            throw new ArgumentException("Instruction cannot be empty.", nameof(instruction));
        }

        if (instruction.Length > 2000)
        {
            throw new ArgumentException("Instruction cannot exceed 2000 characters.", nameof(instruction));
        }

        if (durationMinutes is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(durationMinutes));
        }

        mediaUrl = mediaUrl?.Trim();
        if (mediaUrl is { Length: > 500 })
        {
            throw new ArgumentException("Media URL cannot exceed 500 characters.", nameof(mediaUrl));
        }

        Instruction = instruction.Trim();
        DurationMinutes = durationMinutes;
        MediaUrl = mediaUrl;
    }
}
