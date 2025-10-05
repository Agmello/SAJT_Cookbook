using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;

namespace SAJT.Cookbook.Application.Recipes.Commands.AddTagToRecipe;

public sealed class AddTagToRecipeCommandHandler : IRequestHandler<AddTagToRecipeCommand, AddTagToRecipeResult>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddTagToRecipeCommandHandler(
        IRecipeRepository recipeRepository,
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AddTagToRecipeResult> Handle(AddTagToRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetByIdAsync(request.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return AddTagToRecipeResult.RecipeNotFound();
        }

        var tag = await _tagRepository.GetByIdAsync(request.TagId, cancellationToken);
        if (tag is null)
        {
            return AddTagToRecipeResult.TagNotFound();
        }

        if (recipe.Tags.Any(recipeTag => recipeTag.TagId == tag.Id))
        {
            return AddTagToRecipeResult.TagAlreadyAssigned();
        }

        recipe.AddTag(tag);

        _recipeRepository.Update(recipe);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return AddTagToRecipeResult.Success();
    }
}
