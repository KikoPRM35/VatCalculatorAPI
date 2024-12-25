using VatCalculatorAPI.Models.Requests;
using VatCalculatorAPI.Services;

namespace VatCalculatorAPI.UnitTests.Services;

/// <summary>
///  Unit tests for <see cref="VatCalculatorService"/>.
/// </summary>
[TestFixture]
public class VatCalculatorServiceTests
{
    private VatCalculatorService _vatCalculatorService;

    /// <summary>
    ///  Set up the test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _vatCalculatorService = new VatCalculatorService();
    }

    [Test]
    [Description("When Net amount is provided, the service should return the correct VAT and Gross amount.")]
    public void WhenNetAmountProvidedThenCalculatorServiceShouldReturnCorrectVatAndGrossAmount()
    {
        // Arrange
        var request = new VatCalculatorRequest
        {
            NetAmount = "100",
            VatRate = "20"
        };

        // Act
        var result = _vatCalculatorService.Calculate(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.VatAmount, Is.EqualTo(20));
            Assert.That(result.Value!.GrossAmount, Is.EqualTo(120));
        });
    }
    
    [Test]
    [Description("When Gross amount is provided, the service should return the correct VAT and Net amount.")]
    public void WhenGrossAmountProvidedThenCalculatorServiceShouldReturnCorrectVatAndNetAmount()
    {
        // Arrange
        var request = new VatCalculatorRequest
        {
            GrossAmount = "120",
            VatRate = "20"
        };

        // Act
        var result = _vatCalculatorService.Calculate(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.VatAmount, Is.EqualTo(20));
            Assert.That(result.Value!.NetAmount, Is.EqualTo(100));
        });
    }
    
    [Test]
    [Description("When VAT amount is provided, the service should return the correct Net and Gross amount.")]
    public void WhenVatAmountProvidedThenCalculatorServiceShouldReturnCorrectNetAndGrossAmount()
    {
        // Arrange
        var request = new VatCalculatorRequest
        {
            VatAmount = "20",
            VatRate = "20"
        };

        // Act
        var result = _vatCalculatorService.Calculate(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.NetAmount, Is.EqualTo(100));
            Assert.That(result.Value!.GrossAmount, Is.EqualTo(120));
        });
    }
    
    [Test]
    [Description("When no amount is provided, the service should return a failure result.")]
    public void WhenNoAmountIsProvidedThenCalculatorServiceShouldReturnFailureResult()
    {
        // Arrange
        var request = new VatCalculatorRequest
        {
            VatRate = "20"
        };

        // Act
        var result = _vatCalculatorService.Calculate(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Is.Not.Null);
            Assert.That(result.Errors!.Count(), Is.GreaterThan(0));
        });
    }
    
    [Test]
    [Description("When invalid VAT rate is provided, the service should return a failure result.")]
    public void WhenInvalidVatRateIsProvidedThenCalculatorServiceShouldReturnFailureResult()
    {
        // Arrange
        var request = new VatCalculatorRequest
        {
            GrossAmount = "100",
        };

        // Act
        var result = _vatCalculatorService.Calculate(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Is.Not.Null);
            Assert.That(result.Errors!.Count(), Is.GreaterThan(0));
        });
    }
}
