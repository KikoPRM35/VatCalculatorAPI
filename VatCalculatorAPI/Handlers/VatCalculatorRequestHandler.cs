using MediatR;
using VatCalculatorAPI.Exceptions;
using VatCalculatorAPI.Models.Requests;
using VatCalculatorAPI.Models.Responses;
using VatCalculatorAPI.Services.Interfaces;

namespace VatCalculatorAPI.Handlers;

/// <summary>
/// The handler for the VAT calculator request.
/// </summary>
public class VatCalculatorRequestHandler(IVatCalculatorService vatCalculatorService) : IRequestHandler<VatCalculatorRequest, VatCalculatorResponse?>
{
    /// <inheritdoc />
    public Task<VatCalculatorResponse?> Handle(VatCalculatorRequest request, CancellationToken cancellationToken)
    {
        var result = vatCalculatorService.Calculate(request);
        
        return result.IsSuccess ? Task.FromResult(result.Value) : throw new VatCalculationFailedException(result.Message, result.Errors);
    }
}