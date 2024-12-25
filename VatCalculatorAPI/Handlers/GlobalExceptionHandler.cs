using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VatCalculatorAPI.Exceptions;

namespace VatCalculatorAPI.Handlers;

/// <summary>
/// The global exception handler.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
        };

        if (exception is VatCalculationFailedException vatCalculationFailedException)
        {
            problemDetails.Title = vatCalculationFailedException.ErrorMessage;
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            if (vatCalculationFailedException.Errors != null)
            {
                var validationErrors = vatCalculationFailedException.Errors.ToList();
                problemDetails.Extensions.Add("errors", validationErrors);
            }
        }
        else
        {
            problemDetails.Title = exception.Message;
        }
        
        logger.LogError("{ProblemDetailsTitle}", problemDetails.Title);

        problemDetails.Status = httpContext.Response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
        return true;
    }
}