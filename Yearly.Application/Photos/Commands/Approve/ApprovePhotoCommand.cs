using ErrorOr;
using MediatR;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;

namespace Yearly.Application.Photos.Commands.Approve;

public record ApprovePhotoCommand(PhotoId PhotoId, string SessionCookie) : IRequest<ErrorOr<Unit>>;