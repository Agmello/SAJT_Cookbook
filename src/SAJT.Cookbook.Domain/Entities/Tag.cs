using System;
using System.Collections.Generic;

namespace SAJT.Cookbook.Domain.Entities;

public sealed class Tag
{
    private readonly List<RecipeTag> _recipes = new();

    private Tag()
    {
    }

    private Tag(string name, string slug)
    {
        Name = name;
        Slug = slug;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public long Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<RecipeTag> Recipes => _recipes.AsReadOnly();

    public static Tag Create(string name, string slug)
    {
        ValidateName(name);
        slug = NormalizeSlug(slug);

        var tag = new Tag(name.Trim(), slug);
        return tag;
    }

    public void Rename(string name)
    {
        ValidateName(name);
        Name = name.Trim();
        Touch();
    }

    public void SetSlug(string slug)
    {
        Slug = NormalizeSlug(slug);
        Touch();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }

        if (name.Length > 100)
        {
            throw new ArgumentException("Name cannot exceed 100 characters.", nameof(name));
        }
    }

    private static string NormalizeSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Slug cannot be empty.", nameof(slug));
        }

        if (slug.Length > 100)
        {
            throw new ArgumentException("Slug cannot exceed 100 characters.", nameof(slug));
        }

        return slug.Trim().ToLowerInvariant();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
