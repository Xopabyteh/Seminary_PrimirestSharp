using FluentValidation;

namespace Yearly.Application.Photos.Commands.Publish;

public class PublishPhotoCommandValidator : AbstractValidator<PublishPhotoCommand>
{
    public PublishPhotoCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull();

        RuleFor(x => x.FoodId).NotNull();
    }
}