using System;

namespace SAJT.Cookbook.Application.Users.Models;

public sealed record UserSummaryDto(Guid Id, string Name, DateTime CreatedAtUtc, DateTime UpdatedAtUtc);
