using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Ingredients.Queries.GetIngredients;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Application.Ingredients.Commands.CreateIngredient;

public sealed class CreateIngredientCommandHandler : IRequestHandler<CreateIngredientCommand, CreateIngredientResult>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateIngredientCommandHandler(IIngredientRepository ingredientRepository, IUnitOfWork unitOfWork)
    {
        _ingredientRepository = ingredientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateIngredientResult> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return CreateIngredientResult.InvalidName();
        }

        var normalizedName = request.Name.Trim();
        var isTaken = await _ingredientRepository.IsNameTakenAsync(normalizedName, 0, cancellationToken);
        if (isTaken)
        {
            return CreateIngredientResult.NameAlreadyExists();
        }

        var ingredient = Ingredient.Create(normalizedName, request.PluralName?.Trim(), request.DefaultUnit);
        if (!request.IsActive)
        {
            ingredient.SetStatus(false);
        }

        _ingredientRepository.Add(ingredient);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new IngredientSummaryDto(
            ingredient.Id,
            ingredient.Name,
            ingredient.PluralName,
            ingredient.DefaultUnit,
            ingredient.IsActive);

        return CreateIngredientResult.Success(dto);
    }
}
