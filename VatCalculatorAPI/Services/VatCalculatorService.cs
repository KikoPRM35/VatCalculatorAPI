using VatCalculatorAPI.Models.Requests;
using VatCalculatorAPI.Models.Responses;
using VatCalculatorAPI.Services.Errors;
using VatCalculatorAPI.Services.Interfaces;
using VatCalculatorAPI.Utils;

namespace VatCalculatorAPI.Services;

/// <summary>
///  The VAT calculator service.
/// </summary>
public class VatCalculatorService : IVatCalculatorService
{
    /// <inheritdoc />
    public Result<VatCalculatorResponse> Calculate(VatCalculatorRequest request)
    {
        if(!decimal.TryParse(request.VatRate, out var vatRate))
        {
            return Result<VatCalculatorResponse>.Failure(CalculatorErrors.InvalidRequest,
                [CalculatorErrors.VatRateMustBeValidDecimalNumber]);
        }
        
        decimal? netAmount = !string.IsNullOrWhiteSpace(request.NetAmount) ? decimal.Parse(request.NetAmount) : null;
        decimal? grossAmount = !string.IsNullOrWhiteSpace(request.GrossAmount) ? decimal.Parse(request.GrossAmount) : null;
        decimal? vatAmount = !string.IsNullOrWhiteSpace(request.VatAmount) ? decimal.Parse(request.VatAmount) : null;

        var response = new VatCalculatorResponse() { VatRate = vatRate };

        if (netAmount.HasValue)
        {
            response.NetAmount = netAmount.Value;
            response.VatAmount = Math.Round(response.NetAmount * (vatRate / 100), 2);
            response.GrossAmount = Math.Round(response.NetAmount + response.VatAmount, 2);
        }
        else if (grossAmount.HasValue)
        {
            response.GrossAmount = grossAmount.Value;
            response.NetAmount = Math.Round(response.GrossAmount / (1 + vatRate / 100), 2);
            response.VatAmount = Math.Round(response.GrossAmount - response.NetAmount, 2);
        }
        else if (vatAmount.HasValue)
        {
            response.VatAmount = vatAmount.Value;
            response.NetAmount = Math.Round(response.VatAmount * 100 / vatRate, 2);
            response.GrossAmount = Math.Round(response.NetAmount + response.VatAmount, 2);
        }
        else
        {
            return Result<VatCalculatorResponse>.Failure(CalculatorErrors.InvalidRequest,
                [CalculatorErrors.AtLeastOneAmountMustBeProvided]);
        }

        return Result<VatCalculatorResponse>.Success(response);
    }
}