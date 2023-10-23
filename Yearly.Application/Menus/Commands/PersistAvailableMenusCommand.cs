using ErrorOr;
using MediatR;

namespace Yearly.Application.Menus.Commands;

public record PersistAvailableMenusCommand(string SessionCookie) : IRequest<ErrorOr<Unit>>;
