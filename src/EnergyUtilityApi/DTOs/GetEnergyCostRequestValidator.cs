using FluentValidation;
public class GetEnergyCostRequestValidator : AbstractValidator<GetEnergyCostRequest>
{
    private readonly EnergyUtilityService _service;
    private bool IsValidPostcode { get; set; }
    private bool IsScotlandPostcode { get; set; }
    public GetEnergyCostRequestValidator(EnergyUtilityService service)
    {
        _service = service;

        RuleFor(x => x.Postcode).NotNull().NotEmpty()
        .WithMessage("Postcode is required")
        .Length(6, 8).WithMessage("Postcode have a length between 6 and 8 character")
        .MustAsync(BeAValidPostcode)
        .WithMessage("Invalid postcode");

        RuleFor(x => x.PaymentMethodId).InclusiveBetween(1, 3)
        .WithMessage("Payment method values must be between 1 and 3");

        RuleFor(x => x.MeterTypeId).InclusiveBetween(1, 2)
        .WithMessage("Meter type values must be 1 or 2");

        RuleFor(x => x.PropertyType).InclusiveBetween(1, 7)
        .WithMessage("Property type values must be between 1 and 7");

        RuleFor(x => x.PropertyAge).InclusiveBetween(1, 12)
        .WithMessage("Property age values must be between 1 and 12");

        RuleFor(x => x.FloorArea).InclusiveBetween(1, 5)
        .WithMessage("Floor area values must be between 1 and 5");

        RuleFor(x => x.HouseholdSize).InclusiveBetween(1, 3)
        .WithMessage("Household size values must be between 1 and 3");

        RuleFor(x => x.NumberOfAdults).InclusiveBetween(1, 5)
        .WithMessage("Number of adult values must be between 1 and 5");

        RuleFor(x => x.NumberOfBedrooms).InclusiveBetween(1, 5)
        .WithMessage("Number of bedroom values must be between 1 and 5");

        // payment method id and meter type id should be provided together
        When(r => r.PaymentMethodId != null || r.MeterTypeId != null, () =>
        {
            RuleFor(x => x.PaymentMethodId).NotEmpty();
            RuleFor(x => x.MeterTypeId).NotEmpty();
        });
    }

    private async Task<bool> BeAValidPostcode(string postcode, CancellationToken token)
    {
        try
        {
            if (postcode == null) return IsValidPostcode = false;
            IsValidPostcode = await _service.PostcodeExists(postcode);
            return IsValidPostcode;
        }
        catch (Exception)
        {
            return false;
        }
    }
    // private async Task<bool> BeAScotlandPostcode(string postcode)
    // {
    //     if (IsValidPostcode && IsScotlandPostcode == null)
    //     {
    //         IsScotlandPostcode = await _service.PostcodeFromScotland(postcode);
    //     }
    //     return IsValidPostcode && IsScotlandPostcode;
    // }

}