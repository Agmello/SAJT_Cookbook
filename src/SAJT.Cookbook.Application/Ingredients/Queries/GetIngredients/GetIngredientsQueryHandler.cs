using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SAJT.Cookbook.Application.Abstractions.Repositories;

namespace SAJT.Cookbook.Application.Ingredients.Queries.GetIngredients;

public sealed class GetIngredientsQueryHandler : IRequestHandler<GetIngredientsQuery, IReadOnlyList<IngredientSummaryDto>>
{
    private readonly IIngredientRepository _ingredientRepository;

    public GetIngredientsQueryHandler(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public async Task<IReadOnlyList<IngredientSummaryDto>> Handle(GetIngredientsQuery request, CancellationToken cancellationToken)
    {
        var ingredients = await _ingredientRepository.ListAsync(cancellationToken);

        return ingredients
            .Select(ingredient => new IngredientSummaryDto(
                ingredient.Id,
                ingredient.Name,
                ingredient.PluralName,
                ingredient.DefaultUnit,
                ingredient.IsActive))
            .ToList();
    }
}
