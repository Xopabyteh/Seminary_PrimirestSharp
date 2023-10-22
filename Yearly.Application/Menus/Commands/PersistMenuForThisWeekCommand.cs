using ErrorOr;
using MediatR;

namespace Yearly.Application.Menus.Commands;

/// <summary>
/// int: How many menus were newly persisted to our database
/// </summary>
public record PersistMenuForThisWeekCommand(string SessionCookie) : IRequest<ErrorOr<int>>;
