using ErrorOr;
using MediatR;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Photos.Commands.Approve;

public record ApprovePhotoCommand(Guid PhotoId, User Approver) : IRequest<ErrorOr<Unit>>;