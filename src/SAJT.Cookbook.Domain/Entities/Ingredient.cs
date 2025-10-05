using System;
using System.Collections.Generic;
using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.Domain.Entities;

public sealed class Ingredient
{
    private readonly List<RecipeIngredient> _recipeIngredients = new();

    private Ingredient()
    {
    }

    private Ingredient(string name, string? pluralName, MeasurementUnit? defaultUnit)
    {
        Name = name;
        PluralName = pluralName;
        DefaultUnit = defaultUnit;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public long Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? PluralName { get; private set; }

    public MeasurementUnit? DefaultUnit { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<RecipeIngredient> RecipeIngredients => _recipeIngredients.AsReadOnly();

    public static Ingredient Create(string name, string? pluralName = null, MeasurementUnit? defaultUnit = null)
    {
        ValidateName(name);

        var ingredient = new Ingredient(name.Trim(), pluralName?.Trim(), defaultUnit);
        return ingredient;
    }

    public void Rename(string name, string? pluralName = null)
    {
        ValidateName(name);

        Name = name.Trim();
        PluralName = pluralName?.Trim();
        Touch();
    }

    public void SetDefaultUnit(MeasurementUnit? defaultUnit)
    {
        DefaultUnit = defaultUnit;
        Touch();
    }

    public void SetStatus(bool isActive)
    {
        if (IsActive == isActive)
        {
            return;
        }

        IsActive = isActive;
        Touch();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }

        if (name.Length > 150)
        {
            throw new ArgumentException("Name cannot exceed 150 characters.", nameof(name));
        }
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
