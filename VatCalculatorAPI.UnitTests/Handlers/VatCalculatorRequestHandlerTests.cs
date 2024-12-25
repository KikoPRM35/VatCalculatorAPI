using NSubstitute;
using VatCalculatorAPI.Exceptions;
using VatCalculatorAPI.Handlers;
using VatCalculatorAPI.Models.Requests;
using VatCalculatorAPI.Models.Responses;
using VatCalculatorAPI.Services.Interfaces;
using VatCalculatorAPI.Utils;

namespace VatCalculatorAPI.UnitTests.Handlers;

/// <summary>
///  Unit tests for <see cref="VatCalculatorRequestHandler"/>.
/// </summary>
[TestFixture]
public class VatCalculatorRequestHandlerTests
{
    private VatCalculatorRequestHandler _vatCalculatorRequestHandler;
    private IVatCalculatorService _vatCalculatorServiceMock;
    private Random _random;

    /// <summary>
    ///  Set up the test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _vatCalculatorServiceMock = Substitute.For<IVatCalculatorService>();
        _vatCalculatorRequestHandler = new VatCalculatorRequestHandler(_vatCalculatorServiceMock);

        _random = new Random();
    }

    [Test]
    [Description("When calculator service returns a success result, handler should return the response value.")]
    public async Task WhenCalculatorServiceReturnsASuccessResultHandlerShouldReturnTheResponseValue()
    {
        // Arrange
        var request = new VatCalculatorRequest
        {
            NetAmount = _random.Next(1, 100).ToString(),
            VatRate = _random.Next(1, 10).ToString()
        };

        var expectedResponse = new VatCalculatorResponse
        {
            VatAmount = _random.Next(1, 100),
            GrossAmount = _random.Next(1, 100),
            NetAmount = decimal.Parse(request.NetAmount),
            VatRate = decimal.Parse(request.VatRate)
        };

        _vatCalculatorServiceMock.Calculate(Arg.Any<VatCalculatorRequest>()).Returns(
            Result<VatCalculatorResponse>.Success(expectedResponse));

        // Act
        var result = await _vatCalculatorRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.VatAmount, Is.EqualTo(expectedResponse.VatAmount));
            Assert.That(result.GrossAmount, Is.EqualTo(expectedResponse.GrossAmount));
            Assert.That(result.NetAmount, Is.EqualTo(expectedResponse.NetAmount));
            Assert.That(result.VatRate, Is.EqualTo(expectedResponse.VatRate));
        });
    }

    [Test]
    [Description("When calculator service returns an unsuccessful result, handler should throw an exception.")]
    public void WhenCalculatorServiceReturnsAnUnsuccessfulResultHandlerShouldThrowAnException()
    {
        // Arrange
        var request = new VatCalculatorRequest
        {
            NetAmount = _random.Next(1, 100).ToString(),
            VatRate = _random.Next(1, 10).ToString()
        };

        const string dummyErrorMessage = "Dummy error message";
        string[] dummyErrors = ["Dummy error"];

        _vatCalculatorServiceMock.Calculate(Arg.Any<VatCalculatorRequest>()).Returns(
            Result<VatCalculatorResponse>.Failure(dummyErrorMessage, dummyErrors));

        // Act / Assert
        var exception = Assert.ThrowsAsync<VatCalculationFailedException>(async () =>
            await _vatCalculatorRequestHandler.Handle(request, CancellationToken.None));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(exception.ErrorMessage, Is.EqualTo(dummyErrorMessage));
            Assert.That(exception.Errors, Is.EquivalentTo(dummyErrors));
        });
    }
}