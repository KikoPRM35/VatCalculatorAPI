namespace VatCalculatorAPI.Utils;

/// <summary>
/// The generic result entity.
/// </summary>
public class Result<T>
{
    /// <summary>
    /// The result value.
    /// </summary>
    public T? Value { get; private init; }
    
    /// <summary>
    /// Determines whether the result is successful.
    /// </summary>
    public bool IsSuccess { get; private init; }
    
    /// <summary>
    /// The message of the result.
    /// </summary>
    public string? Message { get; private init; }
    
    /// <summary>
    /// A list of errors.
    /// </summary>
    public IEnumerable<string>? Errors { get; private init; }
    
    /// <summary>
    /// The success constructor in case of success.
    /// </summary>
    /// <param name="value">The result value.</param>
    /// <param name="message">The result message.</param>
    /// <returns>The generic result object.</returns>
    public static Result<T> Success(T value, string? message = null) => new()
    {
        Value = value,
        IsSuccess = true,
        Message = message
    };

    /// <summary>
    /// The failure constructor in case of failure.
    /// </summary>
    /// <param name="message">The result message.</param>
    /// <param name="errors">The list of errors.</param>
    /// <returns>The generic result object.</returns>
    public static Result<T> Failure(string message, IEnumerable<string>? errors = null) => new()
    {
        IsSuccess = false,
        Message = message,
        Errors = errors
    };

    /// <summary>
    /// The implicit operator to convert the result to the value.
    /// </summary>
    /// <param name="result">The generic result.</param>
    /// <returns>The result value.</returns>
    public static implicit operator T?(Result<T> result) => result.Value;
}