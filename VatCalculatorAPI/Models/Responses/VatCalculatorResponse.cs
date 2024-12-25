namespace VatCalculatorAPI.Models.Responses;

/// <summary>
/// The response entity.
/// </summary>
public record VatCalculatorResponse
{
    /// <summary>
    ///  The net amount.
    /// </summary>
    public decimal NetAmount { get; set; }
    
    /// <summary>
    ///  The gross amount.
    /// </summary>
    public decimal GrossAmount { get; set; }
    
    /// <summary>
    ///  The VAT amount.
    /// </summary>
    public decimal VatAmount { get; set; }
    
    /// <summary>
    ///  The VAT rate.
    /// </summary>
    public decimal VatRate { get; init; }
}