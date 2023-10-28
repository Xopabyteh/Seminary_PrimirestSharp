using ErrorOr;
using MediatR;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;

namespace Yearly.Application.Photos.Commands.Reject;

public record RejectPhotoCommand(PhotoId PhotoId, string SessionCookie) : IRequest<ErrorOr<Unit>>;