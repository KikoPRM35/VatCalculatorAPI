using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using VatCalculatorAPI.Behaviors;
using VatCalculatorAPI.Models.Requests;
using VatCalculatorAPI.Models.Responses;

namespace VatCalculatorAPI.UnitTests.Behaviors;

/// <summary>
///  Unit tests for <see cref="RequestResponseLoggingBehavior{TRequest,TResponse}"/>.
/// </summary>
[TestFixture]
public class RequestResponseLoggingBehaviorTests
{
    [Test]
    [Description("When the handler is called, then the logging details is structured and logged based on the request and response.")]
    public async Task WhenHandlerIsCalledThenTheLoggingDetailsIsStructuredAndLoggedBasedOnTheRequestAndResponse()
    {
        // Arrange
        var loggerMock = Substitute.For<ILogger<RequestResponseLoggingBehavior<VatCalculatorRequest, VatCalculatorResponse>>>();
        var random = new Random();
        var request = new VatCalculatorRequest
        {
            NetAmount = random.Next(1,100).ToString(),
            VatRate = random.Next(1,10).ToString()
        };
        var expectedResponse = new VatCalculatorResponse
        {
            NetAmount = decimal.Parse(request.NetAmount),
            VatRate = decimal.Parse(request.VatRate),
            VatAmount = random.Next(1,100),
            GrossAmount = random.Next(1,100)
        };
        
        var next = Substitute.For<RequestHandlerDelegate<VatCalculatorResponse>>();
        next.Invoke().Returns(Task.FromResult(expectedResponse));
        
        var behavior = new RequestResponseLoggingBehavior<VatCalculatorRequest, VatCalculatorResponse>(loggerMock);

        // Act
        var result = behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Result.VatAmount, Is.EqualTo(expectedResponse.VatAmount));
            Assert.That(result.Result.GrossAmount, Is.EqualTo(expectedResponse.GrossAmount));
            Assert.That(result.Result.NetAmount, Is.EqualTo(expectedResponse.NetAmount));
            Assert.That(result.Result.VatRate, Is.EqualTo(expectedResponse.VatRate));
        });
        
        await next.Received(1).Invoke();
        loggerMock.ReceivedWithAnyArgs(2).LogInformation("Validation String");
    }
}