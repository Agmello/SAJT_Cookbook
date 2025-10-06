using System.ComponentModel.DataAnnotations;

namespace SAJT.Cookbook.WebApi.Requests.Users;

public sealed class CreateUserRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;
}
