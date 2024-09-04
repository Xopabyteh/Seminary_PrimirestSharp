using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Yearly.Application.Common.Behaviours;


public class LoggingBehaviour<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName}", requestName);

        var result = await next();

        if(!result.IsError)
        {
            _logger.LogInformation("Handled {RequestName}", requestName);
            return result;
        }

        // -> Error
        using var context = LogContext.PushProperty("Errors", result.Errors, true);
        _logger.LogError("Handled {RequestName} with errors", requestName);

        return result;
    }
}