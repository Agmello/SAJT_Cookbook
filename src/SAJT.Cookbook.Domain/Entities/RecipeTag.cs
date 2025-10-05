using System;

namespace SAJT.Cookbook.Domain.Entities;

public sealed class RecipeTag
{
    private RecipeTag()
    {
    }

    internal RecipeTag(Recipe recipe, Tag tag)
    {
        Recipe = recipe ?? throw new ArgumentNullException(nameof(recipe));
        Tag = tag ?? throw new ArgumentNullException(nameof(tag));
        RecipeId = recipe.Id;
        TagId = tag.Id;
    }

    public long RecipeId { get; private set; }

    public Recipe Recipe { get; private set; } = null!;

    public long TagId { get; private set; }

    public Tag Tag { get; private set; } = null!;
}
