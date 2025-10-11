using System;
using System.Collections.Generic;
using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.Application.Recipes.Models;

public sealed record RecipeDetailsDto(
    long Id,
    Guid AuthorId,
    string? AuthorName,
    string Slug,
    string Title,
    string? Description,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    byte Servings,
    RecipeDifficulty Difficulty,
    bool IsPublished,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    IReadOnlyList<RecipeIngredientDto> Ingredients,
    IReadOnlyList<RecipeStepDto> Steps,
    IReadOnlyList<RecipeTagDto> Tags);
