using VatCalculatorAPI.Models.Requests;
using VatCalculatorAPI.Models.Responses;
using VatCalculatorAPI.Utils;

namespace VatCalculatorAPI.Services.Interfaces;

/// <summary>
/// The VAT calculator service interface.
/// </summary>
public interface IVatCalculatorService
{
    /// <summary>
    /// The calculate method.
    /// </summary>
    /// <param name="request">The request entity provided.</param>
    /// <returns>The result <see cref="VatCalculatorResponse"/> based on the request. If any errors, those would be reported.</returns>
    Result<VatCalculatorResponse> Calculate(VatCalculatorRequest request);
}