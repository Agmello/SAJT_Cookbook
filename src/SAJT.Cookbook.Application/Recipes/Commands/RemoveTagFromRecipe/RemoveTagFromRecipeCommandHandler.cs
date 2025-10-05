using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;

namespace SAJT.Cookbook.Application.Recipes.Commands.RemoveTagFromRecipe;

public sealed class RemoveTagFromRecipeCommandHandler : IRequestHandler<RemoveTagFromRecipeCommand, RemoveTagFromRecipeResult>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveTagFromRecipeCommandHandler(
        IRecipeRepository recipeRepository,
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RemoveTagFromRecipeResult> Handle(RemoveTagFromRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetByIdAsync(request.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return RemoveTagFromRecipeResult.RecipeNotFound();
        }

        var tag = await _tagRepository.GetByIdAsync(request.TagId, cancellationToken);
        if (tag is null)
        {
            return RemoveTagFromRecipeResult.TagNotFound();
        }

        if (recipe.Tags.All(recipeTag => recipeTag.TagId != tag.Id))
        {
            return RemoveTagFromRecipeResult.TagNotAssigned();
        }

        recipe.RemoveTag(tag.Id);

        _recipeRepository.Update(recipe);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RemoveTagFromRecipeResult.Success();
    }
}
