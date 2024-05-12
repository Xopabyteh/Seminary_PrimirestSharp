using MediatR;
using Yearly.Contracts.Common;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Application.Photos.Queries;

public record GetUsersPhotosDataFragmentQuery(
    UserId UserId,
    int PageOffset,
    int PageSize)
    : IRequest<DataFragmentDTO<PhotoLinkDTO>>;