namespace SAJT.Cookbook.Application.Recipes.Models;

public sealed record RecipeTagDto(
    long Id,
    string Name,
    string Slug);
