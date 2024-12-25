namespace VatCalculatorAPI.Validators;

/// <summary>
/// The class that contains the error messages for the validation rules.
/// </summary>
public static class ValidationErrors
{
    /// <summary>
    /// The error message for the missing value to be calculated.
    /// </summary>
    public const string NotFoundAmountValue = "The {0} amount to be calculated is missing.";
    
        /// <summary>
    /// The error message for the missing amounts value.
    /// </summary>
    public const string NotFoundAmountsValue = "At least one amount (net, gross, or VAT) should be provided";
    
    /// <summary>
    /// The error message for the missing vat rate value.
    /// </summary>
    public const string NotFoundVatRateValue = "The vat rate to use in calculation is missing.";

    /// <summary>
    /// The error message for the invalid price value.
    /// </summary>
    public const string InvalidAmountValue = "The provided {0} value is invalid, it must be greater than 0.";
    
    /// <summary>
    /// The error message for the invalid vat rate value.
    /// </summary>
    public const string InvalidVatRateValue = "The provided vat rate is invalid, it must be greater than 0.";
    
    /// <summary>
    /// The error message for the price non-numeric value.
    /// </summary>
    public const string AmountMustBeNumericValue = "The {0} value must be numeric.";

    /// <summary>
    /// The error message for the more than one amount defined.
    /// </summary>
    public const string MoreThanOneAmountIsDefined = "Only one amount (net, gross, or VAT) should be provided";

    /// <summary>
    /// The error message for the vat rate non-numeric value.
    /// </summary>
    public const string VatRateMustBeNumericValue = "The vat rate value must be numeric.";
}