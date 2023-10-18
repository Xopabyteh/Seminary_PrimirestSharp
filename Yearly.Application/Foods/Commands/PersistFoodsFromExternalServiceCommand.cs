using ErrorOr;
using MediatR;

namespace Yearly.Application.Foods.Commands;

/// <summary>
/// int: How many foods were newly persisted to our database
/// </summary>
public sealed record PersistFoodsFromExternalServiceCommand() : IRequest<ErrorOr<int>>;
