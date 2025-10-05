using System.ComponentModel.DataAnnotations;

namespace SAJT.Cookbook.WebApi.Requests.Recipes;

public sealed class AddRecipeTagRequest
{
    [Required]
    public long TagId { get; set; }
}
