using MediatR;
using Yearly.Contracts.Common;
using Yearly.Contracts.Users;

namespace Yearly.Application.Users.Queries;

public record GetUsersWithContextDataFragmentQuery(
    UsersWithContextFilter Filter,
    int PageOffset,
    int PageSize)
    : IRequest<DataFragmentDTO<UserWithContextDTO>>;
