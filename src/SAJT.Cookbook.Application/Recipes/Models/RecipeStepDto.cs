namespace SAJT.Cookbook.Application.Recipes.Models;

public sealed record RecipeStepDto(
    long Id,
    int StepNumber,
    string Instruction,
    int? DurationMinutes,
    string? MediaUrl);
