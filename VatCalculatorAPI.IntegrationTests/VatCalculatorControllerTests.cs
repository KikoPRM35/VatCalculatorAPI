using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using VatCalculatorAPI.Models.Requests;
using VatCalculatorAPI.Models.Responses;
using VatCalculatorAPI.Utils;
using VatCalculatorAPI.Validators;

namespace VatCalculatorAPI.IntegrationTests;

/// <summary>
///  Integration tests for the VatCalculatorController.
/// </summary>
[TestFixture]
public class VatCalculatorControllerTests
{
    private HttpClient _client;
    private Random _random;
    private JsonSerializerOptions _jsonOptions;
    private IMediator _mediatorMock;
    private const string Url = "/api/vatcalculator/calculate";

    /// <summary>
    ///  Set up the test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        var factory = new WebApplicationFactory<Program>();

        _mediatorMock = Substitute.For<IMediator>();
        
        _client = factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddTransient<IMediator>(_ => _mediatorMock);
                    });
                }).CreateClient(new WebApplicationFactoryClientOptions());
        
        _random = new Random();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    ///  Tear down the test environment.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }
    
    [Test]
    [Description("Given VAT rate is missing, when calling the API, should return a meaningful error.")]
    public async Task GivenVatRateValueIsMissingWhenCallingApiShouldReturnAMeaningfulError()
    {
        // Arrange
        var vatCalculatorRequest = new VatCalculatorRequest()
        {
            NetAmount = _random.Next(1, 100).ToString(),
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(vatCalculatorRequest), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.PostAsync(Url, jsonContent);

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.False);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.That(responseBody, Is.Not.Null.And.Not.Empty);

        var validationErrors = JsonSerializer.Deserialize<ValidationProblemDetails>(responseBody);
        Assert.That(validationErrors, Is.Not.Null);
        Assert.That(validationErrors.Errors, Has.Count.EqualTo(1));

        var error = validationErrors.Errors.First();
        Assert.Multiple(() =>
        {
            Assert.That(error.Key, Is.EqualTo(nameof(VatCalculatorRequest.VatRate)));
            Assert.That(error.Value.First(), Is.EqualTo(ValidationErrors.NotFoundVatRateValue));
        });
    }
    
    [Test]
    [Description("Given VAT rate is not a numeric value, when calling the API, should return a meaningful error.")]
    public async Task GivenVatRateValueIsNotANumericValueWhenCallingApiShouldReturnAMeaningfulError()
    {
        // Arrange
        var vatCalculatorRequest = new VatCalculatorRequest()
        {
            NetAmount = _random.Next(1, 100).ToString(),
            VatRate = "abc",
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(vatCalculatorRequest), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.PostAsync(Url, jsonContent);

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.False);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.That(responseBody, Is.Not.Null.And.Not.Empty);

        var validationErrors = JsonSerializer.Deserialize<ValidationProblemDetails>(responseBody);
        Assert.That(validationErrors, Is.Not.Null);
        Assert.That(validationErrors.Errors, Has.Count.EqualTo(1));

        var error = validationErrors.Errors.First();
        Assert.Multiple(() =>
        {
            Assert.That(error.Key, Is.EqualTo(nameof(VatCalculatorRequest.VatRate)));
            Assert.That(error.Value.First(), Is.EqualTo(ValidationErrors.VatRateMustBeNumericValue));
        });
    }
    
    [Test]
    [Description("Given VAT rate is not greater than 0, when calling the API, should return a meaningful error.")]
    public async Task GivenVatRateValueIsNotGreaterThanZeroWhenCallingApiShouldReturnAMeaningfulError()
    {
        // Arrange
        var vatCalculatorRequest = new VatCalculatorRequest()
        {
            NetAmount = _random.Next(1, 100).ToString(),
            VatRate = "0"
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(vatCalculatorRequest), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.PostAsync(Url, jsonContent);

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.False);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.That(responseBody, Is.Not.Null.And.Not.Empty);

        var validationErrors = JsonSerializer.Deserialize<ValidationProblemDetails>(responseBody,_jsonOptions);
        Assert.That(validationErrors, Is.Not.Null);
        Assert.That(validationErrors.Errors, Has.Count.EqualTo(1));

        var error = validationErrors.Errors.First();
        Assert.Multiple(() =>
        {
            Assert.That(error.Key, Is.EqualTo(nameof(VatCalculatorRequest.VatRate)));
            Assert.That(error.Value.First(), Is.EqualTo(ValidationErrors.InvalidVatRateValue));
        });
    }
    
    [Test]
    [Description("Given no amount value defined, when calling the API, should return a meaningful error.")]
    public async Task GivenNoAmountValueDefinedWhenCallingApiShouldReturnAMeaningfulError()
    {
        // Arrange
        var vatCalculatorRequest = new VatCalculatorRequest()
        {
            VatRate = _random.Next(1, 10).ToString()
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(vatCalculatorRequest), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.PostAsync(Url, jsonContent);

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.False);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.That(responseBody, Is.Not.Null.And.Not.Empty);

        var validationErrors = JsonSerializer.Deserialize<ValidationProblemDetails>(responseBody,_jsonOptions);
        Assert.That(validationErrors, Is.Not.Null);
        Assert.That(validationErrors.Errors, Has.Count.EqualTo(1));

        var error = validationErrors.Errors.First();
        Assert.Multiple(() =>
        {
            Assert.That(error.Key, Is.Empty);
            Assert.That(error.Value.First(), Is.EqualTo(ValidationErrors.NotFoundAmountsValue));
        });
    }
    
    [Test]
    [Description("Given more than one amount value is defined, when calling the API, should return a meaningful error.")]
    public async Task GivenMoreThanOneAmountValueIsDefinedWhenCallingApiShouldReturnAMeaningfulError()
    {
        // Arrange
        var vatCalculatorRequest = new VatCalculatorRequest()
        {
            NetAmount = _random.Next(1, 100).ToString(),
            GrossAmount = _random.Next(1, 100).ToString(),
            VatRate = _random.Next(1, 10).ToString()
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(vatCalculatorRequest), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.PostAsync(Url, jsonContent);

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.False);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.That(responseBody, Is.Not.Null.And.Not.Empty);

        var validationErrors = JsonSerializer.Deserialize<ValidationProblemDetails>(responseBody,_jsonOptions);
        Assert.That(validationErrors, Is.Not.Null);
        Assert.That(validationErrors.Errors, Has.Count.EqualTo(1));

        var error = validationErrors.Errors.First();
        Assert.Multiple(() =>
        {
            Assert.That(error.Key, Is.Empty);
            Assert.That(error.Value.First(), Is.EqualTo(ValidationErrors.MoreThanOneAmountIsDefined));
        });
    }
    
    [TestCase("Net")]
    [TestCase("Gross")]
    [TestCase("Vat")]
    [Description("Given the amount value is missing, when calling the API, should return a meaningful error.")]
    public async Task GivenTheAmountValueIsMissingWhenCallingApiShouldReturnAMeaningfulError(string amountType)
    {
        // Arrange
        var vatCalculatorRequest = CreateRequest(amountType, string.Empty);

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(vatCalculatorRequest), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.PostAsync(Url, jsonContent);

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.False);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.That(responseBody, Is.Not.Null.And.Not.Empty);

        var validationErrors = JsonSerializer.Deserialize<ValidationProblemDetails>(responseBody,_jsonOptions);
        Assert.That(validationErrors, Is.Not.Null);
        Assert.That(validationErrors.Errors, Has.Count.EqualTo(1));

        var error = validationErrors.Errors.First();
        Assert.Multiple(() =>
        {
            var expectedKey = amountType switch
            {
                "Net" => nameof(VatCalculatorRequest.NetAmount),
                "Gross" => nameof(VatCalculatorRequest.GrossAmount),
                "Vat" => nameof(VatCalculatorRequest.VatAmount),
                _ => string.Empty
            };
            
            Assert.That(error.Key, Is.EqualTo(expectedKey));
            Assert.That(error.Value.First(), Is.EqualTo(string.Format(ValidationErrors.NotFoundAmountValue, amountType)));
        });
    }
    
    [TestCase("Net")]
    [TestCase("Gross")]
    [TestCase("Vat")]
    [Description("Given the amount value is not numeric, when calling the API, should return a meaningful error.")]
    public async Task GivenTheAmountValueIsNotNumericWhenCallingApiShouldReturnAMeaningfulError(string amountType)
    {
        // Arrange
        var vatCalculatorRequest = CreateRequest(amountType, "abc");

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(vatCalculatorRequest), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.PostAsync(Url, jsonContent);

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.False);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.That(responseBody, Is.Not.Null.And.Not.Empty);

        var validationErrors = JsonSerializer.Deserialize<ValidationProblemDetails>(responseBody,_jsonOptions);
        Assert.That(validationErrors, Is.Not.Null);
        Assert.That(validationErrors.Errors, Has.Count.EqualTo(1));

        var error = validationErrors.Errors.First();
        Assert.Multiple(() =>
        {
            var expectedKey = amountType switch
            {
                "Net" => nameof(VatCalculatorRequest.NetAmount),
                "Gross" => nameof(VatCalculatorRequest.GrossAmount),
                "Vat" => nameof(VatCalculatorRequest.VatAmount),
                _ => string.Empty
            };
            
            Assert.That(error.Key, Is.EqualTo(expectedKey));
            Assert.That(error.Value.First(), Is.EqualTo(string.Format(ValidationErrors.AmountMustBeNumericValue, amountType)));
        });
    }

    [TestCase("Net")]
    [TestCase("Gross")]
    [TestCase("Vat")]
    [Description("Given the amount value is not greater than zero, when calling the API, should return a meaningful error.")]
    public async Task GivenTheAmountValueIsNotGreaterThanZeroWhenCallingApiShouldReturnAMeaningfulError(string amountType)
    {
        // Arrange
        var vatCalculatorRequest = CreateRequest(amountType, "0");

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(vatCalculatorRequest), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.PostAsync(Url, jsonContent);

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.False);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.That(responseBody, Is.Not.Null.And.Not.Empty);

        var validationErrors = JsonSerializer.Deserialize<ValidationProblemDetails>(responseBody,_jsonOptions);
        Assert.That(validationErrors, Is.Not.Null);
        Assert.That(validationErrors.Errors, Has.Count.EqualTo(1));

        var error = validationErrors.Errors.First();
        Assert.Multiple(() =>
        {
            var expectedKey = amountType switch
            {
                "Net" => nameof(VatCalculatorRequest.NetAmount),
                "Gross" => nameof(VatCalculatorRequest.GrossAmount),
                "Vat" => nameof(VatCalculatorRequest.VatAmount),
                _ => string.Empty
            };
            
            Assert.That(error.Key, Is.EqualTo(expectedKey));
            Assert.That(error.Value.First(), Is.EqualTo(string.Format(ValidationErrors.InvalidAmountValue, amountType)));
        });
    }
    
    [Test]
    [Description("When the calculator service calculates with success, should return ok result with the vat calculator response value.")]
    public async Task WhenTheCalculatorServiceCalculatesWithSuccessShouldReturnOkResultWithTheVatCalculatorResponseValue()
    {
        // Arrange
        var expectedResponse = new VatCalculatorResponse()
        {
            NetAmount = _random.Next(1, 100),
            GrossAmount = _random.Next(1, 100),
            VatAmount = _random.Next(1, 100),
            VatRate = _random.Next(1, 10)
        };

        _mediatorMock.Send(Arg.Any<VatCalculatorRequest>())
            .Returns(Result<VatCalculatorResponse>.Success(expectedResponse)!);
        
        var vatCalculatorRequest = new VatCalculatorRequest()
        {
            NetAmount = _random.Next(1, 100).ToString(),
            VatRate = _random.Next(1,10).ToString(),
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(vatCalculatorRequest), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.PostAsync(Url, jsonContent);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.That(responseBody, Is.Not.Null.And.Not.Empty);
        
        var resultResponse = JsonSerializer.Deserialize<VatCalculatorResponse>(responseBody, _jsonOptions);
        Assert.That(resultResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultResponse.GrossAmount, Is.EqualTo(expectedResponse.GrossAmount));
            Assert.That(resultResponse.NetAmount, Is.EqualTo(expectedResponse.NetAmount));
            Assert.That(resultResponse.VatAmount, Is.EqualTo(expectedResponse.VatAmount));
            Assert.That(resultResponse.VatRate, Is.EqualTo(expectedResponse.VatRate));
        });
    }

    private VatCalculatorRequest CreateRequest(string amountType, string amountValue)
    {
        return amountType switch
        {
            "Net" => new VatCalculatorRequest() { NetAmount = amountValue, VatRate = _random.Next(1, 10).ToString() },
            "Gross" => new VatCalculatorRequest()
            {
                GrossAmount = amountValue, VatRate = _random.Next(1, 10).ToString()
            },
            "Vat" => new VatCalculatorRequest() { VatAmount = amountValue, VatRate = _random.Next(1, 10).ToString() },
            _ => new VatCalculatorRequest()
        };
    }
}
