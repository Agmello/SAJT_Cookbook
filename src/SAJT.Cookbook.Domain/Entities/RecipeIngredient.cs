using System;
using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.Domain.Entities;

public sealed class RecipeIngredient
{
    private RecipeIngredient()
    {
    }

    private RecipeIngredient(Recipe recipe, Ingredient ingredient, decimal amount, MeasurementUnit unit, string? note)
    {
        Recipe = recipe;
        RecipeId = recipe.Id;
        Ingredient = ingredient;
        IngredientId = ingredient.Id;
        Amount = amount;
        Unit = unit;
        Note = note;
    }

    public long Id { get; private set; }

    public long RecipeId { get; private set; }

    public Recipe Recipe { get; private set; } = null!;

    public long IngredientId { get; private set; }

    public Ingredient Ingredient { get; private set; } = null!;

    public decimal Amount { get; private set; }

    public MeasurementUnit Unit { get; private set; }

    public string? Note { get; private set; }

    public static RecipeIngredient Create(Recipe recipe, Ingredient ingredient, decimal amount, MeasurementUnit unit, string? note = null)
    {
        if (recipe is null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        if (ingredient is null)
        {
            throw new ArgumentNullException(nameof(ingredient));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        if (note is { Length: > 200 })
        {
            throw new ArgumentException("Note cannot exceed 200 characters.", nameof(note));
        }

        var roundedAmount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);

        return new RecipeIngredient(recipe, ingredient, roundedAmount, unit, note?.Trim());
    }

    public void Update(decimal amount, MeasurementUnit unit, string? note)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        if (note is { Length: > 200 })
        {
            throw new ArgumentException("Note cannot exceed 200 characters.", nameof(note));
        }

        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Unit = unit;
        Note = note?.Trim();
    }
}
