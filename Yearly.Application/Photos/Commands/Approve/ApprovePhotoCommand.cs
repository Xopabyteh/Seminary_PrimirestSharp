using ErrorOr;
using MediatR;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Photos.Commands.Approve;

public record ApprovePhotoCommand(PhotoId PhotoId, User Approver) : IRequest<ErrorOr<Unit>>;