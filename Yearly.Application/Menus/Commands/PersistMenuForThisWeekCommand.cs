using MediatR;
using ErrorOr;

namespace Yearly.Application.Menus.Commands;

public record PersistMenuForThisWeekCommand() : IRequest<ErrorOr<Unit>>;
