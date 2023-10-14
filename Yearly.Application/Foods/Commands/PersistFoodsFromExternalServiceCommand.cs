using ErrorOr;
using MediatR;

namespace Yearly.Application.Foods.Commands;

public sealed record PersistFoodsFromExternalServiceCommand() : IRequest<ErrorOr<Unit>>;
