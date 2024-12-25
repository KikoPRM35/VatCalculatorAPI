namespace VatCalculatorAPI.Services.Errors;

/// <summary>
///  The calculator errors.
/// </summary>
public static class CalculatorErrors
{
    /// <summary>
    ///  The invalid request error.
    /// </summary>
    public const string InvalidRequest = "Invalid request";
    
    /// <summary>
    ///  The vat rate must be a valid decimal number error.
    /// </summary>
    
    public const string VatRateMustBeValidDecimalNumber = "Vat rate must be a valid decimal number";
    
    /// <summary>
    ///  The at least one amount must be provided error.
    /// </summary>
    public const string AtLeastOneAmountMustBeProvided = "At least one amount (net, gross or vat) must be provided";
}