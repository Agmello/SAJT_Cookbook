using System.Collections.Generic;
using MediatR;
using SAJT.Cookbook.Application.Users.Models;

namespace SAJT.Cookbook.Application.Users.Queries.GetUsers;

public sealed record GetUsersQuery() : IRequest<IReadOnlyList<UserSummaryDto>>;
