using System.Text;
using MediatR;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Recipes.Models;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.Application.Recipes.Commands.CreateRecipe;

public sealed class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, CreateRecipeResult>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRecipeCommandHandler(IRecipeRepository recipeRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateRecipeResult> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        if (request.AuthorId == Guid.Empty)
        {
            return CreateRecipeResult.InvalidAuthor();
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return CreateRecipeResult.InvalidTitle();
        }

        if (request.Servings is 0)
        {
            return CreateRecipeResult.InvalidServings();
        }

        if (request.PrepTimeMinutes < 0 || request.CookTimeMinutes < 0)
        {
            return CreateRecipeResult.InvalidTiming();
        }

        var author = await _userRepository.GetByIdAsync(request.AuthorId, cancellationToken);
        if (author is null)
        {
            return CreateRecipeResult.InvalidAuthor();
        }

        var slug = GenerateSlug(request.Title);

        var recipe = Recipe.Create(
            request.AuthorId,
            slug,
            request.Title.Trim(),
            request.Description?.Trim(),
            request.PrepTimeMinutes,
            request.CookTimeMinutes,
            request.Servings,
            request.Difficulty);

        if (request.IsPublished)
        {
            recipe.Publish();
        }

        _recipeRepository.Add(recipe);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var summary = new RecipeSummaryDto(
            recipe.Id,
            recipe.Title,
            recipe.Description,
            recipe.PrepTimeMinutes,
            recipe.CookTimeMinutes,
            recipe.Servings,
            recipe.Difficulty,
            recipe.IsPublished,
            recipe.UpdatedAtUtc);

        return CreateRecipeResult.Success(summary);
    }

    private static string GenerateSlug(string title)
    {
        var normalized = title.Trim().ToLowerInvariant();
        var builder = new StringBuilder();

        foreach (var ch in normalized)
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
            }
            else if (char.IsWhiteSpace(ch) || ch is '-' or '_')
            {
                if (builder.Length > 0 && builder[^1] != '-')
                {
                    builder.Append('-');
                }
            }
        }

        var slugBody = builder.ToString().Trim('-');
        var suffix = Guid.NewGuid().ToString("N")[..8];

        if (string.IsNullOrWhiteSpace(slugBody))
        {
            return $"recipe-{suffix}";
        }

        return $"{slugBody}-{suffix}";
    }
}
