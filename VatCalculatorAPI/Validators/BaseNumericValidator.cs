using System.Linq.Expressions;
using FluentValidation;
using VatCalculatorAPI.Models.Requests;

namespace VatCalculatorAPI.Validators;

/// <summary>
///  Base class for numeric validators
/// </summary>
/// <typeparam name="T">The generic type to use for validation.</typeparam>
public abstract class BaseNumericValidator<T> : AbstractValidator<T>
{
    protected bool HasAmountsValidationPassed = true;

    /// <summary>
    ///  Check if the value is numeric
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>true if hte value is numeric, otherwise, false.</returns>
    protected static bool BeNumeric(string? value)
    {
        return decimal.TryParse(value, out _);
    }

    /// <summary>
    ///  Check if the value is greater than the provided value
    /// </summary>
    /// <param name="value">The string value</param>
    /// <param name="greaterThan">The value to validate against.</param>
    /// <returns>true if the value is greater than the <paramref name="greaterThan"/> defined, otherwise, false.</returns>
    protected static bool BeGreaterThan(string? value, int greaterThan)
    {
        if (decimal.TryParse(value, out var decimalValue))
        {
            return decimalValue > greaterThan;
        }
        
        return false;
    }


    /// <summary>
    ///  The validation rule for the request amount values.
    /// </summary>
    protected void AmountValidationRule(
        Expression<Func<T, string?>> propertyExpression, 
        string propertyName,
        Func<T, bool> whenCondition)
    {
        RuleFor(propertyExpression)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(string.Format(ValidationErrors.NotFoundAmountValue, propertyName))
            .Must(BeNumeric)
            .WithMessage(string.Format(ValidationErrors.AmountMustBeNumericValue, propertyName))
            .Must(value => BeGreaterThan(value, 0))
            .WithMessage(string.Format(ValidationErrors.InvalidAmountValue, propertyName))
            .When(whenCondition);
    }

    /// <summary>
    ///  The validation rule for the request amounts values.
    /// </summary>
    protected void AmountsValidationRules(Expression<Func<T, VatCalculatorRequest>> expression)
    {
        RuleFor(expression)
            .Cascade(CascadeMode.Stop)
            .Must(x => 
            {
                var result = HaveAmountsDefined(x);
                HasAmountsValidationPassed = result;
                return result;
            })
            .WithMessage(ValidationErrors.NotFoundAmountsValue)
            .Must(x =>
            {
                var result = HaveOnlyOneAmount(x);
                HasAmountsValidationPassed = HasAmountsValidationPassed && result;
                return result;
            })
            .WithMessage(ValidationErrors.MoreThanOneAmountIsDefined);
    }
    
    /// <summary>
    ///  Check if the amounts are defined
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <returns>true if the request object has the amounts defined. otherwise, false.</returns>
    private static bool HaveAmountsDefined(VatCalculatorRequest request) => ProvidedAmounts(request) > 0;

    /// <summary>
    ///  Check if only one amount is defined
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <returns>true if the request has only one amount defined. otherwise, false.</returns>
    private static bool HaveOnlyOneAmount(VatCalculatorRequest request) => ProvidedAmounts(request) == 1;
    
    /// <summary>
    ///  Check how many amounts are provided
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <returns>The amount of objets that have value.</returns>
    private static int ProvidedAmounts(VatCalculatorRequest request)
    {
        var providedAmounts = 0;
        if (request.NetAmount is not null) providedAmounts++;
        if (request.GrossAmount is not null) providedAmounts++;
        if (request.VatAmount is not null) providedAmounts++;
        
        return providedAmounts;
    }
}