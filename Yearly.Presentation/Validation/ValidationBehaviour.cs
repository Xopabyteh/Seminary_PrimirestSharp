using ErrorOr;
using FluentValidation;
using MediatR;

namespace Yearly.Presentation.Validation;

public class ValidationBehaviour<TRequest,TResponse>
    : IPipelineBehavior<TRequest, ErrorOr<TResponse>>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{

    private readonly IValidator<TRequest>? _validator;

    public ValidationBehaviour(IValidator<TRequest>? validator = null)
    {
        _validator = validator;
    }

    public async Task<ErrorOr<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ErrorOr<TResponse>> next, CancellationToken cancellationToken)
    {
        if (_validator is null)
            return await next();

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
            return await next();

        //Request is invalid
        var errors = validationResult.Errors
            .ConvertAll(e => Error.Validation(e.ErrorCode, e.ErrorMessage));

        return errors;
    }
}