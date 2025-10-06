using System;
using System.Collections.Generic;
using System.Linq;
using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.Domain.Entities;

public sealed class Recipe
{
    private readonly List<RecipeStep> _steps = new();
    private readonly List<RecipeIngredient> _ingredients = new();
    private readonly List<RecipeTag> _tags = new();

    private Recipe()
    {
    }

    private Recipe(
        Guid authorId,
        string slug,
        string title,
        string? description,
        int prepTimeMinutes,
        int cookTimeMinutes,
        byte servings,
        RecipeDifficulty difficulty)
    {
        AuthorId = authorId;
        Slug = slug;
        Title = title;
        Description = description;
        PrepTimeMinutes = prepTimeMinutes;
        CookTimeMinutes = cookTimeMinutes;
        Servings = servings;
        Difficulty = difficulty;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public long Id { get; private set; }

    public Guid AuthorId { get; private set; }

    public string Slug { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public int PrepTimeMinutes { get; private set; }

    public int CookTimeMinutes { get; private set; }

    public byte Servings { get; private set; }

    public RecipeDifficulty Difficulty { get; private set; }

    public bool IsPublished { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public IReadOnlyCollection<RecipeStep> Steps => _steps.AsReadOnly();

    public IReadOnlyCollection<RecipeIngredient> Ingredients => _ingredients.AsReadOnly();

    public IReadOnlyCollection<RecipeTag> Tags => _tags.AsReadOnly();

    public static Recipe Create(
        Guid authorId,
        string slug,
        string title,
        string? description,
        int prepTimeMinutes,
        int cookTimeMinutes,
        byte servings,
        RecipeDifficulty difficulty)
    {
        if (authorId == Guid.Empty)
        {
            throw new ArgumentException("Author id is required.", nameof(authorId));
        }

        slug = NormalizeSlug(slug);
        ValidateTitle(title);
        ValidateTimings(prepTimeMinutes, cookTimeMinutes);

        var recipe = new Recipe(
            authorId,
            slug,
            title.Trim(),
            description,
            prepTimeMinutes,
            cookTimeMinutes,
            servings,
            difficulty);

        return recipe;
    }

    public void UpdateDetails(
        string title,
        string? description,
        int prepTimeMinutes,
        int cookTimeMinutes,
        byte servings,
        RecipeDifficulty difficulty)
    {
        ValidateTitle(title);
        ValidateTimings(prepTimeMinutes, cookTimeMinutes);

        Title = title.Trim();
        Description = description;
        PrepTimeMinutes = prepTimeMinutes;
        CookTimeMinutes = cookTimeMinutes;
        Servings = servings;
        Difficulty = difficulty;

        Touch();
    }

    public void SetSlug(string slug)
    {
        Slug = NormalizeSlug(slug);
        Touch();
    }

    public void Publish()
    {
        if (IsPublished)
        {
            return;
        }

        IsPublished = true;
        Touch();
    }

    public void Unpublish()
    {
        if (!IsPublished)
        {
            return;
        }

        IsPublished = false;
        Touch();
    }

    public RecipeStep AddStep(int stepNumber, string instruction, int? durationMinutes = null, string? mediaUrl = null)
    {
        if (_steps.Any(step => step.StepNumber == stepNumber))
        {
            throw new InvalidOperationException($"Step number {stepNumber} already exists for this recipe.");
        }

        var step = RecipeStep.Create(this, stepNumber, instruction, durationMinutes, mediaUrl);

        _steps.Add(step);
        Touch();
        return step;
    }

    public void RemoveStep(int stepNumber)
    {
        var step = _steps.SingleOrDefault(step => step.StepNumber == stepNumber);
        if (step is null)
        {
            return;
        }

        _steps.Remove(step);
        Touch();
    }

    public RecipeIngredient AddIngredient(Ingredient ingredient, decimal amount, MeasurementUnit unit, string? note = null)
    {
        if (ingredient is null)
        {
            throw new ArgumentNullException(nameof(ingredient));
        }

        var entry = RecipeIngredient.Create(this, ingredient, amount, unit, note);

        _ingredients.Add(entry);
        Touch();
        return entry;
    }

    public void RemoveIngredient(long ingredientEntryId)
    {
        var entry = _ingredients.SingleOrDefault(item => item.Id == ingredientEntryId);
        if (entry is null)
        {
            return;
        }

        _ingredients.Remove(entry);
        Touch();
    }

    public void AddTag(Tag tag)
    {
        if (tag is null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (_tags.Any(t => t.TagId == tag.Id))
        {
            return;
        }

        _tags.Add(new RecipeTag(this, tag));
        Touch();
    }

    public void RemoveTag(long tagId)
    {
        var tag = _tags.SingleOrDefault(t => t.TagId == tagId);
        if (tag is null)
        {
            return;
        }

        _tags.Remove(tag);
        Touch();
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        }

        if (title.Length > 200)
        {
            throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));
        }
    }

    private static void ValidateTimings(int prepTimeMinutes, int cookTimeMinutes)
    {
        if (prepTimeMinutes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(prepTimeMinutes));
        }

        if (cookTimeMinutes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cookTimeMinutes));
        }
    }

    private static string NormalizeSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Slug cannot be empty.", nameof(slug));
        }

        return slug.Trim().ToLowerInvariant();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

