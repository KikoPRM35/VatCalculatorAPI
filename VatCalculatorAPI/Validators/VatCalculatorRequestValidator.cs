using FluentValidation;
using VatCalculatorAPI.Models.Requests;

namespace VatCalculatorAPI.Validators;

/// <summary>
/// The validator for the <see cref="VatCalculatorRequest"/> model.
/// </summary>
public class VatCalculatorRequestValidator : BaseNumericValidator<VatCalculatorRequest>
{
    /// <summary>
    /// The constructor of the <see cref="VatCalculatorRequestValidator"/> class.
    /// </summary>
    public VatCalculatorRequestValidator()
    {
        RuleFor(x => x.VatRate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ValidationErrors.NotFoundVatRateValue)
            .Must(BeNumeric)
            .WithMessage(ValidationErrors.VatRateMustBeNumericValue)
            .Must(vatRate=> BeGreaterThan(vatRate, 0))
            .WithMessage(ValidationErrors.InvalidVatRateValue);

        AmountsValidationRules(x => x);

        AmountValidationRule(
            x => x.NetAmount,
            nameof(VatCalculatorRequest.NetAmount).Replace("Amount", string.Empty),
            x => HasAmountsValidationPassed && 
                 x.GrossAmount is null && 
                 x.VatAmount is null
        );

        AmountValidationRule(
            x => x.GrossAmount,
            nameof(VatCalculatorRequest.GrossAmount).Replace("Amount", string.Empty),
            x => HasAmountsValidationPassed && 
                 x.NetAmount is null && 
                 x.VatAmount is null
        );

        AmountValidationRule(
            x => x.VatAmount,
            nameof(VatCalculatorRequest.VatAmount).Replace("Amount", string.Empty),
            x => HasAmountsValidationPassed && 
                 x.NetAmount is null && 
                 x.GrossAmount is null
        );
    }
}