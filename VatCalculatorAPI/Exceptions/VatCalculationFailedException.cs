namespace VatCalculatorAPI.Exceptions;

/// <summary>
///  Exception thrown when the VAT calculation fails.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="errors">The list of errors.</param>
public class VatCalculationFailedException(string? message, IEnumerable<string>? errors) : Exception
{
    /// <summary>
    ///  The errors associated.
    /// </summary>
    public IEnumerable<string>? Errors { get; } = errors;
    
    /// <summary>
    /// The error message.
    /// </summary>

    public string? ErrorMessage { get; } = message;
}