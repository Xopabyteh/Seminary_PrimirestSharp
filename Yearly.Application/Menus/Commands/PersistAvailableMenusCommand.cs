using ErrorOr;
using MediatR;

namespace Yearly.Application.Menus.Commands;

public record PersistAvailableMenusCommand : IRequest<ErrorOr<Unit>>;
