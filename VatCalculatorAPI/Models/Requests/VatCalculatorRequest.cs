using MediatR;
using VatCalculatorAPI.Models.Responses;

namespace VatCalculatorAPI.Models.Requests;

/// <summary>
/// The request entity.
/// </summary>
public record VatCalculatorRequest : IRequest<VatCalculatorResponse>
{
    /// <summary>
    ///  The net amount.
    /// </summary>
    public string? NetAmount { get; init; }
    
    /// <summary>
    ///  The gross amount.
    /// </summary>
    public string? GrossAmount { get; init; }
    
    /// <summary>
    ///  The VAT amount.
    /// </summary>
    public string? VatAmount { get; init; }
    
    /// <summary>
    ///  The VAT rate.
    /// </summary>
    public string? VatRate { get; init; }
}