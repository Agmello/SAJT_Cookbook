using System.Collections.Generic;
using MediatR;

namespace SAJT.Cookbook.Application.Ingredients.Queries.GetIngredients;

public sealed record GetIngredientsQuery : IRequest<IReadOnlyList<IngredientSummaryDto>>;
