using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using VatCalculatorAPI.Exceptions;
using VatCalculatorAPI.Handlers;
using VatCalculatorAPI.UnitTests.Utils;

namespace VatCalculatorAPI.UnitTests.Handlers;

/// <summary>
///  Unit tests for <see cref="GlobalExceptionHandler"/>.
/// </summary>
[TestFixture]
public class GlobalExceptionHandlerTests
{
    private static IEnumerable<Exception> ExceptionsTestCases()
    {
        yield return new Exception("Dummy error message");
        yield return new VatCalculationFailedException("Dummy error message", ["Dummy Error"]);
    }
    
    [TestCaseSource(nameof(ExceptionsTestCases))]
    [Description("When TryHandleAsync of an exception should return true with the correspondent ProblemDetails associated returned back as response.")]
    public async Task WhenTryHandleAsyncOfAnExceptionShouldReturnTrueWithTheCorrespondentProblemDetailsAssociatedReturnedBackAsResponse(Exception exception)
    {
        // Arrange
        var httpContext = new HttpContextWrapper();
        var loggerMock = Substitute.For<ILogger<GlobalExceptionHandler>>();
        var handler = new GlobalExceptionHandler(loggerMock);

        // Act
        var result = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        var problemDetails = await httpContext.GetResponseBodyAsync<ProblemDetails>();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(problemDetails, Is.Not.Null);
        });
        
        if (exception is VatCalculationFailedException vatCalculationFailedException)
        {
            Assert.Multiple(() =>
            {
                Assert.That(problemDetails.Title, Is.EqualTo(vatCalculationFailedException.ErrorMessage));
                Assert.That(problemDetails.Extensions, Is.Not.Empty);
                Assert.That(problemDetails.Extensions.First().Key, Is.EqualTo("errors"));
                
                if(problemDetails.Extensions.First().Value is JsonElement jsonElement)
                {
                    Assert.That(jsonElement.Deserialize<string[]>(), Is.EquivalentTo(vatCalculationFailedException.Errors!));
                }
            });
        }
        else
        {
            Assert.That(problemDetails.Title, Is.EqualTo(exception.Message));
        }
        
        loggerMock.ReceivedWithAnyArgs(1).LogInformation("Validation String");
    }
}


