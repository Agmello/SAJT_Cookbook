using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;

namespace SAJT.Cookbook.Application.Ingredients.Commands.RenameIngredient;

public sealed class RenameIngredientCommandHandler : IRequestHandler<RenameIngredientCommand, RenameIngredientResult>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RenameIngredientCommandHandler(
        IIngredientRepository ingredientRepository,
        IUnitOfWork unitOfWork)
    {
        _ingredientRepository = ingredientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RenameIngredientResult> Handle(RenameIngredientCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return RenameIngredientResult.InvalidName();
        }

        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken);

        if (ingredient is null)
        {
            return RenameIngredientResult.NotFound();
        }

        var normalizedName = request.Name.Trim();

        var isNameTaken = await _ingredientRepository.IsNameTakenAsync(normalizedName, ingredient.Id, cancellationToken);
        if (isNameTaken)
        {
            return RenameIngredientResult.NameAlreadyExists();
        }

        ingredient.Rename(normalizedName, request.PluralName?.Trim());

        _ingredientRepository.Update(ingredient);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RenameIngredientResult.Success();
    }
}
