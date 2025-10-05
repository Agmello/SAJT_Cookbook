using System;
using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.Application.Recipes.Models;

public sealed record RecipeSummaryDto(
    long Id,
    string Title,
    string? Description,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    byte Servings,
    RecipeDifficulty Difficulty,
    bool IsPublished,
    DateTime UpdatedAtUtc);
