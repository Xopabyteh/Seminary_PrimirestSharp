using ErrorOr;
using MediatR;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Photos.Commands.Reject;

public record RejectPhotoCommand(PhotoId PhotoId, User Rejector) : IRequest<ErrorOr<Unit>>;