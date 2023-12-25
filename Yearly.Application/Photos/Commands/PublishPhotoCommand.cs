using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.Services;

namespace Yearly.Application.Photos.Commands;

public record PublishPhotoCommand(
    IFormFile File,
    FoodId FoodId,
    User Publisher) : IRequest<ErrorOr<Photo>>;

public class PublishPhotoCommandValidator : AbstractValidator<PublishPhotoCommand>
{
    public PublishPhotoCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull();

        RuleFor(x => x.FoodId).NotNull();
    }
}

public class PublishPhotoCommandHandler : IRequestHandler<PublishPhotoCommand, ErrorOr<Photo>>
{
    private readonly IPhotoStorage _photoStorage;
    private readonly IPhotoRepository _photoRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IFoodRepository _foodRepository;

    public PublishPhotoCommandHandler(IPhotoStorage photoStorage, IPhotoRepository photoRepository, IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork, IUserRepository userRepository, IFoodRepository foodRepository)
    {
        _photoStorage = photoStorage;
        _photoRepository = photoRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _foodRepository = foodRepository;
    }


    public async Task<ErrorOr<Photo>> Handle(PublishPhotoCommand request, CancellationToken cancellationToken)
    {
        //Check if the food exists
        var food = await _foodRepository.GetFoodByIdAsync(request.FoodId);
        if(food is null)
            return Errors.Errors.Food.FoodNotFound(request.FoodId);

        var photoId = new PhotoId(Guid.NewGuid());
        var linkResult = await _photoStorage.UploadPhotoAsync(request.File, Photo.NameFrom(photoId, food));

        if (linkResult.IsError)
            return linkResult.Errors;

        var photo = request.Publisher.PublishPhoto(
            photoId,
            _dateTimeProvider.UtcNow,
            request.FoodId,
            linkResult.Value);

        //await _userRepository.UpdateAsync(request.Publisher);
        await _photoRepository.AddAsync(photo);
        await _unitOfWork.SaveChangesAsync();

        return photo;
    }
}