using ErrorOr;
using MediatR;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Photos.Commands.Reject;

public record RejectPhotoCommand(Guid PhotoId, User Rejector) : IRequest<ErrorOr<Unit>>;