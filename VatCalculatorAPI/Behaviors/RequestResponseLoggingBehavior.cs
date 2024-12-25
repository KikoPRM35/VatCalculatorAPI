using System.Text.Json;
using MediatR;

namespace VatCalculatorAPI.Behaviors;

/// <summary>
///  The RequestResponseLoggingBehavior class pipeline behavior that logs the request and response of the request.
/// </summary>
public class RequestResponseLoggingBehavior<TRequest, TResponse>(ILogger<RequestResponseLoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();
        
        var requestJson = JsonSerializer.Serialize(request);
        logger.LogInformation("Handling request {CorrelationID}: {Request}", correlationId, requestJson);

        var response = await next();
        
        var responseJson = JsonSerializer.Serialize(response);
        logger.LogInformation("Response for {Correlation}: {Response}", correlationId, responseJson);

        return response;
    }
}