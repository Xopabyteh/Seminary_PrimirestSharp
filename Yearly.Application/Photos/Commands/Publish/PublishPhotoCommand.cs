using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Photos.Commands.Publish;

public record PublishPhotoCommand(
    IFormFile File,
    Guid FoodId,
    User Publisher) : IRequest<ErrorOr<Photo>>;